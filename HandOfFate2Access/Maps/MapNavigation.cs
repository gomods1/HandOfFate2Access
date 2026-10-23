using HandOfFate2Access.Util;

namespace HandOfFate2Access.Maps {
	/// <summary>
	/// The map's own arrows, made to mean directions from the player's card. The game
	/// moves its map selection by screen position among the cards the player can move to
	/// (the orthogonal neighbours, <c>MapLayoutSlot.CanMoveTo</c>), so with a card above
	/// and one to the left, Right on the left one jumps diagonally back to the one above.
	/// Here an arrow selects the neighbour in that direction when the player can move
	/// there; otherwise nothing moves and the press is recorded as blocked, which the focus
	/// pump says ("Can't go right"). Called from the prefix on
	/// UISelectionManager.OnNavigate, only while the map holds the selection.
	/// </summary>
	internal static class MapNavigation {
		public enum Outcome { None, Moved, AlreadyThere, Blocked }

		/// <summary>What the last map arrow did.</summary>
		public static Outcome Last { get; private set; }

		private static readonly int[] Dx = { 0, 0, -1, 1 };
		private static readonly int[] Dy = { -1, 1, 0, 0 };

		private static string Word(int i) {
			switch (i) {
				case 0: return Localization.Strings.DirUp;
				case 1: return Localization.Strings.DirDown;
				case 2: return Localization.Strings.DirLeft;
				default: return Localization.Strings.DirRight;
			}
		}

		/// <summary>The direction word of <paramref name="slot"/> from the player's card when it
		/// is a neighbour the player can move to; null otherwise.</summary>
		public static string DirectionFromPlayer(MapLayoutSlot slot) {
			if (slot == null || !slot.CanMoveTo || !MapBoardReader.TryGetPlayerCell(out int x, out int y)) return null;
			MapBoardReader.Cell(slot, out int sx, out int sy);
			for (int i = 0; i < 4; i++)
				if (sx == x + Dx[i] && sy == y + Dy[i]) return Word(i);
			return null;
		}

		/// <summary>The directions the player can move from their card, as words ("down, right"); null when none.</summary>
		public static string Available() {
			if (!MapBoardReader.TryGetPlayerCell(out int x, out int y)) return null;
			var words = new System.Collections.Generic.List<string>();
			for (int i = 0; i < 4; i++) {
				MapLayoutSlot slot = MapBoardReader.SlotAt(x + Dx[i], y + Dy[i]);
				if (slot != null && slot.CanMoveTo) words.Add(Word(i));
			}
			return words.Count > 0 ? string.Join(", ", words.ToArray()) : null;
		}

		/// <summary>Handles a navigation press; true when it was a map arrow (consumed).</summary>
		public static bool Navigate(InputButton button) {
			int dx = 0, dy = 0;
			switch (button) {
				case InputButton.Up: dy = -1; break;
				case InputButton.Down: dy = 1; break;
				case InputButton.Left: dx = -1; break;
				case InputButton.Right: dx = 1; break;
				default: return false;
			}
			if (!MapBoardReader.TryGetPlayerCell(out int x, out int y)) return false;
			MapLayoutSlot target = MapBoardReader.SlotAt(x + dx, y + dy);
			if (target == null || !target.CanMoveTo || target.Selectable == null || !target.Selectable.enabled) {
				Last = Outcome.Blocked;
				return true;
			}
			UISelectable hovered = target.Selectable.Selection != null ? target.Selectable.Selection.HoveredObject : null;
			if (hovered == target.Selectable) {
				Last = Outcome.AlreadyThere;
				return true;
			}
			if (UISelection.TryHover(target.Selectable)) {
				Last = Outcome.Moved;
				return true;
			}
			// The game refused the hover (a locked group): let its own navigation run.
			Log.Warn("map arrow: could not select (" + (x + dx) + ", " + (y + dy) + "); using the game's navigation");
			Last = Outcome.None;
			return false;
		}
	}
}
