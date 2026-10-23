using System.Collections.Generic;
using System.Reflection;
using HandOfFate2Access.Localization;
using HarmonyLib;
using UnityEngine;

namespace HandOfFate2Access.Focus {
	/// <summary>
	/// Panels whose content is static text beside a lone button (the add-on purchase
	/// panel: a title, a description, what it contains, then "Claim"). Focus lands on
	/// the button, so the text would never be read; it is spoken ahead of the focused
	/// item when focus first enters the panel. The additional content and mods screens
	/// are named on entry (the mods screen with the save slot and state text it shows,
	/// which the game's own focus never reaches). The deck builder that opens a
	/// challenge is introduced by the challenge card itself.
	/// </summary>
	internal sealed class PanelContext {
		private static readonly string[] AddonFields = {
			"m_contentHeader", "m_contentDescription", "m_contentContentsDescription",
			"m_postPurchaseFlavourText", "m_postPurchaseContents" };

		private Component _lastPanel;
		private GameObject _lastMenu;
		private string _lastText;
		// The deck builder's challenge, spoken once per challenge: the card zoom it opens
		// lies outside the deck builder, and coming back must not read it again.
		private Card _deckChallenge;
		private string _deckText;

		/// <summary>The panel's text to speak before <paramref name="go"/>'s own readout,
		/// or null when focus stays within a panel whose text was already spoken.</summary>
		public string Take(GameObject go) {
			// The end-of-run score table: read whole when focus first enters it.
			UIScoreDisplay score = go.GetComponentInParent<UIScoreDisplay>();
			if (score != null) {
				string table = ScoreText(score.transform);
				if (score == _lastPanel && table == _lastText) return null;
				_lastPanel = score;
				_lastText = table;
				return table;
			}
			// The Endless adventure builder (the choice between levels): its prompt ("Choose
			// any optional trial card") and the panel explaining the choice.
			if (go.GetComponentInParent<AdventureDeckBuilder>() != null) {
				GameObject menu = GameObject.Find("UIManager/UI Root (2D)/Camera/MenuManager/PrimaryMenus/AdventureDeckBuilder");
				if (menu == null) return null;
				var parts = new UI.Message();
				foreach (string area in new[] { "Top", "BottomLeft" }) {
					Transform node = menu.transform.Find(area);
					if (node != null) parts.AddRange(UiText.Labels(node.gameObject));
				}
				string about = parts.Resolve();
				if (menu == _lastMenu && about == _lastText) return null;
				_lastMenu = menu;
				_lastPanel = null;
				_lastText = about;
				return string.IsNullOrEmpty(about) ? null : about;
			}
			// The deck builder before a challenge: the challenge card is the context.
			if (go.GetComponentInParent<DeckBuilder>() != null) {
				Challenge challenge = Challenge.Instance;
				Card card = challenge != null ? challenge.Card : null;
				if (card == null) return null;
				string about = ChallengeText(card);
				if (card == _deckChallenge && about == _deckText) return null;
				_deckChallenge = card;
				_deckText = about;
				return about;
			}
			// The additional content screen: named on entry; its details panel is read by
			// AddonNarration as the hovered pack changes.
			AddonContentUI addons = go.GetComponentInParent<AddonContentUI>();
			if (addons != null) {
				if (addons == _lastPanel) return null;
				_lastPanel = addons;
				_lastText = null;
				return Strings.ScreenAddons;
			}
			// The mods screen: named on entry with the save slot it shows and its state
			// text (no mods installed, a slot that does not use mods, the on/off tallies);
			// re-read when that text changes under a focus move. The mod rows and the
			// workshop items read themselves; the inspect panel is read by ModsNarration.
			UIModDialog mods = go.GetComponentInParent<UIModDialog>();
			if (mods != null) {
				string about = ModsText(mods);
				if (mods == _lastPanel && about == _lastText) return null;
				bool entering = mods != _lastPanel;
				_lastPanel = mods;
				_lastText = about;
				return entering ? new UI.Message().Add(Strings.ScreenMods).Add(about).Resolve() : about;
			}
			Component panel = go.GetComponentInParent<AddonContentPrePurchaseUI>();
			if (panel == null) {
				_lastPanel = null;
				_lastText = null;
				return null;
			}
			string text = Read(panel);
			if (panel == _lastPanel && text == _lastText) return null;
			_lastPanel = panel;
			_lastText = text;
			return string.IsNullOrEmpty(text) ? null : text;
		}

