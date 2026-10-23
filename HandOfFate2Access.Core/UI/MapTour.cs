using System.Collections.Generic;

namespace HandOfFate2Access.UI {
	/// <summary>
	/// The order the map's unvisited cards are stepped through (Shift plus period and
	/// comma): nearest the player first by steps along the grid, ties read top to bottom,
	/// then left to right. Board rows grow downward. Recomputed on every press, so it
	/// follows the player as they move and drops a card once it is completed.
	/// </summary>
	public static class MapTour {
		public struct Cell {
			public readonly int X;
			public readonly int Y;

			public Cell(int x, int y) {
				X = x;
				Y = y;
			}
		}

		public static List<Cell> Order(IEnumerable<Cell> cells, int playerX, int playerY) {
			var order = new List<Cell>(cells);
			order.Sort((a, b) => {
				int da = Steps(a, playerX, playerY), db = Steps(b, playerX, playerY);
				if (da != db) return da.CompareTo(db);
				if (a.Y != b.Y) return a.Y.CompareTo(b.Y);
				return a.X.CompareTo(b.X);
			});
			return order;
		}

		/// <summary>
		/// The index <paramref name="step"/> (+1 next, -1 previous) away from
		/// <paramref name="current"/> in <paramref name="order"/>, wrapping at either end.
		/// With no current card in the order (the first press, or it was completed), next
		/// starts at the nearest and previous at the farthest. -1 for an empty order.
		/// </summary>
		public static int Step(List<Cell> order, Cell? current, int step) {
			if (order.Count == 0) return -1;
			int at = -1;
			if (current.HasValue)
				for (int i = 0; i < order.Count; i++)
					if (order[i].X == current.Value.X && order[i].Y == current.Value.Y) { at = i; break; }
			if (at < 0) return step > 0 ? 0 : order.Count - 1;
			return ((at + step) % order.Count + order.Count) % order.Count;
		}

		private static int Steps(Cell c, int x, int y) {
			int dx = c.X - x, dy = c.Y - y;
			return (dx < 0 ? -dx : dx) + (dy < 0 ? -dy : dy);
		}
	}
}
