using System;
using System.Collections.Generic;

namespace HandOfFate2Access.Localization {
	/// <summary>
	/// The mod's authored strings in the player's language. Wording lives in
	/// <see cref="StringTable"/>; this facade resolves every read against the
	/// active table at speech time, so a language switch is reflected live.
	/// The plugin drives <see cref="SetLanguage"/> off the game's own language
	/// setting; a language with no table speaks English.
	/// </summary>
	public static partial class Strings {
		private static readonly Dictionary<string, Func<StringTable>> Tables =
			new Dictionary<string, Func<StringTable>>(StringComparer.OrdinalIgnoreCase) {
				{ "en", () => new StringTable() },
				{ "es", StringTableEs.Create },
				{ "es-lat", StringTableEs.Create },
				{ "fr", StringTableFr.Create },
				{ "de", StringTableDe.Create },
				{ "it", StringTableIt.Create },
				{ "ru", StringTableRu.Create },
				{ "pl", StringTablePl.Create },
				{ "pt-br", StringTablePtBr.Create },
				{ "ja", StringTableJa.Create },
				{ "ko", StringTableKo.Create },
				{ "zh", StringTableZh.Create },
			};

		private static StringTable _active = new StringTable();
		private static string _activeCode = "en";

		public static string ActiveLanguage => _activeCode;

		/// <summary>The active table, for lookups keyed by data (ArtDescriptions).</summary>
		internal static StringTable Active => _active;

		/// <summary>The language codes that have a table (English included).</summary>
		public static IEnumerable<string> Languages => Tables.Keys;

		public static bool SetLanguage(string code) {
			if (!string.IsNullOrEmpty(code) && Tables.TryGetValue(code, out var factory)) {
				_active = factory();
				_activeCode = code.ToLowerInvariant();
				return true;
			}
			_active = new StringTable();
			_activeCode = "en";
			return false;
		}

		internal static StringTable TableFor(string code) => Tables[code]();

		private static readonly List<string> TestCodes = new List<string>();
		internal static void RegisterForTest(string code, Func<StringTable> factory) {
			Tables[code] = factory;
			TestCodes.Add(code);
		}
		internal static void ResetForTest() {
			foreach (string code in TestCodes)
				Tables.Remove(code);
			TestCodes.Clear();
			SetLanguage("en");
		}

		public static string GenderMale => _active.GenderMale;
		public static string GenderFemale => _active.GenderFemale;

		public static string Continue => _active.Continue;
		public static string OptionUnavailable => _active.OptionUnavailable;

		public static string CardFaceDown => _active.CardFaceDown;
		public static string GambitCard(int slot, int count, string where) => string.Format(_active.GambitCardFormat, slot, count, where);
		public static string GambitAtFarLeft => _active.GambitAtFarLeft;
		public static string GambitAtLeft => _active.GambitAtLeft;
		public static string GambitAtCentre => _active.GambitAtCentre;
		public static string GambitAtRight => _active.GambitAtRight;
		public static string GambitAtFarRight => _active.GambitAtFarRight;
		public static string GambitIntro(int count, string tally) => string.Format(_active.GambitIntroFormat, count, tally);
		public static string GambitTally(int count, string outcome) => string.Format(_active.GambitTallyFormat, count, outcome);
		public static string GambitIntroCards(int count, string cards) => string.Format(_active.GambitIntroCardsFormat, count, cards);
		public static string GambitTeachHintCards => _active.GambitTeachHintCards;
		public static string GambitTeachHint => _active.GambitTeachHint;
		public static string GambitShuffleHint => _active.GambitShuffleHint;
		public static string GambitPickHint => _active.GambitPickHint;
		public static string GambitPickShort => _active.GambitPickShort;

