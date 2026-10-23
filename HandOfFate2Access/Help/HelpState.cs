namespace HandOfFate2Access.Help {
	/// <summary>
	/// Whether the help list is open. Read by the input patches (while it is open the
	/// game's UI input is not processed at all) and by the other mod keys, which stand
	/// aside until it closes.
	/// </summary>
	internal static class HelpState {
		public static bool Open;
	}
}
