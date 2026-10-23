using System.Collections.Generic;
using System.Reflection;
using HandOfFate2Access.Focus;
using HandOfFate2Access.UI;
using HandOfFate2Access.Util;
using HarmonyLib;
using NodeCanvas.Framework;
using UnityEngine;

namespace HandOfFate2Access.Maps {
	/// <summary>
	/// Adapter from the live map board to <see cref="MapCellInfo"/>. Reads
	/// <c>Map.Instance.MapLayout</c> on every call (never cached). A slot the fog of war
	/// hides (<c>MapLayoutSlot.Hidden</c>) or that holds no card is treated as absent:
	/// a sighted player does not see it either. A neighbour behind an impassable wall
	/// is not a path. "Reachable" is the game's own <c>CanMoveTo</c>. A slot's marks are
	/// what the board shows on it beyond text: the token or shard on a face-up top card,
	/// and the figures (spices) standing on it. The Servant challenge's goblins are named
	/// by kind (<see cref="MapFigures"/>), a patrol with the way it faces, and a card a
	/// goblin will step onto after the player moves there says so; any other figure is
	/// "a figure", one that blocks the player named so. A figure has only an internal
	/// name, never spoken; each new one is logged, and each goblin's cell and facing
	/// whenever they change.
	/// </summary>
	internal static class MapBoardReader {
		private static readonly HashSet<string> LoggedFigures = new HashSet<string>();
		private static readonly Dictionary<string, string> LoggedGoblins = new Dictionary<string, string>();

		// A spice turning to face a new card rotates after its move animation; until then the
		// rotation it will take is held here.
		private static readonly FieldInfo PendingRotationField = AccessTools.Field(typeof(Spice), "m_hasPendingRotation");
		private static readonly FieldInfo PendingDirectionField = AccessTools.Field(typeof(Spice), "m_pendingDirection");
		private static readonly FieldInfo FaceTimeField = AccessTools.Field(typeof(Spice), "m_faceTime");
		private static readonly FieldInfo FaceToField = AccessTools.Field(typeof(Spice), "m_faceTo");

		public static MapLayout Layout => Map.Instance != null ? Map.Instance.MapLayout : null;

		public static MapLayoutSlot SlotAt(int x, int y) {
			MapLayout layout = Layout;
			if (layout == null) return null;
			MapLayoutSlot slot = layout.GetSlot(new Vector2(x, y));
			if (slot == null || slot.Hidden || slot.Cards.Count == 0) return null;
			return slot;
		}

		public static bool TryGetPlayerCell(out int x, out int y) {
			x = 0;
			y = 0;
			Map map = Map.Instance;
			PlayerCounter counter = map != null ? map.PlayerCounter : null;
			if (counter == null || counter.CurrentSlot == null) return false;
			Cell(counter.CurrentSlot, out x, out y);
			return true;
		}

		public static void Cell(MapLayoutSlot slot, out int x, out int y) {
			x = Mathf.RoundToInt(slot.GridPosition.x);
			y = Mathf.RoundToInt(slot.GridPosition.y);
		}

		public static MapCellInfo Read(MapLayoutSlot slot) {
			Map map = Map.Instance;
			MapLayout layout = map.MapLayout;
			Card top = slot.TopCard;
			IList<string> topText = top != null && IsFaceUp(top) ? CardInfoReader.Read(top) : null;
			var attached = new List<IList<string>>();
			foreach (Card c in slot.Cards) {
				if (c == top) continue;
				attached.Add(IsFaceUp(c) ? (IList<string>)CardInfoReader.Read(c) : null);
			}
			bool here = map.PlayerCounter != null && map.PlayerCounter.CurrentSlot == slot;
			int x, y;
			Cell(slot, out x, out y);
			var exits = new MapExits(
				Path(slot, x, y - 1),
				Path(slot, x, y + 1),
				Path(slot, x - 1, y),
				Path(slot, x + 1, y));
			return new MapCellInfo(topText, attached, here, slot.IsComplete, slot.CanMoveTo,
				layout.ExitSlot == slot, exits, Marks(slot, top));
		}

