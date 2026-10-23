using HandOfFate2Access.Localization;
using HandOfFate2Access.Speech;
using HandOfFate2Access.UI;
using UnityEngine;

namespace HandOfFate2Access.Focus {
	/// <summary>
	/// The deck builder's card browsers (the cards you can add, the cards in your deck)
	/// keep focus on one invisible "layout controller" while left/right slides the cards
	/// past it; the card in the centre is the one confirm acts on. Focus never changes, so
	/// this watches the centre card of the focused browser and reads each new one (its
	/// summary; the full text goes to the details review buffer). Entering a browser says
	/// which list it is and the deck's count; a card Enter cannot move says why
	/// (<see cref="DeckRules"/>: a deck rule, a full deck, a pinned card), and a refused
	/// Enter is answered with that reason.
	/// </summary>
	internal sealed class BrowserWatcher {
		/// <summary>The frame a centre card was last spoken (the focus pump's edge repeat
		/// stays quiet then).</summary>
		public static int SpokeFrame = -100;

		private CardBrowser _browser;
		private Card _centre;
		private DeckBuilderMode _mode;
		private DeckBuilder _builder;
		private float _nextFind;
		private int _count = -1;

		/// <summary>The readout for a focused browser, or null when <paramref name="go"/> is not one.</summary>
		public static UIElement Describe(GameObject go) {
			CardBrowser browser = BrowserOf(go);
			if (browser == null) return null;
			var summary = new System.Collections.Generic.List<string> { Name(browser), DeckRules.Count() };
			Card card = browser.LayoutControllerCard;
			if (card == null) {
				summary.Add(Strings.BrowserEmpty);
				return new CardElement(summary.ToArray(), null);
			}
			CardInfoReader.Parts parts = CardInfoReader.ReadParts(card);
			summary.AddRange(parts.Summary());
			string status = Status(browser, card);
			summary.Add(status);
			var details = parts.All();
			details.Add(status);
			return new CardElement(summary.ToArray(), details.ToArray());
		}

		// The centre card alone, as it slides in, with why Enter would not move it.
		private static UIElement Centre(CardBrowser browser, Card card) {
			if (card == null) return new GenericElement(null, new[] { Strings.BrowserEmpty });
			CardInfoReader.Parts parts = CardInfoReader.ReadParts(card);
			string status = Status(browser, card);
			var summary = parts.Summary();
			summary.Add(status);
			var details = parts.All();
			details.Add(status);
			return new CardElement(summary.ToArray(), details.ToArray());
		}

		private static string Status(CardBrowser browser, Card card) {
			return IsDeck(browser) ? DeckRules.WhyNotRemoved(card) : DeckRules.WhyNotAdded(card);
		}

		public void Pump() {
			PumpCount();
			Card refused = DeckRules.Refused;
			if (refused != null) {
				DeckRules.Refused = null;
				// Only a refusal the player's Enter caused: the recommended-deck fill also
				// adds through the same call and may try past a full deck.
				if (!NavigationState.WasRecent) return;
				string why = DeckRules.WhyNotAdded(refused) ?? Strings.DeckRuleNotMet;
				SpokeFrame = Time.frameCount;
				SpeechPipeline.SpeakInterrupt(Strings.DeckCannotAdd(why));
			}
			CardBrowser browser = BrowserOf(FocusTracker.Current());
			if (browser != _browser) {
				_browser = browser;
				_centre = browser != null ? browser.LayoutControllerCard : null;
				return;
			}
			if (browser == null) return;
			Card centre = browser.LayoutControllerCard;
			if (centre == _centre) return;
			_centre = centre;
			UIElement element = Centre(browser, centre);
			string text = element.Describe().Resolve();
			if (string.IsNullOrEmpty(text)) return;
			if (Review.ReviewController.Instance != null) Review.ReviewController.Instance.Focus(element.Details());
			SpokeFrame = Time.frameCount;
			SpeechPipeline.SpeakInterrupt(text);
		}

		// A card moved into or out of the deck being built: the new count, queued after
		// the card that slides into the centre.
		private void PumpCount() {
			if (_builder == null && Time.unscaledTime >= _nextFind) {
				_nextFind = Time.unscaledTime + 2f;
				_builder = Object.FindObjectOfType<DeckBuilder>();
			}
			DeckBuilder builder = _builder;
			// Only while the player is in the deck builder: its hidden decks also change
			// during a run (the adventure picks), which must not be announced.
			GameObject focus = FocusTracker.Current();
			bool building = builder != null && focus != null && focus.transform.IsChildOf(builder.transform);
			DeckBuilderMode mode = building ? builder.ActiveMode : null;
			if (mode != _mode) {
				_mode = mode;
				_count = mode != null ? mode.Source.Count : -1;
				return;
			}
			if (mode == null || mode.Source.Count == _count) return;
			_count = mode.Source.Count;
			SpeechPipeline.SpeakQueued(string.Format(Strings.DeckCountFormat, mode.Source.Count, mode.CardLimit));
		}

		private static CardBrowser BrowserOf(GameObject go) {
			if (go == null) return null;
			CardBrowser browser = go.GetComponentInParent<CardBrowser>();
			return browser != null && browser.LayoutController != null && browser.LayoutController.gameObject == go ? browser : null;
		}

		// Which of the deck builder's browsers this is, by the object the game names it.
		private static string Name(CardBrowser browser) {
			return IsDeck(browser) ? Strings.BrowserDeck : Strings.BrowserCollection;
		}

		private static bool IsDeck(CardBrowser browser) =>
			browser.name.IndexOf("Deck", System.StringComparison.OrdinalIgnoreCase) >= 0;
	}
}
