# Changelog

Player-facing changes to Hand of Fate 2 Access, newest first. An entry describes what a player will hear or notice: a new or changed announcement or sound, new controls, a fixed silence. Internal work (refactors, tests, tooling, docs) does not get entries.

## 2.1.0

* In the Servant challenge the goblins on the map are named instead of "a figure stands on it": "goblin patrol heading left" (a patrol walks a fixed route and faces the card it steps into next), "goblin guard", "goblin tracker, follows you". A card a goblin will step onto right after you move there says "a goblin moves here next", since that catches you as surely as walking into one (the "Goblin warnings" setting in the mod menu turns this off, for players who would rather work it out from where each patrol is heading). After each step, once the goblins finish moving, the selected card's goblin news is spoken again if it changed.
* Enemy armour is heard: a dull clunk when your hit glances off (and "Armoured, use Bash" with the key the first time on each enemy), a crunch when Bash cracks it, a crash and "Armour broken" when it breaks, and a rising shimmer as it grows back ("Armour restored" when broken armour returns). Each comes from that enemy and has its own volume setting; all four are in the sound glossary and the combat help.
* The game's Stun control is now called Bash everywhere the mod names it (help, key lists, announcements), as on the game's cards and encounters.
* An enemy you can finish (knocked to the ground) plays a rising swish from where it lies, repeating faster as you get closer, until it gets up; when the nearest enemy can be finished, C answers with the swish instead of the usual beep. Once the finisher would land (close enough, in the direction you push or face), the swish gets a bright ding on top, and C gives the ding too.
* On the map, Shift+period takes the survey cursor to the next card you have not visited yet, nearest first, and Shift+comma to the previous one; each read ends with where it lies from you and "unvisited 2 of 5".
* A gamble over gold or food (fortune cards, such as Chester's exotic fruits) now reads each card's amount ("Bad Fortune, 30 Gold") when the cards are shown and when you turn one over. It used to say only "Pain".
* Voiced lines (the dealer, companions, characters) are now read whether or not the game's Subtitles setting is on. Before, an English player with subtitles off heard none of them.
* Attack and Defend get keyboard keys, V and B, because the game binds them only to mouse buttons; a key you bind yourself is left alone.
* Key names in announcements and help prefer a keyboard key, then the controller button when a pad is in use; a mouse button is named only when nothing else is bound, and the combat help then says to bind a key for Defend (the game ships it on the right mouse button alone).
* When an enemy latches on (gnome swarms, a mage's snare) the mod says "Snared" with the key to press repeatedly, and "free" when it is over.
* Dying, starving, forfeiting and failing a challenge are announced with the game's own verdict before the results.
* The challenge map's Campfire section (change appearance) can now be entered from the keyboard; it used to leave nothing focused.
* The enemy count no longer chatters while gnomes burrow in and out; hidden enemies count until they die.
* An offered card reads its kind and stats before its "New" badge ("Soldier's Sword, One-Handed Weapon, Damage 15, New") instead of "Soldier's Sword, New".
* Two objectives shown on one line are read with a pause between them.
* The dice no longer say "1 rolls remaining" before the result of the first throw.
* A README in every game language (docs folder, Markdown and HTML).
* F4 opens help for the screen you are on: the game's keys there, then the mod's keys. Up and down arrows read it, F4 or Tab closes it. The main menu mentions it when it appears.
* Review keys: Ctrl+Up reads the next detail of whatever has focus, Ctrl+Down the previous one; Ctrl+Left and Ctrl+Right switch between Details, Events (everything announced so far, newest first) and Status (health, food, gold, fame, poison and what is equipped). They work everywhere, combat included, and never move the game's selection.
* Cards are now announced short: name, kind and stats (and the price in a shop, or the item it would replace). Traits, rules and the rest of the card are one Ctrl+Up away. Map cards say their name and state; their text is in the details.
* Shield and armour cards now read their defence value with its caption ("Defence 4").
* The inventory says that Tab closes it when it opens.
* The map's survey cursor moved from Ctrl+arrows to Shift+arrows (Ctrl+arrows are now the review keys).

## 2.0.0

A rebuild of the abandoned Hand of Fate 2 accessibility mod, on the design of Hand of Fate Access for the first game.

* Everything the game focuses is spoken: menus, settings (with their values, re-spoken as you change them), cards in full, encounter pages and options (with costs, requirements and unavailable options), questions in the bottom bar with their keys, voiced lines, dialogues, the controls tutorial, objectives, and food, gold, fame and health changes.
* Screen names are spoken as you enter them: main menu, map, challenge map, deck builder, inventory (with what is in every slot, empty ones included), pause and results.
* Character creation reads origins, sigils and a description of every hairstyle and outfit colour.
* The challenge map reads each challenge with its lock, completion and token state; the deck builder reads each deck, every card as you browse, and the count as you add and remove cards.
* The map reads every card with its state and paths; Ctrl+arrows (or the right stick) survey the whole board.
* Minigames by ear: the chance-card shuffle (each card's tone follows it), the dice (confirm throws them; totals, faces and reroll marks spoken), the wheel (outcomes read, ticks per segment, landing spoken) and the pendulum (the outcome under the beam as a tone).
* Combat by ear: attack cues (clang to counter, crack to dodge), wall tones, projectiles, hazards and traps, ability-ready chimes, exit and loot beacons, an enemy locator on C (or left-stick click) that points at the exit once the fight is won, and spoken enemy counts and low-health warnings.
* Exit and loot pings follow the walkable path, so walking toward them leads around walls (trap rooms were walked to the exit by these pings alone); a trap firing down your lane plays the dodge crack.
* Shop items read their price and whether you can afford them; specific confirm actions ("Buy, Enter") are announced with their key.
* Save slots: the delete button (an icon) now reads "Delete this save".
* The end-of-run score table is read in full, and the inventory says what is in every slot, empty ones included.
* A sound glossary in the pause menu plays every mod sound with its meaning.
* H or slash (or right-stick click) speaks your health, food, gold, fame and poison; O repeats the objectives.
* Keyboard fixes: the dice throw on confirm; the challenge map no longer traps the cursor in its section list or ignores confirm on a challenge; a resting mouse pointer no longer steals focus from the keyboard.
* All twelve of the game's languages.
