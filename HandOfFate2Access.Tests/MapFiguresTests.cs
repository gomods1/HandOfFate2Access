using HandOfFate2Access.UI;
using Xunit;

namespace HandOfFate2Access.Tests {
	/// <summary>The Servant challenge's goblins: named from their ids, and which card each steps onto after the player's move.</summary>
	public class MapFiguresTests {
		private static bool Open(int x1, int y1, int x2, int y2) => true;
		private static bool Walled(int x1, int y1, int x2, int y2) => false;

		[Theory]
		[InlineData("Spice_DLC_Goblin_path_A", FigureKind.GoblinPatrol)]
		[InlineData("Spice_DLC_Goblin_path_Multi_v2", FigureKind.GoblinPatrol)]
		[InlineData("Spice_DLC_Goblin_path", FigureKind.GoblinPatrol)]
		[InlineData("Spice_DLC_Goblin_block", FigureKind.GoblinGuard)]
		[InlineData("Spice_DLC_Goblin_track", FigureKind.GoblinTracker)]
		[InlineData("Spice_DLC_Goblin_generic", FigureKind.Goblin)]
		[InlineData("Spice_Food_3", FigureKind.Other)]
		[InlineData(null, FigureKind.Other)]
		public void Kind_from_id(string id, FigureKind kind) {
			Assert.Equal(kind, MapFigures.KindOf(id));
		}

		[Fact]
		public void A_patrol_enters_the_card_it_faces() {
			var patrol = new MapFigures.Figure(FigureKind.GoblinPatrol, 1, 2, 0, -1);
			Assert.True(MapFigures.Enters(patrol, 1, 1, Open));
			Assert.False(MapFigures.Enters(patrol, 1, 3, Open));
			Assert.False(MapFigures.Enters(patrol, 2, 2, Open));
		}

		[Fact]
		public void A_patrol_facing_nowhere_known_enters_nothing() {
			var patrol = new MapFigures.Figure(FigureKind.GoblinPatrol, 1, 2);
			Assert.False(MapFigures.Enters(patrol, 1, 1, Open));
		}

		[Fact]
		public void No_goblin_enters_its_own_card() {
			var patrol = new MapFigures.Figure(FigureKind.GoblinTracker, 1, 2);
			Assert.False(MapFigures.Enters(patrol, 1, 2, Open));
		}

		[Fact]
		public void A_guard_steps_up_or_down_only() {
			var guard = new MapFigures.Figure(FigureKind.GoblinGuard, 3, 2);
			Assert.True(MapFigures.Enters(guard, 3, 1, Open));
			Assert.True(MapFigures.Enters(guard, 3, 3, Open));
			Assert.False(MapFigures.Enters(guard, 2, 2, Open));
			Assert.False(MapFigures.Enters(guard, 3, 4, Open));
			Assert.False(MapFigures.Enters(guard, 3, 1, Walled));
		}

		[Fact]
		public void A_tracker_enters_a_neighbouring_card_with_a_path() {
			var tracker = new MapFigures.Figure(FigureKind.GoblinTracker, 2, 2);
			Assert.True(MapFigures.Enters(tracker, 1, 2, Open));
			Assert.True(MapFigures.Enters(tracker, 2, 3, Open));
			Assert.False(MapFigures.Enters(tracker, 1, 1, Open));
			Assert.False(MapFigures.Enters(tracker, 4, 2, Open));
			Assert.False(MapFigures.Enters(tracker, 1, 2, Walled));
		}

		[Fact]
		public void A_still_goblin_enters_nothing() {
			var goblin = new MapFigures.Figure(FigureKind.Goblin, 2, 2);
			Assert.False(MapFigures.Enters(goblin, 1, 2, Open));
		}

		[Fact]
		public void A_patrol_is_named_with_its_heading() {
			Assert.Equal("goblin patrol heading left", MapFigures.Describe(new MapFigures.Figure(FigureKind.GoblinPatrol, 1, 1, -1, 0)));
			Assert.Equal("goblin patrol", MapFigures.Describe(new MapFigures.Figure(FigureKind.GoblinPatrol, 1, 1)));
			Assert.Null(MapFigures.Describe(new MapFigures.Figure(FigureKind.Other, 1, 1)));
		}
	}
}
