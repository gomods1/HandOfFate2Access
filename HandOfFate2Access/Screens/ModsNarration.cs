using System.Reflection;
using HandOfFate2Access.Focus;
using HandOfFate2Access.Speech;
using HandOfFate2Access.UI;
using HarmonyLib;

namespace HandOfFate2Access.Screens {
	/// <summary>
	/// Speaks the mods screen's inspect panel (a mod's title, version and description,
	/// with the game's warnings about a version mismatch, an error, a conflict or a
	/// missing dependency in front) when it opens or changes to another mod. Inspect
	/// does not move focus, so nothing else would read it.
	/// </summary>
	internal sealed class ModsNarration {
		private static readonly FieldInfo TitleField = AccessTools.Field(typeof(UIModInfo), "m_title");
		private static readonly FieldInfo VersionField = AccessTools.Field(typeof(UIModInfo), "m_version");
		private static readonly FieldInfo DescriptionField = AccessTools.Field(typeof(UIModInfo), "m_description");

		private readonly PanelFinder<UIModInfo> _panel = new PanelFinder<UIModInfo>();
		private readonly LabelWatch _description = new LabelWatch();

		/// <summary>True when the panel was spoken this frame.</summary>
		public bool Pump() {
			UIModInfo panel = _panel.Get();
			if (panel == null || !panel.gameObject.activeInHierarchy) {
				_description.Reset();
				return false;
			}
			string description = _description.Poll((UILabel)DescriptionField.GetValue(panel));
			if (description == null) return false;
			string text = new Message()
				.Add(UiText.Read((UILabel)TitleField.GetValue(panel)))
				.Add(UiText.Read((UILabel)VersionField.GetValue(panel)))
				.Add(description)
				.Resolve();
			SpeechPipeline.SpeakEvent(text);
			return true;
		}
	}
}
