using System.Collections.Generic;
using System.Reflection;
using HandOfFate2Access.Audio;
using HandOfFate2Access.Combat;
using HandOfFate2Access.Glossary;
using HandOfFate2Access.Help;
using HandOfFate2Access.Input;
using HandOfFate2Access.Localization;
using HandOfFate2Access.Settings;
using HandOfFate2Access.Speech;
using HandOfFate2Access.Util;
using HarmonyLib;
using UnityEngine;

namespace HandOfFate2Access.ModMenu {
	/// <summary>
	/// The accessibility mod menu: combat sounds and chance card and minigame sounds (each
	/// entry says what a sound means, Enter plays it), help topics (Enter opens one, its
	/// lines are read with Up and Down, and a line naming a sound plays it on Enter), and
	/// the settings (Left and Right change one; a volume plays a sample of its sounds at
	/// the new level). Control plus up arrow reads an entry's details (a setting's
	/// explanation).
	///
	/// It opens from the "Accessibility mod" option added to the bottom of the main menu
	/// and the pause menu, or with Shift plus F4 anywhere but a running fight. While it is
	/// open the game's UI input is skipped (UIInputManager_Update_Patch) and the game's
	/// pending button press is dropped every frame, so its keys never reach the screen
	/// underneath; Backspace (or a pad's B) goes back and, at the top, closes it.
	/// </summary>
	internal sealed class ModMenuController {
		private const float InitialRepeatDelay = 0.4f;
		private const float RepeatInterval = 0.1f;

		private static readonly FieldInfo PauseCreditsField = AccessTools.Field(typeof(PauseMenu), "m_creditsSelectable");
		private static readonly FieldInfo PauseGroupField = AccessTools.Field(typeof(PauseMenu), "m_selectableGroup");
		private static readonly FieldInfo StartSettingsField = AccessTools.Field(typeof(StartScreenUI), "m_settingsButton");
		private static readonly FieldInfo StartGroupField = AccessTools.Field(typeof(StartScreenUI), "m_selectableGroup");

		private readonly SoundExamples _sounds;
		private readonly MenuNavigator _nav = new MenuNavigator();
		private readonly MenuOptionButton _pauseOption = new MenuOptionButton("pause menu");
		private readonly MenuOptionButton _startOption = new MenuOptionButton("main menu");
		private readonly Screens.PanelFinder<PauseMenu> _pauseFinder = new Screens.PanelFinder<PauseMenu>();
		private readonly Screens.PanelFinder<StartScreenUI> _startFinder = new Screens.PanelFinder<StartScreenUI>();
		private int _openedFrame;
		private KeyCode _held = KeyCode.None;
		private float _nextRepeat;

		public ModMenuController(SoundExamples sounds) {
			_sounds = sounds;
		}

		public void Pump() {
			InjectOptions();
			bool shiftF4 = UnityEngine.Input.GetKeyDown(KeyCode.F4) && KeyClaims.ShiftHeld;
			bool requested = ModMenuState.ConsumeOpen();
			if (!ModMenuState.Open) {
				if ((requested || shiftF4) && !HelpState.Open) {
					if (CombatGate.IsLive) SpeechPipeline.SpeakInterrupt(Strings.ModMenuInFight);
					else Open();
				}
				return;
			}
			DropGameButtons();
			// The key that opened the menu (Enter on its option) is not also its first press.
			if (Time.frameCount == _openedFrame) return;
			if (shiftF4) {
				Close();
				return;
			}
			if (KeyClaims.CtrlHeld) return;   // Control plus arrows: the review keys

			if (Pressed(KeyCode.Backspace) || Gamepad.WasPressed(InControl.InputControlType.Action2)) {
				string back = _nav.Back(out bool closed);
				if (closed) Close();
				else Say(back);
				return;
			}
			if (Pressed(KeyCode.Return) || Pressed(KeyCode.KeypadEnter) || Pressed(KeyCode.Space)
					|| Gamepad.WasPressed(InControl.InputControlType.Action1)) {
				Activate();
				return;
			}
			if (Pressed(KeyCode.Home)) { Say(_nav.Jump(false)); return; }
			if (Pressed(KeyCode.End)) { Say(_nav.Jump(true)); return; }
			if (Gamepad.WasPressed(InControl.InputControlType.DPadUp)) { Say(_nav.Move(-1)); return; }
			if (Gamepad.WasPressed(InControl.InputControlType.DPadDown)) { Say(_nav.Move(1)); return; }
			if (Gamepad.WasPressed(InControl.InputControlType.DPadLeft)) { Adjust(-1); return; }
			if (Gamepad.WasPressed(InControl.InputControlType.DPadRight)) { Adjust(1); return; }
			switch (Arrow()) {
				case KeyCode.UpArrow: Say(_nav.Move(-1)); break;
				case KeyCode.DownArrow: Say(_nav.Move(1)); break;
				case KeyCode.LeftArrow: Adjust(-1); break;
				case KeyCode.RightArrow: Adjust(1); break;
			}
		}

