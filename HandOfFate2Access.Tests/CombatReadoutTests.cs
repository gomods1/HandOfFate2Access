using HandOfFate2Access.Combat;
using HandOfFate2Access.Localization;
using Xunit;

namespace HandOfFate2Access.Tests {
	public class CombatReadoutTests : System.IDisposable {
		public CombatReadoutTests() { Strings.ResetForTest(); }
		public void Dispose() { Strings.ResetForTest(); }

		[Fact]
		public void FightStartCountsEnemies() {
			Assert.Equal("Fight, 3 enemies", CombatReadout.FightStart(3));
			Assert.Equal("Fight, 1 enemy", CombatReadout.FightStart(1));
			Assert.Equal("Fight", CombatReadout.FightStart(0));
		}

		[Fact]
		public void LastEnemyDownSaysNoneLeft() {
			Assert.Equal("no enemies left", CombatReadout.EnemiesLeft(0));
			Assert.Equal("2 enemies", CombatReadout.EnemiesLeft(2));
		}

		[Fact]
		public void ThresholdOnlyWhenCrossedDownward() {
			Assert.Equal(0, CombatReadout.CrossedThreshold(0.6f, 0.45f));
			Assert.Equal(2, CombatReadout.CrossedThreshold(0.6f, 0.05f));
			Assert.Equal(-1, CombatReadout.CrossedThreshold(0.45f, 0.4f));
			Assert.Equal(-1, CombatReadout.CrossedThreshold(0.2f, 0.6f));
		}
	}
}

namespace HandOfFate2Access.Tests {
	public class ArtDescriptionTests : System.IDisposable {
		public ArtDescriptionTests() { HandOfFate2Access.Localization.Strings.ResetForTest(); }
		public void Dispose() { HandOfFate2Access.Localization.Strings.ResetForTest(); }

		[Xunit.Fact]
		public void ReadsTheGamesAssetReference() {
			Xunit.Assert.Equal("braided crest with a ponytail", HandOfFate2Access.Localization.ArtDescriptions.Describe(
				"cardart:Art/3DUI/Cards/Textures/Customisation/c_cus_f_hair_ponytail.png:4879b6b0971118e4dad74a0d0e63bed9"));
			Xunit.Assert.Equal("purple and cream with dark red", HandOfFate2Access.Localization.ArtDescriptions.Describe("c_cus_colour_3_crusade"));
			Xunit.Assert.Null(HandOfFate2Access.Localization.ArtDescriptions.Describe("cardart:Art/x/c_cus_unknown.png:1"));
		}
	}
}
