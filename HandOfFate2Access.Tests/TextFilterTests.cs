using HandOfFate2Access.Speech;
using Xunit;

namespace HandOfFate2Access.Tests {
	public class TextFilterTests {
		[Theory]
		// Empty / null
		[InlineData(null, "")]
		[InlineData("", "")]
		[InlineData("   ", "")]
		// Plain text passes through, whitespace normalized and trimmed
		[InlineData("Hello world", "Hello world")]
		[InlineData("  Hello world  ", "Hello world")]
		// NGUI color tags (6 and 8 hex) stripped, end tag stripped
		[InlineData("[FF0000]Red[-]", "Red")]
		[InlineData("[ff0000]Red[-]", "Red")]
		[InlineData("[FF0000FF]Red[-]", "Red")]
		// NGUI style toggles stripped (case-insensitive)
		[InlineData("[b]Bold[/b]", "Bold")]
		[InlineData("[B]Bold[/B]", "Bold")]
		[InlineData("[i]a[/i][u]b[/u][s]c[/s][c]d[/c]", "abcd")]
		// NGUI url keeps display text
		[InlineData("[url=http://x.com]click here[/url]", "click here")]
		// Unity rich text stripped
		[InlineData("<b>bold</b>", "bold")]
		[InlineData("<color=#ff0000>red</color>", "red")]
		// Mixed markup
		[InlineData("[FFAA00]Gold:[-] [b]5[/b]", "Gold: 5")]
		// Markup-only collapses to empty
		[InlineData("[b][/b]", "")]
		[InlineData("[FF0000][-]", "")]
		// Real (non-tag) brackets are preserved
		[InlineData("deal 5 [50%] damage", "deal 5 [50%] damage")]
		[InlineData("loot [chest]", "loot [chest]")]
		// A leading list bullet is dropped (it reads as "dash"); markup-stripped first
		[InlineData("- Health 100", "Health 100")]
		[InlineData("• Food 3", "Food 3")]
		[InlineData("[b]- Gold 0[/b]", "Gold 0")]
		// A negative number is not a bullet (no space after the dash)
		[InlineData("-10 health", "-10 health")]
		[InlineData("Health Lost from Starving -10", "Health Lost from Starving -10")]
		// Only the leading bullet goes; a mid-line dash stays
		[InlineData("well - maybe", "well - maybe")]
		public void FilterForSpeech_Cases(string input, string expected) {
			Assert.Equal(expected, TextFilter.FilterForSpeech(input));
		}

		[Fact]
		public void FilterForSpeech_StripsControlChars() {
			// Build control chars in code to avoid string-escape ambiguity.
			Assert.Equal("abcdef", TextFilter.FilterForSpeech("abc" + (char)0 + "def"));
			Assert.Equal("hello", TextFilter.FilterForSpeech((char)1 + "hello"));
		}

		[Fact]
		public void FilterForSpeech_KeepsNewlineTabAsWhitespace() {
			// \n, \r and \t are not stripped as control chars; a tab collapses to a space
			// and an authored line break becomes a spoken pause.
			Assert.Equal("a, b", TextFilter.FilterForSpeech("a\r\nb"));
			Assert.Equal("a b", TextFilter.FilterForSpeech("a\tb"));
		}

		[Theory]
		// An authored line break is a pause (a comma), unless punctuation already ends the line.
		[InlineData("a\n\nb\t c", "a, b c")]
		[InlineData("Find the Leader of the Thieves\nRetrieve your Amulet", "Find the Leader of the Thieves, Retrieve your Amulet")]
		[InlineData("First sentence.\nSecond sentence.", "First sentence. Second sentence.")]
		[InlineData("Damage: \n 15", "Damage: 15")]
		[InlineData("\nLeading and trailing\n", "Leading and trailing")]
		[InlineData("[b]a[/b]\n[b]b[/b]", "a, b")]
		public void FilterForSpeech_LineBreaksPause(string input, string expected) {
			Assert.Equal(expected, TextFilter.FilterForSpeech(input));
		}
	}
}
