using HandOfFate2Access.Focus;
using UnityEngine;

namespace HandOfFate2Access.Patches {
	/// <summary>
	/// NGUI re-raycasts the resting mouse pointer every few frames, so when a list
	/// scrolls under a pointer nobody touched, the row that slides beneath it takes the
	/// focus (pressing Down in the controls list would land one row up). Ignore pointer
	/// hovers until the pointer actually moves after the last key or controller input.
	/// </summary>
	internal static class UISelectionManager_OnHover_Patch {
		private static bool Prefix(GameObject a_hitObject, bool a_over) {
			return !NavigationState.MouseIsStale;
		}
	}
}
