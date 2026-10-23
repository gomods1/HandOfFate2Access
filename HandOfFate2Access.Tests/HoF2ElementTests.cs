using System.Collections.Generic;
using HandOfFate2Access.Localization;
using HandOfFate2Access.Resources;
using HandOfFate2Access.UI;
using Xunit;

namespace HandOfFate2Access.Tests {
	public class HoF2ElementTests : System.IDisposable {
		public HoF2ElementTests() {
			Strings.ResetForTest();
		}

		public void Dispose() {
			Strings.ResetForTest();
		}

		[Fact]
		public void ChoiceReadsTextThenUtility() {
			Assert.Equal("Attack them, Lose 5 gold", new ChoiceElement("Attack them", "Lose 5 gold", false).Describe().Resolve());
		}

		[Fact]
		public void ChoiceWithoutTextIsContinue() {
			Assert.Equal("Continue", new ChoiceElement(null, null, false).Describe().Resolve());
		}

		[Fact]
		public void DisabledChoiceSaysUnavailable() {
			Assert.Equal("Pay, unavailable", new ChoiceElement("Pay", "", true).Describe().Resolve());
		}

		[Fact]
		public void MapCellReadsCardNameStateAndPaths() {
			var info = new MapCellInfo(new List<string> { "Goblins", "Encounter" }, null,
				false, false, true, false, new MapExits(true, false, false, true));
			Assert.Equal("Goblins, reachable, paths up, right", new MapCellElement(info).Describe().Resolve());
		}

		[Fact]
		public void MapCellDetailsHoldTheFullCardText() {
			var attached = new List<IList<string>> { new List<string> { "Apple", "Heals 5" } };
			var info = new MapCellInfo(new List<string> { "Goblins", "A band of goblins blocks the road." }, attached,
				false, false, true, false, new MapExits(true, false, false, false));
			var element = new MapCellElement(info);
			Assert.Equal("Goblins, with Apple, reachable, paths up", element.Describe().Resolve());
			Assert.Equal(new List<string> { "Goblins", "A band of goblins blocks the road.", "with Apple, Heals 5", "reachable", "paths up" },
				element.Details());
		}

		[Fact]
		public void MapCellFaceDownWithAttachedCard() {
			var attached = new List<IList<string>> { new List<string> { "Apple" }, null };
			var info = new MapCellInfo(null, attached, true, false, false, false, new MapExits(false, false, false, false));
			Assert.Equal("face down card, with Apple, with face down card, here, dead end", new MapCellElement(info).Describe().Resolve());
		}

		[Fact]
		public void MapCellHereWinsOverCompleted() {
			var info = new MapCellInfo(new List<string> { "Camp" }, null, true, true, false, true, new MapExits(false, true, false, false));
			Assert.Equal("Camp, here, exit, paths down", new MapCellElement(info).Describe().Resolve());
		}

		[Fact]
		public void StatusOmitsPoisonWhenHealthy() {
			Assert.Equal("80 of 100 health, 5 food, 10 gold, 0 fame", StatusReadout.Compose(80, 100, 5, 10, 0, 0));
		}

		[Fact]
		public void StatusIncludesPoison() {
			Assert.Equal("80 of 100 health, 5 food, 10 gold, 3 fame, poisoned 2", StatusReadout.Compose(80, 100, 5, 10, 3, 2));
		}

		[Fact]
		public void DeltaAlwaysSigned() {
			Assert.Equal("+7 food", ResourceDelta.Compose(ResourceKind.Food, 7));
			Assert.Equal("-4 health", ResourceDelta.Compose(ResourceKind.Health, -4));
			Assert.Equal("", ResourceDelta.Compose(ResourceKind.Gold, 0));
		}

		[Fact]
		public void SpanishTableTranslatesAndKeepsPlaceholders() {
			Assert.True(Strings.SetLanguage("es"));
			Assert.Equal("+7 de comida", ResourceDelta.Compose(ResourceKind.Food, 7));
			Assert.Equal("Carta 3 de 4, en el centro, boca abajo", Strings.GambitCard(3, 4, Strings.GambitAtCentre));
			Assert.True(Strings.SetLanguage("es-lat"));
			Assert.Equal("Continuar", Strings.Continue);
		}

