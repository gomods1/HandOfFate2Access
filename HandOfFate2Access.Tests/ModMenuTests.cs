using System.Collections.Generic;
using HandOfFate2Access.Audio;
using HandOfFate2Access.Combat;
using HandOfFate2Access.Gambit;
using HandOfFate2Access.Glossary;
using HandOfFate2Access.Help;
using HandOfFate2Access.Localization;
using HandOfFate2Access.ModMenu;
using HandOfFate2Access.Settings;
using HandOfFate2Access.UI;
using Xunit;

namespace HandOfFate2Access.Tests {
	public class SettingTests : System.IDisposable {
		public SettingTests() {
			Strings.ResetForTest();
			ModSettings.ResetForTest();
		}

		public void Dispose() {
			ModSettings.ResetForTest();
		}

		[Fact]
		public void VolumeStepsByTenAndStopsAtItsEnds() {
			Setting volume = ModSettings.WallVolume;
			Assert.Equal(100, volume.Value);
			Assert.False(volume.Step(1));
			Assert.True(volume.Step(-1));
			Assert.Equal(90, volume.Value);
			for (int i = 0; i < 20; i++) volume.Step(-1);
			Assert.Equal(0, volume.Value);
			Assert.Equal(Strings.SettingVolumeOff, volume.Spoken());
		}

		[Fact]
		public void HandEditedVolumeSnapsToTheStep() {
			ModSettings.WallVolume.Value = 73;
			Assert.Equal(70, ModSettings.WallVolume.Value);
			ModSettings.WallVolume.Value = 250;
			Assert.Equal(100, ModSettings.WallVolume.Value);
		}

		[Fact]
		public void ToggleFlipsEitherWay() {
			Setting hints = ModSettings.ChanceCardHints;
			Assert.True(hints.On);
			hints.Step(-1);
			Assert.False(hints.On);
			Assert.Equal(Strings.SettingOff, hints.Spoken());
			hints.Step(1);
			Assert.True(hints.On);
		}

		[Fact]
		public void ChangedFiresOnlyOnARealChange() {
			int fired = 0;
			System.Action<Setting> count = s => fired++;
			ModSettings.HazardVolume.Changed += count;
			try {
				ModSettings.HazardVolume.Value = 100;
				ModSettings.HazardVolume.Value = 50;
				Assert.Equal(1, fired);
			} finally {
				ModSettings.HazardVolume.Changed -= count;
			}
		}

		[Fact]
		public void GainIsTheOverallVolumeTimesTheFamilys() {
			ModSettings.Volume.Value = 50;
			ModSettings.ProjectileVolume.Value = 40;
			Assert.Equal(0.2f, ModSettings.Gain(SoundCategory.Projectiles), 4);
			Assert.Equal(0.5f, ModSettings.Gain(SoundCategory.Interface), 4);
		}

		[Fact]
		public void EverySoundFamilyButInterfaceHasAVolume() {
			foreach (SoundCategory category in System.Enum.GetValues(typeof(SoundCategory)))
				Assert.Equal(category != SoundCategory.Interface, ModSettings.VolumeOf(category) != null);
		}

		[Fact]
		public void ConfigKeysAreUnique() {
			var seen = new HashSet<string>();
			foreach (Setting s in ModSettings.All)
				Assert.True(seen.Add(s.Section + "." + s.Key), s.Section + "." + s.Key);
		}
	}

	public class MenuNavigatorTests : System.IDisposable {
		public MenuNavigatorTests() {
			Strings.ResetForTest();
			ModSettings.ResetForTest();
		}

		public void Dispose() {
			ModSettings.ResetForTest();
		}

		private static MenuItem Root(Setting setting, List<string> played) {
			return MenuItem.Section(() => "Mod", () => "Hint.", () => new List<MenuItem> {
				MenuItem.Section(() => "Sounds", null, () => new List<MenuItem> {
					MenuItem.Action(() => "Clang", () => played.Add("clang")),
					MenuItem.Action(() => "Crack", () => played.Add("crack")),
				}),
				MenuItem.Topic(() => "Topic", () => new List<string> { "One", "Two" }),
				MenuItem.ForSetting(setting),
			});
		}

