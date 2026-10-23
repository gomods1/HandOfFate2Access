using System.Reflection;
using HarmonyLib;
using InControl;

namespace HandOfFate2Access.Patches {
	/// <summary>
	/// The additional content screen updates its details panel on hover only for a
	/// controller; on keyboard and mouse a hover just animates the row and the panel
	/// changes on a click, with Enter on the current row doing nothing. A keyboard
	/// player therefore heard the first item's details for every row. Keyboard now
	/// behaves as the controller does: hovering a row makes it current (the panel
	/// follows, and the narration reads it), Enter on the current row moves to its
	/// Claim or Buy button when that is selectable.
	/// </summary>
	internal static class AddonContentUI_Patches {
		private static readonly FieldInfo CurrentItemField = AccessTools.Field(typeof(AddonContentUI), "m_currentItem");
		private static readonly FieldInfo ClaimButtonField = AccessTools.Field(typeof(AddonContentUI), "m_claimButtonSelectable");
		private static readonly MethodInfo SetCurrentItem = AccessTools.Method(typeof(AddonContentUI), "SetCurrentItem");

		private static bool Controller => Defiant.InputManager.ActiveDeviceClass == InputDeviceClass.Controller;

		public static bool HoverPrefix(AddonContentUI __instance, UISelectable a_selectable, bool a_isOver) {
			if (Controller || !a_isOver || a_selectable == null) return true;
			AddonContentUIItem item = a_selectable.GetComponent<AddonContentUIItem>();
			if (item == null) return true;
			SetCurrentItem.Invoke(__instance, new object[] { item });
			return false;
		}

		public static bool SelectPrefix(AddonContentUI __instance, UISelectable a_selectable) {
			if (Controller || a_selectable == null) return true;
			AddonContentUIItem item = a_selectable.GetComponent<AddonContentUIItem>();
			if (item == null) return true;
			var current = (AddonContentUIItem)CurrentItemField.GetValue(__instance);
			if (item != current) {
				SetCurrentItem.Invoke(__instance, new object[] { item });
				return false;
			}
			var claim = (UISelectable)ClaimButtonField.GetValue(__instance);
			if (claim != null && claim.gameObject.activeInHierarchy && claim.IsSelectable)
				UISelection.TryHover(claim);
			return false;
		}
	}
}
