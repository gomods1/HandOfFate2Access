using System;
using System.IO;
using System.Reflection;
using BepInEx;
using HandOfFate2Access.Audio;
using HandOfFate2Access.Combat;
using HandOfFate2Access.Diagnostics;
using HandOfFate2Access.Focus;
using HandOfFate2Access.Gambit;
using HandOfFate2Access.Help;
using HandOfFate2Access.Input;
using HandOfFate2Access.Localization;
using HandOfFate2Access.Maps;
using HandOfFate2Access.Minigames;
using HandOfFate2Access.ModMenu;
using HandOfFate2Access.Patches;
using HandOfFate2Access.Patching;
using HandOfFate2Access.Resources;
using HandOfFate2Access.Review;
using HandOfFate2Access.Screens;
using HandOfFate2Access.Settings;
using HandOfFate2Access.Speech;
using HandOfFate2Access.UI;
using HandOfFate2Access.Util;
using HarmonyLib;
using UnityEngine;

namespace HandOfFate2Access {
	/// <summary>
	/// BepInEx entry point. Awake does only non-Unity setup (logging); everything
	/// else is deferred to the Update loop, once a few frames have ticked and the
	/// game is live. Update then pumps every feature once per frame: the audio tier
	/// first (it needs no screen reader), then the speech tier, then input.
	/// </summary>
	[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
	public partial class Plugin : BaseUnityPlugin {
		public const string PluginGuid = "com.handoffate2access";
		public const string PluginName = "Hand of Fate 2 Access";

		private int _initCountdown = StartupDelayFrames;
		private const int StartupDelayFrames = 120;
		private bool _initialized;
		private bool _speechReady;

		// Every feature object is created in Initialize, never in a field initializer:
		// the plugin is constructed inside the BepInEx chainloader while the engine is
		// still loading its first scene, and resolving game types that early (static
		// reflection fields on these classes) intermittently deadlocks the load.
		private ScreenRecon _recon;
		private LanguageWatcher _language;
		private EncounterNarration _encounter;
		private DialogueNarration _dialogue;
		private VoiceNarration _voice;
		private ChallengeNavRescue _navRescue;
		private CombatKeyDefaults _keyDefaults;
		private ResourceWatcher _resources;
		private ObjectiveNarration _objectives;
		private PromptNarration _prompts;
		private RewardNarration _rewards;
		private PendulumWatcher _pendulum;
		private WheelWatcher _wheel;
		private DiceNarration _dice;
		private CombatNarration _combat;
		private SoundExamples _sounds;
		private ModMenuController _modMenu;
		private MapCursor _mapCursor;
		private bool _wasOnMap;
		private GameObject _lastMapFocus;
		private GambitWatcher _gambit;
		private InputRouter _input;
		private ReviewController _review;
		private HelpController _help;
		private bool _wasInCombat;

		private WallTones _wallTones;
		private CollisionCue _collisionCue;
		private ProjectileSonification _projectiles;
		private ZoneSonification _zones;
		private AttackCues _attackCues;
		private RechargeCues _rechargeCues;
		private ObjectBeacons _beacons;
		private FinisherBeacons _finishers;
		private EnemyPresence _enemyPresence;
		private ArmourCues _armour;
		private RemoteControl _remote;
		private EnemyLocator _enemyLocator;

		// Frame on which a screen or narration last spoke; a focus landing within a few
		// frames of it queues behind it instead of cutting it off.
		private int _contextFrame = -100;

		private void Awake() {
			LogBepInExBackend.Install(Logger);
			Log.Info("loaded");
			// Plain config binding (no game types), so it is safe this early.
			SettingsStore.Bind(Config);
		}

		private void OnApplicationQuit() {
			if (AudioEngine.IsInitialized) AudioEngine.Shutdown();
		}

		private void Update() {
			if (!_initialized) {
				if (_initCountdown-- > 0) return;
				_initialized = true;
				Initialize();
				return;
			}

			LatencyProbe.BeginFrame();
			Run("recon", _recon.Pump);
			if (UnityEngine.Input.GetKeyDown(KeyCode.F12)) { Inspector.DumpSelections(); Inspector.DumpMap(); }
			if (UnityEngine.Input.GetKeyDown(KeyCode.F11)) Inspector.DumpMenus();
			if (UnityEngine.Input.GetKeyDown(KeyCode.F10) && AudioEngine.IsAvailable)
				AudioEngine.PlayBuffer(OutcomeTones.Loop(OutcomeKind.HugeSuccess, AudioEngine.OutputSampleRate),
					AudioEngine.OutputSampleRate, false, new SoundParams(0f, 1f, 1f));

			// Audio tier: needs no screen reader.
			Run("audio", AudioEngine.Pump);
			Run("motion", PlayerMotion.Pump);
			Run("wall tones", _wallTones.Pump);
			Run("collision", _collisionCue.Pump);
			Run("projectiles", _projectiles.Pump);
			Run("zones", _zones.Pump);
			Run("attack cues", _attackCues.Pump);
			Run("recharge", _rechargeCues.Pump);
			Run("beacons", _beacons.Pump);
			Run("finish beacons", _finishers.Pump);
			Run("enemy presence", _enemyPresence.Pump);
			Run("armour", _armour.Pump);
			Run("gambit", _gambit.Pump);
			Run("sound examples", _sounds.Pump);
			Run("key defaults", _keyDefaults.Pump);

			if (_speechReady) {
				Run("language", _language.Pump);
				Run("nav state", NavigationState.PollRawInput);
				Run("voice", () => { if (_voice.Pump()) _contextFrame = Time.frameCount; });
				Run("screens", () => { if (_screens.Pump()) _contextFrame = Time.frameCount; });
				Run("encounter", () => { if (_encounter.Pump()) _contextFrame = Time.frameCount; });
				Run("dialogue", () => { if (_dialogue.Pump()) _contextFrame = Time.frameCount; });
				Run("prompts", () => { if (_prompts.Pump()) _contextFrame = Time.frameCount; });
				Run("rewards", () => { if (_rewards.Pump()) _contextFrame = Time.frameCount; });
				Run("pendulum", _pendulum.Pump);
				Run("wheel", _wheel.Pump);
				Run("dice", () => { if (_dice.Pump()) _contextFrame = Time.frameCount; });
				Run("combat", () => { _combat.Pump(); });
				Run("mod menu", _modMenu.Pump);
				Run("map scope", PumpMapScope);
				Run("nav rescue", _navRescue.Pump);
				Run("focus", PumpFocus);
				Run("addons", () => { if (_addons.Pump()) _contextFrame = Time.frameCount; });
				Run("mods", () => { if (_mods.Pump()) _contextFrame = Time.frameCount; });
				Run("browsers", _browsers.Pump);
				Run("resources", _resources.Pump);
				Run("objectives", () => { if (_objectives.Pump()) _contextFrame = Time.frameCount; });
			}

			if (_help != null) Run("help", _help.Pump);
			if (_review != null) Run("review scope", PumpReviewScope);
			Run("input", _input.Pump);
			Run("remote", _remote.Pump);
			LatencyProbe.EndFrame();
		}

		// Each feature pump runs isolated: an exception in one logs (rate-limited, so a
		// failure repeating every frame does not flood the log) and the rest of the frame
		// still runs, instead of one broken feature silencing focus and input with it.
		private readonly System.Collections.Generic.Dictionary<string, float> _faultLogged =
			new System.Collections.Generic.Dictionary<string, float>();

		private readonly System.Diagnostics.Stopwatch _pumpWatch = new System.Diagnostics.Stopwatch();

		private void Run(string name, Action pump) {
			try {
				if (LatencyProbe.Enabled) {
					_pumpWatch.Reset();
					_pumpWatch.Start();
					pump();
					_pumpWatch.Stop();
					LatencyProbe.Pump(name, _pumpWatch.Elapsed.TotalMilliseconds);
				} else {
					pump();
				}
			} catch (Exception ex) {
				float last;
				if (_faultLogged.TryGetValue(name, out last) && Time.unscaledTime - last < 10f) return;
				_faultLogged[name] = Time.unscaledTime;
				Log.Error(name + " pump failed: " + ex);
			}
		}

		private void Initialize() {
			Log.Info("update reached; initializing");
			ScenePersistence.Protect(gameObject);
			Assembly assembly = Assembly.GetExecutingAssembly();
			string pluginDir = Path.GetDirectoryName(assembly.Location);
			NativeLoader.Preload(pluginDir, "Tolk.dll");
			NativeLoader.Preload(pluginDir, "HofSapi.dll");
			LatencyProbe.Initialize(pluginDir);
			_recon = new ScreenRecon();
			_language = new LanguageWatcher();
			_encounter = new EncounterNarration();
			_dialogue = new DialogueNarration();
			_voice = new VoiceNarration();
			_navRescue = new ChallengeNavRescue();
			_keyDefaults = new CombatKeyDefaults();
			_resources = new ResourceWatcher();
			_objectives = new ObjectiveNarration();
			_prompts = new PromptNarration();
			_rewards = new RewardNarration();
			_panelContext = new PanelContext();
			_addons = new AddonNarration();
			_mods = new ModsNarration();
			_browsers = new BrowserWatcher();
			_screens = new ScreenAnnouncer();
			_pendulum = new PendulumWatcher();
			_wheel = new WheelWatcher();
			_dice = new DiceNarration();
			_combat = new CombatNarration();
			_sounds = new SoundExamples();
			_modMenu = new ModMenuController(_sounds);

			// Non-speech audio is independent of the screen reader, so it comes up first.
			if (!AudioEngine.Initialize(new UnityAudioBackend()))
				Log.Warn("audio engine unavailable; sound cues disabled");
			if (AudioEngine.IsOutputMono)
				Log.Warn("audio output is mono; spatial cues will not localize");

			// The minigame voices need the engine up (they check it once, here).
			_pendulum.Initialize();
			_wheel.Initialize();

			_wallTones = new WallTones();
			_wallTones.Initialize(pluginDir);
			_collisionCue = new CollisionCue();
			_collisionCue.Initialize(pluginDir);
			_projectiles = new ProjectileSonification();
			_projectiles.Initialize();
			_zones = new ZoneSonification();
			_zones.Initialize();
			_attackCues = new AttackCues();
			_attackCues.Initialize(pluginDir);
			_rechargeCues = new RechargeCues();
			_rechargeCues.Initialize(pluginDir);
			_beacons = new ObjectBeacons();
			_beacons.Initialize(pluginDir);
			_finishers = new FinisherBeacons();
			_finishers.Initialize(pluginDir);
			_enemyPresence = new EnemyPresence();
			_enemyPresence.Initialize(pluginDir);
			_armour = new ArmourCues();
			_armour.Initialize(pluginDir);
			if (AudioEngine.IsAvailable)
				InstallCombatPatches();
			InstallDiagnosticPatches();
			InstallCameraPatches();

			_gambit = new GambitWatcher();
			_gambit.Initialize(pluginDir);

			// The locator is an audio feature, so the router comes up here and the key
			// works with the screen reader down. C on the keyboard (L is the game's camera
			// orbit in Hand of Fate 2) and left-stick click on a controller, both unbound
			// by the game.
			_enemyLocator = new EnemyLocator(_finishers);
			_enemyLocator.Initialize();
			_input = new InputRouter();
			_input.Register(new ButtonAction(
				"enemy locator",
				Locate,
				new[] { KeyCode.C },
				new[] { InControl.InputControlType.LeftStickButton },
				() => CombatGate.IsLive));

			if (!SpeechEngine.Initialize(new TolkBackend())) {
				Log.Warn("speech unavailable; announcements disabled");
				return;
			}
			_speechReady = true;
			NavigationState.Install();
			InstallPatches();
			_language.Pump();
			SpeechPipeline.SpeakInterrupt(Strings.PluginLoaded(assembly.GetName().Version.ToString(3)));

			// Status key: the player's resources on demand, anywhere. H (the same key on
			// every layout) or slash on the keyboard and right-stick click on a
			// controller, all unbound by the game.
			_input.Register(new ButtonAction(
				"status",
				SpeakStatus,
				new[] { KeyCode.H, KeyCode.Slash },
				new[] { InControl.InputControlType.RightStickButton }));

			// Objectives key: the challenge objectives on demand. O, unbound by the game.
			_input.Register(new ButtonAction(
				"objectives",
				_objectives.SpeakAll,
				new[] { KeyCode.O },
				new InControl.InputControlType[0]));

			// Review buffers on Ctrl+arrows: the focused thing's details, recent events, the
			// run's status. Help on F4 (F1 is the game's inventory key).
			_review = new ReviewController(StatusLines);
			_input.Register(_review);
			_help = new HelpController(HelpContextNow, FightUnderneath, _sounds);

			// Map cursor: a free-roam survey of the visible board on Shift+arrows and the
			// right stick, scoped to the map.
			_mapCursor = new MapCursor();
			_remote = new RemoteControl(pluginDir) {
				Status = SpeakStatus,
				Locator = () => { if (CombatGate.IsLive) Locate(); },
				Objectives = _objectives.SpeakAll,
			};
			_input.Register(new DirectionalAction("map cursor", _mapCursor.Move, () => MapInput.OnMap));
			// Shift plus period and comma: the cursor through the unvisited cards, nearest
			// first. Comma and period are unbound by the game.
			_input.Register(new ButtonAction("map unvisited next", () => _mapCursor.Tour(1),
				new[] { KeyCode.Period }, new InControl.InputControlType[0], () => MapInput.CursorKeysHeld));
			_input.Register(new ButtonAction("map unvisited previous", () => _mapCursor.Tour(-1),
				new[] { KeyCode.Comma }, new InControl.InputControlType[0], () => MapInput.CursorKeysHeld));
		}

		// The locator key. With the fight won, it points at what is left to walk to (the
		// exit, the boss's end circle, dropped loot) instead of an enemy.
		private void Locate() {
			if (CombatNarration.CountEnemies() == 0 && _beacons != null && _beacons.HasTargets)
				_beacons.PingNearest();
			else if (_speechReady && CombatNarration.CountEnemies() == 0)
				SpeechPipeline.SpeakInterrupt(Strings.EnemiesNone);
			else
				_enemyLocator.Trigger();
		}

		private void InstallPatches() {
			var patcher = new HarmonyPatcher(new Harmony(PluginGuid));
			patcher.Patch(typeof(DiceRollStatus), "Show",
				new[] { typeof(int), typeof(int?), typeof(int) },
				prefix: null,
				postfix: AccessTools.Method(typeof(DiceNarration), "ShowPostfix"));
			patcher.Patch(typeof(UISelectionManager), "OnNavigate",
				new[] { typeof(InputButton) },
				prefix: AccessTools.Method(typeof(UISelectionManager_OnNavigate_Patch), "Prefix"),
				postfix: null);
			patcher.Patch(typeof(DeckBuilderMode), "TryAddToDeck",
				new[] { typeof(Card) },
				prefix: null,
				postfix: AccessTools.Method(typeof(DeckRules), "TryAddPostfix"));
			patcher.Patch(typeof(DiceRoller), "BeginDiceRoll",
				new[] { typeof(UISelectable) },
				prefix: AccessTools.Method(typeof(DiceRoller_BeginDiceRoll_Patch), "Prefix"),
				postfix: null);
			patcher.Patch(typeof(CardSetModifierContainer), "OnCardSelected",
				new[] { typeof(UISelectable) },
				prefix: null,
				postfix: AccessTools.Method(typeof(RewardNarration), "OnCardSelectedPostfix"));
			patcher.Patch(typeof(UIChallengeMapMenu), "HandleOnButtonSelect",
				new[] { typeof(UIChallengeMapMenuButton) },
				prefix: AccessTools.Method(typeof(UIChallengeMapMenu_HandleOnButtonSelect_Patch), "Prefix"),
				postfix: null);
			patcher.Patch(typeof(UIChallengeNav), "OnTabSelected",
				new[] { typeof(UINavTab) },
				prefix: AccessTools.Method(typeof(UIChallengeNav_OnTabSelected_Patch), "Prefix"),
				postfix: null);
			patcher.Patch(typeof(UISelectionManager), "OnHover",
				new[] { typeof(GameObject), typeof(bool) },
				prefix: AccessTools.Method(typeof(UISelectionManager_OnHover_Patch), "Prefix"),
				postfix: null);
			patcher.Patch(typeof(UIInputManager), "GetDirection",
				new[] { typeof(bool) },
				prefix: null,
				postfix: AccessTools.Method(typeof(UIInputManager_GetDirection_Patch), "Postfix"));
			patcher.Patch(typeof(UIInputManager), "Update",
				new Type[0],
				prefix: AccessTools.Method(typeof(UIInputManager_Update_Patch), "Prefix"),
				postfix: null);
			patcher.Patch(typeof(SliderControl), "LateUpdate",
				new Type[0],
				prefix: AccessTools.Method(typeof(SliderControl_LateUpdate_Patch), "Prefix"),
				postfix: null);
			patcher.Patch(typeof(AddonContentUI), "OnItemHovered",
				new[] { typeof(UISelectable), typeof(bool) },
				prefix: AccessTools.Method(typeof(AddonContentUI_Patches), "HoverPrefix"),
				postfix: null);
			patcher.Patch(typeof(AddonContentUI), "OnItemSelected",
				new[] { typeof(UISelectable) },
				prefix: AccessTools.Method(typeof(AddonContentUI_Patches), "SelectPrefix"),
				postfix: null);
		}

		// The combat attack-telegraph and hazard hooks, installed with the audio path.
		private void InstallCombatPatches() {
			var patcher = new HarmonyPatcher(new Harmony(PluginGuid + ".combat"));
			patcher.Patch(
				typeof(CombatUtils), "StartMeleeEffect",
				new[] { typeof(EffectSet), typeof(Model), typeof(bool) },
				prefix: AccessTools.Method(typeof(CombatUtils_StartMeleeEffect_Patch), "Prefix"),
				postfix: null);
			patcher.Patch(
				typeof(CombatUtils), "StartRangedEffect",
				new[] { typeof(EffectSet), typeof(Model), typeof(bool) },
				prefix: AccessTools.Method(typeof(CombatUtils_StartRangedEffect_Patch), "Prefix"),
				postfix: null);
			var moverEngagePostfix = AccessTools.Method(typeof(CombatProxyMover_OnEngage_Patch), "Postfix");
			patcher.Patch(typeof(CombatProxyLob), "OnEngage", new Type[0], prefix: null, postfix: moverEngagePostfix);
			patcher.Patch(typeof(CombatProxyLightning), "OnEngage", new Type[0], prefix: null, postfix: moverEngagePostfix);
			patcher.Patch(
				typeof(CombatProxyProjectile), "OnEngage",
				new Type[0],
				prefix: null,
				postfix: AccessTools.Method(typeof(CombatProxyProjectile_OnEngage_Patch), "Postfix"));
			patcher.Patch(
				typeof(CombatProxyBeam), "Engage",
				new[] { typeof(ICombatProxyBeamParent), typeof(Vector3), typeof(float), typeof(float) },
				prefix: null,
				postfix: AccessTools.Method(typeof(CombatProxyBeam_Engage_Patch), "Postfix"));
			var chainEngagePostfix = AccessTools.Method(typeof(CombatProxyChain_OnEngage_Patch), "Postfix");
			patcher.Patch(typeof(CombatProxyTrail), "OnEngage", new Type[0], prefix: null, postfix: chainEngagePostfix);
			patcher.Patch(typeof(CombatProxyLightning), "OnEngage", new Type[0], prefix: null, postfix: chainEngagePostfix);
			patcher.Patch(
				typeof(Trap), "Start",
				new Type[0],
				prefix: null,
				postfix: AccessTools.Method(typeof(Trap_Start_Patch), "Postfix"));
		}

		// The fight camera steadied for listening (lock camera, hear from your character).
		private void InstallCameraPatches() {
			var patcher = new HarmonyPatcher(new Harmony(PluginGuid + ".camera"));
			patcher.Patch(typeof(Defiant.Cameras.PlayerCamera), "UpdateOrbitRotationOffset",
				new Type[0],
				prefix: AccessTools.Method(typeof(PlayerCamera_Patches), "OrbitPrefix"),
				postfix: null);
			patcher.Patch(typeof(Defiant.Cameras.PlayerCamera), "UpdateListener",
				new Type[0],
				prefix: AccessTools.Method(typeof(PlayerCamera_Patches), "ListenerPrefix"),
				postfix: null);
		}

		// Log-only diagnostics: the damage tripwire and hazard spawn recon.
		private void InstallDiagnosticPatches() {
			var patcher = new HarmonyPatcher(new Harmony(PluginGuid + ".diagnostics"));
			patcher.Patch(
				typeof(Destroyable), "ApplyDamage",
				new[] { typeof(Destroyable.Damage), typeof(bool) },
				prefix: AccessTools.Method(typeof(Destroyable_ApplyDamage_Patch), "Prefix"),
				postfix: null);
			patcher.Patch(
				typeof(CombatProxy), "Engage",
				new[] { typeof(CombatEffectProxy), typeof(CombatTarget), typeof(Transform) },
				prefix: null,
				postfix: AccessTools.Method(typeof(CombatProxy_Engage_Patch), "Postfix"));
			patcher.Patch(
				typeof(CombatProxy), "Disengage",
				new Type[0],
				prefix: null,
				postfix: AccessTools.Method(typeof(CombatProxy_Disengage_Patch), "Postfix"));
		}

		// The map is live when the game's map selection group holds its selection and that
		// selection is the active one: the inventory opens over the map in a selection of its
		// own and leaves the map's untouched. The survey cursor re-anchors on the player each
		// time the map becomes active and follows the game's own cursor whenever that moves.
		private void PumpMapScope() {
			Map map = Map.Instance;
			UISelection mapSelection = map != null && map.SelectableGroup != null ? map.SelectableGroup.Selection : null;
			UISelection top = FocusTracker.TopSelection();
			bool onMap = mapSelection != null && Game.Instance != null && Game.Instance.ActiveGameState is GameState_Map
				&& mapSelection.SelectedGroup == map.SelectableGroup
				&& (top == null || top == mapSelection);
			MapInput.OnMap = onMap;
			if (onMap) {
				if (!_wasOnMap) _mapCursor.AnchorToPlayer();
				GameObject focus = FocusTracker.Current();
				if (focus != _lastMapFocus && focus != null) {
					MapLayoutSlot slot = focus.GetComponent<MapLayoutSlot>();
					if (slot != null) _mapCursor.Anchor(slot);
				}
				_lastMapFocus = focus;
			}
			_wasOnMap = onMap;
		}

		// The player of a run in progress (on the map or in a fight), or null.
		private static Player RunPlayer() {
			Player player = Player.Instance;
			GameState state = Game.Instance != null ? Game.Instance.ActiveGameState : null;
			bool inRun = player != null && (state is GameState_Map || state is GameState_Level_Play
				|| state is GameState_Level_In || state is GameState_Level_Out || state is GameState_Level_Pause);
			return inRun ? player : null;
		}

		private static readonly Equipment.EquipmentType[] StatusSlots = {
			Equipment.EquipmentType.Weapon, Equipment.EquipmentType.Shield, Equipment.EquipmentType.Artifact,
			Equipment.EquipmentType.Armour, Equipment.EquipmentType.Helm, Equipment.EquipmentType.Ring };

		private static string SlotName(Equipment.EquipmentType type) {
			switch (type) {
				case Equipment.EquipmentType.Weapon: return Strings.SlotWeapon;
				case Equipment.EquipmentType.Shield: return Strings.SlotShield;
				case Equipment.EquipmentType.Artifact: return Strings.SlotArtifact;
				case Equipment.EquipmentType.Armour: return Strings.SlotArmour;
				case Equipment.EquipmentType.Helm: return Strings.SlotHelm;
				default: return Strings.SlotRing;
			}
		}

		// The status review buffer: the resources, then what is equipped in each slot.
		private static System.Collections.Generic.IList<string> StatusLines() {
			Player player = RunPlayer();
			if (player == null) return null;
			var lines = StatusReadout.Lines(
				Mathf.RoundToInt(player.Health.Get()),
				Mathf.RoundToInt(player.MaxHealth.Get()),
				player.Food.Get(),
				player.Gold.Get(),
				player.Fame.Get(),
				player.Poison.Get());
			Inventory.Data inventory = player.InventoryData;
			if (inventory == null) return lines;
			foreach (Equipment.EquipmentType type in StatusSlots) {
				EquipmentCard card = Inventory.GetEquippedCard(inventory, type);
				lines.Add(SlotName(type) + " " + (card != null ? CardInfoReader.ShortName(card) : Strings.SlotEmpty));
			}
			return lines;
		}

		// Stale details are not read in a fight: the focus they came from is gone.
		private void PumpReviewScope() {
			bool inCombat = CombatGate.IsLive;
			if (inCombat && !_wasInCombat) _review.ClearFocus();
			_wasInCombat = inCombat;
		}

		// The screen or mode the help key explains, most specific first.
		private HelpContext HelpContextNow() {
			if (CombatGate.IsLive) return HelpContext.Combat;
			ScreenId screen = _screens.Current;
			if (screen == ScreenId.Paused) return HelpContext.Paused;
			if (ChanceCardTask.RunningTask != null) return HelpContext.ChanceCards;
			GameObject focus = FocusTracker.Current();
			if (focus != null) {
				if (focus.GetComponentInParent<DiceRoller>() != null || focus.GetComponentInParent<Die>() != null)
					return HelpContext.Dice;
				if (focus.GetComponentInParent<Pendulum>() != null) return HelpContext.Pendulum;
				if (focus.GetComponentInParent<WheelOfFortune>() != null) return HelpContext.Wheel;
				if (focus.GetComponentInParent<PageOptionButton>() != null) return HelpContext.Encounter;
				if (focus.GetComponentInParent<AddonContentUI>() != null) return HelpContext.Addons;
				if (focus.GetComponentInParent<UIModDialog>() != null) return HelpContext.Mods;
			}
			switch (screen) {
				case ScreenId.Inventory: return HelpContext.Inventory;
				case ScreenId.DeckBuilder: return HelpContext.DeckBuilder;
				case ScreenId.ChallengeMap: return HelpContext.ChallengeMap;
				case ScreenId.Map: return HelpContext.Map;
				case ScreenId.MainMenu: return HelpContext.MainMenu;
				case ScreenId.Customisation: return HelpContext.Customisation;
				default: return HelpContext.Menu;
			}
		}

		// The pause menu opened over a fight.
		private static bool FightUnderneath() {
			return CombatEncounter.Instance != null && Game.Instance != null
				&& Game.Instance.ActiveGameState is GameState_Level_Pause;
		}

		private void SpeakStatus() {
			Player player = RunPlayer();
			if (player == null) {
				SpeechPipeline.SpeakInterrupt(Strings.StatusNoRun);
				return;
			}
			SpeechPipeline.SpeakInterrupt(StatusReadout.Compose(
				Mathf.RoundToInt(player.Health.Get()),
				Mathf.RoundToInt(player.MaxHealth.Get()),
				player.Food.Get(),
				player.Gold.Get(),
				player.Fame.Get(),
				player.Poison.Get()));
		}

		// Frames after a navigation press with no focus change before the current item is
		// re-read: an arrow at the end of a list, or on a lone option, is never silent.
		private const int EdgeRepeatFrames = 5;

		// A focus that had nothing to say yet (its label fills in a few frames after it
		// spawns) is re-read for a short while.
		private PanelContext _panelContext;
		private AddonNarration _addons;
		private ModsNarration _mods;
		private BrowserWatcher _browsers;
		private ScreenAnnouncer _screens;
		private GameObject _retryFocus;
		private int _retryUntilFrame;

		// The control under focus whose value changes in place (a settings row, a die in
		// the reroll step) and the value last spoken for it: a change re-speaks just the value.
		private Component _valueOwner;
		private string _lastValue;
		private int _valueSpokenFrame = -100;

		private void PumpValue() {
			if (_valueOwner == null) return;
			GameObject current = FocusTracker.Current();
			if (current == null || ValueReader.Owner(current) != _valueOwner) {
				_valueOwner = null;
				return;
			}
			string value = ValueReader.Read(current);
			if (value == null || value == _lastValue) return;
			_lastValue = value;
			if (value.Length == 0) return;
			_valueSpokenFrame = Time.frameCount;
			SpeechPipeline.SpeakInterrupt(value);
		}

		private void PumpFocus() {
			PumpValue();
			if (_retryFocus != null) {
				if (FocusTracker.Current() != _retryFocus || Time.frameCount > _retryUntilFrame) {
					_retryFocus = null;
				} else {
					string late = BuildReadout(_retryFocus, out var lateDetails);
					if (!string.IsNullOrEmpty(late)) {
						_retryFocus = null;
						_review.Focus(lateDetails);
						SpeechPipeline.SpeakQueued(late);
						return;
					}
				}
			}
			if (!FocusTracker.TryConsume(out GameObject go)) {
				if (Time.frameCount - NavigationState.LastNavigateFrame == EdgeRepeatFrames
						&& Time.frameCount - FocusTracker.LastChangeFrame > EdgeRepeatFrames
						&& !KeyClaims.ArrowsClaimed
						&& !CombatGate.IsLive && !ModMenuState.Open
						&& Time.frameCount - _valueSpokenFrame > 3 * EdgeRepeatFrames
						&& Time.frameCount - BrowserWatcher.SpokeFrame > 3 * EdgeRepeatFrames) {
					// On the map an arrow that goes nowhere says so: there is no card that
					// way, or a wall between (MapNavigation decided it).
					string blocked = MapInput.OnMap && MapNavigation.Last == MapNavigation.Outcome.Blocked
						? MapBlocked(NavigationState.LastNavigateButton) : null;
					if (blocked != null) {
						SpeechPipeline.SpeakInterrupt(blocked);
						return;
					}
					GameObject current = FocusTracker.Current();
					string again = current != null ? BuildReadout(current, out _) : null;
					if (!string.IsNullOrEmpty(again)) SpeechPipeline.SpeakInterrupt(again);
				}
				return;
			}
			LatencyProbe.Mark("focus changed");
			Inspector.Dump("focus", go);
			// Focus moving within one settings row (onto its arrows) re-reads only the value.
			Component owner = ValueReader.Owner(go);
			if (owner != null && owner == _valueOwner) {
				string value = ValueReader.Read(go);
				if (!string.IsNullOrEmpty(value) && value != _lastValue) {
					_lastValue = value;
					_valueSpokenFrame = Time.frameCount;
					SpeechPipeline.SpeakInterrupt(value);
				}
				return;
			}
			_valueOwner = owner;
			_lastValue = owner != null ? ValueReader.Read(go) : null;
			string announcement = BuildReadout(go, out var details);
			_review.Focus(details);
			string context = _panelContext.Take(go);
			if (string.IsNullOrEmpty(announcement)) {
				// Nothing to read yet (a label still fading in): retry for a while; a panel's
				// context, if this focus opened one, is read meanwhile.
				_retryFocus = go;
				_retryUntilFrame = Time.frameCount + 90;
				if (context == null) return;
				announcement = context;
			} else if (context != null) {
				announcement = context + ", " + announcement;
			}
			SpeechMode mode = FocusAnnouncePolicy.Decide(NavigationState.WasRecent, Time.frameCount - _contextFrame <= 3);
			if (mode == SpeechMode.Interrupt)
				SpeechPipeline.SpeakInterrupt(announcement);
			else
				SpeechPipeline.SpeakQueued(announcement);
		}

		// "Can't go left, you can go down, right".
		private static string MapBlocked(InputButton button) {
			string direction;
			switch (button) {
				case InputButton.Up: direction = Strings.DirUp; break;
				case InputButton.Down: direction = Strings.DirDown; break;
				case InputButton.Left: direction = Strings.DirLeft; break;
				case InputButton.Right: direction = Strings.DirRight; break;
				default: return null;
			}
			string available = MapNavigation.Available();
			var message = new Message().Add(Strings.MapCannotGo(direction));
			if (available != null) message.Add(Strings.MapCanGo(available));
			return message.Resolve();
		}

		// Reading the focused control touches live game state, which can throw on
		// malformed data. Catch at this boundary so a bad control logs instead of
		// silently dropping the readout. Details are the focused thing's lines for the
		// review buffer.
		private string BuildReadout(GameObject go, out System.Collections.Generic.List<string> details) {
			details = null;
			string label = "focus";
			try {
				label = go.name;
				if (_gambit.TryCardName(go, out string gambitCard))
					return gambitCard;
				UIElement element = ProxyFactory.Create(go);
				if (element == null) {
					if (go != _retryFocus) Log.Debug("focus suppressed for structural selectable '" + label + "'");
					return null;
				}
				details = element.Details();
				return element.Describe().Resolve();
			} catch (Exception ex) {
				Log.Error("focus readout failed for '" + label + "': " + ex);
				return null;
			}
		}
	}
}