		[Fact]
		public void OpeningSaysTitleHintAndFirstEntry() {
			var nav = new MenuNavigator();
			Assert.Equal("Mod. Hint. Sounds", nav.Open(Root(ModSettings.WallVolume, new List<string>())));
		}

		[Fact]
		public void MenusWrapAndTopicsStopAtTheirEnds() {
			var nav = new MenuNavigator();
			nav.Open(Root(ModSettings.WallVolume, new List<string>()));
			Assert.StartsWith(Strings.SettingWallVolume, nav.Move(-1));
			Assert.Equal("Sounds", nav.Move(1));
			nav.Move(1);
			Assert.Equal("Topic. One", nav.Enter(out _));
			Assert.Equal(Strings.ReviewTop, nav.Move(-1));
			Assert.Equal("Two", nav.Move(1));
			Assert.Equal(Strings.ReviewEnd, nav.Move(1));
		}

		[Fact]
		public void EnterHandsBackAnActionToCarryOut() {
			var played = new List<string>();
			var nav = new MenuNavigator();
			nav.Open(Root(ModSettings.WallVolume, played));
			nav.Enter(out _);
			nav.Move(1);
			Assert.Null(nav.Enter(out MenuItem activated));
			activated.Activate();
			Assert.Equal(new List<string> { "crack" }, played);
		}

		[Fact]
		public void LeftAndRightChangeASettingAndSayItsValue() {
			var nav = new MenuNavigator();
			nav.Open(Root(ModSettings.WallVolume, new List<string>()));
			nav.Jump(true);
			Assert.Equal(Strings.Percent(90), nav.Adjust(-1));
			Assert.Equal(90, ModSettings.WallVolume.Value);
			Assert.Equal(Strings.SettingWallVolume + ", " + Strings.Percent(90), nav.Current.Spoken);
		}

		[Fact]
		public void BackReturnsToTheSectionThenCloses() {
			var nav = new MenuNavigator();
			nav.Open(Root(ModSettings.WallVolume, new List<string>()));
			nav.Enter(out _);
			Assert.Equal("Sounds", nav.Back(out bool closed));
			Assert.False(closed);
			nav.Back(out closed);
			Assert.True(closed);
			Assert.False(nav.IsOpen);
		}
	}

	public class SoundCatalogTests : System.IDisposable {
		public SoundCatalogTests() {
			Strings.ResetForTest();
		}

		public void Dispose() {
			Strings.ResetForTest();
		}

		[Fact]
		public void CombatHelpNamesEveryCombatSoundByItsExampleLabel() {
			List<string> entries = HelpCatalog.ScreenEntries(HelpContext.Combat, true, null);
			foreach (string label in new[] { Strings.GlossaryBlock, Strings.GlossaryDodge, Strings.GlossaryEnemyStep, Strings.GlossaryEnemyReach })
				Assert.Contains(label, entries);
		}

		[Fact]
		public void CardInstrumentsRepeatAnOctaveUpPastFour() {
			Assert.Equal("card_harp", GambitSounds.CardKey(0));
			Assert.Equal("card_harp", GambitSounds.CardKey(4));
			Assert.Equal(1f, GambitSounds.CardPitch(3));
			Assert.Equal(2f, GambitSounds.CardPitch(4));
		}

		[Fact]
		public void ShuffleExampleTakesTurnsAndCrossesTheHarpOver() {
			GlossaryEntry shuffle = null;
			foreach (GlossaryEntry e in GlossaryCatalog.MinigameEntries)
				if (e.Label == Strings.GlossaryShuffle) shuffle = e;
			Assert.NotNull(shuffle);
			GlossaryStep first = shuffle.Steps[0], lastHarp = shuffle.Steps[shuffle.Steps.Length - 4];
			Assert.Equal("card_harp", first.ClipKey);
			Assert.Equal(-1f, first.Params.Pan);
			Assert.Equal(1f, lastHarp.Params.Pan);
			Assert.Equal(GambitLayout.TurnSeconds, first.HoldSeconds);
		}

