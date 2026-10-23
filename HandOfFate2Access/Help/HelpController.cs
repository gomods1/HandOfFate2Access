using System;
using System.Collections.Generic;
using HandOfFate2Access.Localization;
using HandOfFate2Access.Speech;
using UnityEngine;

namespace HandOfFate2Access.Help {
	/// <summary>
	/// The help list on F4 (the game binds F1 to its inventory). Opening it says
	/// "Help, (screen)" and the first entry: the game's keys for the screen the player
	/// is on, then the mod's own keys. Up and Down step through the entries, Home and
	/// End jump to the ends, F4 or Backspace closes it. While it is open the game's UI
	/// input is skipped (UIInputManager_Update_Patch), and on every frame of it the game's
	/// pending button press is dropped, so the Backspace that closes the list (also the
	/// game's cancel) does not also go back a screen. Shift plus F4 is the mod menu's key.
	/// Enter on an entry that names a mod sound (the combat cues) plays it.
	/// </summary>
	internal sealed class HelpController {
		private readonly Func<HelpContext> _context;
		private readonly Func<bool> _fightUnderneath;
		private readonly ModMenu.SoundExamples _sounds;
		private List<string> _entries = new List<string>();
		private int _index;

		/// <param name="context">The screen or mode the player is on now.</param>
		/// <param name="fightUnderneath">A fight runs under the current screen (the pause menu in a fight).</param>
		/// <param name="sounds">Plays the sound an entry names.</param>
		public HelpController(Func<HelpContext> context, Func<bool> fightUnderneath, ModMenu.SoundExamples sounds) {
			_context = context;
			_fightUnderneath = fightUnderneath;
			_sounds = sounds;
		}

		public void Pump() {
			if (!HelpState.Open) {
				if (UnityEngine.Input.GetKeyDown(KeyCode.F4) && !Input.KeyClaims.ShiftHeld && !ModMenu.ModMenuState.Open) Open();
				return;
			}
			DropGameButtons();
			if (UnityEngine.Input.GetKeyDown(KeyCode.F4) || UnityEngine.Input.GetKeyDown(KeyCode.Backspace)) {
				Close();
				return;
			}
			if (UnityEngine.Input.GetKeyDown(KeyCode.DownArrow)) Move(_index + 1);
			else if (UnityEngine.Input.GetKeyDown(KeyCode.UpArrow)) Move(_index - 1);
			else if (UnityEngine.Input.GetKeyDown(KeyCode.Home)) Move(0);
			else if (UnityEngine.Input.GetKeyDown(KeyCode.End)) Move(_entries.Count - 1);
			else if (UnityEngine.Input.GetKeyDown(KeyCode.Return) || UnityEngine.Input.GetKeyDown(KeyCode.KeypadEnter)) PlayEntry();
		}

		private void PlayEntry() {
			if (_index < 0 || _index >= _entries.Count) return;
			Glossary.GlossaryEntry sound = ModMenu.SoundExamples.Find(_entries[_index]);
			if (sound != null) _sounds.Play(sound);
		}

		private void Open() {
			HelpContext context = _context();
			bool fight = context == HelpContext.Combat || _fightUnderneath();
			_entries = HelpCatalog.Entries(context, fight, fight ? CombatKeyNames.Read() : null);
			_index = 0;
			HelpState.Open = true;
			DropGameButtons();
			SpeechPipeline.SpeakInterrupt(Strings.HelpTitle(HelpCatalog.Title(context)));
			if (_entries.Count > 0) SpeechPipeline.SpeakQueued(_entries[0]);
		}

		private void Move(int index) {
			if (index < 0) {
				SpeechPipeline.SpeakInterrupt(Strings.ReviewTop);
				return;
			}
			if (index >= _entries.Count) {
				SpeechPipeline.SpeakInterrupt(Strings.ReviewEnd);
				return;
			}
			_index = index;
			_sounds.Stop();
			SpeechPipeline.SpeakInterrupt(_entries[_index]);
		}

		private void Close() {
			_sounds.Stop();
			HelpState.Open = false;
			DropGameButtons();
			SpeechPipeline.SpeakInterrupt(Strings.HelpClosed);
		}

		// The game takes a key as pressed for half a second after it goes down, unless
		// something consumes it; drop it so it is not acted on once the list closes.
		private static void DropGameButtons() {
			Defiant.InputManager input = Defiant.InputManager.Instance;
			if (input != null) input.ClearCurrentButtonPress();
		}
	}
}
