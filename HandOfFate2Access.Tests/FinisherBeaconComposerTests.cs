using HandOfFate2Access.Combat;
using Xunit;

namespace HandOfFate2Access.Tests {
	public class FinisherBeaconComposerTests {
		[Fact]
		public void Its_sample_is_not_another_beacons() {
			Assert.NotEqual(BeaconComposer.ChestKey, FinisherBeaconComposer.Key);
			Assert.NotEqual(BeaconComposer.ExitKey, FinisherBeaconComposer.Key);
			Assert.NotEqual(FinisherBeaconComposer.Key, FinisherBeaconComposer.ReadyKey);
		}

		[Fact]
		public void Nearer_enemy_pings_more_often_and_louder() {
			Assert.True(FinisherBeaconComposer.GapFor(5f) < FinisherBeaconComposer.GapFor(15f));
			Assert.True(FinisherBeaconComposer.VolumeFor(5f) > FinisherBeaconComposer.VolumeFor(15f));
		}

		[Fact]
		public void Within_a_swing_it_is_at_its_fastest_and_loudest() {
			Assert.Equal(FinisherBeaconComposer.NearPingGap, FinisherBeaconComposer.GapFor(RangingCurve.FullRange), 3);
			Assert.Equal(FinisherBeaconComposer.MaxVolume, FinisherBeaconComposer.VolumeFor(0f), 3);
		}

		[Fact]
		public void Far_away_it_is_faint_but_never_silent_and_still_quicker_than_the_exit() {
			Assert.Equal(FinisherBeaconComposer.MinVolume, FinisherBeaconComposer.VolumeFor(100f), 3);
			Assert.True(FinisherBeaconComposer.MinVolume > 0f);
			Assert.True(FinisherBeaconComposer.GapFor(100f) < BeaconComposer.GapFor(100f));
		}

		[Fact]
		public void Next_ping_waits_for_the_clip_then_the_gap() {
			Assert.Equal(10f + 0.32f / 0.5f + FinisherBeaconComposer.NearPingGap,
				FinisherBeaconComposer.NextPingTime(10f, 0.32f, 0.5f, 0f), 3);
		}

		[Fact]
		public void Compose_reports_the_ground_distance() {
			FinisherBeaconComposer.Compose(3f, 4f, out float distance);
			Assert.Equal(5f, distance, 3);
		}
	}
}
