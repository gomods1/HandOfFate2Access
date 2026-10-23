using System;
using HandOfFate2Access.Localization;

namespace HandOfFate2Access.UI {
	/// <summary>The kinds of figure the mod can name; every other figure is "a figure".</summary>
	public enum FigureKind {
		Other,
		/// <summary>Walks a fixed route, one card per player step, facing the card it enters next.</summary>
		GoblinPatrol,
		/// <summary>Steps one card up or down toward the player's row after each player step.</summary>
		GoblinGuard,
		/// <summary>Steps one card along the shortest path toward the player after each player step.</summary>
		GoblinTracker,
		/// <summary>Stands still and catches no one.</summary>
		Goblin
	}

	/// <summary>
	/// The goblin figures of the Servant challenge (DLC_00_Shadow), named from their spice
	/// ids, and where each moves after the player's step. Every goblin that moves does so
	/// once the player's counter arrives (the game's travel-end event), and one that ends on
	/// the player's card catches them, as does a card the player steps onto with a goblin on
	/// it; neither happens in the shadow realm (the challenge's "night"), where no goblin
	/// moves. Board rows grow downward.
	/// </summary>
	public static class MapFigures {
		public struct Figure {
			public readonly FigureKind Kind;
			public readonly int X;
			public readonly int Y;
			/// <summary>The grid step the figure faces; (0, 0) when unknown.</summary>
			public readonly int FacingX;
			public readonly int FacingY;

			public Figure(FigureKind kind, int x, int y, int facingX = 0, int facingY = 0) {
				Kind = kind;
				X = x;
				Y = y;
				FacingX = facingX;
				FacingY = facingY;
			}
		}

		public static FigureKind KindOf(string id) {
			if (string.IsNullOrEmpty(id) || !id.StartsWith("Spice_DLC_Goblin", StringComparison.Ordinal)) return FigureKind.Other;
			if (id.StartsWith("Spice_DLC_Goblin_path", StringComparison.Ordinal)) return FigureKind.GoblinPatrol;
			if (id == "Spice_DLC_Goblin_block") return FigureKind.GoblinGuard;
			if (id == "Spice_DLC_Goblin_track") return FigureKind.GoblinTracker;
			return FigureKind.Goblin;
		}

		/// <summary>
		/// Whether <paramref name="figure"/> steps onto (x, y) when the player has just moved
		/// there. A patrol enters the card it faces; a guard the next card up or down toward
		/// the player's row; a tracker the player's card when it is next to it.
		/// <paramref name="connected"/> says whether two neighbouring cards have a path
		/// between them (both dealt, no wall).
		/// </summary>
		public static bool Enters(Figure figure, int x, int y, Func<int, int, int, int, bool> connected) {
			if (figure.X == x && figure.Y == y) return false;
			int dx = x - figure.X, dy = y - figure.Y;
			bool next = (dx == 0 && (dy == 1 || dy == -1)) || (dy == 0 && (dx == 1 || dx == -1));
			switch (figure.Kind) {
				case FigureKind.GoblinPatrol:
					return (figure.FacingX != 0 || figure.FacingY != 0) && dx == figure.FacingX && dy == figure.FacingY;
				case FigureKind.GoblinGuard:
					return dx == 0 && next && connected(figure.X, figure.Y, x, y);
				case FigureKind.GoblinTracker:
					return next && connected(figure.X, figure.Y, x, y);
				default:
					return false;
			}
		}

		/// <summary>The figure's spoken name, a patrol with the way it faces; null for <see cref="FigureKind.Other"/>.</summary>
		public static string Describe(Figure figure) {
			switch (figure.Kind) {
				case FigureKind.GoblinPatrol:
					string facing = Direction(figure.FacingX, figure.FacingY);
					return facing != null ? Strings.MapGoblinPatrolFacing(facing) : Strings.MapGoblinPatrol;
				case FigureKind.GoblinGuard: return Strings.MapGoblinGuard;
				case FigureKind.GoblinTracker: return Strings.MapGoblinTracker;
				case FigureKind.Goblin: return Strings.MapGoblin;
				default: return null;
			}
		}

		public static string Direction(int dx, int dy) {
			if (dx == 0 && dy == -1) return Strings.DirUp;
			if (dx == 0 && dy == 1) return Strings.DirDown;
			if (dx == -1 && dy == 0) return Strings.DirLeft;
			if (dx == 1 && dy == 0) return Strings.DirRight;
			return null;
		}
	}
}
