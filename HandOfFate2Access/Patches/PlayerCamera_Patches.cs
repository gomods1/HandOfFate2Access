using System.Reflection;
using Defiant.Cameras;
using HandOfFate2Access.Settings;
using HarmonyLib;
using UnityEngine;

namespace HandOfFate2Access.Patches {
	/// <summary>
	/// The fight camera, steadied for listening. With the lock camera setting on, the
	/// player's orbit input (the camera keys, the right stick) is ignored and the orbit
	/// offset held at zero, so the camera keeps the arena's own angle and left and right
	/// mean the same for the whole fight; every mod cue and the game's own sounds are
	/// placed in that frame. With the hear from your character setting on, the game's
	/// listener sits on the player's character instead of part way toward the camera, so
	/// the game's own sounds (footsteps, swings, grunts) are heard from where the player
	/// stands. No game sound is changed or silenced.
	/// </summary>
	internal static class PlayerCamera_Patches {
		private static readonly FieldInfo OrbitField = AccessTools.Field(typeof(PlayerCamera), "m_orbitOffsetAnglesEuler");
		private static readonly FieldInfo ListenerShareField = AccessTools.Field(typeof(PlayerCamera), "m_listenerDistancePercentageToCamera");

		// The game's own share, restored when the setting is turned off.
		private static float _gameListenerShare = -1f;

		/// <summary>Prefix on PlayerCamera.UpdateOrbitRotationOffset: skipped while locked.</summary>
		private static bool OrbitPrefix(PlayerCamera __instance) {
			if (!ModSettings.LockCamera.On) return true;
			OrbitField.SetValue(__instance, Vector3.zero);
			return false;
		}

		/// <summary>Prefix on PlayerCamera.UpdateListener: places the listener by the setting.</summary>
		private static void ListenerPrefix(PlayerCamera __instance) {
			float share = (float)ListenerShareField.GetValue(__instance);
			if (ModSettings.ListenAtPlayer.On) {
				if (share != 0f) {
					_gameListenerShare = share;
					ListenerShareField.SetValue(__instance, 0f);
				}
			} else if (_gameListenerShare >= 0f && share == 0f) {
				ListenerShareField.SetValue(__instance, _gameListenerShare);
			}
		}
	}
}
