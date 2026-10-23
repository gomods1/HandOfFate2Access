using UnityEngine;

namespace HandOfFate2Access.Input {
	/// <summary>
	/// Whether the arrow keys belong to the mod this frame, so the game's own
	/// navigation must not also move: Ctrl+arrows are the review keys everywhere,
	/// Shift+arrows the map's survey cursor, and plain arrows the help list or the mod
	/// menu while one is open. Read from the physical keys, so the frame that opens a mode is claimed
	/// whatever order the game and the mod run in.
	/// </summary>
	internal static class KeyClaims {
		public static bool CtrlHeld =>
			UnityEngine.Input.GetKey(KeyCode.LeftControl) || UnityEngine.Input.GetKey(KeyCode.RightControl);

		public static bool ShiftHeld =>
			UnityEngine.Input.GetKey(KeyCode.LeftShift) || UnityEngine.Input.GetKey(KeyCode.RightShift);

		public static bool ArrowsClaimed =>
			CtrlHeld || Maps.MapInput.CursorKeysHeld || Help.HelpState.Open || ModMenu.ModMenuState.Open;
	}
}
