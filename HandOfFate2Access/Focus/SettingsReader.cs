using System.Collections.Generic;
using System.Reflection;
using HandOfFate2Access.Localization;
using HandOfFate2Access.Screens;
using HandOfFate2Access.UI;
using HarmonyLib;
using UnityEngine;

namespace HandOfFate2Access.Focus {
	/// <summary>
	/// Reads a row of the pause menu's settings screens (a PauseMenuControl): its
	/// name and its current value, whichever part of the row holds focus (a choice
	/// row's left and right arrows are selectables of their own). The value comes from
	/// what the row shows: a choice row's value label, a checkbox's state, a slider's
	/// number, a button's caption, a key binding's key names (or the "press a key"
	/// prompt while it waits for one).
	/// </summary>
	internal static class SettingsReader {
		private static readonly FieldInfo FieldLabelField = AccessTools.Field(typeof(PauseMenuControl), "m_fieldLabel");
		private static readonly FieldInfo DescriptionLabelField = AccessTools.Field(typeof(PauseMenuControl), "m_descriptionLabel");
		private static readonly FieldInfo ButtonLabelField = AccessTools.Field(typeof(ButtonControl), "m_buttonLabel");
		private static readonly FieldInfo PrimaryBindingField = AccessTools.Field(typeof(UserInputControl), "m_primaryBindingDisplay");
		private static readonly FieldInfo SecondaryBindingField = AccessTools.Field(typeof(UserInputControl), "m_secondaryBindingDisplay");
		private static readonly FieldInfo PressKeyField = AccessTools.Field(typeof(UserInputControl), "m_pressKeyPrompt");
		private static readonly FieldInfo PressKeyXboxField = AccessTools.Field(typeof(UserInputControl), "m_pressKeyPromptXbox");

		public static SettingElement TryRead(GameObject go) {
			PauseMenuControl control = go.GetComponentInParent<PauseMenuControl>();
			if (control == null) return null;
			string title = LabelOf((UILocalize)FieldLabelField.GetValue(control));
			string description = LabelOf((UILocalize)DescriptionLabelField.GetValue(control));
			return new SettingElement(title, Value(control), description);
		}

		private static string Value(PauseMenuControl control) {
			var choice = control as EnumControl;
			if (choice != null) return UiText.Read(choice.FieldLabel);
			var toggle = control as ToggleControl;
			if (toggle != null) return toggle.GetValue() ? Strings.SettingOn : Strings.SettingOff;
			var slider = control as SliderControl;
			if (slider != null) return slider.GetValue().ToString();
			var button = control as ButtonControl;
			if (button != null) return LabelOf((UILocalize)ButtonLabelField.GetValue(button));
			var binding = control as UserInputControl;
			if (binding != null) return BindingValue(binding);
			return null;
		}

		private static string BindingValue(UserInputControl control) {
			foreach (FieldInfo field in new[] { PressKeyField, PressKeyXboxField }) {
				var prompt = (GameObject)field.GetValue(control);
				if (prompt != null && prompt.activeInHierarchy) {
					string text = string.Join(" ", UiText.Labels(prompt).ToArray());
					if (text.Length > 0) return text;
				}
			}
			var keys = new List<string>();
			foreach (FieldInfo field in new[] { PrimaryBindingField, SecondaryBindingField }) {
				var display = (UserInputControl.BindingDisplay)field.GetValue(control);
				if (display.Container == null || !display.Container.activeInHierarchy) continue;
				string key = KeyName(display.KMButtonLabel, display.KMButtonSprite)
					?? KeyName(display.ControllerButtonLabel, display.ControllerButtonSprite);
				if (!string.IsNullOrEmpty(key)) keys.Add(key);
			}
			return keys.Count == 0 ? null : string.Join(", ", keys.ToArray());
		}

		// A key cap carries the key's name as text; a mouse button or pad button is a
		// sprite only.
		private static string KeyName(UILabel label, UISprite sprite) {
			string text = label != null && label.gameObject.activeInHierarchy ? label.text : null;
			if (!string.IsNullOrEmpty(text)) return KeyNames.Localize(text);
			if (sprite == null || !sprite.gameObject.activeInHierarchy) return null;
			string glyph = InputGlyphs.Name(sprite.spriteName);
			if (glyph != null) return glyph;
			if (sprite.spriteName == UIManager.KeySpriteName) return null;
			return sprite.spriteName;
		}

		private static string LabelOf(UILocalize localize) {
			if (localize == null) return null;
			UILabel label = localize.GetComponent<UILabel>();
			return label != null && label.gameObject.activeInHierarchy ? label.text : null;
		}
	}
}
