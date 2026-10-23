# Hand of Fate 2 Access

A mod that makes Hand of Fate 2 playable without sight. Everything the game shows is spoken through your screen reader, and the real-time parts (combat, the pendulum, the wheel, the chance-card shuffle) are made playable by ear with added sound cues.

This is a rebuild of the abandoned "Hand of Fate 2 Accessibility Mod", based on Hand of Fate Access, the mod for the first game.

## Requirements

- Windows.
- Hand of Fate 2 (Steam).
- A screen reader (NVDA, JAWS and others). Without one the mod falls back to Windows' built-in speech.

## Installation

The easiest way is the installer, `HandOfFate2AccessibilityInstaller.exe`, attached to every release: it finds the game through Steam, installs or updates the mod, and can uninstall it. It works with a screen reader. To install by hand instead:

1. Extract the release zip into the Hand of Fate 2 folder (the one containing `Hand of Fate 2.exe`; on a default Steam install, `C:\Program Files (x86)\Steam\steamapps\common\Hand of Fate 2`). Allow it to merge folders.
2. Start the game. A few seconds after launch you hear "Hand of Fate 2 Access version ... loaded".

The zip contains the mod loader (BepInEx) already configured.

## How it works

Hand of Fate 2 already has full keyboard and controller navigation; the mod reads it. Move through menus, cards and choices with the game's own controls (arrows, Enter or Space to confirm, Tab to go back, Escape for the pause menu) and each item speaks:

