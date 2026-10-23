namespace HandOfFate2Access.Maps {
	/// <summary>
	/// Whether Shift+arrows belong to the map cursor this frame. Shared by the cursor's
	/// binding and the direction patch that stops the game's own selection from also
	/// moving on those keys.
	/// </summary>
	internal static class MapInput {
		public static bool OnMap;

		public static bool CursorKeysHeld => OnMap && Input.KeyClaims.ShiftHeld;
	}
}
