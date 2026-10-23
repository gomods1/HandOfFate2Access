using System.Collections.Generic;
using HandOfFate2Access.Help;
using HandOfFate2Access.Localization;
using HandOfFate2Access.Review;
using HandOfFate2Access.UI;
using Xunit;

namespace HandOfFate2Access.Tests {
	public class ReviewTests : System.IDisposable {
		public ReviewTests() {
			Strings.ResetForTest();
		}

		public void Dispose() {
			Strings.ResetForTest();
		}

		private static ReviewBuffer Buffer(string name, params string[] items) {
			var buffer = new ReviewBuffer(() => name);
			buffer.Set(items);
			return buffer;
		}

		[Fact]
		public void FirstStepDownReadsTheTopItem() {
			var set = new ReviewSet(Buffer("Details", "Sword", "Damage 13"));
			Assert.Equal("Sword", set.Deeper());
			Assert.Equal("Damage 13", set.Deeper());
		}

		[Fact]
		public void EdgesAreSpokenNotWrapped() {
			var set = new ReviewSet(Buffer("Details", "Sword", "Damage 13"));
			Assert.Equal("top", set.TowardTop());
			set.Deeper();
			set.Deeper();
			Assert.Equal("end", set.Deeper());
			Assert.Equal("Sword", set.TowardTop());
			Assert.Equal("top", set.TowardTop());
			Assert.Equal("Sword", set.Deeper());
		}

		[Fact]
		public void AnEmptyBufferSaysSo() {
			var set = new ReviewSet(Buffer("Details"));
			Assert.Equal("Details, empty", set.Deeper());
		}

		[Fact]
		public void SwitchingSkipsEmptyBuffersAndWraps() {
			var details = Buffer("Details", "Sword");
			var events = Buffer("Events");
			var status = Buffer("Status", "80 of 100 health", "5 food");
			var set = new ReviewSet(details, events, status);
			Assert.Equal("Status, 2 items", set.Switch(1));
			Assert.Equal("80 of 100 health", set.Deeper());
			Assert.Equal("Details, 1 item", set.Switch(1));
			Assert.Equal("Status, 2 items", set.Switch(-1));
		}

		[Fact]
		public void NothingToSwitchToIsSpoken() {
			var set = new ReviewSet(Buffer("Details"), Buffer("Events"));
			Assert.Equal("Nothing to review", set.Switch(1));
		}

		[Fact]
		public void FocusingMakesDetailsCurrentFromTheTop() {
			var details = Buffer("Details", "Sword", "Damage 13");
			var events = Buffer("Events", "Fight, 3 enemies");
			var set = new ReviewSet(details, events);
			set.Switch(1);
			set.Deeper();
			details.Set(new[] { "Shield", "Defence 4" });
			set.MakeCurrent(details);
			Assert.Equal("Shield", set.Deeper());
		}

		[Fact]
		public void SettingTheSameContentKeepsThePlace() {
			var details = Buffer("Details", "Sword", "Damage 13");
			var set = new ReviewSet(details);
			set.Deeper();
			details.Set(new[] { "Sword", "Damage 13" });
			Assert.Equal("Damage 13", set.Deeper());
		}

		[Fact]
		public void NewEventsKeepTheReaderOnTheirItemAndDropTheOldest() {
			var events = new ReviewBuffer(() => "Events", 3);
			var set = new ReviewSet(events);
			events.Add("one");
			events.Add("two");
			Assert.Equal("two", set.Deeper());
			events.Add("three");
			events.Add("four");
			Assert.Equal(new List<string> { "four", "three", "two" }, events.Items);
			Assert.Equal("end", set.Deeper());
		}

		[Fact]
		public void ARefresherRebuildsOnEveryRead() {
			int gold = 10;
			var status = new ReviewBuffer(() => "Status") { Refresher = () => new[] { gold + " gold" } };
			var set = new ReviewSet(status);
			Assert.Equal("10 gold", set.Deeper());
			gold = 25;
			Assert.Equal("25 gold", set.Deeper());
		}

		[Fact]
		public void ANullRefresherResultEmptiesTheBuffer() {
			var status = new ReviewBuffer(() => "Status") { Refresher = () => null };
			Assert.Equal(0, status.Count);
		}

		[Fact]
		public void BufferHeadingsAreTranslated() {
			Assert.True(Strings.SetLanguage("es"));
			Assert.Equal("Eventos, 4 elementos", Strings.ReviewBufferHeading(Strings.ReviewEvents, 4));
			Assert.Equal("Eventos, 1 elemento", Strings.ReviewBufferHeading(Strings.ReviewEvents, 1));
		}

		[Fact]
		public void CardSpeaksItsSummaryAndKeepsEverythingForReview() {
			var card = new CardElement(
				new[] { "Neglected Sword", "37 gold", "One-Handed Weapon", "Damage 13" },
				new[] { "Neglected Sword", "One-Handed Weapon", "Damage 13", "Critical Strike, Requires: 7 Hits" });
			Assert.Equal("Neglected Sword, 37 gold, One-Handed Weapon, Damage 13", card.Describe().Resolve());
			Assert.Equal(new List<string> { "Neglected Sword", "One-Handed Weapon", "Damage 13", "Critical Strike, Requires: 7 Hits", "37 gold" },
				card.Details());
		}

		[Fact]
		public void PlainElementsListTheirPartsAsDetails() {
			Assert.Equal(new List<string> { "Subtitles", "on" }, new SettingElement("Subtitles", "on", null).Details());
		}

		[Fact]
		public void HelpListsTheScreensKeysThenTheModsKeys() {
			List<string> entries = HelpCatalog.Entries(HelpContext.Inventory, false, null);
			Assert.Equal(Strings.HelpInventoryMove, entries[0]);
			Assert.Contains(Strings.HelpInventoryClose, entries);
			Assert.Equal(Strings.HelpModHelp, entries[2]);
			Assert.DoesNotContain(Strings.HelpCombatCues, entries);
		}

		[Fact]
		public void CombatHelpNamesTheBoundKeys() {
			var keys = new CombatKeys { Move = "W A S D", Dodge = "space" };
			List<string> entries = HelpCatalog.Entries(HelpContext.Combat, true, keys);
			Assert.Equal("Move: W A S D", entries[0]);
			Assert.Equal("Dodge: space", entries[1]);
			Assert.Contains(Strings.HelpCombatCues, entries);
		}

		[Fact]
		public void PauseOverAFightAddsTheCombatKeys() {
			List<string> entries = HelpCatalog.Entries(HelpContext.Paused, true, new CombatKeys { Attack = "left click" });
			Assert.Equal(Strings.HelpPaused, entries[0]);
			Assert.Contains("Attack: left click", entries);
		}

		[Fact]
		public void EventsAreRecordedButFocusEchoesAreNot() {
			var heard = new List<string>();
			Speech.SpeechPipeline.SpeakAction = (text, interrupt) => { };
			Speech.SpeechPipeline.EventSink = heard.Add;
			try {
				Speech.SpeechPipeline.SpeakEvent("Fight, 3 enemies");
				Speech.SpeechPipeline.SpeakInterrupt("Neglected Sword");
				Speech.SpeechPipeline.SpeakQueued("Shield");
				Assert.Equal(new List<string> { "Fight, 3 enemies" }, heard);
			} finally {
				Speech.SpeechPipeline.EventSink = null;
				Speech.SpeechPipeline.SpeakAction = Speech.SpeechEngine.Say;
			}
		}
	}
}