		private void InjectOptions() {
			PauseMenu pause = _pauseFinder.Get();
			if (pause != null && pause.isActiveAndEnabled)
				_pauseOption.Pump(pause, (UISelectable)PauseCreditsField.GetValue(pause), (UISelectableGroup)PauseGroupField.GetValue(pause));
			StartScreenUI start = _startFinder.Get();
			if (start != null && start.isActiveAndEnabled)
				_startOption.Pump(start, (UISelectable)StartSettingsField.GetValue(start), (UISelectableGroup)StartGroupField.GetValue(start));
		}

		private void Open() {
			ModMenuState.Open = true;
			_openedFrame = Time.frameCount;
			_held = KeyCode.None;
			DropGameButtons();
			SpeechPipeline.SpeakInterrupt(_nav.Open(BuildRoot()));
			Review();
			Log.Info("mod menu opened");
		}

		private void Close() {
			_sounds.Stop();
			_nav.Close();
			ModMenuState.Open = false;
			DropGameButtons();
			SpeechPipeline.SpeakInterrupt(Strings.ModMenuClosed);
			Log.Info("mod menu closed");
		}

		private void Activate() {
			string spoken = _nav.Enter(out MenuItem activated);
			if (activated != null) {
				activated.Activate();
				return;
			}
			if (spoken == null) return;
			MenuItem current = _nav.Current;
			if (current != null && current.Setting != null) SettingChanged(current.Setting, spoken);
			else Say(spoken);
		}

		private void Adjust(int direction) {
			string spoken = _nav.Adjust(direction);
			if (spoken == null) return;
			SettingChanged(_nav.Current.Setting, spoken);
		}

		// A changed setting says its new value; a volume also plays a sample of its sounds.
		private void SettingChanged(Setting setting, string spoken) {
			SpeechPipeline.SpeakInterrupt(spoken);
			if (setting.Kind != SettingKind.Percent || setting.Value == 0) return;
			GlossaryEntry sample = SampleFor(setting);
			if (sample != null) _sounds.Play(sample);
		}

		private static GlossaryEntry SampleFor(Setting setting) {
			if (setting == ModSettings.Volume || setting == ModSettings.AttackVolume) return SoundExamples.Find(Strings.GlossaryBlock);
			if (setting == ModSettings.EnemyMovementVolume) return SoundExamples.Find(Strings.GlossaryEnemyReach);
			if (setting == ModSettings.LocatorVolume) return SoundExamples.Find(Strings.GlossaryEnemyPing);
			if (setting == ModSettings.WallVolume) return SoundExamples.Find(Strings.GlossaryWallCollision);
			if (setting == ModSettings.ProjectileVolume) return SoundExamples.Find(Strings.GlossaryProjectile);
			if (setting == ModSettings.HazardVolume) return SoundExamples.Find(Strings.GlossaryZoneActive);
			if (setting == ModSettings.ChimeVolume) return SoundExamples.Find(Strings.GlossaryRecharge);
			if (setting == ModSettings.MinigameVolume) return SoundExamples.Find(Strings.GlossaryCardHarp);
			return null;
		}

		private void Say(string text) {
			if (string.IsNullOrEmpty(text)) return;
			_sounds.Stop();
			SpeechPipeline.SpeakInterrupt(text);
			Review();
		}

		// The entry's details become the review buffer, as for any focus.
		private void Review() {
			MenuItem current = _nav.Current;
			if (current == null || HandOfFate2Access.Review.ReviewController.Instance == null) return;
			IList<string> lines = current.DetailLines() ?? new List<string> { current.Spoken };
			HandOfFate2Access.Review.ReviewController.Instance.Focus(lines);
		}