		/// <summary>
		/// The challenge being opened: the deck builder's overview (title and the
		/// challenge's description), read even while it fades in; the challenge card's
		/// own text when the overview is not found.
		/// </summary>
		public static string ChallengeText(Card card) {
			MenuManager menus = MenuManager.Instance;
			Menu menu = menus != null ? menus.DeckBuilderSelectModeMenu : null;
			Transform overview = menu != null ? Find(menu.transform, "ChallengeOverview") : null;
			if (overview != null) {
				var message = new UI.Message();
				foreach (UILabel label in overview.GetComponentsInChildren<UILabel>(true))
					if (label.gameObject.activeInHierarchy) message.Add(label.text);
				string text = message.Resolve();
				if (text.Length > 0) return text;
			}
			return new UI.Message().AddRange(CardInfoReader.Read(card)).Resolve();
		}

		/// <summary>
		/// The score table as a sighted player reads it: title, total, companion and
		/// attempts, then each row with its four numbers named by their column headings
		/// ("Adventure cards completed: Tally, This run 3, Best 5; Score, This run 30,
		/// Best 50").
		/// </summary>
		private static string ScoreText(Transform root) {
			var message = new UI.Message();
			message.Add(Text(root, "TitleLabel"));
			message.Add(Text(root, "ScoreLabel"));
			Transform companion = Find(root, "CompanionContainer");
			if (companion != null)
				foreach (UILabel label in companion.GetComponentsInChildren<UILabel>(false)) message.Add(label.text);
			string tally = Text(root, "TallyHeadingLabel"), scoreHead = Text(root, "ScoreHeadingLabel");
			string prevTally = Text(root, "PrevTallyLabel"), bestTally = Text(root, "BestTallyLabel");
			string prevScore = Text(root, "PrevScoreLabel"), bestScore = Text(root, "BestScoreLabel");
			Transform rows = Find(root, "Rows");
			if (rows != null) {
				for (int i = 0; i < rows.childCount; i++) {
					Transform row = rows.GetChild(i);
					if (!row.gameObject.activeInHierarchy) continue;
					string name = Text(row, "RowLabel");
					var values = new string[4];
					for (int c = 0; c < 4; c++) values[c] = Text(row, "DiffRow_" + c);
					message.Add(name + ": " + tally + ", " + prevTally + " " + values[0] + ", " + bestTally + " " + values[1]
						+ "; " + scoreHead + ", " + prevScore + " " + values[2] + ", " + bestScore + " " + values[3]);
				}
			}
			return message.Resolve();
		}

		// The text of the first label at or under the child named <paramref name="name"/>.
		private static string Text(Transform root, string name) {
			Transform node = Find(root, name);
			if (node == null) return null;
			UILabel label = node.GetComponentInChildren<UILabel>(true);
			return label != null ? label.text : null;
		}

		private static Transform Find(Transform root, string name) {
			if (root.name == name) return root;
			for (int i = 0; i < root.childCount; i++) {
				Transform found = Find(root.GetChild(i), name);
				if (found != null) return found;
			}
			return null;
		}

		private static readonly FieldInfo ModsStateField = AccessTools.Field(typeof(UIModDialog), "m_state");
		private static readonly FieldInfo ModsContainersField = AccessTools.Field(typeof(UIModDialog), "m_stateContainers");
		private static readonly string[] ModsSlotFields = { "m_slotNameLabel", "m_slotPercentLabel", "m_slotLastSavedLabel" };

		// The selected save slot (name, progress, last saved) and every visible label of
		// the containers the dialog shows for its current state, except the rows and
		// panels that read themselves.
		private static string ModsText(UIModDialog dialog) {
			var message = new UI.Message();
			foreach (string name in ModsSlotFields) {
				FieldInfo field = AccessTools.Field(typeof(UIModDialog), name);
				if (field != null) message.Add(UiText.Read((UILabel)field.GetValue(dialog)));
			}
			var state = (UIModDialog.ModDialogState)ModsStateField.GetValue(dialog);
			var containers = (UIModDialog.ModDialogStateContainer[])ModsContainersField.GetValue(dialog);
			if (containers == null) return message.Resolve();
			foreach (UIModDialog.ModDialogStateContainer entry in containers) {
				if (entry.State != state || entry.Containers == null) continue;
				foreach (Transform node in entry.Containers) {
					if (node == null) continue;
					foreach (UILabel label in node.GetComponentsInChildren<UILabel>(false)) {
						if (!UiText.IsVisible(label)) continue;
						if (label.GetComponentInParent<UIMod>() != null || label.GetComponentInParent<UIModFeaturePanel>() != null
							|| label.GetComponentInParent<UIModInfo>() != null) continue;
						message.Add(label.text);
					}
				}
			}
			return message.Resolve();
		}

		private static string Read(Component panel) {
			var parts = new List<string>();
			foreach (string name in AddonFields) {
				FieldInfo field = AccessTools.Field(panel.GetType(), name);
				if (field == null) continue;
				string text = UiText.Read((UILabel)field.GetValue(panel));
				if (!string.IsNullOrEmpty(text)) parts.Add(text);
			}
			return string.Join(", ", parts.ToArray());
		}
	}
}
