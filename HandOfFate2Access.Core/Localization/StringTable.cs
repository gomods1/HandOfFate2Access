namespace HandOfFate2Access.Localization {
	/// <summary>
	/// Every word the mod itself authors and speaks, in one language. The field
	/// initializers are the English source of truth; a translation is another
	/// instance built with an object initializer that overrides the fields it
	/// translates (see the registry in <see cref="Strings"/>). A field a translation
	/// leaves unset keeps its English default.
	///
	/// TRANSLATOR NOTES:
	/// - Translate the string VALUES only; leave field names alone.
	/// - These are read aloud by a screen reader to a blind player. Keep them short.
	/// - "{0}" / "{1}" are placeholders filled at runtime; keep them, move them freely.
	/// - Text the game itself shows (card names, descriptions) is NOT here.
	/// </summary>
	public sealed class StringTable {
		// The two cards of the opening gender choice carry only art, no text.
		public string GenderMale = "Male";
		public string GenderFemale = "Female";

		// An encounter's "next page" arrow button, which has no text of its own.
		public string Continue = "Continue";

		// Appended to an encounter option the game greys out (requirements not met).
		public string OptionUnavailable = "unavailable";

		// Spoken in place of a face-down card's identity, which the player cannot see.
		public string CardFaceDown = "face down card";

		// The chance gambit: a face-down card while picking, spoken in place of its hidden
		// outcome. {0} its place from the left, {1} the number of cards, {2} where it lies
		// (one of the GambitAt* words): "Card 2 of 4, left of centre, face down".
		public string GambitCardFormat = "Card {0} of {1}, {2}, face down";
		public string GambitAtFarLeft = "far left";
		public string GambitAtLeft = "left of centre";
		public string GambitAtCentre = "centre";
		public string GambitAtRight = "right of centre";
		public string GambitAtFarRight = "far right";
		// Spoken when the chance cards are shown face up: {0} the number of cards, {1}
		// the tally of outcomes ("3 Success, 1 Failure", in the game's own words, each
		// from GambitTallyFormat: {0} the count, {1} the outcome).
		public string GambitIntroFormat = "{0} chance cards: {1}.";
		public string GambitTallyFormat = "{0} {1}";
		// A gambit over prize cards (food, gold) instead of chance cards: {0} the number of
		// cards, {1} each card from the left with its amount ("Supplies, 3 Food; ...").
		public string GambitIntroCardsFormat = "{0} cards, from the left: {1}.";
		// Its hint: the instruments play in the order just read, with no outcome sound.
		public string GambitTeachHintCards = "From left to right, each card plays its own instrument, in the order just read.";
		// The chance cards explained as they happen (the chance card hints setting). After
		// the tally, before the cards are played left to right:
		public string GambitTeachHint = "From left to right, each card plays its own instrument, then the sound of what it holds.";
		// When the cards turn face down and the shuffle starts:
		public string GambitShuffleHint = "Face down, shuffling. The cards take turns playing their instruments where they are on the table: follow the one you want.";
		// When the shuffle ends: with hints, and without.
		public string GambitPickHint = "Shuffle done. Each card is where its instrument was last heard. Left and right move between the cards, Enter turns the chosen one over.";
		public string GambitPickShort = "Pick a card.";

		// The status key's resource readout. {0} is the amount ({1} the maximum for health).
		public string StatHealthFormat = "{0} of {1} health";
		public string StatFoodFormat = "{0} food";
		public string StatGoldFormat = "{0} gold";
		public string StatFameFormat = "{0} fame";
		public string StatPoisonFormat = "poisoned {0}";
		// Live changes. {0} is the signed amount ("+7", "-4").
		public string DeltaHealthFormat = "{0} health";
		public string DeltaMaxHealthFormat = "{0} max health";
		// Spoken by the status key when no run is in progress.
		public string StatusNoRun = "No run in progress";

		// Mouse buttons, which the game's control hints draw as icons with no text.
		public string MouseLeft = "left click";
		public string MouseRight = "right click";
		public string MouseMiddle = "middle click";

		// Keyboard keys, which the game prints in English on its key caps. KeyArrowFormat's
		// {0} is a direction word (DirUp...).
		public string KeyLeftShift = "left shift";
		public string KeyRightShift = "right shift";
		public string KeyLeftCtrl = "left control";
		public string KeyRightCtrl = "right control";
		public string KeyLeftAlt = "left alt";
		public string KeyRightAlt = "right alt";
		public string KeySpace = "space";
		public string KeyEnter = "enter";
		public string KeyTab = "tab";
		public string KeyEscape = "escape";
		public string KeyBackspace = "backspace";
		public string KeyArrowFormat = "{0} arrow";

		// Map slots. "with" joins an encounter to a card stacked under it ("Goblins, with
		// Apple"). The state word follows the card: "here" is the player's slot,
		// "completed" a finished encounter, "reachable" a move the player can make now.
		// "exit" marks the level's exit. The directions list where the board continues
		// from a slot with no wall in between ("paths up, left").
		public string MapSlotAttached = "with";
		// What a card shows beyond its text, as icons and models: a token or a shard that
		// winning the encounter earns, or one already won this challenge. Spoken after the
		// card's name wherever a card is read (focus, the map, the deck builder).
		public string CardTokenToWin = "has a token";
		public string CardTokenWon = "token won";
		public string CardShardToWin = "has a shard";
		public string CardShardWon = "shard won";
		// A figure (a piece the game stands on a map card), and one that stops the player
		// moving there.
		public string MapCellFigure = "a figure stands on it";
		public string MapCellFigureBlocks = "a figure blocks the way";
		// The goblins of the Servant challenge, named by how they move: a patrol walks a
		// fixed route ({0} the way it faces, the card it enters next), a guard steps up or
		// down toward the player's row, a tracker follows the player, a plain goblin stands
		// still. Spoken on the card they stand on. MapGoblinNext: a goblin steps onto this
		// card after the player moves there, which catches them.
		public string MapGoblinPatrol = "goblin patrol";
		public string MapGoblinPatrolFacingFormat = "goblin patrol heading {0}";
		public string MapGoblinGuard = "goblin guard";
		public string MapGoblinTracker = "goblin tracker, follows you";
		public string MapGoblin = "goblin";
		public string MapGoblinNext = "a goblin moves here next";
		public string MapCellHere = "here";
		public string MapCellComplete = "completed";
		public string MapCellReachable = "reachable";
		public string MapCellExit = "exit";
		public string MapExits = "paths";
		public string MapNoExits = "dead end";
		public string DirUp = "up";
		public string DirDown = "down";
		public string DirLeft = "left";
		public string DirRight = "right";
		// The map's survey cursor (Shift plus arrows). A step with no card that way says so
		// before re-reading the current card ({0} a direction word): "No card further left".
		// Each read ends with where the card lies from the player: MapStepsFormat per axis
		// ({0} a count, {1} a direction word), joined into MapFromYouFormat's {0}:
		// "2 left, 1 up from you".
		public string MapCursorBlockedFormat = "No card further {0}";
		// Shift plus period and comma step the survey cursor through the cards not yet
		// visited, nearest first; each read ends with its place among them ({0} of {1}).
		public string MapUnvisitedFormat = "unvisited {0} of {1}";
		public string MapNoUnvisited = "No unvisited cards";
		// The map's own arrows (moving the player), pressed toward no reachable card. {0} a
		// direction word: "Can't go left".
		public string MapCannotGoFormat = "Can't go {0}";
		// Follows it: where the player can move. {0} is direction words ("down, right").
		public string MapCanGoFormat = "you can go {0}";
		public string MapStepsFormat = "{0} {1}";
		public string MapFromYouFormat = "{0} from you";

		// Challenge objectives: appended to an objective the game marks done or failed
		// (shown only by colour and a strikethrough). The last is the objectives key's
		// answer when none are shown.
		public string ObjectiveComplete = "completed";
		public string ObjectiveFailed = "failed";
		public string NoObjectives = "No objectives";

		// Joins an offered item to the item it would replace when equipped ("Warrior's
		// Axe, Damage 21, replacing Neglected Sword, Damage 13").
		public string Replacing = "replacing";

		// Dice minigame. {0} is a number (target, total, a die's face) or, for the dice
		// list, the faces joined by commas. "marked for reroll" follows a die the
		// player has chosen to throw again.
		public string DiceTargetFormat = "target {0}";
		public string DiceResultFormat = "total {0}";
		public string DiceValuesFormat = "dice {0}";
		public string DiceReached = "target reached";
		public string DiceMissed = "below target";
		public string DieFormat = "die {0}";
		public string DieMarked = "marked for reroll";
		// The minigames' moments of action, each with the key that acts ({0}: KeyEnter on a
		// keyboard, KeyConfirmButton on a controller). The dice in hand, waiting to be thrown:
		public string DiceThrowFormat = "dice in hand, {0} to throw";
		// The wheel, spinning: confirm starts it slowing down.
		public string WheelStopFormat = "wheel spinning, {0} to slow it down";
		// The pendulum, swinging: the tone under it is the outcome (higher is better).
		public string PendulumStopFormat = "pendulum swinging, higher tones are better outcomes, {0} to stop it";
		// A controller's confirm button, named generically (pads label it differently).
		public string KeyConfirmButton = "the confirm button";

		// The button that takes the revealed reward cards into your collection.
		public string RewardsCollect = "Collect the cards";
		// A save slot's delete button (an icon in the game).
		public string SaveDelete = "Delete this save";
		// A shop item the player cannot pay for.
		public string ShopCannotAfford = "not enough to buy";

		// A challenge on the challenge map: locked, or the token its completion earned
		// (gold for the full objective, silver for a plain win).
		public string ChallengeLocked = "locked";
		// The option currently chosen in a list that marks it only by highlight.
		public string Selected = "selected";
		// A deck in the deck builder: {0} cards in it, {1} the deck's size.
		public string DeckCountFormat = "{0} of {1} cards";
		// The deck builder's two card lists, and an empty one.
		public string BrowserCollection = "available cards";
		public string BrowserDeck = "your deck";
		public string BrowserEmpty = "no cards";
		// Why Enter would not move the centre card in the deck builder. DeckFull and
		// DeckRuleNotMet follow an available card (the game's own rule text is used when it
		// has one); DeckCardPinned follows a card in the deck. DeckCannotAddFormat answers a
		// refused Enter, {0} the reason.
		public string DeckFull = "deck full, remove a card first";
		public string DeckRuleNotMet = "a deck rule stops it being added";
		public string DeckCardPinned = "can't be removed";
		public string DeckCannotAddFormat = "Can't add it: {0}";
		public string ChallengeGoldToken = "gold token earned";
		public string ChallengeSilverToken = "silver token earned";

		// The additional content screen (the main menu's DLC list). Each item's state
		// is shown only as a banner: a tick, "Free", or a price.
		public string ScreenAddons = "Additional content";
		public string AddonOwned = "owned";
		public string AddonFree = "free, not claimed yet";
		public string AddonPaid = "for sale, not owned";
		public string AddonComingSoon = "coming soon";
		// The mods screen: a mod row's warning icon, and its arrow buttons (icons only).
		public string ScreenMods = "Mods";
		public string ModWarning = "warning, inspect it for details";
		public string ModMoveUp = "Move up";
		public string ModMoveDown = "Move down";

		// Character customisation options the game shows only as art. {0} is the
		// option's number within its kind.
		public string CustomStyleFormat = "style {0}";
		public string CustomColourFormat = "colour {0}";
		public string CustomWarpaintFormat = "war paint {0}";
		public string CustomSigilFormat = "sigil {0}";
		// What kind of option a customisation card is, said after its name on a reward
		// card ("The Mage, sigil"). Looks and war paint add which characters they are for.
		public string CustomKindOrigin = "origin";
		public string CustomKindSigil = "sigil";
		public string CustomKindStyle = "look";
		public string CustomKindColour = "outfit colour";
		public string CustomKindHairColour = "hair colour";
		public string CustomKindWarpaint = "war paint";
		public string CustomKindWarpaintColour = "war paint colour";
		public string CustomForMale = "for male characters";
		public string CustomForFemale = "for female characters";
		// After a sigil card that is the player's own (chosen at character creation).
		public string CustomYours = "yours";
		// A card's type where the game has no name for it, said after the card's name when
		// the card prints no kind of its own.
		public string CardTypeEncounter = "encounter";
		public string CardTypeCompanion = "companion";

		// Character customisation looks, described from the card art (the game shows
		// hairstyles and outfit colours only as pictures). Short noun phrases.
		public string StyleShaved = "shaved head";
		public string StyleShortCrop = "very short crop";
		public string StyleCurlyTop = "short curly top, faded sides";
		public string StyleTiedBack = "long hair tied back";
		public string StyleHighBun = "hair swept up into a high bun";
		public string StyleSweptBack = "shoulder-length hair swept back";
		public string StyleBuzzCut = "buzz cut";
		public string StyleShortSpiky = "short spiky crop";
		public string StyleMessyFringe = "short messy hair with a fringe";
		public string StyleBald = "bald";
		public string StyleHoodBlueMask = "dark blue hood and face mask with red trim";
		public string StyleHoodSandMask = "sand-coloured hood and face mask with gold trim";
		public string StyleMohawk = "mohawk";
		public string StyleTopknot = "samurai topknot";
		public string StyleSlickedBack = "short hair slicked back, shaved sides";
		public string StyleShortNeat = "short neat hair";
		public string StyleWavyFringe = "wavy hair falling over the forehead";
		public string StyleCombedBack = "medium hair combed back to the neck";
		public string StyleWildBeard = "wild swept-back hair with a short beard";
		public string StyleSpikedCrest = "tall spiked crest";
		public string StyleSpikedTufts = "shaved crown with spiked tufts at the back";
		public string StyleBraidedCrest = "braided crest with a braid down the back";
		public string StyleCornrows = "short cornrows";
		public string StyleCurlyBun = "curly hair in a high bun";
		public string StyleTwoBuns = "curly hair in two buns";
		public string StyleShortBraidedCrest = "short braided crest, shaved sides";
		public string StyleBraidedKnot = "braided crest with a loose knot on top";
		public string StyleBraidedShortTail = "braided crest with a short tail";
		public string StyleBraidedPonytail = "braided crest with a ponytail";
		public string StyleHoodBlue = "dark blue hood";
		public string StyleHoodBrown = "brown hood";
		public string StyleHoodDarkRed = "dark red hood";
		public string StyleHoodGrey = "pale grey hood";
		public string StyleLongWavy = "long loose wavy hair";
		public string StyleWavyBun = "wavy hair in a loose bun";
		public string StyleBraidedSpikes = "braids piled into a spiky crest";
		public string StyleChoppy = "short choppy hair";
		public string ColourWoodland = "rust red and cream with an orange stripe";
		public string ColourRanger = "forest green and red";
		public string ColourCrusade = "purple and cream with dark red";
		public string ColourTwilight = "navy blue and gold with a red stripe";
		public string ColourNavyRed = "navy blue with a red stripe";
		public string ColourCreamGold = "cream with a gold stripe";

		// Combat bookkeeping. FightStartFormat's {0} is the enemy count phrase below.
		// EnemiesFormat's {0} is a number of two or more; EnemyOne is exactly one.
		// EnemiesNone is spoken when the last enemy falls, and by the locator key when
		// there is nothing to point at.
		public string FightStartFormat = "Fight, {0}";
		public string FightStartPlain = "Fight";
		public string EnemiesFormat = "{0} enemies";
		public string EnemyOne = "1 enemy";
		public string EnemiesNone = "no enemies left";
		// An enemy has latched on (gnomes, a mage's snare): the game wants the counter
		// key mashed until free. {0} is the live name of that key.
		public string SnaredFormat = "Snared, press {0} repeatedly to break free";
		public string SnareFree = "free";

		// The sound examples in the accessibility mod menu: every mod sound with its
		// meaning. Each entry leads with the sound's name so the player can move on fast.
		public string GlossaryBlock = "Clang: an attack you can counter, press Counter now";
		public string GlossaryDodge = "Crack: an attack you cannot block, dodge out of the way";
		public string GlossaryEnemyStep = "Footstep: an enemy on the move, from where it is. The nearest three are heard";
		public string GlossaryEnemyReach = "Double knock: an enemy has come within reach of your weapon";
		public string GlossaryEnemyPing = "Enemy ping: the nearest enemy, from its direction, answers the C key";
		public string GlossaryFinisher = "Rising swish: an enemy is down and can be finished. It repeats from where the enemy lies, faster as you get closer: go to it and press the finisher key. The C key answers with it when the nearest enemy can be finished";
		public string GlossaryFinisherReady = "Swish with a bright ding: the finisher will land now. Press the finisher key, pushing toward the sound";
		public string ArmourUseBashFormat = "Armoured, use Bash, {0}";
		public string ArmourUseBash = "Armoured, use Bash";
		public string ArmourBroken = "Armour broken";
		public string ArmourRestored = "Armour restored";
		public string GlossaryArmourAbsorbed = "Dull clunk: your hit glanced off armour. Break it with Bash";
		public string GlossaryArmourCrack = "Crunch: your Bash cracked the armour";
		public string GlossaryArmourBroken = "Crash: the armour is broken, your hits land in full again";
		public string GlossaryArmourRestored = "Rising shimmer: the armour is growing back. Quiet while it mends, louder when it is whole again";
		public string GlossaryWallTones = "Wall tones: wind that grows as you near a wall, right, left, ahead, behind";
		public string GlossaryWallCollision = "Bump: you walked into a wall";
		public string GlossaryProjectile = "Projectile: a shot flying at you, move out of its path";
		public string GlossaryProjectileReflected = "Reflected projectile: your reflected shot flying back at an enemy";
		public string GlossaryZonePrimed = "Primed trap: idle, approaching sets it off";
		public string GlossaryZoneArming = "Arming hazard: harmless for a moment, get away";
		public string GlossaryZoneActive = "Active hazard: an area that hurts, move away from the sound";
		public string GlossaryZoneInside = "Inside a hazard: you are taking damage, move now";
		public string GlossaryRecharge = "Chime: an ability is ready again, weapon on the left, artefact on the right";
		public string GlossaryOutcomeHugeSuccess = "Pendulum and wheel, bright shimmer: huge success";
		public string GlossaryOutcomeSuccess = "Pendulum and wheel, clear high tone: success";
		public string GlossaryOutcomeNeutral = "Wheel, plain middle tone: a card that is not a chance outcome";
		public string GlossaryOutcomeFailure = "Pendulum and wheel, dull low hum: failure";
		public string GlossaryOutcomeHugeFailure = "Pendulum and wheel, low buzz: huge failure";
		// The chance cards' own sounds: each card's instrument (by where the card lies when
		// the cards are shown), the shuffle, and the sound of each outcome.
		public string GlossaryCardHarp = "Low harp: the first card from the left";
		public string GlossaryCardFlute = "Flute: the second card";
		public string GlossaryCardHandpan = "Handpan: the third card";
		public string GlossaryCardMusicBox = "Music box: the fourth card";
		public string GlossaryShuffle = "Shuffle: the cards take turns where they lie. Here the harp moves from left to right and the music box from right to left";
		public string GlossaryChanceHugeSuccess = "Chance cards, rising chimes: huge success";
		public string GlossaryChanceSuccess = "Chance cards, two wooden notes going up: success";
		public string GlossaryChanceFailure = "Chance cards, two low notes going down: failure";
		public string GlossaryChanceHugeFailure = "Chance cards, drum and a falling boom: huge failure";

		// Screen names, spoken on entering each screen.
		public string ScreenMainMenu = "Main menu";
		public string ScreenMap = "Map";
		public string ScreenChallengeMap = "Challenge map";
		public string ScreenDeckBuilder = "Deck builder";
		public string ScreenInventory = "Inventory";
		public string ScreenPaused = "Paused";
		public string ScreenResults = "Results";
		// The character appearance screen, and how to leave it (spoken on entering it).
		public string ScreenCustomisation = "Appearance";
		public string CustomisationFinish = "F3 to finish";
		// The inventory's equipment slots, read on entering it; "empty" for a free slot.
		public string SlotWeapon = "Weapon";
		public string SlotShield = "Shield";
		public string SlotArtifact = "Artefact";
		public string SlotArmour = "Armour";
		public string SlotHelm = "Helmet";
		public string SlotRing = "Ring";
		public string SlotEmpty = "empty";
		// An inventory slot under focus: "Weapon slot, currently Soldier's Sword, 2 other
		// items to equip". {0} of InventorySlotFormat is a slot name above, of
		// InventoryCurrentFormat an item, of InventoryOthersFormat a number of two or more.
		// The modifier slot holds blessings, curses and the like: "Blessings and curses, 3 cards".
		public string InventorySlotFormat = "{0} slot";
		public string InventoryCurrentFormat = "currently {0}";
		public string InventoryOthersFormat = "{0} other items to equip";
		public string InventoryOtherOne = "1 other item to equip";
		public string InventoryModifierSlot = "Blessings and curses";
		public string InventoryCardsFormat = "{0} cards";
		public string InventoryCardOne = "1 card";

		// A settings checkbox's state, which the game shows only as a cross in a box.
		public string SettingOn = "on";
		public string SettingOff = "off";

		// Spoken after the inventory's name when it opens: how to look through it, and
		// InventoryCloseFormat with {0} the key that closes it (KeyBackspace).
		public string InventoryBrowse = "arrows to browse";
		public string InventoryCloseFormat = "{0} to close";
		// Spoken after "Main menu" when it appears.
		public string HelpHint = "Press F4 for help at any time. The last option, Accessibility mod, has help topics, sound examples and settings.";

		// Review buffers (Ctrl+arrows). The names are spoken when switching buffers:
		// "Events, 12 items". Top / end are the edges of a buffer.
		public string ReviewDetails = "Details";
		public string ReviewEvents = "Events";
		public string ReviewStatus = "Status";
		public string ReviewBufferFormat = "{0}, {1} items";
		public string ReviewBufferOneFormat = "{0}, 1 item";
		public string ReviewEmptyFormat = "{0}, empty";
		public string ReviewNothing = "Nothing to review";
		public string ReviewTop = "top";
		public string ReviewEnd = "end";

		// The help list (F4). The title names the screen: "Help, Map".
		public string HelpTitleFormat = "Help, {0}";
		public string HelpClosed = "Help closed";
		// Context names for help that are not screens announced elsewhere.
		public string ScreenMenu = "Menu";
		public string ScreenEncounter = "Encounter";
		public string ScreenCombat = "Combat";
		public string ScreenDice = "Dice";
		public string ScreenWheel = "Wheel";
		public string ScreenPendulum = "Pendulum";
		public string ScreenChanceCards = "Chance cards";
		// Help entries: one line each, the game's keys for the screen.
		public string HelpMenuNav = "Arrows move, Enter or Space confirms, Backspace goes back.";
		public string HelpAddons = "Up and down move through the content; each says whether it is owned, free to claim or for sale. Enter on an item moves to its Claim or Buy button, Backspace goes back to the list.";
		public string HelpMods = "Up and down move through the installed mods, Enter turns one on or off. Left and right brackets switch the save slot.";
		public string HelpBarKeys = "Actions on F1, F2 and F3 are announced with their key when they appear.";
		public string HelpPauseKey = "Escape opens the pause menu.";
		public string HelpMapMove = "Arrows choose a neighbouring card on the map, Enter moves to it.";
		public string HelpMapInventory = "F1 opens the inventory, Backspace closes it.";
		public string HelpMapView = "F3 shows the whole map, F2 makes camp when you can.";
		public string HelpMapCursor = "Shift plus arrows moves a survey cursor over the whole board, reading each card. It only reads.";
		public string HelpMapUnvisited = "Shift plus period takes the survey cursor to the next card you have not visited, nearest first, and Shift plus comma to the previous one.";
		public string HelpInventoryMove = "Arrows move between the equipment slots. Enter on a slot lists what you can put in it, Enter again equips it.";
		public string HelpInventoryClose = "Backspace goes back, and closes the inventory.";
		public string HelpEncounterChoose = "Up and down choose an option, Enter takes it. Number keys 1 to 9 take an option directly.";
		public string HelpEncounterUnavailable = "An option you cannot take says unavailable.";
		public string HelpEncounterInventory = "F1 opens the inventory when the encounter allows it.";
		public string HelpDeckModes = "Up and down move between the decks, Enter opens one.";
		public string HelpDeckCards = "In a deck, left and right move through the cards and up and down switch between the available cards and your deck. Enter on an available card adds it to your deck; Enter on a card in your deck takes it out. A card that can't move says why. F2 or I shows the card large, Backspace returns.";
		public string HelpDeckStart = "F3 starts the challenge. Backspace goes back.";
		public string HelpDeckRecommend = "F2 fills your decks with the game's recommended cards, keeping your choices or starting over. F3 only starts once every deck has enough cards.";
		public string HelpDeckFilters = "In a deck, F1 shows filters to narrow the available cards.";
		public string HelpChallengeTokens = "Each challenge says whether it is locked and which token you earned.";
		public string HelpCustomisation = "Up and down move between what you can change, Enter opens it, left and right browse the choices, Enter takes one. F3 finishes; there is no going back from this screen.";
		public string HelpPaused = "Up and down move, Enter chooses. Escape resumes.";
		public string HelpGlossary = "The last option, Accessibility mod, has help topics, every sound the mod plays with an example, and the settings.";
		public string HelpDice = "Enter throws the dice. The total and whether it reached the target are spoken.";
		public string HelpDiceReroll = "If the total is under the target and you have rolls left, choose which dice to throw again: left and right arrows move between the dice, Enter marks or unmarks one, F3 throws the marked dice. Reaching or passing the target ends the roll.";
		public string HelpWheel = "Enter slows the wheel down. Each card passing the pointer plays its outcome tone: higher is better.";
		public string HelpPendulum = "Enter stops the pendulum. The tone under it is the outcome: higher is better.";
		public string HelpChanceShown = "First the cards are shown face up. You hear how many there are and what they hold, then each card from left to right: its own instrument, then the sound of its outcome.";
		public string HelpChanceShuffle = "Then the cards turn face down and are shuffled. The cards take turns playing their instruments from where they are on the table, left to right in your ears: follow the one you want.";
		public string HelpChancePick = "When the shuffle stops, left and right move between the cards from far left to far right, and Enter turns the chosen card over. Its outcome is spoken.";
		public string HelpChanceSounds = "Every card instrument and outcome sound is in the Accessibility mod menu, under chance cards and minigame sounds.";
		// Combat actions, each spoken with its bound key: "Dodge: space".
		public string HelpKeyFormat = "{0}: {1}";
		public string CombatMove = "Move";
		public string CombatAttack = "Attack";
		public string CombatBash = "Bash";
		public string CombatCounter = "Counter";
		public string CombatDodge = "Dodge";
		public string CombatWeapon = "Weapon ability";
		public string CombatArtifact = "Artefact ability";
		public string CombatCompanion = "Companion";
		public string CombatFinisher = "Finish a fallen enemy";
		public string CombatRun = "Run";
		public string CombatCamera = "Turn the camera";
		public string HelpCombatCues = "A clang means counter now, a crack means dodge. On the lines that follow, Enter plays each sound.";
		public string HelpCombatSnare = "Snared means an enemy has latched on: press counter repeatedly until free.";
		// Spoken in the combat help while Counter has no key or pad button bound.
		public string HelpCombatMouseOnly = "Counter is bound only to a mouse button. Give it a key in Settings, Controls, Bind Input.";
		public string HelpCombatLocator = "C: a beep from the nearest enemy. Once they are gone, from the exit or dropped loot.";
		public string HelpCombatGlossary = "Every combat sound, with an example, is in the Accessibility mod menu, the last option of the pause menu.";
		// The mod's own keys, listed on every screen.
		public string HelpModHelp = "F4: this help. Up and down arrows read it, F4 or Backspace closes it.";
		public string HelpModReview = "Control plus up arrow reads the next detail of what has focus, control plus down arrow the previous one.";
		public string HelpModReviewSwitch = "Control plus left or right arrow switches between details, recent events and your status.";
		public string HelpModStatus = "Slash or H: health, food, gold, fame and poison.";
		public string HelpModObjectives = "O: the challenge objectives.";
		public string HelpModMenu = "Shift plus F4: the Accessibility mod menu, with help topics, sound examples and settings. It is also the last option of the main menu and the pause menu.";

		// The accessibility mod menu, an option added at the bottom of the main menu and the
		// pause menu. The description is the option's second line in the pause menu.
		public string ModMenuTitle = "Accessibility mod";
		public string ModMenuDescription = "Help topics, sound examples and settings.";
		public string ModMenuClosed = "Accessibility mod menu closed";
		// Spoken when the menu opens, after its title.
		public string ModMenuHint = "Up and down move, Enter opens, Backspace goes back.";
		// Spoken when Shift plus F4 is pressed in a running fight.
		public string ModMenuInFight = "Pause first: the Accessibility mod menu is the last option of the pause menu.";
		// The menu's sections, and what is said on entering them.
		public string ModMenuCombatSounds = "Combat sounds";
		public string ModMenuMinigameSounds = "Chance cards and minigame sounds";
		public string ModMenuHelpTopics = "Help topics";
		public string ModMenuSettings = "Settings";
		public string ModMenuSoundsHint = "Enter plays the sound.";
		public string ModMenuSettingsHint = "Left and right change the value. Control plus up arrow explains the setting.";
		// Help topics that are not screens named elsewhere, and the getting started lines.
		public string TopicStart = "Getting started";
		public string TopicModKeys = "Mod keys";
		public string TopicStartFocus = "Everything you move to is read. Control plus up arrow reads more about it, one line at a time; control plus down arrow goes back.";
		public string TopicStartEvents = "What the game says on its own, the dealer included, is read as it happens. Control plus right arrow switches to the list of recent events.";
		public string TopicStartFights = "Fights are played by ear: a clang means counter now, a crack means dodge, wind means a wall is near, and C pings the nearest enemy. Hear each one under Combat sounds in this menu.";
		public string TopicStartMinigames = "Chance cards, dice, the wheel and the pendulum each have their own sounds; the Chance cards topic explains them.";

		// Settings in the accessibility mod menu: each name is spoken with its value
		// ("Wall tones volume, 80 percent"), and its explanation is the details line
		// (Control plus up arrow). {0} of PercentFormat is a number.
		public string PercentFormat = "{0} percent";
		public string SettingVolumeOff = "off";
		public string SettingVolume = "Mod sounds volume";
		public string SettingVolumeHelp = "The volume of every sound the mod plays. The game's own sounds are not affected.";
		public string SettingAttackVolume = "Attack warnings volume";
		public string SettingAttackVolumeHelp = "The clang that means counter and the crack that means dodge. Turning these off makes fights much harder.";
		public string SettingArmourVolume = "Armour volume";
		public string SettingArmourVolumeHelp = "The sounds of an enemy's armour: your hit glancing off, Bash cracking it, breaking and growing back.";
		public string SettingEnemyVolume = "Enemy movement volume";
		public string SettingEnemyVolumeHelp = "The footsteps of the nearest moving enemies, and the double knock when one comes within reach of your weapon.";
		public string SettingWallVolume = "Wall tones volume";
		public string SettingWallVolumeHelp = "The wind that grows as you near a wall, and the bump when you walk into one.";
		public string SettingProjectileVolume = "Projectiles volume";
		public string SettingProjectileVolumeHelp = "Arrows, bolts and spells flying through the air.";
		public string SettingHazardVolume = "Traps and hazards volume";
		public string SettingHazardVolumeHelp = "Traps, and areas that hurt you while you stand in them.";
		public string SettingLocatorVolume = "Locator volume";
		public string SettingLocatorVolumeHelp = "The ping of the nearest enemy when you press C, the exit and loot beacons, and the swish of an enemy you can finish.";
		public string SettingChimeVolume = "Ability chimes volume";
		public string SettingChimeVolumeHelp = "The chime when your weapon or artefact ability is ready again.";
		public string SettingMinigameVolume = "Chance cards and minigames volume";
		public string SettingMinigameVolumeHelp = "The chance cards' instruments and outcome sounds, and the tones of the pendulum and the wheel.";
		public string SettingLockCamera = "Lock the camera";
		public string SettingLockCameraHelp = "In fights, the camera keys and the right stick no longer turn the camera, so left and right always mean the same.";
		public string SettingListenAtPlayer = "Hear from your character";
		public string SettingListenAtPlayerHelp = "The game's own sounds, such as footsteps and swings, are heard from where your character stands instead of from toward the camera.";
		public string SettingVoiceLines = "Read spoken lines";
		public string SettingVoiceLinesHelp = "Reads what the dealer and the other characters say, as they say it.";
		public string SettingResourceChanges = "Announce resource changes";
		public string SettingResourceChangesHelp = "Says when your health, food, gold or fame goes up or down.";
		public string SettingActionKeys = "Announce action keys";
		public string SettingActionKeysHelp = "Says the F1, F2 and F3 actions when they appear, with their key.";
		public string SettingChanceHints = "Chance card hints";
		public string SettingChanceHintsHelp = "Explains each step of the chance cards as it happens. Turn it off once you know them.";
		public string SettingGoblinWarnings = "Goblin warnings";
		public string SettingGoblinWarningsHelp = "On the map, says when a goblin will step onto a card right after you move there. Turn it off to work it out yourself from where each patrol is heading.";

		// Plugin lifecycle. Spoken once at startup. {0} is the mod version.
		public string PluginLoadedFormat = "Hand of Fate 2 Access version {0} loaded";
	}
}