- Cards read their name, type, stats and every trait with its rules text. Equipment you are offered is compared with what you are wearing.
- Encounters read their title, story text, results ("+7 food") and options, including the cost or requirement printed beside an option and whether it is unavailable.
- The dealer's and characters' voiced lines are read in full in your language, whether or not the game's Subtitles setting is on.
- Questions in the bottom bar ("Equip the Warrior's Axe? Yes, No") are read when they appear.
- The challenge objectives are read when they appear, change, are completed or fail.
- The end of a run is announced with the game's own verdict (death by wounds or starvation, a forfeit, a failed challenge), then the results.
- Dialogues, the combat controls tutorial (with the key for each action), creature introductions and character customisation (origins, sigils, and a description of each hairstyle and outfit colour) are read.
- Food, gold, fame and health changes are announced as they happen.
- Screen names are spoken as you enter them (main menu, map, challenge map, deck builder, inventory, pause, results). The inventory also says what is in every slot, empty ones included, since the game's cursor skips empty slots.
- The end-of-run score table (Endless) is read row by row with its column names.
- Shops: each item reads its price ("37 gold") and says when you cannot afford it; confirm actions that do something specific ("Buy, Enter") are announced with their key.
- Actions on a function key (F1 Inventory, F3 Map, F3 Finish on the character screen, F3 Start in the deck builder) are announced with their key when they appear. The inventory says that Tab (the game's back key) closes it.
- In an encounter, the number keys 1 to 9 take an option directly (a game feature).
- The challenge map: each challenge reads its name and whether it is locked (with what unlocks it), completed, or has earned its gold or silver token; Campaign, Adventure, Endless and Campfire say which is showing (Campfire holds the change-appearance screen). Opening a challenge reads its title and description.
- Settings: every row reads its name and value (lists, checkboxes, sliders, key bindings), and the new value is spoken as you change it.
- Reward cards turned over at the end of a challenge are read as they are revealed.
- The deck builder: each deck says its name, what it is for and how full it is ("Encounters, 13 of 13 cards"); inside, left and right move through the available cards or your deck (down and up switch between them) and each card says its name, kind and stats (Ctrl + Up reads the rest); confirm adds or removes it and the new count is spoken.
- Questions in the bottom bar name their keys on a keyboard ("Leave, Tab; Accept die, F3"). Key names are spoken in your language.

## Keys the mod adds

- **F4**: help for the screen you are on, the game's keys there first, then the mod's. Up and down arrows read it one entry at a time; F4 or Tab closes it. (F1 is the game's own inventory key.)
- **Ctrl + Up / Ctrl + Down**: read the details of whatever has focus one line at a time. A card is announced short (name, kind, stats); its traits and rules are here. Works everywhere, combat included.
- **Ctrl + Left / Ctrl + Right**: switch what Ctrl + Up and Down read: **Details** (the focused thing), **Events** (everything announced so far, newest first: encounter text, prompts, fight starts, health warnings, results) and **Status** (health, food, gold, fame, poison and what is equipped in each slot). Empty lists are skipped.
- **H** (or slash, or right-stick click): your health, food, gold, fame and poison.
- **O**: the current challenge objectives.
- **Shift + arrows** (or the right stick) on the map: a survey cursor that walks the whole visible board card by card, reading each card, whether you are there, whether you can move to it, whether it is the exit, and which directions continue from it. It only reads; move with the game's own arrows and Enter. Ctrl + Up then reads the surveyed card in full.
- **Shift + period** on the map: the survey cursor jumps to the next card you have not visited yet, nearest first; **Shift + comma** goes back to the previous one. Each read ends with where the card lies from you and its place in the list ("unvisited 2 of 5"); after the last it starts again from the nearest. A shop or other card you have already entered no longer counts. It only reads, like Shift + arrows.
- **C** (or left-stick click) in combat: a beep from the direction of the nearest enemy. Once the enemies are gone it points at what is left to walk to instead (the exit, a boss's closing circle, dropped gold or food), or says "no enemies left".
- **Enemies you can finish**: an enemy knocked to the ground plays a rising swish from where it lies, repeating faster as you get closer, until it gets up. Walk to it and press the game's finisher key. When the nearest enemy can be finished, **C** answers with that swish instead of the usual beep. Once the finisher would land (close enough, and in the direction you push or face), the swish gets a bright ding on top: press the finisher key now, pushing toward the sound. C gives the ding too when the nearest enemy is in reach.

## Combat by ear

- The fight starts with the number of enemies ("Fight, 3 enemies") and the count is spoken as it changes. Your health is spoken when it falls below half, a quarter and a tenth.
- **Clang**: an attack you can counter. Press defend. The game binds Attack and Defend only to mouse buttons, so the mod gives them keyboard keys: **V attacks and B defends** (unless you bind your own in Settings, Controls, Bind Input). Announcements name the keyboard key, or your controller's button when a pad is in use.
- **Crack**: an attack you cannot block. Dodge (Space).
- **Wind**: walls. The wind on each side grows as you get close to a wall on that side; a bump means you walked into one.
- Projectiles, ground hazards, beams and traps each have their own sound, always placed where the danger is: move away from the sound.
- A chime tells you a weapon (left) or artefact (right) ability is ready again.
- **Snared**: when an enemy latches on (gnome swarms, a mage's snare) you hear "Snared" with the key to press repeatedly until "free". The small hits you take meanwhile are the snare itself.
- **Armour**: some enemies (the Terror of Westwend, corrupted foes) wear armour that only **Bash** damages (E on the keyboard by default, B on a controller; the mod adds X when Bash has no key). A dull clunk means your hit glanced off; the first time on each enemy you also hear "Armoured, use Bash" with the key. A crunch means your Bash cracked it; a crash and "Armour broken" mean your hits land in full again; a rising shimmer means the armour is growing back (quiet while it mends, louder with "Armour restored" when broken armour comes back). Each sound comes from that enemy, and they have their own volume in the settings. The game's Stun control is called Bash everywhere in the mod, as on the game's cards.
- If combat is too hard, the game's Apprentice Mode (Settings, General; the game also offers it after a few deaths) automates ripostes and finishers and raises your damage. The mod works the same either way.
- In fights that end at an exit (trap rooms), the exit pings from the next turn of the walkable path toward it, so walking toward the ping leads around walls to the exit; dropped gold and food ping the same way with a lighter sound.
- A trap that fires down the lane you are standing in (arrows, darts) plays the crack: move.

The pause menu has a new option at the bottom, **Sound glossary**: arrow through it to hear what each sound means, Enter plays it, Tab closes it.

## Minigames

- **Chance cards**: the cards are shown, each with its own tone and its outcome spoken from its position, then each card's tone follows it through the shuffle. Pick the position you tracked; the cards read as "Slot 1", "Slot 2"...
- **Pendulum**: while it swings you hear the outcome under the beam, moving left and right with it: higher and brighter is better. Press confirm on the tone you want; focusing it reminds you of this.
- **Wheel**: the cards on the wheel are read when it is laid out; while it spins each segment ticks past in its outcome's voice; press confirm to slow it down. Where it lands is spoken.
- **Dice**: press confirm to throw (the game otherwise wants a mouse drag). The target, each roll's total and dice, and whether you reached the target are spoken; dice you can reroll read their value and whether they are marked, and marking one is announced. F3 rerolls.

## Languages

The mod follows the game's language: all twelve of the game's languages are covered (everything the game itself says is always in your language). This README exists in every one of them in the `docs` folder, as Markdown and as HTML (`docs/README.<language>.md` and `docs/html/README.<language>.html`).

## For developers

See `CLAUDE.md`. `setup-bepinex.ps1` installs the loader, `build.ps1` builds and deploys, `test.ps1` runs the offline tests.
