using System.Linq;
using HandOfFate2Access.Combat;
using Xunit;

namespace HandOfFate2Access.Tests {
	public class ArmourCueComposerTests {
		[Fact]
		public void Each_event_but_mending_has_its_own_sample() {
			string[] keys = {
				ArmourCueComposer.KeyFor(ArmourEvent.Absorbed),
				ArmourCueComposer.KeyFor(ArmourEvent.Cracked),
				ArmourCueComposer.KeyFor(ArmourEvent.Broken),
				ArmourCueComposer.KeyFor(ArmourEvent.Restored),
			};
			Assert.Equal(keys.Length, keys.Distinct().Count());
			Assert.All(keys, k => Assert.Contains(k, ArmourCueComposer.Keys));
		}

		[Fact]
		public void Mending_is_the_restore_sample_played_quietly() {
			Assert.Equal(ArmourCueComposer.RestoredKey, ArmourCueComposer.KeyFor(ArmourEvent.Mending));
			Assert.True(ArmourCueComposer.VolumeFor(ArmourEvent.Mending) < ArmourCueComposer.VolumeFor(ArmourEvent.Restored));
		}

		[Fact]
		public void Cues_come_from_the_enemys_side() {
			Assert.True(ArmourCueComposer.Compose(ArmourEvent.Cracked, 2f, 0f).Pan > 0f);
			Assert.True(ArmourCueComposer.Compose(ArmourEvent.Cracked, -2f, 0f).Pan < 0f);
		}

		[Fact]
		public void None_of_its_samples_is_a_beacons() {
			Assert.DoesNotContain(FinisherBeaconComposer.Key, ArmourCueComposer.Keys);
			Assert.DoesNotContain(BeaconComposer.ExitKey, ArmourCueComposer.Keys);
		}
	}
}
