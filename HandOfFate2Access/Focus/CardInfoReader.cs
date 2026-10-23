using System.Collections.Generic;
using System.Reflection;
using HandOfFate2Access.Util;
using HarmonyLib;
using UnityEngine;

namespace HandOfFate2Access.Focus {
	/// <summary>
	/// Reads a card the way the game's own info panel would present it. Hand of Fate 2
	/// draws card text on a separate panel (title, type, stats, traits with their
	/// rules text, token and shard rewards) that the game shows only in some contexts
	/// (zoom, shop, display tasks). To read any focused card with the same complete,
	/// localized content, the panel's own layout prefab for that card is instantiated
	/// into a hidden zero-scale holder, filled by the game's SetCard, its labels read,
	/// and destroyed. Nothing is rendered and no game state changes.
	/// </summary>
	internal static class CardInfoReader {
		private static readonly MethodInfo GetLayoutMethod = AccessTools.Method(typeof(CardInfoPanel), "GetLayout");
		private static GameObject _holder;
		private static readonly FieldInfo[] TraitFields = {
			AccessTools.Field(typeof(CardInfoTrait), "m_title"),
			AccessTools.Field(typeof(CardInfoTrait), "m_requirements"),
			AccessTools.Field(typeof(CardInfoTrait), "m_description"),
		};

		/// <summary>A card's text sorted by what it is, for a summary and a full review.</summary>
		internal sealed class Parts {
			/// <summary>The card's name (null when the game prints only a placeholder).</summary>
			public string Title;
			/// <summary>What else the card face prints (a description, a subtitle, the "new" badge).</summary>
			public readonly List<string> Face = new List<string>();
			/// <summary>The info panel's lines outside its traits (type, stats, rewards), stats paired with their captions.</summary>
			public readonly List<string> Info = new List<string>();
			/// <summary>The stats among <see cref="Info"/> ("Damage 13").</summary>
			public readonly List<string> Stats = new List<string>();
			/// <summary>One line per trait: its name, requirement and rules text.</summary>
			public readonly List<string> Traits = new List<string>();
			/// <summary>What the card shows as icons and models, not text: a token or shard to
			/// win, or one won this challenge (<see cref="Marks"/>).</summary>
			public readonly List<string> Marks = new List<string>();
			/// <summary>What sort of card it is ("Equipment", "encounter"), said when the
			/// card prints no kind of its own.</summary>
			public string Type;

			/// <summary>Everything, in reading order: name, type, face, marks, panel lines, traits.</summary>
			public List<string> All() {
				var all = new List<string>();
				if (!string.IsNullOrEmpty(Title)) all.Add(Title);
				if (!string.IsNullOrEmpty(Type)) all.Add(Type);
				all.AddRange(Face);
				all.AddRange(Marks);
				all.AddRange(Info);
				all.AddRange(Traits);
				return all;
			}

			/// <summary>
			/// What focus says: the name, the card's kind (the info panel's first short line
			/// that is not a stat, "One-Handed Weapon"; a card without a panel line takes its
			/// face's), its stats, its marks ("has a token"), then the short badges printed on
			/// the face ("New"). The rest is for review.
			/// </summary>
			public List<string> Summary() {
				var summary = new List<string>();
				// A card the game titles with a placeholder is named by its first face line.
				if (!string.IsNullOrEmpty(Title)) summary.Add(Title);
				else if (Face.Count > 0) summary.Add(Face[0]);
				bool kind = false;
				foreach (string line in Concat(Info, Face)) {
					if (Stats.Contains(line) || summary.Contains(line) || Strip(line).Length > KindMaxLength) continue;
					summary.Add(line);
					kind = true;
					break;
				}
				if (!kind && !string.IsNullOrEmpty(Type)) summary.Add(Type);
				summary.AddRange(Stats);
				summary.AddRange(Marks);
				foreach (string line in Face)
					if (!summary.Contains(line) && Strip(line).Length <= BadgeMaxLength) summary.Add(line);
				return summary;
			}

			// A face badge ("New") is a word or two; a face description is a sentence.
			private const int BadgeMaxLength = 12;

			private const int KindMaxLength = 40;

			private static IEnumerable<string> Concat(List<string> a, List<string> b) {
				foreach (string x in a) yield return x;
				foreach (string x in b) yield return x;
			}

			private static string Strip(string line) => Speech.TextFilter.FilterForSpeech(line) ?? "";
		}

