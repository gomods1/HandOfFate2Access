using System.Collections.Generic;
using System.Reflection;
using HandOfFate2Access.Localization;
using HandOfFate2Access.UI;
using HarmonyLib;
using UnityEngine;

namespace HandOfFate2Access.Focus {
	/// <summary>
	/// Reads the rows of the mods screen (a UIMod per installed mod). The row itself
	/// speaks the mod's title, whether it is on for the selected save (the game shows a
	/// tick only) and a warning or loading state (icons only). The row's own buttons,
	/// shown while it is selected, are icons: the two arrows that reorder the mod
	/// (named by the mod) and the inspect button (the game's own word for it).
	/// </summary>
	internal static class ModsReader {
		private static readonly FieldInfo TitleField = AccessTools.Field(typeof(UIMod), "m_title");
		private static readonly FieldInfo MoveUpField = AccessTools.Field(typeof(UIMod), "m_moveUp");
		private static readonly FieldInfo MoveDownField = AccessTools.Field(typeof(UIMod), "m_moveDown");
		private static readonly FieldInfo InspectField = AccessTools.Field(typeof(UIMod), "m_inspect");
		private static readonly FieldInfo StatusField = AccessTools.Field(typeof(UIMod), "m_status");
		private static readonly FieldInfo ConfigField = AccessTools.Field(typeof(UIMod), "m_config");

		public static UIElement TryRead(GameObject go) {
			UIMod mod = go.GetComponentInParent<UIMod>();
			if (mod == null) return null;
			string title = UiText.Read((UILabel)TitleField.GetValue(mod));
			var parts = new List<string>();
			string button = ButtonName(go, mod);
			if (button != null) {
				parts.Add(button);
				parts.Add(title);
				return new GenericElement(go.name, parts.ToArray());
			}
			parts.Add(title);
			parts.Add(mod.InProfile ? Strings.SettingOn : Strings.SettingOff);
			var status = (UIModStatus)StatusField.GetValue(mod);
			if (status == UIModStatus.Warning) parts.Add(Strings.ModWarning);
			return new GenericElement(go.name, parts.ToArray());
		}

		// The name of one of the row's own buttons when <paramref name="go"/> is one of
		// them; null for the row itself.
		private static string ButtonName(GameObject go, UIMod mod) {
			if (Is(go, MoveUpField, mod)) return Strings.ModMoveUp;
			if (Is(go, MoveDownField, mod)) return Strings.ModMoveDown;
			if (Is(go, InspectField, mod)) {
				var labels = UiText.Labels(go);
				if (labels.Count > 0) return string.Join(", ", labels.ToArray());
				var config = (UIModConfig)ConfigField.GetValue(mod);
				return config != null ? ProxyFactory.LocalizeUi(config.ModInspectString) : null;
			}
			return null;
		}

		private static bool Is(GameObject go, FieldInfo field, UIMod mod) {
			var selectable = (UISelectable)field.GetValue(mod);
			return selectable != null && selectable.gameObject == go;
		}
	}
}
