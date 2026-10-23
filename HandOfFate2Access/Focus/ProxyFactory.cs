using HandOfFate2Access.Localization;
using HandOfFate2Access.UI;
using UnityEngine;

namespace HandOfFate2Access.Focus {
	/// <summary>
	/// The adapter between a focused GameObject and the Core element that describes
	/// it. Targeted proxies come first (most specific), the generic label sweep last.
	/// Extraction only: no wording is decided here.
	/// </summary>
	internal static class ProxyFactory {
		// The shop's price for a card on offer (or what it sells for), read the way the
		// game's own price tag computes it; null outside a shop.
		private static string ShopPrice(Card card) {
			Encounter encounter = Encounter.Instance;
			Shop shop = encounter != null ? encounter.Shop : null;
			var purchasable = card as IPurchasable;
			if (shop == null || shop.ActiveState == null || purchasable == null) return null;
			if (UIShop.Instance == null || !UIShop.Instance.isActiveAndEnabled) return null;
			int value = shop.GetActiveValue(purchasable);
			string text = shop.ActiveState.Currency == ShopState.ShopCurrency.Gold
				? string.Format(Strings.StatGoldFormat, value)
				: string.Format(Strings.StatFameFormat, value);
			if (!shop.CanTransact(purchasable)) text += ", " + Strings.ShopCannotAfford;
			return text;
		}

		// A UI string link resolved against the libraries the game's own menus use.
		/// <summary>The key that confirms, as the player would press it: Enter on a keyboard,
		/// the confirm button on a controller.</summary>
		internal static string ConfirmKey() {
			bool pad = Defiant.InputManager.ActiveDeviceClass == InControl.InputDeviceClass.Controller;
			return pad ? Strings.KeyConfirmButton : Strings.KeyEnter;
		}

		/// <summary>The dice in hand waiting to be thrown: the focus readout names the key, so
		/// the game's bar (a mouse instruction and the same Throw button) is not read.</summary>
		internal static bool IsDiceInHand(GameObject go) {
			return go != null && go.name == "IdleSelectable" && go.GetComponentInParent<DiceRoller>() != null;
		}

		internal static string LocalizeUi(StringLink link) {
			if (link == null) return null;
			foreach (StringLibrary library in new StringLibrary[] {
					UIStrings.Instance != null ? UIStrings.Instance.StringLibrary : null,
					GlobalStrings.Instance != null ? GlobalStrings.Instance.StringLibrary : null,
					UIManager.StringLibrary }) {
				if (library == null) continue;
				string text = link.GetValue(library);
				if (!HandOfFate2Access.UI.Placeholder.IsMissing(text)) return text;
			}
			return null;
		}

		private static readonly System.Reflection.FieldInfo BlockerGroupField =
			HarmonyLib.AccessTools.Field(typeof(UISelection), "m_selectionBlockerGroup");

		private static readonly System.Reflection.FieldInfo UtilityTextField =
			HarmonyLib.AccessTools.Field(typeof(PageOptionButton), "m_utilityText");