		// --- The menu's contents ---

		private MenuItem BuildRoot() {
			return MenuItem.Section(() => Strings.ModMenuTitle, () => Strings.ModMenuHint, () => new List<MenuItem> {
				MenuItem.Section(() => Strings.ModMenuCombatSounds, () => Strings.ModMenuSoundsHint,
					() => SoundItems(GlossaryCatalog.CombatEntries)),
				MenuItem.Section(() => Strings.ModMenuMinigameSounds, () => Strings.ModMenuSoundsHint,
					() => SoundItems(GlossaryCatalog.MinigameEntries)),
				MenuItem.Section(() => Strings.ModMenuHelpTopics, null, Topics),
				MenuItem.Section(() => Strings.ModMenuSettings, () => Strings.ModMenuSettingsHint, SettingItems),
			});
		}

		private IList<MenuItem> SoundItems(GlossaryEntry[] entries) {
			var items = new List<MenuItem>();
			foreach (GlossaryEntry entry in entries) {
				GlossaryEntry e = entry;
				items.Add(MenuItem.Action(() => e.Label, () => _sounds.Play(e)));
			}
			return items;
		}

		private IList<MenuItem> Topics() {
			var topics = new List<MenuItem> { Topic(() => Strings.TopicStart, HelpCatalog.StartEntries) };
			foreach (HelpContext context in HelpCatalog.TopicContexts) {
				HelpContext c = context;
				topics.Add(Topic(() => HelpCatalog.Title(c), () => HelpCatalog.ScreenEntries(c, false, c == HelpContext.Combat ? CombatKeys() : null)));
			}
			topics.Add(Topic(() => Strings.TopicModKeys, HelpCatalog.ModKeyEntries));
			return topics;
		}

		// A topic's lines; one that names a sound plays it on Enter.
		private MenuItem Topic(System.Func<string> title, System.Func<List<string>> lines) {
			return MenuItem.Section(title, null, () => {
				var items = new List<MenuItem>();
				foreach (string line in lines()) {
					GlossaryEntry sound = SoundExamples.Find(line);
					if (sound != null) items.Add(MenuItem.Action(() => line, () => _sounds.Play(sound)));
					else items.Add(MenuItem.Text(line));
				}
				return items;
			}, wraps: false);
		}

		private static CombatKeys CombatKeys() {
			try {
				return CombatKeyNames.Read();
			} catch (System.Exception ex) {
				Log.Error("combat key names unavailable for the help topic: " + ex.Message);
				return null;
			}
		}

		private static IList<MenuItem> SettingItems() {
			var items = new List<MenuItem>();
			foreach (Setting setting in ModSettings.All) items.Add(MenuItem.ForSetting(setting));
			return items;
		}

		// --- Keys ---

		private static bool Pressed(KeyCode key) => UnityEngine.Input.GetKeyDown(key);

		// An arrow on its press, then repeating while held.
		private KeyCode Arrow() {
			KeyCode key = KeyCode.None;
			if (UnityEngine.Input.GetKey(KeyCode.UpArrow)) key = KeyCode.UpArrow;
			else if (UnityEngine.Input.GetKey(KeyCode.DownArrow)) key = KeyCode.DownArrow;
			else if (UnityEngine.Input.GetKey(KeyCode.LeftArrow)) key = KeyCode.LeftArrow;
			else if (UnityEngine.Input.GetKey(KeyCode.RightArrow)) key = KeyCode.RightArrow;
			if (key == KeyCode.None) {
				_held = KeyCode.None;
				return KeyCode.None;
			}
			float now = Time.unscaledTime;
			if (key != _held) {
				_held = key;
				_nextRepeat = now + InitialRepeatDelay;
				return key;
			}
			if (now < _nextRepeat) return KeyCode.None;
			_nextRepeat = now + RepeatInterval;
			return key;
		}

		// The game takes a key as pressed for half a second after it goes down, unless
		// something consumes it; drop it so it is not acted on once the menu closes.
		private static void DropGameButtons() {
			Defiant.InputManager input = Defiant.InputManager.Instance;
			if (input != null) input.ClearCurrentButtonPress();
		}
	}
}
