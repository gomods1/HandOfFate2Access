using System.Collections.Generic;
using System.Reflection;
using HandOfFate2Access.Localization;
using HarmonyLib;

namespace HandOfFate2Access.Focus {
	/// <summary>
	/// A challenge on the challenge map (an icon in the bottom row): its name, then
	/// whether it is locked (with the game's own unlock requirement), completed, and
	/// which token it has earned (the icon's gold or silver border).
	/// </summary>
	internal static class ChallengeNavReader {
		private static readonly FieldInfo LockedDisplayField = AccessTools.Field(typeof(UIChallengeNav), "m_lockedDisplay");
		private static readonly FieldInfo DescriptionField = AccessTools.Field(typeof(UIChallengeNavDisplay), "m_description");

		public static List<string> Read(UIChallengeNavTab tab) {
			var parts = new List<string>();
			ChallengeCard card = tab.ChallengeCardPrefab;
			if (card == null) return parts;
			parts.Add(CardInfoReader.Title(card));
			if (tab.Locked) {
				parts.Add(Strings.ChallengeLocked);
				UIChallengeNav nav = tab.GetComponentInParent<UIChallengeNav>();
				var display = nav != null ? (UIChallengeNavDisplay)LockedDisplayField.GetValue(nav) : null;
				var description = display != null ? (UILabel)DescriptionField.GetValue(display) : null;
				if (description != null && !string.IsNullOrEmpty(description.text)) parts.Add(description.text);
				return parts;
			}
			if (card.PlayerHasGoldToken) parts.Add(Strings.ChallengeGoldToken);
			else if (card.PlayerHasSilverToken) parts.Add(Strings.ChallengeSilverToken);
			else if (tab.Completed) parts.Add(Strings.ObjectiveComplete);
			return parts;
		}
	}
}