		public static UIElement Create(GameObject go) {
			UISelectable selectable = go.GetComponent<UISelectable>();
			// The selection blocker is the placeholder the game hovers while it locks
			// input (transitions, cutscenes); it has no content and is never spoken.
			if (selectable != null && selectable.Selection != null
				&& selectable.Group == (UISelectableGroup)BlockerGroupField.GetValue(selectable.Selection))
				return null;
			// A settings row (or one of its arrows) reads its name and value.
			SettingElement setting = SettingsReader.TryRead(go);
			if (setting != null) return setting;
			// A pack on the additional content screen, or a mod row (and its buttons) on
			// the mods screen.
			UIElement addon = AddonReader.TryRead(go);
			if (addon != null) return addon;
			UIElement modRow = ModsReader.TryRead(go);
			if (modRow != null) return modRow;
			Die die = go.GetComponentInParent<Die>();
			if (die != null)
				return Minigames.DiceNarration.Describe(die);
			// The dice in hand before a throw: an invisible target with no text.
			DiceRoller roller = go.GetComponentInParent<DiceRoller>();
			if (roller != null && UiText.Labels(go).Count == 0) {
				if (go.name == "IdleSelectable")
					return new GenericElement(go.name, new[] { Strings.DiceThrow(ConfirmKey()) });
				// Waiting on the game (a blessing that marks dice for you): the dice on the
				// table and which are marked to be thrown again.
				if (roller.WaitingSelectable != null && roller.WaitingSelectable.gameObject == go) {
					var dice = new System.Collections.Generic.List<string>();
					foreach (Die d in roller.ActiveDice)
						dice.Add(Minigames.DiceNarration.Describe(d).Describe().Resolve());
					return new GenericElement(null, dice.ToArray());
				}
				return null;
			}
			MapLayoutSlot slot = go.GetComponent<MapLayoutSlot>();
			if (slot != null) {
				Maps.MapBoardReader.LogFace(slot);
				return new MapCellElement(Maps.MapBoardReader.Read(slot), Maps.MapNavigation.DirectionFromPlayer(slot));
			}
			// A customisation slot (origin, style, sigil, colour) reads its prompt and the
			// option currently chosen in it.
			UICustomisationSlot customSlot = go.GetComponentInParent<UICustomisationSlot>();
			if (customSlot != null && go.GetComponent<CardContainer>() != null) {
				var chosen = CustomisationReader.Chosen(customSlot, go.GetComponent<CardContainer>());
				return new GenericElement(go.name, new[] {
					CustomisationReader.Prompt(customSlot),
					chosen != null ? CustomisationReader.Name(chosen) : null });
			}
			// The swinging pendulum: its outcome tones carry the content; the focus is the
			// prompt to stop it.
			if (go.GetComponentInParent<Pendulum>() != null && UiText.Labels(go).Count == 0)
				return new GenericElement(null, new[] { Strings.PendulumStop(ConfirmKey()) });
			// The spinning wheel: the cards on it were read when it was laid out; the
			// focus is the prompt to stop it.
			if (go.GetComponent<WheelOfFortune>() != null)
				return new GenericElement(null, new[] { Strings.WheelStop(ConfirmKey()) });
			// A deck builder card browser: the centre card.
			UIElement browser = BrowserWatcher.Describe(go);
			if (browser != null) return browser;
			// A deck in the deck builder's mode list (encounters, equipment, companion):
			// the mode's name, what it is for, and how full the deck is. Its cards are
			// read when the deck is opened.
			DeckBuilderMode mode = go.GetComponentInParent<DeckBuilderMode>();
			if (mode != null && go.GetComponent<DeckBuilderDeck>() != null) {
				var modeParts = new System.Collections.Generic.List<string>();
				modeParts.Add(LocalizeUi(mode.Title));
				modeParts.Add(LocalizeUi(mode.Description));
				modeParts.Add(string.Format(Strings.DeckCountFormat, mode.Source.Count, mode.CardLimit));
				if (mode.Locked) modeParts.Add(Strings.ChallengeLocked);
				return new GenericElement(null, modeParts.ToArray());
			}
			// An inventory slot: the slot, what is equipped, how many others could be.
			UIElement inventorySlot = InventorySlotReader.Describe(go);
			if (inventorySlot != null) return inventorySlot;
			// A focused card container (a deck pile) reads the cards it holds; the
			// container itself has no text of its own.
			CardContainer container = go.GetComponent<CardContainer>();
			if (container != null && container.Cards.Count > 0) {
				var summary = new System.Collections.Generic.List<string>();
				var details = new System.Collections.Generic.List<string>();
				foreach (Card held in container.Cards) {
					if (held.Flipped) {
						summary.Add(Strings.CardFaceDown);
						continue;
					}
					CardInfoReader.Parts heldParts = CardInfoReader.ReadParts(held);
					summary.AddRange(heldParts.Summary());
					details.AddRange(heldParts.All());
				}
				return new CardElement(summary.ToArray(), details.ToArray());
			}
			// A challenge-map section (Campaign, Adventure, Endless): the one showing is
			// marked only by highlight.
			UIChallengeMapMenuButton section = go.GetComponent<UIChallengeMapMenuButton>();
			if (section != null) {
				var sectionParts = UiText.Labels(go);
				if (section.IsSelected) sectionParts.Add(Strings.Selected);
				return new GenericElement(go.name, sectionParts.ToArray());
			}
			UIChallengeNavTab challengeTab = go.GetComponentInParent<UIChallengeNavTab>();
			if (challengeTab != null)
				return new GenericElement(go.name, ChallengeNavReader.Read(challengeTab).ToArray());
			// A reward token: the encounter that gave it and what it holds.
			Token token = go.GetComponent<Token>();
			if (token != null && token.Owner != null)
				return new GenericElement(go.name, TokenReader.Read(token.Owner).ToArray());
			// A save slot's delete button, an icon with no text.
			if (go.name == "DeleteButton" && go.GetComponentInParent<UISaveSlotContainer>() != null)
				return new GenericElement(go.name, new[] { Strings.SaveDelete });
			// The reward circle's collect button, an icon with no text.
			if (go.name == "AddSelectable" && go.GetComponentInParent<CardSetModifierContainer>() != null)
				return new GenericElement(go.name, new[] { Strings.RewardsCollect });
			PageOptionButton option = go.GetComponent<PageOptionButton>();
			if (option != null)
				return new ChoiceElement(
					UiText.Read(option.TextLabel),
					UiText.Read((UILabel)UtilityTextField.GetValue(option)),
					option.State == PageOptionButton.StateType.Disabled);
			Card card = go.GetComponentInParent<Card>();
			var custom = card as CustomisationCard;
			if (custom != null)
				return new CardElement(new[] { CustomisationReader.Name(custom), CustomisationReader.Kind(custom) },
					new[] { CustomisationReader.Name(custom), CustomisationReader.Kind(custom) });
			if (card != null && card.Flipped)
				return new GenericElement(go.name, new[] { Strings.CardFaceDown });
			if (card != null) {
				CardInfoReader.Parts cardParts = CardInfoReader.ReadParts(card);
				var summary = cardParts.Summary();
				var details = cardParts.All();
				// In a shop, the price the game prints under the card, and whether it is
				// out of reach, right after the card's name.
				string price = ShopPrice(card);
				if (price != null)
					summary.Insert(System.Math.Min(1, summary.Count), price);
				// Equipping over an item already worn: the game shows the equipped card
				// beside the offered one ("Equipped") for comparison.
				UI2D ui = UI2D.Instance;
				Card equipped = ui != null && ui.OldCardInfoPanel != null ? ui.OldCardInfoPanel.Card : null;
				if (equipped != null && equipped != card) {
					CardInfoReader.Parts worn = CardInfoReader.ReadParts(equipped);
					summary.Add(Strings.Replacing + " " + new Message().AddRange(worn.Summary()).Resolve());
					details.Add(Strings.Replacing + " " + new Message().AddRange(worn.All()).Resolve());
				}
				return new CardElement(summary.ToArray(), details.ToArray());
			}
			var labels = UiText.Labels(go);
			// The deck builder's mode list with no deck to edit yet (the first challenges):
			// focus rests on the empty list itself. The challenge card, spoken as the
			// panel's context, is all there is to read; the readout says nothing more.
			if (labels.Count == 0 && go.GetComponent<UISelectableGroup>() != null && go.GetComponentInParent<DeckBuilder>() != null)
				return new GenericElement(null, new string[0]);
			// The mods screen's background or fall-through selectable (no mods installed,
			// or no save slot that uses mods): the screen's text is the panel context.
			if (labels.Count == 0 && go.GetComponentInParent<UIModDialog>() != null)
				return new GenericElement(null, new string[0]);
			// No text yet: a bare structural selectable ("Selectable": a dialogue's
			// catch-all input target, spoken by the dialogue narration), a freshly spawned
			// button whose label fills in a frame later, or one still fading in (its label
			// fully transparent). Never speak an internal object name like
			// "UIDialogueButton(Clone)"; the focus pump retries.
			if (labels.Count == 0)
				return null;
			return new GenericElement(go.name, labels.ToArray());
		}
	}
}
