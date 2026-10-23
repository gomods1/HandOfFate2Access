using System.Collections.Generic;
using System.Reflection;
using HandOfFate2Access.Localization;
using HandOfFate2Access.UI;
using HandOfFate2Access.Util;
using HarmonyLib;
using UnityEngine;

namespace HandOfFate2Access.Focus {
	/// <summary>
	/// Reads a row of the additional content screen (an AddonContentUIItem: a DLC
	/// pack, a free pack, or a content feed entry). Focus speaks its title, its short
	/// description and its state, which the game shows only as a banner (a tick for
	/// owned, "Free", a price); the details buffer adds the pack's description and
	/// what it contains.
	/// </summary>
	internal static class AddonReader {
		private static readonly FieldInfo TitleLabelField = AccessTools.Field(typeof(AddonContentUIItem), "m_titleLabel");
		private static readonly FieldInfo SubheadingLabelField = AccessTools.Field(typeof(AddonContentUIItem), "m_subheadingLabel");

		public static UIElement TryRead(GameObject go) {
			AddonContentUIItem item = go.GetComponentInParent<AddonContentUIItem>();
			if (item == null) return null;
			var summary = new List<string>();
			summary.Add(UiText.Read((UILabel)TitleLabelField.GetValue(item)));
			summary.Add(UiText.Read((UILabel)SubheadingLabelField.GetValue(item)));
			summary.Add(StateWord(item.State));
			var details = new List<string>(summary);
			details.Add(Resolve(item.Description, item.StringLibrary));
			details.Add(Resolve(item.ContentsDescription, item.StringLibrary));
			return new DetailedElement(summary.ToArray(), details.ToArray());
		}

		/// <summary>The state of a pack in the mod's words; null for a state with no word.</summary>
		public static string StateWord(ContentFeedData.ContentState state) {
			switch (state) {
				case ContentFeedData.ContentState.Claimed: return Strings.AddonOwned;
				case ContentFeedData.ContentState.Free: return Strings.AddonFree;
				case ContentFeedData.ContentState.Paid: return Strings.AddonPaid;
				case ContentFeedData.ContentState.ComingSoon: return Strings.AddonComingSoon;
				default: return null;
			}
		}

		/// <summary>
		/// A pack's string link in the player's language: through the pack's own
		/// library when it has one, else the UI's, as the game's own labels resolve it.
		/// Null for a link with no key.
		/// </summary>
		public static string Resolve(StringLink link, StringLibrary library) {
			if (link == null || !link.HasKey) return null;
			try {
				string text = link.GetValue(library ?? UIManager.StringLibrary);
				return HandOfFate2Access.UI.Placeholder.IsMissing(text) ? null : text;
			} catch (System.Exception ex) {
				Log.Error("add-on text failed to resolve: " + ex);
				return null;
			}
		}
	}
}
