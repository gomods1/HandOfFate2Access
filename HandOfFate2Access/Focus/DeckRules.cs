using System.Reflection;
using HandOfFate2Access.Localization;
using HandOfFate2Access.Util;
using HarmonyLib;
using UnityEngine;

namespace HandOfFate2Access.Focus {
	/// <summary>
	/// What the deck builder would say about the card in a browser's centre, decided as
	/// its tooltip decides it (DeckBuilderTooltipSet, which shows it only beside a hovered
	/// card): an available card that a deck rule refuses (the rule's own failure text,
	/// rendered on a hidden copy of the tooltip's label, as the game renders it), one the
	/// full deck cannot take, and a deck card the game pins in place. Null when Enter
	/// simply adds or removes the card. Also the hand-off from the add patch: a refused
	/// Enter is said with its reason.
	/// </summary>
	internal static class DeckRules {
		private static readonly FieldInfo FailedTooltipField = AccessTools.Field(typeof(DeckBuilderTooltipSet), "m_failedRequirementTooltip");
		private static readonly FieldInfo TooltipLabelField = AccessTools.Field(typeof(DeckBuilderTooltip), "m_label");
		private static GameObject _holder;
		private static DeckBuilderTooltipSet _tooltips;

		/// <summary>The card the game last refused to add, for the pump to explain; cleared on read.</summary>
		public static Card Refused;

		/// <summary>Postfix on DeckBuilderMode.TryAddToDeck (every mode adds through it): a refusal is recorded.</summary>
		public static void TryAddPostfix(Card a_card, bool __result) {
			if (!__result) Refused = a_card;
		}

		/// <summary>Why <paramref name="card"/> cannot be added to the active deck, or null when it can.</summary>
		public static string WhyNotAdded(Card card) {
			DeckBuilderMode mode = DeckBuilder.Instance != null ? DeckBuilder.Instance.ActiveMode : null;
			if (mode == null || card == null) return null;
			IDeckRequirement rule = PlayerDeckUtils.CheckRequirements(card, mode.PlayerDeckDataOwner, mode.Source, mode.RarityRules);
			if (rule != null) return RuleText(rule) ?? Strings.DeckRuleNotMet;
			if (!mode.IsInsufficient) return Strings.DeckFull;
			return null;
		}

		/// <summary>Why a card in the deck cannot be taken out, or null when it can.</summary>
		public static string WhyNotRemoved(Card card) {
			return card != null && card.CardData != null && card.CardData.Pinned ? Strings.DeckCardPinned : null;
		}

		/// <summary>The active deck's count: "12 of 20 cards".</summary>
		public static string Count() {
			DeckBuilderMode mode = DeckBuilder.Instance != null ? DeckBuilder.Instance.ActiveMode : null;
			return mode != null ? string.Format(Strings.DeckCountFormat, mode.Source.Count, mode.CardLimit) : null;
		}

		// The rule's failure text with its parameters, filled in by the game's own label
		// code on an inactive copy of the tooltip's label (nothing is drawn).
		private static string RuleText(IDeckRequirement rule) {
			try {
				if (_tooltips == null) _tooltips = Object.FindObjectOfType<DeckBuilderTooltipSet>();
				var tooltip = _tooltips != null ? (DeckBuilderTooltip)FailedTooltipField.GetValue(_tooltips) : null;
				var label = tooltip != null ? (UILabel)TooltipLabelField.GetValue(tooltip) : null;
				if (label == null || rule.FailureText == null) return null;
				if (_holder == null) {
					_holder = new GameObject("HoF2Access.DeckRuleHolder");
					_holder.SetActive(false);
					Object.DontDestroyOnLoad(_holder);
				}
				GameObject copy = Object.Instantiate(label.gameObject, _holder.transform, false);
				try {
					UILabel copyLabel = copy.GetComponent<UILabel>();
					UIUtils.SetLabelText(copyLabel, rule.FailureText, rule.FailureTextParams);
					string text = copyLabel.text;
					return UI.Placeholder.IsMissing(text) ? null : text;
				} finally {
					Object.DestroyImmediate(copy);
				}
			} catch (System.Exception ex) {
				Log.Error("deck rule text unreadable: " + ex.Message);
				return null;
			}
		}
	}
}
