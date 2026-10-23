using HandOfFate2Access.Audio;

namespace HandOfFate2Access.Combat {
	/// <summary>The armour events the player hears.</summary>
	public enum ArmourEvent {
		/// <summary>A hit glanced off the armour (only Bash damages it).</summary>
		Absorbed,
		/// <summary>A Bash damaged the armour without breaking it.</summary>
		Cracked,
		/// <summary>The armour broke: hits land in full again.</summary>
		Broken,
		/// <summary>Damaged armour mending a step.</summary>
		Mending,
		/// <summary>Broken armour back whole.</summary>
		Restored,
	}

	/// <summary>
	/// Decides what the player hears for an enemy's armour: a sample per event, played
	/// from the armoured enemy by the shared placement law (they are within a swing when
	/// it happens, so the bearing says which enemy). Mending plays the restore sample
	/// quietly, so armour regrowing a step at a time while the player stops bashing is
	/// heard without the weight of armour coming back whole.
	/// </summary>
	public static class ArmourCueComposer {
		public const string AbsorbedKey = "armour_absorbed";
		public const string CrackKey = "armour_crack";
		public const string BrokenKey = "armour_broken";
		public const string RestoredKey = "armour_restored";

		/// <summary>Loudness of every armour event but mending: feedback on the player's own blows.</summary>
		public const float Volume = 0.9f;

		/// <summary>Loudness of a mending step.</summary>
		public const float MendingVolume = 0.35f;

		public static readonly string[] Keys = { AbsorbedKey, CrackKey, BrokenKey, RestoredKey };

		public static string KeyFor(ArmourEvent e) {
			switch (e) {
				case ArmourEvent.Absorbed: return AbsorbedKey;
				case ArmourEvent.Cracked: return CrackKey;
				case ArmourEvent.Broken: return BrokenKey;
				default: return RestoredKey;
			}
		}

		public static float VolumeFor(ArmourEvent e) => e == ArmourEvent.Mending ? MendingVolume : Volume;

		/// <summary>The voice parameters for an event on an enemy <paramref name="right"/> world
		/// units to the camera's right and <paramref name="forward"/> toward screen-north.</summary>
		public static SoundParams Compose(ArmourEvent e, float right, float forward) =>
			Spatializer.Place(right, forward, VolumeFor(e));
	}
}
