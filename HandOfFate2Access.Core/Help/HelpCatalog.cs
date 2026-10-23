using System.Collections.Generic;
using HandOfFate2Access.Localization;

namespace HandOfFate2Access.Help {
	/// <summary>The screen or mode the help key explains.</summary>
	public enum HelpContext {
		Menu,
		MainMenu,
		Map,
		Inventory,
		Encounter,
		DeckBuilder,
		ChallengeMap,
		Paused,
		Combat,
		Dice,
		Wheel,
		Pendulum,
		ChanceCards,
		Addons,
		Mods,
		Customisation,
	}

	/// <summary>
	/// The keys bound to the combat actions, already in spoken form (the player can
	/// rebind them, so they are read from the live bindings). Null where unbound.
	/// </summary>
	public sealed class CombatKeys {
		public string Move;
		public string Attack;
		public string Bash;
		public string Counter;
		public string Dodge;
		public string Weapon;
		public string Artifact;
		public string Companion;
		public string Finisher;
		public string Run;
		public string Camera;
		/// <summary>Defend can only be pressed with the mouse (its default binding): the help says to bind a key.</summary>
		public bool CounterMouseOnly;
		/// <summary>The one preferred Defend binding, for announcements that name a single key.</summary>
		public string CounterKey;
		/// <summary>The one preferred Bash binding, for announcements that name a single key.</summary>
		public string BashKey;
	}

	/// <summary>
	/// The help list for each context: the game's own keys for that screen first, then
	/// the mod's keys, which work everywhere. Each entry is one line the player steps
	/// through. The mod menu's help topics are the same lists, each without the mod's
	/// keys (those are a topic of their own), led by a getting-started topic.
	/// </summary>
	public static class HelpCatalog {
		public static string Title(HelpContext context) {
			switch (context) {
				case HelpContext.MainMenu: return Strings.ScreenMainMenu;
				case HelpContext.Map: return Strings.ScreenMap;
				case HelpContext.Inventory: return Strings.ScreenInventory;
				case HelpContext.Encounter: return Strings.ScreenEncounter;
				case HelpContext.DeckBuilder: return Strings.ScreenDeckBuilder;
				case HelpContext.ChallengeMap: return Strings.ScreenChallengeMap;
				case HelpContext.Paused: return Strings.ScreenPaused;
				case HelpContext.Combat: return Strings.ScreenCombat;
				case HelpContext.Dice: return Strings.ScreenDice;
				case HelpContext.Wheel: return Strings.ScreenWheel;
				case HelpContext.Pendulum: return Strings.ScreenPendulum;
				case HelpContext.ChanceCards: return Strings.ScreenChanceCards;
				case HelpContext.Addons: return Strings.ScreenAddons;
				case HelpContext.Mods: return Strings.ScreenMods;
				case HelpContext.Customisation: return Strings.ScreenCustomisation;
				default: return Strings.ScreenMenu;
			}
		}

		/// <param name="inCombat">A fight is running under this context (the pause menu over a fight).</param>
		/// <param name="keys">The combat bindings, for the combat entries; may be null outside a fight.</param>
		public static List<string> Entries(HelpContext context, bool inCombat, CombatKeys keys) {
			List<string> entries = ScreenEntries(context, inCombat, keys);
			entries.AddRange(ModKeyEntries());
			return entries;
		}

		/// <summary>The mod's own keys, which work on every screen.</summary>
		public static List<string> ModKeyEntries() {
			return new List<string> {
				Strings.HelpModHelp,
				Strings.HelpModMenu,
				Strings.HelpModReview,
				Strings.HelpModReviewSwitch,
				Strings.HelpModStatus,
				Strings.HelpModObjectives,
			};
		}

		/// <summary>The contexts the mod menu offers as help topics, in its order.</summary>
		public static readonly HelpContext[] TopicContexts = {
			HelpContext.Menu, HelpContext.Map, HelpContext.Inventory, HelpContext.Encounter, HelpContext.DeckBuilder,
			HelpContext.Combat, HelpContext.ChanceCards, HelpContext.Dice, HelpContext.Wheel, HelpContext.Pendulum,
		};

		/// <summary>The getting-started topic: how the mod reads the game, in four lines.</summary>
		public static List<string> StartEntries() {
			return new List<string> {
				Strings.TopicStartFocus,
				Strings.TopicStartEvents,
				Strings.TopicStartFights,
				Strings.TopicStartMinigames,
			};
		}