		/// <summary>
		/// The card's text: first what is printed on the card face itself (title,
		/// description, subtitle, the "new" badge), then everything the info panel adds
		/// (type, stats, rewards, traits with their rules). Duplicates are collapsed by
		/// the Message the caller composes.
		/// </summary>
		public static List<string> Read(Card card) {
			return ReadParts(card).All();
		}

		public static Parts ReadParts(Card card) {
			var parts = new Parts();
			string title = Title(card);
			if (!HandOfFate2Access.UI.Placeholder.IsMissing(title))
				parts.Title = title;
			AddFaceLabels(card, parts.Face);
			parts.Face.RemoveAll(line => line == parts.Title);
			AddPanelLabels(card, parts.Info, parts.Traits, parts.Stats);
			parts.Info.AddRange(parts.Stats);
			parts.Marks.AddRange(Marks(card));
			parts.Type = TypeName(card);
			return parts;
		}

		/// <summary>
		/// The card's type in words: the game's own name where it has one (equipment,
		/// monsters, blessings and curses, fortunes: <c>Card.SimpleTypeDesc</c>, the words of
		/// its "you received" announcements), the mod's for encounters and companions, and a
		/// customisation card's kind (and the gender it is for).
		/// </summary>
		public static string TypeName(Card card) {
			var custom = card as CustomisationCard;
			if (custom != null) return CustomisationReader.Kind(custom);
			try {
				IStringLibraryReader desc = card.SimpleTypeDesc;
				if (desc != null && desc.HasKey) {
					string text = desc.GetValue(GlobalStringLibrary.Instance);
					if (HandOfFate2Access.UI.Placeholder.IsMissing(text)) text = desc.GetValue(card.StringLibrary);
					if (!HandOfFate2Access.UI.Placeholder.IsMissing(text)) return text;
				}
			} catch (System.Exception ex) {
				Log.Error("card type unreadable for " + card.name + ": " + ex.Message);
			}
			if (card is EncounterCard) return Localization.Strings.CardTypeEncounter;
			if (card is CompanionCard) return Localization.Strings.CardTypeCompanion;
			return null;
		}

		/// <summary>
		/// What a card shows beyond its text, decided as the game decides to draw it: the
		/// token model on the card (GetTokenAppearance, which also carries a token from a
		/// card the encounter refers to) or, once won this challenge, the info panel's ticked
		/// token; the same for a shard.
		/// </summary>
		public static List<string> Marks(Card card) {
			var marks = new List<string>();
			if (card == null) return marks;
			try {
				if (card.GetTokenAppearance() != null) marks.Add(Localization.Strings.CardTokenToWin);
				else if (card.TokenOwner != null && card.TokenOwner.WonThisChallenge) marks.Add(Localization.Strings.CardTokenWon);
				if (!ShardManager.DISABLE_SHARDS) {
					var profile = ProfileManager.Instance != null ? ProfileManager.Instance.Profile : null;
					if (card.GetShardAppearance() != null) marks.Add(Localization.Strings.CardShardToWin);
					else if (profile != null && card.ShardType != null && ShardManager.IsShardPending(card, profile))
						marks.Add(Localization.Strings.CardShardWon);
				}
			} catch (System.Exception ex) {
				Log.Error("token and shard marks unreadable for card " + card.name + ": " + ex);
			}
			return marks;
		}

		private static bool IsNumber(string s) {
			s = s.Trim().TrimEnd('%');
			if (s.Length == 0) return false;
			foreach (char c in s)
				if (!char.IsDigit(c) && c != '+' && c != '-' && c != '.') return false;
			return true;
		}

		// The labels on the card's own template, skipping the stream-vote overlay (a
		// Twitch feature, not card content).
		private static void AddFaceLabels(Card card, List<string> result) {
			CardTemplate template = card.CardTemplate;
			if (template == null) return;
			foreach (UILabel label in template.GetComponentsInChildren<UILabel>(false)) {
				if (!label.enabled || IsUnder(label.transform, "VoteNode", template.transform)) continue;
				Add(result, label.text);
			}
		}

