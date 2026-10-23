using System.Collections.Generic;
using HandOfFate2Access.UI;
using Xunit;

namespace HandOfFate2Access.Tests {
	/// <summary>The unvisited cards' order: nearest first, ties top to bottom then left to right; stepping wraps.</summary>
	public class MapTourTests {
		private static MapTour.Cell C(int x, int y) => new MapTour.Cell(x, y);

		[Fact]
		public void Nearest_first_then_top_to_bottom_then_left_to_right() {
			List<MapTour.Cell> order = MapTour.Order(new[] { C(4, 4), C(3, 2), C(1, 2), C(2, 1), C(2, 3) }, 2, 2);
			Assert.Equal(new[] { C(2, 1), C(1, 2), C(3, 2), C(2, 3), C(4, 4) }, order);
		}

		[Fact]
		public void Next_wraps_from_last_to_first() {
			var order = new List<MapTour.Cell> { C(0, 0), C(1, 0), C(2, 0) };
			Assert.Equal(1, MapTour.Step(order, C(0, 0), 1));
			Assert.Equal(0, MapTour.Step(order, C(2, 0), 1));
		}

		[Fact]
		public void Previous_wraps_from_first_to_last() {
			var order = new List<MapTour.Cell> { C(0, 0), C(1, 0), C(2, 0) };
			Assert.Equal(2, MapTour.Step(order, C(0, 0), -1));
		}

		[Fact]
		public void Without_a_current_card_next_starts_nearest_and_previous_farthest() {
			var order = new List<MapTour.Cell> { C(0, 0), C(1, 0), C(2, 0) };
			Assert.Equal(0, MapTour.Step(order, null, 1));
			Assert.Equal(2, MapTour.Step(order, null, -1));
			Assert.Equal(0, MapTour.Step(order, C(9, 9), 1));
		}

		[Fact]
		public void Empty_order_has_no_step() {
			Assert.Equal(-1, MapTour.Step(new List<MapTour.Cell>(), null, 1));
		}
	}
}
