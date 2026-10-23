using System.Reflection;
using HandOfFate2Access.Localization;
using HarmonyLib;

namespace HandOfFate2Access.Focus {
	/// <summary>
	/// Names for the character-customisation cards, which carry only art. Origins and
	/// sigils use the game's own localized names (the same strings the story quotes back:
	/// "the amulet of the Lion"); gender is authored; styles, colours and war paint have no
	/// names in the game at all, so they read as numbered options of their kind.
	/// </summary>
	internal static class CustomisationReader {
		private static readonly FieldInfo PromptField = AccessTools.Field(typeof(UICustomisationSlot), "m_promptText");

		public static string Name(CustomisationCard card) {
			GlobalStrings strings = GlobalStrings.Instance;
			switch (card.Type) {
				case CustomisationCard.CustomisationType.Gender:
					return card.Gender == PlayerGender.Male ? Strings.GenderMale : Strings.GenderFemale;
				case CustomisationCard.CustomisationType.Origin:
					return strings != null ? Localize(OriginLink(strings, card.Ethnicity), strings) : null;
				case CustomisationCard.CustomisationType.Sigil:
					if (strings != null && strings.Sigils != null && card.CustomisationIndex >= 0 && card.CustomisationIndex < strings.Sigils.Length)
						return Localize(strings.Sigils[card.CustomisationIndex], strings);
					return string.Format(Strings.CustomSigilFormat, card.CustomisationIndex + 1);
				default:
					return Described(card);
			}
		}

		/// <summary>
		/// What kind of option a customisation card is ("sigil", "look, for male
		/// characters"), said beside its name where nothing else says it (a reward card, an
		/// encounter's choice; the customisation screen's slots name their kind already). The
		/// player's own sigil adds "yours": a sighted player sees it on their character, and
		/// encounters test it (ConditionTaskPlayerSigil: "which amulet is yours?").
		/// </summary>
		public static string Kind(CustomisationCard card) {
			string kind = KindOnly(card);
			if (card.Type == CustomisationCard.CustomisationType.Sigil && IsPlayersSigil(card))
				return kind + ", " + Strings.CustomYours;
			return kind;
		}

		private static bool IsPlayersSigil(CustomisationCard card) {
			try {
				ProfileManager profiles = ProfileManager.Instance;
				var data = profiles != null && profiles.Profile != null ? profiles.Profile.CustomisationData : null;
				return data != null && data.SigilSet && data.Sigil == card.CustomisationIndex;
			} catch (System.Exception ex) {
				HandOfFate2Access.Util.Log.Error("player's sigil unreadable: " + ex.Message);
				return false;
			}
		}

		private static string KindOnly(CustomisationCard card) {
			string kind;
			bool gendered = false;
			switch (card.Type) {
				case CustomisationCard.CustomisationType.Gender: return null;
				case CustomisationCard.CustomisationType.Origin: kind = Strings.CustomKindOrigin; break;
				case CustomisationCard.CustomisationType.Sigil: kind = Strings.CustomKindSigil; break;
				case CustomisationCard.CustomisationType.Style: kind = Strings.CustomKindStyle; gendered = true; break;
				case CustomisationCard.CustomisationType.Warpaint: kind = Strings.CustomKindWarpaint; gendered = true; break;
				case CustomisationCard.CustomisationType.HairColour: kind = Strings.CustomKindHairColour; break;
				case CustomisationCard.CustomisationType.WarpaintColour: kind = Strings.CustomKindWarpaintColour; break;
				default: kind = Strings.CustomKindColour; break;
			}
			if (!gendered) return kind;
			return kind + ", " + (card.Gender == PlayerGender.Male ? Strings.CustomForMale : Strings.CustomForFemale);
		}

		/// <summary>
		/// The option chosen in a slot. The slot's stack holds every option of its kind;
		/// the chosen one is whichever matches the game's current value (a pick is dealt
		/// to the bottom of the stack with the rest piled on top), found the way the game
		/// finds it.
		/// </summary>
		public static CustomisationCard Chosen(UICustomisationSlot slot, CardContainer container) {
			if (container.Cards.Count == 0) return null;
			UICustomisation ui = slot.GetComponentInParent<UICustomisation>();
			Card chosen = ui != null ? ui.FindTopCard(slot.CardType, container.Cards) : null;
			return (chosen ?? container.Cards[0]) as CustomisationCard;
		}

		/// <summary>The slot's prompt ("Choose your origin"), the game's own text.</summary>
		public static string Prompt(UICustomisationSlot slot) {
			var link = (StringLink)PromptField.GetValue(slot);
			GlobalStrings strings = GlobalStrings.Instance;
			return link != null && strings != null ? Localize(link, strings) : null;
		}

		// Styles, colours and war paint: the description written for the card's art, or a
		// numbered option of its kind (logged, so the missing art can be described).
		private static string Described(CustomisationCard card) {
			string described = ArtDescriptions.Describe(card.CardArt);
			if (described != null) return described;
			HandOfFate2Access.Util.Log.Warn("no description for customisation art '" + card.CardArt + "' (" + card.Type + ")");
			int n = card.CustomisationIndex + 1;
			switch (card.Type) {
				case CustomisationCard.CustomisationType.Style: return string.Format(Strings.CustomStyleFormat, n);
				case CustomisationCard.CustomisationType.Warpaint: return string.Format(Strings.CustomWarpaintFormat, n);
				default: return string.Format(Strings.CustomColourFormat, n);
			}
		}

		private static StringLink OriginLink(GlobalStrings s, PlayerEthnicity ethnicity) {
			switch (ethnicity) {
				case PlayerEthnicity.African: return s.Origin_AF;
				case PlayerEthnicity.Asian: return s.Origin_AS;
				case PlayerEthnicity.Caucasian: return s.Origin_CA;
				default: return s.Origin_HI;
			}
		}

		private static string Localize(StringLink link, GlobalStrings strings) {
			if (link == null) return null;
			string text = link.GetValue(strings.StringLibrary);
			return string.IsNullOrEmpty(text) ? null : text;
		}
	}
}
