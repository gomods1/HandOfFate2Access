using System.Collections.Generic;

namespace HandOfFate2Access.Focus {
	/// <summary>
	/// A reward token: the card (or add-on) it came from, its own text, and the cards it
	/// adds to the decks.
	/// </summary>
	internal static class TokenReader {
		public static List<string> Read(TokenOwner owner) {
			var parts = new List<string>();
			if (owner.Card != null) parts.Add(CardInfoReader.ShortName(owner.Card));
			if (owner.TokenDataCollection == null) return parts;
			foreach (Token.Data data in owner.TokenDataCollection) {
				if (data == null) continue;
				if (data.Text != null && data.Library != null) parts.Add(data.Text.GetValue(data.Library));
				if (data.Rewards == null || data.Rewards.Add == null) continue;
				foreach (CardRewards.Entry entry in data.Rewards.Add)
					if (entry != null && entry.Card != null) parts.Add(CardInfoReader.ShortName(entry.Card));
			}
			return parts;
		}
	}
}