		private static List<string> Marks(MapLayoutSlot slot, Card top) {
			var marks = new List<string>();
			if (top != null && IsFaceUp(top)) marks.AddRange(CardInfoReader.Marks(top));
			bool figure = false, blocks = false;
			foreach (Spice spice in slot.Spices) {
				if (spice == null || !spice.HasModel) continue;
				if (LoggedFigures.Add(spice.Id))
					Log.Info("map figure id=" + spice.Id + " name=" + spice.name + " blocks=" + spice.BlocksPlayer);
				if (MapFigures.KindOf(spice.Id) != FigureKind.Other) continue;
				figure = true;
				if (spice.BlocksPlayer) blocks = true;
			}
			if (blocks) marks.Add(Localization.Strings.MapCellFigureBlocks);
			else if (figure) marks.Add(Localization.Strings.MapCellFigure);
			marks.AddRange(GoblinMarks(slot));
			return marks;
		}

		/// <summary>
		/// The goblins on <paramref name="slot"/> and whether one steps onto it after the
		/// player moves there; empty when neither. The focus pump polls this while a map
		/// card holds focus, since the goblins move and turn just after each player step.
		/// </summary>
		public static List<string> GoblinMarks(MapLayoutSlot slot) {
			var marks = new List<string>();
			foreach (Spice spice in slot.Spices) {
				if (spice == null || !spice.HasModel) continue;
				string name = MapFigures.Describe(Figure(spice));
				if (name != null) marks.Add(name);
			}
			if (Settings.ModSettings.GoblinWarnings.On && GoblinEnters(slot)) marks.Add(Localization.Strings.MapGoblinNext);
			return marks;
		}

		/// <summary>
		/// <see cref="GoblinMarks"/> as one line once every figure on the board stands still;
		/// null while one is walking or turning, when its heading is not yet the one it keeps.
		/// </summary>
		public static string GoblinValue(MapLayoutSlot slot) {
			MapLayout layout = Layout;
			if (layout == null) return null;
			foreach (MapLayoutSlot other in layout.Slots) {
				if (other == null) continue;
				foreach (Spice spice in other.Spices) {
					if (spice == null || MapFigures.KindOf(spice.Id) == FigureKind.Other) continue;
					if (spice.MoveTarget != null || FaceTimeField.GetValue(spice) != null) return null;
				}
			}
			return new Message().AddRange(GoblinMarks(slot)).Resolve();
		}

		// Whether a goblin steps onto this card once the player moves there. In the shadow
		// realm no goblin moves.
		private static bool GoblinEnters(MapLayoutSlot slot) {
			MapLayout layout = Layout;
			if (layout == null || ShadowNight()) return false;
			int x, y;
			Cell(slot, out x, out y);
			foreach (MapLayoutSlot other in layout.Slots) {
				if (other == null || other == slot) continue;
				foreach (Spice spice in other.Spices) {
					if (spice == null || !spice.HasModel) continue;
					if (MapFigures.Enters(Figure(spice), x, y, Connected)) return true;
				}
			}
			return false;
		}

		private static bool Connected(int x1, int y1, int x2, int y2) {
			MapLayout layout = Layout;
			MapLayoutSlot a = layout.GetSlot(new Vector2(x1, y1)), b = layout.GetSlot(new Vector2(x2, y2));
			return a != null && b != null && a.TopCard != null && b.TopCard != null && !a.WallBetween(b);
		}

		// The Servant challenge's shadow realm: its blackboard's "night", which stops the
		// goblins moving and catching.
		private static bool ShadowNight() {
			GlobalBlackboard board = GlobalBlackboard.Find("DLC_00_Shadow");
			Variable night = board != null ? board.GetVariable("night", typeof(bool)) : null;
			return night != null && night.value is bool && (bool)night.value;
		}

