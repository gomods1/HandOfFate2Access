using HandOfFate2Access.Localization;
using HandOfFate2Access.Util;

namespace HandOfFate2Access.Localization {
	/// <summary>
	/// Keeps the mod's authored strings in the game's text language: polls
	/// LanguageManager.CurrentLanguage each frame and switches the table on change.
	/// </summary>
	internal sealed class LanguageWatcher {
		private string _last;

		public void Pump() {
			LanguageManager manager = LanguageManager.Instance;
			if (manager == null) return;
			string code = manager.CurrentLanguage;
			if (code == _last) return;
			_last = code;
			bool found = Strings.SetLanguage(code);
			Log.Info("language " + code + (found ? "" : " (no mod table; English)"));
		}
	}
}