		[Fact]
		public void AMoveIsReadFromThePlayerWithoutItsOwnPaths() {
			var info = new MapCellInfo(null, null, false, false, true, true, new MapExits(false, false, true, false),
				new List<string> { Strings.CardTokenToWin });
			Assert.Equal("right, face down card, has a token, exit", new MapCellElement(info, Strings.DirRight).Describe().Resolve());
			Assert.Equal("face down card, has a token, reachable, exit, paths left", new MapCellElement(info).Describe().Resolve());
		}

		[Fact]
		public void MissingTextPlaceholdersAreNeverSpoken() {
			Assert.True(Placeholder.IsMissing("****"));
			Assert.True(Placeholder.IsMissing("**7b79f6b0-3291-44d4-a259-534e557cbf1c**"));
			Assert.False(Placeholder.IsMissing("Supplies"));
			Assert.Equal("Supplies, 3 Food", new Message().Add("**7b79f6b0-3291-44d4-a259-534e557cbf1c**").Add("Supplies").Add("3 Food").Resolve());
		}

		[Fact]
		public void MapOffsetSaysWhereACardLiesFromThePlayer() {
			Assert.Equal("2 left, 1 up from you", MapOffset.Describe(-2, -1));
			Assert.Equal("3 down from you", MapOffset.Describe(0, 3));
			Assert.Null(MapOffset.Describe(0, 0));
		}
	}

	public class EnemyPresenceTests {
		private static EnemyPresenceTracker.Enemy At(int id, float x, float z) {
			return new EnemyPresenceTracker.Enemy { Id = id, WorldX = x, WorldZ = z, Right = x, Forward = z };
		}

		[Fact]
		public void AStandingEnemyIsSilent() {
			var tracker = new EnemyPresenceTracker();
			tracker.Update(new[] { At(1, 10, 0) }, 0f, 0.1f);
			Assert.Empty(tracker.Update(new[] { At(1, 10, 0) }, 0.1f, 0.1f));
		}

		[Fact]
		public void AMovingEnemyStepsAtWalkingCadence() {
			var tracker = new EnemyPresenceTracker();
			tracker.Update(new[] { At(1, 10, 0) }, 0f, 0.1f);
			var first = tracker.Update(new[] { At(1, 10, 1) }, 0.1f, 0.1f);
			Assert.Single(first);
			Assert.Equal(EnemyPresenceTracker.StepKey, first[0].Key);
			Assert.True(first[0].Params.Pan > 0f);
			Assert.Empty(tracker.Update(new[] { At(1, 10, 2) }, 0.2f, 0.1f));
			Assert.Single(tracker.Update(new[] { At(1, 10, 6) }, 0.1f + EnemyPresenceTracker.StepSeconds, 0.1f));
		}

		[Fact]
		public void OnlyTheNearestFewStep() {
			var tracker = new EnemyPresenceTracker();
			var start = new List<EnemyPresenceTracker.Enemy>();
			var moved = new List<EnemyPresenceTracker.Enemy>();
			for (int i = 0; i < 6; i++) {
				start.Add(At(i, 5 + i, 0));
				moved.Add(At(i, 5 + i, 1));
			}
			tracker.Update(start, 0f, 0.1f);
			Assert.Equal(EnemyPresenceTracker.MaxStepping, tracker.Update(moved, 0.1f, 0.1f).Count);
		}

		[Fact]
		public void ComingIntoReachKnocksOnceUntilTheEnemyBacksOff() {
			var tracker = new EnemyPresenceTracker();
			tracker.Update(new[] { At(1, 6, 0) }, 0f, 0.5f);
			var cues = tracker.Update(new[] { At(1, 2, 0) }, 0.5f, 0.5f);
			Assert.Contains(cues, c => c.Key == EnemyPresenceTracker.ReachKey);
			Assert.DoesNotContain(tracker.Update(new[] { At(1, 3.5f, 0) }, 1f, 0.5f), c => c.Key == EnemyPresenceTracker.ReachKey);
			Assert.DoesNotContain(tracker.Update(new[] { At(1, 2, 0) }, 1.5f, 0.5f), c => c.Key == EnemyPresenceTracker.ReachKey);
			tracker.Update(new[] { At(1, 8, 0) }, 2f, 0.5f);
			Assert.Contains(tracker.Update(new[] { At(1, 2, 0) }, 2.5f, 0.5f), c => c.Key == EnemyPresenceTracker.ReachKey);
		}
	}
}