		private static MapFigures.Figure Figure(Spice spice) {
			FigureKind kind = MapFigures.KindOf(spice.Id);
			int x = 0, y = 0, fx = 0, fy = 0;
			if (spice.Slot != null) Cell(spice.Slot, out x, out y);
			if (kind == FigureKind.GoblinPatrol) Facing(spice, out fx, out fy);
			if (kind != FigureKind.Other) {
				string key = spice.InstanceId ?? spice.Id;
				string state = "(" + x + ", " + y + ") facing (" + fx + ", " + fy + ")";
				string seen;
				if (!LoggedGoblins.TryGetValue(key, out seen) || seen != state) {
					LoggedGoblins[key] = state;
					Log.Info("map goblin " + spice.Id + " at " + state);
				}
			}
			return new MapFigures.Figure(kind, x, y, fx, fy);
		}

		// The grid step a figure faces: its forward (or the rotation it is about to take),
		// matched to the board's row and column directions within 45 degrees. (0, 0) when
		// it faces no grid direction, the board has no neighbouring cards to measure, or it
		// is still walking (a patrol turns toward its next card only once it arrives).
		private static void Facing(Spice spice, out int fx, out int fy) {
			fx = 0;
			fy = 0;
			Quaternion local = spice.transform.localRotation;
			try {
				if ((bool)PendingRotationField.GetValue(spice)) local = (Quaternion)PendingDirectionField.GetValue(spice);
				else if (spice.MoveTarget != null) return;
				else if (FaceTimeField.GetValue(spice) != null) local = (Quaternion)FaceToField.GetValue(spice);
			} catch (System.Exception e) {
				Log.Warn("map figure facing: " + e.Message);
			}
			Transform parent = spice.transform.parent;
			Vector3 forward = ((parent != null ? parent.rotation : Quaternion.identity) * local * Vector3.forward).normalized;
			Vector3 right, down;
			if (!GridAxes(out right, out down)) return;
			float best = Mathf.Cos(45f * Mathf.Deg2Rad);
			Vector3[] axes = { right, -right, down, -down };
			int[] dx = { 1, -1, 0, 0 }, dy = { 0, 0, 1, -1 };
			for (int i = 0; i < 4; i++) {
				float dot = Vector3.Dot(forward, axes[i]);
				if (dot > best) {
					best = dot;
					fx = dx[i];
					fy = dy[i];
				}
			}
		}

		// World directions of one column right and one row down on the board, from the
		// first pair of neighbouring slots found each way.
		private static bool GridAxes(out Vector3 right, out Vector3 down) {
			right = Vector3.zero;
			down = Vector3.zero;
			MapLayout layout = Layout;
			if (layout == null) return false;
			foreach (MapLayoutSlot slot in layout.Slots) {
				if (slot == null) continue;
				if (right == Vector3.zero) {
					MapLayoutSlot next = layout.GetSlot(slot.GridPosition + new Vector2(1, 0));
					if (next != null) right = (next.transform.position - slot.transform.position).normalized;
				}
				if (down == Vector3.zero) {
					MapLayoutSlot next = layout.GetSlot(slot.GridPosition + new Vector2(0, 1));
					if (next != null) down = (next.transform.position - slot.transform.position).normalized;
				}
				if (right != Vector3.zero && down != Vector3.zero) return true;
			}
			return false;
		}

		private static bool Path(MapLayoutSlot from, int x, int y) {
			MapLayoutSlot to = SlotAt(x, y);
			return to != null && !from.WallBetween(to);
		}

		// A map card lies face down by default; the layout turns it face up when it is
		// flipped or, for an encounter, revealed (CardContainerLayoutRotationReveal), and
		// the two cancel.
		public static bool IsFaceUp(Card card) {
			var encounter = card as EncounterCard;
			bool revealed = encounter != null && encounter.Revealed;
			return card.Flipped ^ revealed;
		}

		/// <summary>Diagnostic: the flags and orientation behind a slot's face-up decision.</summary>
		public static void LogFace(MapLayoutSlot slot) {
			Card top = slot.TopCard;
			if (top == null) return;
			var encounter = top as EncounterCard;
			Log.Info("map face " + slot.name + ": flipped=" + top.Flipped + " revealed="
				+ (encounter != null && encounter.Revealed) + " complete=" + slot.IsComplete
				+ " up=" + (top.CardTemplate != null ? top.CardTemplate.transform.up.ToString() : "?"));
		}
	}
}