		public static string StatHealthFormat => _active.StatHealthFormat;
		public static string StatFoodFormat => _active.StatFoodFormat;
		public static string StatGoldFormat => _active.StatGoldFormat;
		public static string StatFameFormat => _active.StatFameFormat;
		public static string StatPoisonFormat => _active.StatPoisonFormat;
		public static string DeltaHealthFormat => _active.DeltaHealthFormat;
		public static string DeltaMaxHealthFormat => _active.DeltaMaxHealthFormat;
		public static string StatusNoRun => _active.StatusNoRun;

		public static string MouseLeft => _active.MouseLeft;
		public static string MouseRight => _active.MouseRight;
		public static string MouseMiddle => _active.MouseMiddle;

		public static string MapSlotAttached => _active.MapSlotAttached;
		public static string MapCellHere => _active.MapCellHere;
		public static string MapCellComplete => _active.MapCellComplete;
		public static string MapCellReachable => _active.MapCellReachable;
		public static string MapCellExit => _active.MapCellExit;
		public static string MapExits => _active.MapExits;
		public static string MapNoExits => _active.MapNoExits;
		public static string CustomKindOrigin => _active.CustomKindOrigin;
		public static string CustomKindSigil => _active.CustomKindSigil;
		public static string CustomKindStyle => _active.CustomKindStyle;
		public static string CustomKindColour => _active.CustomKindColour;
		public static string CustomKindHairColour => _active.CustomKindHairColour;
		public static string CustomKindWarpaint => _active.CustomKindWarpaint;
		public static string CustomKindWarpaintColour => _active.CustomKindWarpaintColour;
		public static string CustomForMale => _active.CustomForMale;
		public static string CustomForFemale => _active.CustomForFemale;
		public static string CustomYours => _active.CustomYours;
		public static string CardTypeEncounter => _active.CardTypeEncounter;
		public static string CardTypeCompanion => _active.CardTypeCompanion;
		public static string DeckFull => _active.DeckFull;
		public static string DeckRuleNotMet => _active.DeckRuleNotMet;
		public static string DeckCardPinned => _active.DeckCardPinned;
		public static string DeckCannotAdd(string why) => string.Format(_active.DeckCannotAddFormat, why);
		public static string CardTokenToWin => _active.CardTokenToWin;
		public static string CardTokenWon => _active.CardTokenWon;
		public static string CardShardToWin => _active.CardShardToWin;
		public static string CardShardWon => _active.CardShardWon;
		public static string MapCellFigure => _active.MapCellFigure;
		public static string MapCellFigureBlocks => _active.MapCellFigureBlocks;
		public static string MapGoblinPatrol => _active.MapGoblinPatrol;
		public static string MapGoblinPatrolFacing(string direction) => string.Format(_active.MapGoblinPatrolFacingFormat, direction);
		public static string MapGoblinGuard => _active.MapGoblinGuard;
		public static string MapGoblinTracker => _active.MapGoblinTracker;
		public static string MapGoblin => _active.MapGoblin;
		public static string MapGoblinNext => _active.MapGoblinNext;
		public static string DirUp => _active.DirUp;
		public static string MapCursorBlocked(string direction) => string.Format(_active.MapCursorBlockedFormat, direction);
		public static string MapUnvisited(int index, int count) => string.Format(_active.MapUnvisitedFormat, index, count);
		public static string MapNoUnvisited => _active.MapNoUnvisited;
		public static string MapCannotGo(string direction) => string.Format(_active.MapCannotGoFormat, direction);
		public static string MapCanGo(string directions) => string.Format(_active.MapCanGoFormat, directions);
		public static string MapSteps(int count, string direction) => string.Format(_active.MapStepsFormat, count, direction);
		public static string MapFromYou(string steps) => string.Format(_active.MapFromYouFormat, steps);
		public static string DirDown => _active.DirDown;
		public static string DirLeft => _active.DirLeft;
		public static string DirRight => _active.DirRight;

		public static string ObjectiveComplete => _active.ObjectiveComplete;
		public static string ObjectiveFailed => _active.ObjectiveFailed;
		public static string NoObjectives => _active.NoObjectives;