		// A string added to the English table must be translated in every language table:
		// an untranslated field silently speaks English to that language's players.
		[Fact]
		public void EveryTableTranslatesEveryField() {
			string dir = System.AppContext.BaseDirectory;
			while (dir != null && !System.IO.Directory.Exists(System.IO.Path.Combine(dir, "HandOfFate2Access.Core")))
				dir = System.IO.Path.GetDirectoryName(dir);
			Assert.NotNull(dir);
			string tables = System.IO.Path.Combine(dir, "HandOfFate2Access.Core", "Localization", "Tables");
			string[] files = System.IO.Directory.GetFiles(tables, "StringTable.*.cs");
			Assert.Equal(10, files.Length);
			foreach (string file in files) {
				string source = System.IO.File.ReadAllText(file);
				foreach (var field in typeof(StringTable).GetFields())
					Assert.True(System.Text.RegularExpressions.Regex.IsMatch(source, @"\b" + field.Name + @"\s*="),
						System.IO.Path.GetFileName(file) + " is missing " + field.Name);
			}
		}

		[Fact]
		public void KeyCapsAreNamedInThePlayersLanguage() {
			Assert.True(Strings.SetLanguage("es"));
			Assert.Equal("mayúsculas izquierda", KeyNames.Localize("L Shift"));
			Assert.Equal("flecha abajo", KeyNames.Localize("Down"));
			Assert.Equal("E", KeyNames.Localize("E"));
			Assert.Equal("Inspeccionar", KeyNames.Localize("Inspeccionar"));
		}

		[Fact]
		public void ScreensAreIdentifiedFromTheMenuStacks() {
			Assert.Equal(HandOfFate2Access.Screens.ScreenId.Map, HandOfFate2Access.Screens.ScreenCatalog.Identify("Map", null));
			Assert.Equal(HandOfFate2Access.Screens.ScreenId.Paused, HandOfFate2Access.Screens.ScreenCatalog.Identify("Map", "Pause"));
			Assert.Equal(HandOfFate2Access.Screens.ScreenId.DeckBuilder, HandOfFate2Access.Screens.ScreenCatalog.Identify("Deckbuilder_Build", null));
			Assert.Equal(HandOfFate2Access.Screens.ScreenId.None, HandOfFate2Access.Screens.ScreenCatalog.Identify("Encounter", null));
			Assert.Equal("Map", HandOfFate2Access.Screens.ScreenCatalog.NameOf(HandOfFate2Access.Screens.ScreenId.Map));
		}

		[Fact]
		public void MissingKeyPlaceholderIsNotSpoken() {
			Assert.Equal("Good fortune, 10 Gold", new Message().Add("****").Add("Good fortune").Add("10 Gold").Resolve());
		}

		[Fact]
		public void SettingReadsNameValueThenDescription() {
			var row = new SettingElement("Subtitles", "on", null);
			Assert.Equal("Subtitles, on", row.Describe().Resolve());
			Assert.Equal("on", row.DescribeValue().Resolve());
		}

		[Fact]
		public void UnknownLanguageFallsBackToEnglish() {
			Assert.False(Strings.SetLanguage("xx"));
			Assert.Equal("Continue", Strings.Continue);
		}

		// Every format string in every table must keep the placeholders the English
		// source has, or string.Format throws (or drops the value) in that language.
		[Fact]
		public void EveryTableKeepsEnglishPlaceholders() {
			var english = new StringTable();
			foreach (string code in Strings.Languages) {
				StringTable table = Strings.TableFor(code);
				foreach (var field in typeof(StringTable).GetFields()) {
					string en = (string)field.GetValue(english);
					string tr = (string)field.GetValue(table);
					Assert.False(string.IsNullOrEmpty(tr), code + "." + field.Name + " is empty");
					for (int i = 0; i < 4; i++) {
						string ph = "{" + i + "}";
						Assert.True(en.Contains(ph) == tr.Contains(ph), code + "." + field.Name + " placeholder " + ph);
					}
				}
			}
		}
	}
}
