using HarmonyLib;
using UnityEngine;

namespace HandOfFate2Access.Patches {
	/// <summary>
	/// On keyboard the game throws the dice only when the left mouse button is released
	/// after a drag (confirm switches it into its mouse-drag mode), so a keyboard player
	/// is stuck holding the dice. Confirm pressed without the mouse button throws them
	/// at once, as a controller's confirm does.
	/// </summary>
	internal static class DiceRoller_BeginDiceRoll_Patch {
		private static readonly System.Reflection.FieldInfo StateField = AccessTools.Field(typeof(DiceRoller), "m_state");

		private static bool Prefix(DiceRoller __instance) {
			if ((DiceRoller.State)StateField.GetValue(__instance) != DiceRoller.State.Catapault) return true;
			if (UnityEngine.Input.GetMouseButton(0) || UnityEngine.Input.GetMouseButtonDown(0)) return true;
			__instance.SetReady();
			return false;
		}
	}
}
