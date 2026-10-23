using HandOfFate2Access.Focus;
using HandOfFate2Access.Speech;
using HandOfFate2Access.UI;

namespace HandOfFate2Access.Screens {
	/// <summary>
	/// The reward cards a won challenge or a token hands out, dealt face down; confirm
	/// turns one over and focus jumps straight to the next, so the revealed card would
	/// never be read. The game's reveal handler is postfixed to flag the card, and the
	/// pump speaks its summary before the next focus (name, type, stats, a token); its full
	/// text is pinned in the details review buffer (Control plus up arrow) until the
	/// player moves, and kept in the events buffer.
	/// </summary>
	internal sealed class RewardNarration {
		public static Card PendingReveal;

		public static void OnCardSelectedPostfix(UISelectable a_selectable) {
			Card card = a_selectable != null ? a_selectable.GetComponent<Card>() : null;
			if (card != null) PendingReveal = card;
		}

		public bool Pump() {
			Card card = PendingReveal;
			if (card == null) return false;
			PendingReveal = null;
			CardInfoReader.Parts parts = CardInfoReader.ReadParts(card);
			string text = new Message().AddRange(parts.Summary()).Resolve();
			if (string.IsNullOrEmpty(text)) return false;
			// Focus jumps to the next card at once, so the full text is pinned in the details
			// over that focus, and kept in the events buffer too.
			SpeechPipeline.SpeakEventInterrupt(text, new Message().AddRange(parts.All()).Resolve());
			if (Review.ReviewController.Instance != null) Review.ReviewController.Instance.Pin(parts.All());
			return true;
		}
	}
}
