using System;
using HandOfFate2Access.Audio;

namespace HandOfFate2Access.Combat {
	/// <summary>
	/// Decides what the player hears for an enemy that can be finished (knocked down, the
	/// game's prone finisher can reach it): a rising swish that repeats from where the
	/// enemy lies, positioned by the shared placement law, with the beacons' grammar (the
	/// gap of silence after each ping is the distance, tightening as the player closes
	/// in; volume floors rather than fading out). Its cadence is quicker than the walk-in
	/// beacons': an enemy stays down for only a few seconds, so even a far one must be
	/// heard more than once before it gets up. Once the finisher would land on it (in
	/// reach, in the direction the player pushes or faces) the swish carries a bright
	/// ding on top (<see cref="ReadyKey"/>): press Finish now. The locator key plays the
	/// same sounds at the nearest enemy when that one can be finished.
	/// </summary>
	public static class FinisherBeaconComposer {
		/// <summary>Sample key (and sounds-folder file stem).</summary>
		public const string Key = "beacon_finish";

		/// <summary>Sample key for the same enemy once the finisher would land on it.</summary>
		public const string ReadyKey = "beacon_finish_ready";

		/// <summary>Seconds of silence between pings with the enemy within a swing.</summary>
		public const float NearPingGap = 0.12f;

		/// <summary>Seconds of silence between pings at <see cref="RangingCurve.FloorRange"/>
		/// and beyond.</summary>
		public const float FarPingGap = 0.7f;

		/// <summary>Loudness within a swing: an opportunity, under the attack telegraphs.</summary>
		public const float MaxVolume = 0.8f;

		/// <summary>Loudness at and beyond <see cref="RangingCurve.FloorRange"/>: faint, never silent.</summary>
		public const float MinVolume = 0.15f;

		/// <summary>The voice parameters for an enemy <paramref name="right"/> world units to
		/// the camera's right and <paramref name="forward"/> toward screen-north of the
		/// player, and its ground <paramref name="distance"/>.</summary>
		public static SoundParams Compose(float right, float forward, out float distance) {
			distance = (float)Math.Sqrt(right * right + forward * forward);
			return Spatializer.Place(right, forward, VolumeFor(distance));
		}

		/// <summary>When a ping played at <paramref name="now"/> should ping again: the clip's
		/// length at the played <paramref name="pitch"/>, then <see cref="GapFor"/> in silence.</summary>
		public static float NextPingTime(float now, float clipDuration, float pitch, float distance) =>
			now + clipDuration / pitch + GapFor(distance);

		public static float GapFor(float distance) =>
			FarPingGap + (NearPingGap - FarPingGap) * RangingCurve.Closeness(distance);

		public static float VolumeFor(float distance) =>
			MinVolume + (MaxVolume - MinVolume) * RangingCurve.Closeness(distance);
	}
}
