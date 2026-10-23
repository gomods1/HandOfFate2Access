using System.Reflection;
using HandOfFate2Access.Focus;
using HandOfFate2Access.Speech;
using HandOfFate2Access.UI;
using HarmonyLib;

namespace HandOfFate2Access.Screens {
	/// <summary>
	/// Speaks the additional content screen's details panel (a pack's description and
	/// what it contains) each time the pack it shows changes. The panel follows the
	/// hovered row (AddonContentUI_Patches), so the details come right after the row's
	/// own readout, queued, and the next arrow cuts them short. Spoken once per pack
	/// per visit: the watch resets when the panel's header is no longer visible.
	/// </summary>
	internal sealed class AddonNarration {
		private static readonly FieldInfo CurrentItemField = AccessTools.Field(typeof(AddonContentUI), "m_currentItem");
		private static readonly FieldInfo HeaderField = AccessTools.Field(typeof(AddonContentUI), "m_contentHeader");

		private readonly PanelFinder<AddonContentUI> _panel = new PanelFinder<AddonContentUI>();
		private AddonContentUIItem _last;

		/// <summary>True when the panel was spoken this frame.</summary>
		public bool Pump() {
			AddonContentUI panel = _panel.Get();
			if (panel == null || !panel.gameObject.activeInHierarchy || !UiText.IsVisible((UILabel)HeaderField.GetValue(panel))) {
				_last = null;
				return false;
			}
			var item = (AddonContentUIItem)CurrentItemField.GetValue(panel);
			if (item == null || item == _last) return false;
			_last = item;
			string text = new Message()
				.Add(AddonReader.Resolve(item.Description, item.StringLibrary))
				.Add(AddonReader.Resolve(item.ContentsDescription, item.StringLibrary))
				.Resolve();
			if (text.Length == 0) return false;
			SpeechPipeline.SpeakEvent(text);
			return true;
		}
	}
}
