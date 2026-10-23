using System;
using System.Collections.Generic;
using HandOfFate2Access.Input;
using HandOfFate2Access.Localization;
using HandOfFate2Access.Speech;
using UnityEngine;

namespace HandOfFate2Access.Review {
	/// <summary>
	/// The review buffers and their keys. Details holds what the focused thing has to say,
	/// line by line (fed by the focus pump: a focus move makes it current, the cursor
	/// before the top); Events every unsolicited announcement, newest first; Status the
	/// run's resources and equipment, rebuilt on each read.
	///
	/// Keys: Ctrl+Up the next line, Ctrl+Down the previous one, Ctrl+Left / Ctrl+Right
	/// the other buffers. Held, a key repeats. The game never sees these arrows (the
	/// GetDirection patch hides them while Ctrl is down).
	/// </summary>
	internal sealed class ReviewController : IInputBinding {
		private const int EventCap = 200;
		private const float InitialRepeatDelay = 0.4f;
		private const float RepeatInterval = 0.12f;

		private readonly ReviewBuffer _details = new ReviewBuffer(() => Strings.ReviewDetails);
		private readonly ReviewBuffer _events = new ReviewBuffer(() => Strings.ReviewEvents, EventCap);
		private readonly ReviewBuffer _status = new ReviewBuffer(() => Strings.ReviewStatus);
		private readonly ReviewSet _set;

		private KeyCode _held = KeyCode.None;
		private float _nextRepeat;
		private int _pinnedFrame = -1;

		/// <summary>The live controller, for readers that speak a card without moving focus
		/// (the map cursor, the deck builder's centre card).</summary>
		public static ReviewController Instance { get; private set; }

		public ReviewController(Func<IList<string>> status) {
			Instance = this;
			_status.Refresher = status;
			_set = new ReviewSet(_details, _events, _status);
			SpeechPipeline.EventSink = line => _events.Add(line);
		}

		public string Name => "review";

		/// <summary>Focus moved to something new: its lines become the current buffer
		/// (unless a pinned card still holds it: see <see cref="Pin"/>).</summary>
		public void Focus(IList<string> details) {
			if (_pinnedFrame >= 0) {
				if (HandOfFate2Access.Focus.NavigationState.LastNavigateFrame <= _pinnedFrame) return;
				_pinnedFrame = -1;
			}
			_details.Set(details);
			_set.MakeCurrent(_details);
		}

		/// <summary>
		/// A card the game showed without leaving focus on it (a reward turned over, after
		/// which the game moves focus to the next face-down card at once): its lines become
		/// the details and stay there until the player next moves with an arrow, so
		/// Control plus up arrow reads the card just revealed.
		/// </summary>
		public void Pin(IList<string> details) {
			_pinnedFrame = -1;
			Focus(details);
			_pinnedFrame = Time.frameCount;
		}

		/// <summary>The focused thing's lines changed in place (a value, a late label): the
		/// buffer follows, and keeps the player's place if nothing changed.</summary>
		public void Refresh(IList<string> details) {
			_details.Set(details);
		}

		/// <summary>A fight began: the menu focus the details came from is gone, and what
		/// there is to review is what happens, so the events become current.</summary>
		public void ClearFocus() {
			_details.Clear();
			_set.MakeCurrent(_events);
		}

		public void Poll() {
			if (Help.HelpState.Open || !KeyClaims.CtrlHeld) {
				_held = KeyCode.None;
				return;
			}
			KeyCode key = HeldArrow();
			if (key == KeyCode.None) {
				_held = KeyCode.None;
				return;
			}
			float now = Time.unscaledTime;
			if (key != _held) {
				_held = key;
				_nextRepeat = now + InitialRepeatDelay;
			} else if (now >= _nextRepeat) {
				_nextRepeat = now + RepeatInterval;
			} else {
				return;
			}
			SpeechPipeline.SpeakInterrupt(Step(key));
		}

		private string Step(KeyCode key) {
			switch (key) {
				case KeyCode.UpArrow: return _set.Deeper();
				case KeyCode.DownArrow: return _set.TowardTop();
				case KeyCode.LeftArrow: return _set.Switch(-1);
				default: return _set.Switch(1);
			}
		}

		private static KeyCode HeldArrow() {
			if (UnityEngine.Input.GetKey(KeyCode.UpArrow)) return KeyCode.UpArrow;
			if (UnityEngine.Input.GetKey(KeyCode.DownArrow)) return KeyCode.DownArrow;
			if (UnityEngine.Input.GetKey(KeyCode.LeftArrow)) return KeyCode.LeftArrow;
			if (UnityEngine.Input.GetKey(KeyCode.RightArrow)) return KeyCode.RightArrow;
			return KeyCode.None;
		}
	}
}