		// The panel's labels: traits as one line each, stats (the widgets in its grid, a
		// number and a caption in either order: "13" over "Damage", "Defence" beside "4")
		// as "Damage 13", so a number is never heard without its meaning; the rest in
		// hierarchy order.
		private static void AddPanelLabels(Card card, List<string> result, List<string> traits, List<string> stats) {
			UI2D ui = UI2D.Instance;
			if (ui == null || ui.NewCardInfoPanel == null) {
				Log.Warn("card info panel unavailable; reading the card face only");
				return;
			}
			CardInfoPanel panel = ui.NewCardInfoPanel;
			var layoutPrefab = (CardInfoLayout)GetLayoutMethod.Invoke(panel, new object[] { card });
			if (layoutPrefab == null) return;
			CardInfoLayout layout = Object.Instantiate(layoutPrefab, Holder().transform, false);
			try {
				layout.SetCard(card, panel);
				// Static captions ("Damage", "Defence") localize themselves in Start, which
				// never runs on this short-lived copy; localize them now.
				foreach (UILocalize localize in layout.GetComponentsInChildren<UILocalize>(false))
					if (localize.localizeOnStart) localize.Localize();
				// A trait reads name, requirement, then its rules text, whatever order its
				// labels sit in the hierarchy, as one line.
				var traitsDone = new HashSet<CardInfoTrait>();
				var statWidgets = new List<Transform>();
				var captions = new Dictionary<Transform, string>();
				var values = new Dictionary<Transform, string>();
				foreach (UILabel label in layout.GetComponentsInChildren<UILabel>(false)) {
					if (!label.enabled) continue;
					CardInfoTrait trait = label.GetComponentInParent<CardInfoTrait>();
					if (trait == null && IsUnder(label.transform, "Grid", layout.transform)) {
						string text = label.text != null ? label.text.Trim() : "";
						if (HandOfFate2Access.UI.Placeholder.IsMissing(text)) continue;
						Transform widget = label.transform.parent;
						if (!statWidgets.Contains(widget)) statWidgets.Add(widget);
						if (IsNumber(text)) values[widget] = text;
						else captions[widget] = text;
						continue;
					}
					if (trait == null) {
						Add(result, label.text);
						continue;
					}
					if (!traitsDone.Add(trait)) continue;
					var line = new UI.Message();
					foreach (FieldInfo field in TraitFields) {
						var traitLabel = (UILabel)field.GetValue(trait);
						if (traitLabel != null && traitLabel.enabled && traitLabel.gameObject.activeInHierarchy)
							line.Add(traitLabel.text);
					}
					Add(traits, line.Resolve());
				}
				foreach (Transform widget in statWidgets) {
					captions.TryGetValue(widget, out string caption);
					values.TryGetValue(widget, out string value);
					Add(stats, ((caption ?? "") + " " + (value ?? "")).Trim());
				}
				// An equipment card without a stat is a reading gap: name what the game
				// held for it so the missing number can be traced.
				var equipment = card as EquipmentCard;
				if (equipment != null && stats.Count == 0)
					Log.Info("card " + card.name + " read without stats: equipment="
						+ (equipment.Equipment != null ? equipment.Equipment.GetType().Name : "null")
						+ " layout=" + layoutPrefab.name + " damage=" + equipment.WeaponDamage
						+ " defence=" + equipment.DefenceValue);
			} finally {
				Object.DestroyImmediate(layout.gameObject);
			}
		}

		// Skips empty text and the game's "****" stand-in for a missing localization key.
		private static void Add(List<string> result, string text) {
			if (HandOfFate2Access.UI.Placeholder.IsMissing(text)) return;
			result.Add(text);
		}

		private static bool IsUnder(Transform t, string name, Transform stop) {
			for (Transform p = t; p != null && p != stop; p = p.parent)
				if (p.name == name) return true;
			return false;
		}

		/// <summary>
		/// A short name for a card: its title, or, for a card whose title is the game's
		/// "****" placeholder (the reward cards the player stakes in a gamble), its first
		/// readable line ("Good fortune").
		/// </summary>
		public static string ShortName(Card card) {
			string title = Speech.TextFilter.FilterForSpeech(Title(card) ?? "");
			if (IsName(title)) return title;
			foreach (string part in Read(card)) {
				string text = Speech.TextFilter.FilterForSpeech(part);
				if (IsName(text)) return text;
			}
			return title;
		}

		// A usable card name: not the "****" placeholder, and not a fragment of a title
		// template whose variable came back empty ("de 1"): a name never starts lower-case.
		private static bool IsName(string text) {
			return !HandOfFate2Access.UI.Placeholder.IsMissing(text) && !char.IsLower(text[0]);
		}

		public static string Title(Card card) {
			if (card.Title == null) return null;
			return card.Title.GetValue(card.StringLibrary);
		}

		private static GameObject Holder() {
			if (_holder != null) return _holder;
			_holder = new GameObject("HoF2Access.CardInfoReader");
			ScenePersistence.Protect(_holder);
			_holder.transform.position = new Vector3(0f, -100000f, 0f);
			_holder.transform.localScale = Vector3.zero;
			return _holder;
		}
	}
}
