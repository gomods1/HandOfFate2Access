using HandOfFate2Access.Input;
using UnityEngine;

namespace HandOfFate2Access.Patches {
	/// <summary>
	/// Every directional UI input the game acts on comes out of
	/// UIInputManager.GetDirection (the selection manager, the score table and the deck
	/// builder's bars all subscribe to the navigate event it feeds). While the mod
	/// claims the arrows (review, the map cursor, the help list) the direction reads as
	/// none, so the game's selection does not move under the mod's keys.
	/// </summary>
	internal static class UIInputManager_GetDirection_Patch {
		private static void Postfix(ref Vector2 __result) {
			if (__result != Vector2.zero && KeyClaims.ArrowsClaimed)
				__result = Vector2.zero;
		}
	}
}
