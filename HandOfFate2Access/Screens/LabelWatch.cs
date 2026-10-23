using HandOfFate2Access.Focus;

namespace HandOfFate2Access.Screens {
	/// <summary>
	/// Edge detector over one live UILabel: <see cref="Poll"/> returns the label's text
	/// the first frame it becomes visible with new content, and null otherwise. A label
	/// that hides or empties resets, so the same text appearing again is spoken again.
	/// Holds no copy of game data beyond the last text, used only for the edge.
	/// </summary>
	internal sealed class LabelWatch {
		private string _last;
		private readonly bool _ignoreFade;

		/// <param name="ignoreFade">Treat the label as visible while its panel is still
		/// fading in (alpha near zero), so text is spoken as soon as it is set, ahead of
		/// the focus that lands on the panel's buttons in the same frames.</param>
		public LabelWatch(bool ignoreFade = false) {
			_ignoreFade = ignoreFade;
		}

		public string Poll(UILabel label) {
			bool visible = label != null && (_ignoreFade
				? label.enabled && label.gameObject.activeInHierarchy
				: UiText.IsVisible(label));
			string text = visible ? label.text : null;
			if (text == _last) return null;
			_last = text;
			return string.IsNullOrEmpty(text) ? null : text;
		}

		public void Reset() => _last = null;
	}
}
