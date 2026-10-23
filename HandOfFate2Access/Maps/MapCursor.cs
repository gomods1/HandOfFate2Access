using System.Collections.Generic;
using HandOfFate2Access.Input;
using HandOfFate2Access.Speech;
using HandOfFate2Access.UI;

namespace HandOfFate2Access.Maps {
	/// <summary>
	/// A free-roam survey cursor over the visible board, independent of the game's own
	/// selection (which reaches only the slots next to the player). Steps card to card;
	/// a step toward no visible card stays put and re-reads the current one, so the key
	/// is never silent. Shift plus period and comma jump it through the cards not yet
	/// visited. It only reads: moving the player is the game's own selection.
	/// </summary>
	internal sealed class MapCursor {
		private int _x;
		private int _y;
		private bool _anchored;
		// The unvisited card the last Shift plus period or comma reached.
		private MapTour.Cell? _toured;

		public void Anchor(MapLayoutSlot slot) {
			if (slot == null) return;
			MapBoardReader.Cell(slot, out _x, out _y);
			_anchored = true;
		}

		public void AnchorToPlayer() {
			_anchored = MapBoardReader.TryGetPlayerCell(out _x, out _y);
		}

		public void Move(Direction dir) {
			if (!_anchored) AnchorToPlayer();
			if (!_anchored) return;
			int x = _x, y = _y;
			switch (dir) {
				case Direction.Up: y -= 1; break;
				case Direction.Down: y += 1; break;
				case Direction.Left: x -= 1; break;
				case Direction.Right: x += 1; break;
			}
			bool blocked = MapBoardReader.SlotAt(x, y) == null;
			if (!blocked) {
				_x = x;
				_y = y;
			}
			Speak(blocked ? Localization.Strings.MapCursorBlocked(DirectionWord(dir)) : null, null);
		}

		/// <summary>
		/// Moves the cursor to the next (<paramref name="step"/> +1) or previous (-1) card the
		/// player has not visited: visible, not the player's own, and an encounter the game
		/// has never finished (so a shop already entered counts as visited, though it can be
		/// entered again). Nearest first (<see cref="MapTour"/>), wrapping at either end; the
		/// read ends with its place among them ("unvisited 2 of 5").
		/// </summary>
		public void Tour(int step) {
			if (!MapBoardReader.TryGetPlayerCell(out int px, out int py)) return;
			MapLayout layout = MapBoardReader.Layout;
			if (layout == null) return;
			var cells = new List<MapTour.Cell>();
			foreach (MapLayoutSlot candidate in layout.Slots) {
				if (candidate == null) continue;
				MapBoardReader.Cell(candidate, out int x, out int y);
				if ((x == px && y == py) || MapBoardReader.SlotAt(x, y) != candidate) continue;
				var encounter = candidate.TopCard as EncounterCard;
				if (encounter != null && encounter.ActivationCount > 0) continue;
				cells.Add(new MapTour.Cell(x, y));
			}
			List<MapTour.Cell> order = MapTour.Order(cells, px, py);
			int at = MapTour.Step(order, _toured, step);
			if (at < 0) {
				_toured = null;
				SpeechPipeline.SpeakInterrupt(Localization.Strings.MapNoUnvisited);
				return;
			}
			_toured = order[at];
			_x = order[at].X;
			_y = order[at].Y;
			_anchored = true;
			Speak(null, Localization.Strings.MapUnvisited(at + 1, order.Count));
		}

		// Reads the card under the cursor and where it lies from the player, between an
		// optional lead-in and closing part.
		private void Speak(string before, string after) {
			MapLayoutSlot slot = MapBoardReader.SlotAt(_x, _y);
			if (slot == null) return;
			var cell = new MapCellElement(MapBoardReader.Read(slot));
			var message = new Message();
			message.Add(before);
			message.Add(cell.Describe().Resolve());
			if (MapBoardReader.TryGetPlayerCell(out int px, out int py))
				message.Add(MapOffset.Describe(_x - px, _y - py));
			message.Add(after);
			string line = message.Resolve();
			if (line.Length == 0) return;
			SpeechPipeline.SpeakInterrupt(line);
			// The surveyed card's full text is one Ctrl+Up away, as for a focused one.
			if (Review.ReviewController.Instance != null) Review.ReviewController.Instance.Focus(cell.Details());
		}

		private static string DirectionWord(Direction dir) {
			switch (dir) {
				case Direction.Up: return Localization.Strings.DirUp;
				case Direction.Down: return Localization.Strings.DirDown;
				case Direction.Left: return Localization.Strings.DirLeft;
				default: return Localization.Strings.DirRight;
			}
		}
	}
}