		/// <summary>A screen's own lines: its keys and what it says, without the mod's keys.</summary>
		public static List<string> ScreenEntries(HelpContext context, bool inCombat, CombatKeys keys) {
			var entries = new List<string>();
			switch (context) {
				case HelpContext.MainMenu:
				case HelpContext.Menu:
					entries.Add(Strings.HelpMenuNav);
					break;
				case HelpContext.Map:
					entries.Add(Strings.HelpMapMove);
					entries.Add(Strings.HelpMapInventory);
					entries.Add(Strings.HelpMapView);
					entries.Add(Strings.HelpMapCursor);
					entries.Add(Strings.HelpMapUnvisited);
					entries.Add(Strings.HelpPauseKey);
					break;
				case HelpContext.Inventory:
					entries.Add(Strings.HelpInventoryMove);
					entries.Add(Strings.HelpInventoryClose);
					break;
				case HelpContext.Encounter:
					entries.Add(Strings.HelpEncounterChoose);
					entries.Add(Strings.HelpEncounterUnavailable);
					entries.Add(Strings.HelpEncounterInventory);
					entries.Add(Strings.HelpBarKeys);
					break;
				case HelpContext.DeckBuilder:
					entries.Add(Strings.HelpDeckModes);
					entries.Add(Strings.HelpDeckCards);
					entries.Add(Strings.HelpDeckStart);
					entries.Add(Strings.HelpDeckRecommend);
					entries.Add(Strings.HelpDeckFilters);
					break;
				case HelpContext.ChallengeMap:
					entries.Add(Strings.HelpMenuNav);
					entries.Add(Strings.HelpChallengeTokens);
					break;
				case HelpContext.Paused:
					entries.Add(Strings.HelpPaused);
					entries.Add(Strings.HelpGlossary);
					break;
				case HelpContext.Dice:
					entries.Add(Strings.HelpDice);
					entries.Add(Strings.HelpDiceReroll);
					entries.Add(Strings.HelpBarKeys);
					break;
				case HelpContext.Wheel:
					entries.Add(Strings.HelpWheel);
					break;
				case HelpContext.Pendulum:
					entries.Add(Strings.HelpPendulum);
					break;
				case HelpContext.ChanceCards:
					entries.Add(Strings.HelpChanceShown);
					entries.Add(Strings.HelpChanceShuffle);
					entries.Add(Strings.HelpChancePick);
					entries.Add(Strings.HelpChanceSounds);
					break;
				case HelpContext.Addons:
					entries.Add(Strings.HelpAddons);
					entries.Add(Strings.HelpMenuNav);
					break;
				case HelpContext.Customisation:
					entries.Add(Strings.HelpCustomisation);
					break;
				case HelpContext.Mods:
					entries.Add(Strings.HelpMods);
					entries.Add(Strings.HelpMenuNav);
					break;
			}
			if (context == HelpContext.Combat || inCombat)
				AddCombat(entries, keys);
			return entries;
		}

		private static void AddCombat(List<string> entries, CombatKeys keys) {
			if (keys != null) {
				AddKey(entries, Strings.CombatMove, keys.Move);
				AddKey(entries, Strings.CombatAttack, keys.Attack);
				AddKey(entries, Strings.CombatCounter, keys.Counter);
				AddKey(entries, Strings.CombatDodge, keys.Dodge);
				AddKey(entries, Strings.CombatBash, keys.Bash);
				AddKey(entries, Strings.CombatWeapon, keys.Weapon);
				AddKey(entries, Strings.CombatArtifact, keys.Artifact);
				AddKey(entries, Strings.CombatCompanion, keys.Companion);
				AddKey(entries, Strings.CombatFinisher, keys.Finisher);
				AddKey(entries, Strings.CombatRun, keys.Run);
				AddKey(entries, Strings.CombatCamera, keys.Camera);
				if (keys.CounterMouseOnly) entries.Add(Strings.HelpCombatMouseOnly);
			}
			// Every combat sound and what it means, so the help in a fight explains what
			// the player is hearing; the menu's combat sounds play each one.
			entries.Add(Strings.HelpCombatCues);
			entries.Add(Strings.GlossaryBlock);
			entries.Add(Strings.GlossaryDodge);
			entries.Add(Strings.GlossaryEnemyStep);
			entries.Add(Strings.GlossaryEnemyReach);
			entries.Add(Strings.GlossaryFinisher);
			entries.Add(Strings.GlossaryFinisherReady);
			entries.Add(Strings.GlossaryArmourAbsorbed);
			entries.Add(Strings.GlossaryArmourCrack);
			entries.Add(Strings.GlossaryArmourBroken);
			entries.Add(Strings.GlossaryArmourRestored);
			entries.Add(Strings.GlossaryProjectile);
			entries.Add(Strings.GlossaryWallTones);
			entries.Add(Strings.GlossaryWallCollision);
			entries.Add(Strings.GlossaryZoneActive);
			entries.Add(Strings.GlossaryZoneInside);
			entries.Add(Strings.GlossaryRecharge);
			entries.Add(Strings.HelpCombatSnare);
			entries.Add(Strings.HelpCombatLocator);
			entries.Add(Strings.HelpCombatGlossary);
			entries.Add(Strings.HelpPauseKey);
		}

		private static void AddKey(List<string> entries, string action, string key) {
			if (!string.IsNullOrEmpty(key))
				entries.Add(Strings.HelpKey(action, key));
		}
	}
}