		public static string Replacing => _active.Replacing;

		public static string DiceTargetFormat => _active.DiceTargetFormat;
		public static string DiceResultFormat => _active.DiceResultFormat;
		public static string DiceValuesFormat => _active.DiceValuesFormat;
		public static string DiceReached => _active.DiceReached;
		public static string DiceMissed => _active.DiceMissed;
		public static string DieFormat => _active.DieFormat;
		public static string DieMarked => _active.DieMarked;

		public static string CustomStyleFormat => _active.CustomStyleFormat;
		public static string CustomColourFormat => _active.CustomColourFormat;
		public static string CustomWarpaintFormat => _active.CustomWarpaintFormat;
		public static string CustomSigilFormat => _active.CustomSigilFormat;

		public static string FightStartFormat => _active.FightStartFormat;
		public static string FightStartPlain => _active.FightStartPlain;
		public static string EnemiesFormat => _active.EnemiesFormat;
		public static string EnemyOne => _active.EnemyOne;
		public static string EnemiesNone => _active.EnemiesNone;
		public static string Snared(string key) => string.Format(_active.SnaredFormat, key);
		public static string SnareFree => _active.SnareFree;

		public static string GlossaryBlock => _active.GlossaryBlock;
		public static string GlossaryDodge => _active.GlossaryDodge;
		public static string GlossaryEnemyPing => _active.GlossaryEnemyPing;
		public static string GlossaryFinisher => _active.GlossaryFinisher;
		public static string GlossaryFinisherReady => _active.GlossaryFinisherReady;
		public static string GlossaryArmourAbsorbed => _active.GlossaryArmourAbsorbed;
		public static string GlossaryArmourCrack => _active.GlossaryArmourCrack;
		public static string GlossaryArmourBroken => _active.GlossaryArmourBroken;
		public static string GlossaryArmourRestored => _active.GlossaryArmourRestored;
		// "Armoured, use Bash, E"; without a key when Bash has none to name.
		public static string ArmourUseBash(string key) =>
			string.IsNullOrEmpty(key) ? _active.ArmourUseBash : string.Format(_active.ArmourUseBashFormat, key);
		public static string ArmourBroken => _active.ArmourBroken;
		public static string ArmourRestored => _active.ArmourRestored;
		public static string GlossaryEnemyStep => _active.GlossaryEnemyStep;
		public static string GlossaryEnemyReach => _active.GlossaryEnemyReach;
		public static string GlossaryWallTones => _active.GlossaryWallTones;
		public static string GlossaryWallCollision => _active.GlossaryWallCollision;
		public static string GlossaryProjectile => _active.GlossaryProjectile;
		public static string GlossaryProjectileReflected => _active.GlossaryProjectileReflected;
		public static string GlossaryZonePrimed => _active.GlossaryZonePrimed;
		public static string GlossaryZoneArming => _active.GlossaryZoneArming;
		public static string GlossaryZoneActive => _active.GlossaryZoneActive;
		public static string GlossaryZoneInside => _active.GlossaryZoneInside;
		public static string GlossaryRecharge => _active.GlossaryRecharge;
		public static string GlossaryOutcomeHugeSuccess => _active.GlossaryOutcomeHugeSuccess;
		public static string GlossaryOutcomeSuccess => _active.GlossaryOutcomeSuccess;
		public static string GlossaryOutcomeNeutral => _active.GlossaryOutcomeNeutral;
		public static string GlossaryOutcomeFailure => _active.GlossaryOutcomeFailure;
		public static string GlossaryOutcomeHugeFailure => _active.GlossaryOutcomeHugeFailure;
		public static string GlossaryCardHarp => _active.GlossaryCardHarp;
		public static string GlossaryCardFlute => _active.GlossaryCardFlute;
		public static string GlossaryCardHandpan => _active.GlossaryCardHandpan;
		public static string GlossaryCardMusicBox => _active.GlossaryCardMusicBox;
		public static string GlossaryShuffle => _active.GlossaryShuffle;
		public static string GlossaryChanceHugeSuccess => _active.GlossaryChanceHugeSuccess;
		public static string GlossaryChanceSuccess => _active.GlossaryChanceSuccess;
		public static string GlossaryChanceFailure => _active.GlossaryChanceFailure;
		public static string GlossaryChanceHugeFailure => _active.GlossaryChanceHugeFailure;

