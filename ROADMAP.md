# Roadmap

Hand of Fate 2 Access is rebuilt from scratch on the architecture of `../hand-of-fate-access` (the original game's mod). The original repository of this project contained only a README; no source survived. Items are ticked only when exercised in the running game.

## Phase 1 - Loader, logging, speech
- [x] Vendored BepInEx 5.4.23.2 x86 with the `Application` entrypoint; `setup-bepinex.ps1`, `build.ps1`, `test.ps1`.
- [x] Tolk speech (NVDA verified), every spoken line logged.
- [x] Startup line spoken in the game's language once the profile loads.
- [x] Language follows `LanguageManager.CurrentLanguage`; English and Spanish tables.

## Phase 2 - Focus and cards
- [x] Focus spine: poll the top `UISelection`'s hovered object (HoF2's own selection model).
- [x] Generic label readout; selection-blocker placeholder suppressed.
- [x] Cards read through the game's own info-panel layout (title, type, stats, traits, rewards).
- [x] Face-down cards read as face down.
- [x] Gender choice cards (art only) read as male/female.
- [x] Character customisation carousel: origins, sigils, gender, and a spoken description of each hairstyle and outfit colour (written from the card art).
- [ ] Descriptions for war paint, hair colour and war-paint colour art (fall back to numbered options, logged). Not seen in the base game's appearance screen (gender, origin, hairstyle, amulet, outfit colour only); likely add-on content.

## Phase 3 - Narration and screens
- [x] Subtitled voice lines spoken in full (dealer, companions, characters).
- [x] Encounter page text and announcement line; encounter option buttons with cost/requirement and unavailable state; the page arrow reads Continue.
- [x] Encounter card title announced when an encounter starts.
- [x] Modal dialogues (confirmations, the controls tutorial with each action's key) read on open; bottom-bar questions with their buttons and the equipped-item comparison.
- [x] Pause menu; settings rows read name and value (choice lists, checkboxes, sliders, buttons, key bindings and the rebind prompt), values re-spoken on change.
- [x] A mouse pointer left resting over a scrolling list no longer steals focus from the keyboard.
- [x] Key names in bindings are spoken in full ("left shift", "left control"; verified in the Controls settings and the combat tutorial).
- [x] Creature showcase reads title and description before Continue (verified: Thug, Guard, Raider, Inflicted, Trapper, Berserker, Soldier, bosses).
- [x] Challenge objectives read on appearance, change, completion and failure (`O` repeats them).
- [x] Fight start with enemy count, count changes, low-health warnings.
- [x] Results screen announced; the end-of-run verdict (death by wounds or starvation, forfeit, failed challenge) is read from the game's own end menu (verified with a forfeit: "Abandonar, Te rindes ante lo inevitable.").
- [x] Challenge map (sections, challenges with lock/completion/token state; keyboard traps fixed), deck builder intro, add-on panels, reward reveal.
- [x] Deck builder: mode decks (name, purpose, count), card browsers (centre card read as it moves), add/remove with the new count.
- [x] Bottom-bar questions name the confirm/cancel keys on keyboard; key caps spoken in the player's language.
- [x] Shop menu (verified); trade card choice (verified).
- [x] Screen names on entry (main menu, map, challenge map, deck builder, inventory with a slot summary, pause, results).
- [x] Endless score table read row by row.
- [x] Shop purchase screens: price and affordability per item, the Buy action with its key; a purchase verified end to end.

## Phase 4 - Input
- [x] Input router; status key (`/`, right-stick click); enemy locator (`C`, left-stick click).
- [x] Map cursor (Shift+arrows / right stick; Ctrl+arrows are the review keys).
- [x] Review buffers (Ctrl+arrows): Details of the focus, Events, Status; verified on the main menu, map, inventory, encounter and in combat.
- [x] F4 help per screen (main menu, map, inventory, encounter, chance cards, combat with the live bindings verified); announced on the main menu.
- [x] Concise card focus (name, kind, stats) with the full card under Ctrl+Up; map cards by name.
- [x] Help verified on the deck builder, challenge map, pause and dice.
- [ ] Help verified on the wheel and pendulum (the minigames were not reached in the tutorial, The Magician or The High Priestess).
- [x] Input latency: the game ran at 16 fps because the desktop was at 16 Hz; at 60 Hz a key is spoken 34 ms after its frame (see CLAUDE.md).

## Phase 5 - Map
- [x] Map slots read with state (here, complete, reachable), walls and exits; hidden slots left out.
- [x] Free-roam survey cursor.

## Phase 6 - Minigames
- [x] Chance cards: teaching walk (tones + outcome words), shuffle following, slot numbers, pick result. (Audio output verified by level; not yet judged by a human tester.)
- [x] Dice: throw on confirm (keyboard otherwise needs a mouse drag), target/total/faces, reroll marking.
- [x] Wheel: outcomes read when laid out, "press confirm to slow it down" on focus, per-segment ticks (output verified), landing spoken.
- [x] Pendulum: outcome tone under the beam (output verified), "press confirm to stop" on focus, result spoken.
- [x] Divine intervention (the game marks dice for you): the dice and their marks are read; F3 rerolls.

## Phase 7 - Combat audio
- [x] Ported: attack telegraphs, wall tones, collision bump, projectile/mover voices, ground zones, beams, chains, legacy traps, recharge chime, enemy locator, damage and hazard recon logs. Mod audio verified reaching the output device in a live fight (loopback recording).
- [x] Beacons for `ExitTrigger`, the court circle and dropped loot; the locator key points at them once enemies are gone. Exit and loot pings follow the NavMesh path's next turn; two trap rooms were completed by steering on the locator's bearing alone. Not yet judged by a human ear.
- [x] HoF2's newer trap system (`Defiant.Traps`): damage triggers and emitters voiced through the zone sounds (verified in a trap room: 9 traps, 31 zones, audible).
- [x] Snares (gnome swarms, mage snares) announced with the counter key to mash, and "free" when over; hidden enemies counted so a gnome swarm's dives do not chatter.
- [ ] Companion and boss specifics.
- [ ] Combo attacks (Northern warriors, berserkers): the single counter press after the cue blocks the first swing; the later swings of a combo still land one hit per combo in the by-ear bot's runs.

## Follow-ups found in the 2026-10-18 pass
- [x] Deck builder: the badges beside the centre card (Dice, Shard, Gain Gold, Unknown...) are the card's traits and are read by Ctrl+Up as the card's details (verified in Spanish: "Dados, Los resultados se ven afectados por Dados").
- [ ] Map spices (gates, lairs, roaming figures drawn on map slots) are not read; `MapLayoutSlot.Spices` holds them with prefab ids only, so each needs a mod-authored name.
- [ ] A freshly dealt equipment card sometimes reads without its stats (the info layout's stat widgets are empty until the game's own panel has shown it once); `CardInfoReader` now logs "read without stats" with the equipment and layout it saw.

## Cross-cutting
- [x] Offline tests (xUnit over Core).
- [x] Translations of the mod's strings into all 12 game languages (machine-assisted; native review welcome).
- [x] Player README in all 12 game languages (`docs/README.<lang>.md`, rendered to HTML by `tools/Build-Readme.ps1`).
- [x] Player README with install steps and keys.
- [x] Sound glossary in the pause menu.