		public static string KeyEnter => _active.KeyEnter;
		public static string KeyTab => _active.KeyTab;
		public static string KeyBackspace => _active.KeyBackspace;
		public static string DiceThrow(string key) => string.Format(_active.DiceThrowFormat, key);
		public static string WheelStop(string key) => string.Format(_active.WheelStopFormat, key);
		public static string PendulumStop(string key) => string.Format(_active.PendulumStopFormat, key);
		public static string KeyConfirmButton => _active.KeyConfirmButton;
		public static string RewardsCollect => _active.RewardsCollect;
		public static string SaveDelete => _active.SaveDelete;
		public static string ShopCannotAfford => _active.ShopCannotAfford;
		public static string ChallengeLocked => _active.ChallengeLocked;
		public static string Selected => _active.Selected;
		public static string DeckCountFormat => _active.DeckCountFormat;
		public static string BrowserCollection => _active.BrowserCollection;
		public static string BrowserDeck => _active.BrowserDeck;
		public static string BrowserEmpty => _active.BrowserEmpty;
		public static string ChallengeGoldToken => _active.ChallengeGoldToken;
		public static string ChallengeSilverToken => _active.ChallengeSilverToken;
		public static string ScreenAddons => _active.ScreenAddons;
		public static string AddonOwned => _active.AddonOwned;
		public static string AddonFree => _active.AddonFree;
		public static string AddonPaid => _active.AddonPaid;
		public static string AddonComingSoon => _active.AddonComingSoon;
		public static string ScreenMods => _active.ScreenMods;
		public static string ModWarning => _active.ModWarning;
		public static string ModMoveUp => _active.ModMoveUp;
		public static string ModMoveDown => _active.ModMoveDown;
		public static string ScreenMainMenu => _active.ScreenMainMenu;
		public static string ScreenMap => _active.ScreenMap;
		public static string ScreenChallengeMap => _active.ScreenChallengeMap;
		public static string ScreenDeckBuilder => _active.ScreenDeckBuilder;
		public static string ScreenInventory => _active.ScreenInventory;
		public static string ScreenCustomisation => _active.ScreenCustomisation;
		public static string CustomisationFinish => _active.CustomisationFinish;
		public static string ScreenPaused => _active.ScreenPaused;
		public static string ScreenResults => _active.ScreenResults;
		public static string SlotWeapon => _active.SlotWeapon;
		public static string SlotShield => _active.SlotShield;
		public static string SlotArtifact => _active.SlotArtifact;
		public static string SlotArmour => _active.SlotArmour;
		public static string SlotHelm => _active.SlotHelm;
		public static string SlotRing => _active.SlotRing;
		public static string SlotEmpty => _active.SlotEmpty;
		public static string InventorySlot(string name) => string.Format(_active.InventorySlotFormat, name);
		public static string InventoryCurrent(string item) => string.Format(_active.InventoryCurrentFormat, item);
		public static string InventoryOthers(int count) =>
			count == 1 ? _active.InventoryOtherOne : string.Format(_active.InventoryOthersFormat, count);
		public static string InventoryModifierSlot => _active.InventoryModifierSlot;
		public static string InventoryCards(int count) =>
			count == 1 ? _active.InventoryCardOne : string.Format(_active.InventoryCardsFormat, count);
		public static string SettingOn => _active.SettingOn;
		public static string SettingOff => _active.SettingOff;

		public static string PluginLoaded(string version) => string.Format(_active.PluginLoadedFormat, version);
	}
}
