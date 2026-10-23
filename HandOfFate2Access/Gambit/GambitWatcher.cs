using System.Collections.Generic;
using HandOfFate2Access.Audio;
using HandOfFate2Access.Focus;
using HandOfFate2Access.Localization;
using HandOfFate2Access.Settings;
using HandOfFate2Access.Speech;
using HandOfFate2Access.Util;
using UnityEngine;

namespace HandOfFate2Access.Gambit {
	/// <summary>
	/// Makes the chance-card gambit playable by ear.
	///
	/// Each card has an instrument for the whole gambit, chosen by the slot it holds when
	/// the cards are shown (harp, flute, handpan, music box from the left; see
	/// <see cref="GambitSounds"/>).
	///
	/// Shown: when the game turns the chance cards face up to show the odds
	/// (ChanceCardTask.OnRevealChances with the reshuffle flag set), the screen reader
	/// says how many cards there are and the tally of outcomes (in the game's own words;
	/// a gambit over prizes rather than chance cards, such as food and gold, lists each
	/// card from the left with its amount, since those cards carry no outcome sound),
	/// game time is held, and after a short pause the cards are walked left to right:
	/// each card plays its instrument, then its outcome's stinger, both from where it
	/// lies. Time is then restored and the game hides and shuffles.
	///
	/// Shuffle: from the moment the cards turn face down, the cards take turns playing
	/// their instruments (one every <see cref="GambitLayout.TurnSeconds"/>, by starting
	/// slot), each panned to where that card is on the table at that moment and following
	/// it while the note rings, so the player follows the card they want. The pan comes
	/// from the card's real position along the row (the game gathers the cards into a pile
	/// and spreads them out again), mapped so a card at rest in a slot sounds exactly
	/// where that slot did when shown. The shuffle ends when the game hands focus to a card.
	///
	/// Pick: focus on a face-down card says which card it is from the left and where it
	/// lies ("Card 2 of 4, left of centre, face down"); picking it reveals it
	/// (OnRevealChances without the reshuffle flag) and its outcome is spoken. A redraw
	/// (guardian angel and similar) reveals with the reshuffle flag again and re-runs the
	/// showing.
	///
	/// With the chance card hints setting on, each step is explained as it starts.
	/// Cards are held by live reference for the duration of one gambit (identity is the
	/// starting slot); type and position are re-read every frame.
	/// </summary>
	public sealed class GambitWatcher {
		private const int MaxSlots = 9;
		private const float SlotGapSeconds = 0.15f;
		private const float IntroSeconds = 1.5f;
		private const float IntroWithHintSeconds = 5f;
		private const float EstablishSafetySeconds = 25f;
		private const float ShuffleSafetySeconds = 30f;
		private const float ShuffleSettleSeconds = 0.15f;

		private enum Phase { Idle, Establishing, AwaitingShuffle, Shuffling, Selecting }

		private bool _available;

		private Phase _phase;
		private ChanceCardTask _subscribed;
		private bool _revealPending;
		private bool _frozen;
		private float _restoreTimeScale = 1f;

		private Card[] _cards;
		private int _count;

		// The row as shown: where each slot lay along the line from the first card to the
		// last, in the container's space, so a live position maps back to a pan.
		private Vector3 _rowOrigin;
		private Vector3 _rowAxis;
		private float[] _slotAlong;
		private bool _rowValid;

		private int _cursor;
		private float _nextEventTime;
		private bool _outcomePending;
		private string _pendingOutcome;
		private float _pendingPan;
		private float _establishDeadline;

		private HandOfFate2Access.Audio.Voice[] _turnVoices;
		private int _turn;
		private float _nextTurn;
		private float _shuffleDeadline;
		private bool _settling;
		private float _settleDeadline;

		/// <param name="pluginDir">The plugin folder; the card sounds are its sounds/card_*.wav and outcome_*.wav.</param>
		public bool Initialize(string pluginDir) {
			if (!AudioEngine.IsAvailable) {
				Log.Warn("audio backend unavailable; gambit audio disabled");
				return false;
			}
			CardSounds.Load(pluginDir);
			_available = true;
			// Filled with None: a default handle is slot 0 and would resolve to another feature's voice.
			_turnVoices = new HandOfFate2Access.Audio.Voice[MaxSlots];
			for (int i = 0; i < MaxSlots; i++) _turnVoices[i] = HandOfFate2Access.Audio.Voice.None;
			Log.Info("gambit sounds ready");
			return true;
		}

		public void Pump() {
			if (!_available) return;
			try {
				PumpInner();
			} catch (System.Exception ex) {
				Log.Error("gambit pump failed: " + ex);
				Reset();
			}
		}

		private void PumpInner() {
			ChanceCardTask task = ChanceCardTask.RunningTask;
			if (task == null) {
				if (_phase != Phase.Idle || _subscribed != null) Reset();
				return;
			}
			if (_subscribed != task) {
				Reset();
				_subscribed = task;
				task.OnRevealChances = (ChanceCardTask.ChanceDelegate)System.Delegate.Combine(
					task.OnRevealChances, new ChanceCardTask.ChanceDelegate(OnReveal));
			}
			CardContainer container = Container();
			if (container == null) return;

			if (_revealPending) {
				_revealPending = false;
				if (task.ReShuffleConsumed) {
					if (Capture(container)) BeginEstablish();
				} else {
					AnnouncePick(task, container);
				}
				return;
			}

			switch (_phase) {
				case Phase.Establishing: DriveEstablish(); break;
				case Phase.AwaitingShuffle:
					if (AllFaceDown(container)) BeginShuffle(container);
					break;
				case Phase.Shuffling: DriveShuffle(container); break;
			}
		}

		private void OnReveal(ChanceCardTask task) => _revealPending = true;

		private static CardContainer Container() {
			UI3D ui = UI3D.Instance;
			return ui != null && ui.CardChoice != null ? ui.CardChoice.Container : null;
		}

		private bool Capture(CardContainer container) {
			var cards = container.Cards;
			if (cards.Count == 0 || cards.Count > MaxSlots) {
				Log.Warn("gambit has " + cards.Count + " cards; skipping gambit audio");
				return false;
			}
			_cards = cards.ToArray();
			_count = _cards.Length;
			CaptureRow(container);
			return true;
		}

		// The row's line and each slot's place along it, from the cards as dealt face up.
		// A row whose slots do not run strictly one way (one card, or cards still flying in)
		// falls back to panning by list position.
		private void CaptureRow(CardContainer container) {
			_rowValid = false;
			_slotAlong = new float[_count];
			if (_count < 2) return;
			Vector3 first = Local(container, _cards[0]);
			Vector3 last = Local(container, _cards[_count - 1]);
			Vector3 axis = last - first;
			if (axis.sqrMagnitude < 1e-6f) {
				Log.Warn("gambit row has no length; panning by slot");
				return;
			}
			_rowOrigin = first;
			_rowAxis = axis.normalized;
			for (int i = 0; i < _count; i++) {
				_slotAlong[i] = Vector3.Dot(Local(container, _cards[i]) - _rowOrigin, _rowAxis);
				if (i > 0 && _slotAlong[i] <= _slotAlong[i - 1] + 1e-4f) {
					Log.Warn("gambit row slots out of order; panning by slot");
					return;
				}
			}
			_rowValid = true;
		}

		private static Vector3 Local(CardContainer container, Card card) =>
			container.transform.InverseTransformPoint(card.transform.position);

		private static bool AllFaceDown(CardContainer container) {
			if (container.Cards.Count == 0) return false;
			foreach (Card c in container.Cards)
				if (!c.Flipped) return false;
			return true;
		}

		private static bool Hints => ModSettings.ChanceCardHints.On;

		// --- Shown ---

		private void BeginEstablish() {
			StopVoices();
			_phase = Phase.Establishing;
			Freeze();
			_cursor = 0;
			_outcomePending = false;
			_nextEventTime = Time.unscaledTime + (Hints ? IntroWithHintSeconds : IntroSeconds);
			_establishDeadline = Time.unscaledTime + EstablishSafetySeconds;
			bool chance = AllChance();
			SpeechPipeline.SpeakEventInterrupt(chance ? Strings.GambitIntro(_count, Tally()) : Strings.GambitIntroCards(_count, InOrder()));
			if (Hints) SpeechPipeline.SpeakEvent(chance ? Strings.GambitTeachHint : Strings.GambitTeachHintCards);
			Log.Info("gambit establish: " + _count + " cards, time held, row " + (_rowValid ? "mapped" : "by slot"));
		}

		private bool AllChance() {
			foreach (Card card in _cards)
				if (!(card is ChanceCard)) return false;
			return true;
		}

		// Prize cards from the left, each by its name and amount: "Supplies, 3 Food; Treasure, 15 Gold".
		private string InOrder() {
			var parts = new List<string>();
			foreach (Card card in _cards)
				if (card != null) parts.Add(Label(card));
			return string.Join("; ", parts.ToArray());
		}

		// A card's name and amount ("Bad Fortune, 30 Gold"). A fortune card has no title of
		// its own (its face prints the fortune's name) and no stats: the amount is its face's
		// description line.
		private static string Label(Card card) {
			CardInfoReader.Parts parts = CardInfoReader.ReadParts(card);
			string name = parts.Title ?? (parts.Face.Count > 0 ? parts.Face[0] : CardInfoReader.ShortName(card));
			var message = new UI.Message().Add(name).AddRange(parts.Stats);
			string amount = FortuneAmount(card);
			if (amount != null && amount != name) message.Add(amount);
			if (parts.Stats.Count == 0 && amount == null)
				Log.Warn("gambit card '" + card.name + "' read without an amount: " + string.Join(" | ", parts.All().ToArray()));
			return message.Resolve();
		}

		// The description line on a fortune card's face ("30 Gold"), as the game set it.
		private static string FortuneAmount(Card card) {
			if (!(card is FortuneCard) || card.CardTemplate == null) return null;
			var description = card.CardTemplate.GetComponentInChildren<FortuneDescriptionCardTemplateComponent>(true);
			if (description == null || description.Label == null) {
				Log.Warn("fortune card '" + card.name + "' has no description label");
				return null;
			}
			string text = description.Label.text;
			return HandOfFate2Access.UI.Placeholder.IsMissing(text) ? null : text;
		}

		// The outcomes on the table, counted by the game's own names in slot order:
		// "3 Success, 1 Failure".
		private string Tally() {
			var names = new List<string>();
			var counts = new List<int>();
			foreach (Card card in _cards) {
				string name = card != null ? CardInfoReader.ShortName(card) : null;
				if (string.IsNullOrEmpty(name)) continue;
				int at = names.IndexOf(name);
				if (at < 0) { names.Add(name); counts.Add(1); } else counts[at]++;
			}
			var parts = new List<string>();
			for (int i = 0; i < names.Count; i++) parts.Add(Strings.GambitTally(counts[i], names[i]));
			return string.Join(", ", parts.ToArray());
		}

		private void DriveEstablish() {
			if (Time.unscaledTime >= _establishDeadline) {
				EndEstablish("safety deadline");
				return;
			}
			if (Time.unscaledTime < _nextEventTime) return;
			if (_outcomePending) {
				float seconds = 0.2f;
				if (_pendingOutcome != null) {
					AudioEngine.PlayOneShot(_pendingOutcome, new SoundParams(_pendingPan, 1f, 1f), SoundCategory.Minigames);
					seconds = CardSounds.Length(_pendingOutcome);
				}
				_nextEventTime = Time.unscaledTime + seconds + SlotGapSeconds;
				_outcomePending = false;
				_cursor++;
				return;
			}
			if (_cursor < _count) {
				float pan = GambitLayout.SlotPan(_cursor, _count);
				string card = GambitSounds.CardKey(_cursor);
				AudioEngine.PlayOneShot(card, new SoundParams(pan, GambitSounds.CardPitch(_cursor), 1f), SoundCategory.Minigames);
				_pendingOutcome = OutcomeOf(_cards[_cursor]);
				_pendingPan = pan;
				_nextEventTime = Time.unscaledTime + CardSounds.Length(card) / GambitSounds.CardPitch(_cursor);
				_outcomePending = true;
				return;
			}
			EndEstablish(null);
		}

		// The stinger key for a card's outcome; null for a card that is not a chance card or
		// an outcome whose sound did not load (logged).
		private static string OutcomeOf(Card card) {
			var chance = card as ChanceCard;
			if (chance == null) {
				Log.Warn("gambit card '" + (card != null ? card.name : "null") + "' is not a chance card; no outcome sound");
				return null;
			}
			string key = GambitSounds.OutcomeKey(Outcome(chance.ChanceType));
			if (CardSounds.IsLoaded(key)) return key;
			Log.Warn("gambit outcome " + chance.ChanceType + " has no sound loaded");
			return null;
		}

		private static ChanceOutcome Outcome(ChanceType type) {
			switch (type) {
				case ChanceType.HugeSuccess: return ChanceOutcome.HugeSuccess;
				case ChanceType.Success: return ChanceOutcome.Success;
				case ChanceType.HugeFailure: return ChanceOutcome.HugeFailure;
				default: return ChanceOutcome.Failure;
			}
		}

		private void EndEstablish(string reason) {
			Unfreeze();
			_phase = Phase.AwaitingShuffle;
			if (reason != null)
				Log.Warn("gambit establish ended early (" + reason + "); time restored");
			else
				Log.Info("gambit establish complete; shuffle released");
		}

		// --- Shuffle ---

		private void BeginShuffle(CardContainer container) {
			_phase = Phase.Shuffling;
			_settling = false;
			_turn = 0;
			_nextTurn = Time.unscaledTime;
			_shuffleDeadline = Time.unscaledTime + ShuffleSafetySeconds;
			if (Hints) SpeechPipeline.SpeakEvent(Strings.GambitShuffleHint);
			Log.Info("gambit shuffle: cards taking turns");
		}

		private void DriveShuffle(CardContainer container) {
			if (container.Cards.Count == 0 || Time.unscaledTime >= _shuffleDeadline) {
				EndShuffle();
				return;
			}
			// A ringing note follows its card.
			for (int i = 0; i < _count; i++) {
				if (_cards[i] == null || !_turnVoices[i].IsValid) continue;
				if (!AudioEngine.IsPlaying(_turnVoices[i])) {
					_turnVoices[i] = HandOfFate2Access.Audio.Voice.None;
					continue;
				}
				AudioEngine.Update(_turnVoices[i], new SoundParams(LivePan(container, i), GambitSounds.CardPitch(i), 1f));
			}
			if (!_settling && Time.unscaledTime >= _nextTurn) {
				int slot = _turn % _count;
				_turn++;
				_nextTurn = Time.unscaledTime + GambitLayout.TurnSeconds;
				if (_cards[slot] != null) {
					if (_turnVoices[slot].IsValid) AudioEngine.Stop(_turnVoices[slot]);
					_turnVoices[slot] = AudioEngine.Play(GambitSounds.CardKey(slot),
						new SoundParams(LivePan(container, slot), GambitSounds.CardPitch(slot), 1f), false, SoundCategory.Minigames);
				}
			}
			if (!_settling) {
				if (FocusedIndex() >= 0) {
					_settling = true;
					_settleDeadline = Time.unscaledTime + ShuffleSettleSeconds;
				}
			} else if (Time.unscaledTime >= _settleDeadline) {
				EndShuffle();
			}
		}

		// Where the card that started in <paramref name="slot"/> is now, as a pan: its place
		// along the row, interpolated between the shown slots' pans (and held at the ends).
		private float LivePan(CardContainer container, int slot) {
			Card card = _cards[slot];
			if (!_rowValid) {
				int index = container.Cards.IndexOf(card);
				return GambitLayout.SlotPan(index >= 0 ? index : slot, _count);
			}
			float along = Vector3.Dot(Local(container, card) - _rowOrigin, _rowAxis);
			if (along <= _slotAlong[0]) return GambitLayout.SlotPan(0, _count);
			for (int i = 1; i < _count; i++) {
				if (along > _slotAlong[i]) continue;
				float t = (along - _slotAlong[i - 1]) / (_slotAlong[i] - _slotAlong[i - 1]);
				float a = GambitLayout.SlotPan(i - 1, _count), b = GambitLayout.SlotPan(i, _count);
				return a + (b - a) * t;
			}
			return GambitLayout.SlotPan(_count - 1, _count);
		}

		private void EndShuffle() {
			StopVoices();
			_phase = Phase.Selecting;
			SpeechPipeline.SpeakEvent(Hints ? Strings.GambitPickHint : Strings.GambitPickShort);
			Log.Info("gambit shuffle ended; cards pickable");
		}

		// --- Pick ---

		private void AnnouncePick(ChanceCardTask task, CardContainer container) {
			StopVoices();
			int index = task.ChanceCardIndex;
			if (index < 0 || index >= container.Cards.Count) {
				Log.Warn("gambit pick index " + index + " out of range");
				return;
			}
			Card picked = container.Cards[index];
			string title = picked is ChanceCard ? CardInfoReader.ShortName(picked) : Label(picked);
			if (!string.IsNullOrEmpty(title))
				SpeechPipeline.SpeakEventInterrupt(title);
			Log.Info("gambit pick revealed: " + title);
		}

		/// <summary>
		/// While picking, the focused face-down gambit card in words, said instead of its
		/// hidden outcome: its place from the left, of how many, and where that lies
		/// ("Card 2 of 4, left of centre, face down").
		/// </summary>
		public bool TryCardName(GameObject focused, out string text) {
			text = null;
			if (!_available || focused == null) return false;
			if (_phase != Phase.Selecting && _phase != Phase.Shuffling) return false;
			Card card = focused.GetComponentInParent<Card>();
			if (card == null || !card.Flipped) return false;
			CardContainer container = Container();
			if (container == null) return false;
			int index = container.Cards.IndexOf(card);
			if (index < 0) return false;
			int count = container.Cards.Count;
			text = Strings.GambitCard(index + 1, count, Where(GambitLayout.SlotPan(index, count)));
			return true;
		}

		private static string Where(float pan) {
			if (pan <= -0.75f) return Strings.GambitAtFarLeft;
			if (pan < -0.1f) return Strings.GambitAtLeft;
			if (pan <= 0.1f) return Strings.GambitAtCentre;
			if (pan < 0.75f) return Strings.GambitAtRight;
			return Strings.GambitAtFarRight;
		}

		private int FocusedIndex() {
			UISelectable s = FocusTracker.CurrentSelectable();
			if (s == null) return -1;
			Card card = s.GetComponentInParent<Card>();
			if (card == null) return -1;
			for (int i = 0; i < _count; i++)
				if (_cards[i] == card) return i;
			return -1;
		}

		private void StopVoices() {
			if (_turnVoices == null) return;
			for (int i = 0; i < _turnVoices.Length; i++) {
				if (_turnVoices[i].IsValid) AudioEngine.Stop(_turnVoices[i]);
				_turnVoices[i] = HandOfFate2Access.Audio.Voice.None;
			}
		}

		private void Freeze() {
			_restoreTimeScale = Time.timeScale > 0f ? Time.timeScale : 1f;
			Time.timeScale = 0f;
			_frozen = true;
		}

		private void Unfreeze() {
			if (!_frozen) return;
			Time.timeScale = _restoreTimeScale;
			_frozen = false;
		}

		private void Reset() {
			StopVoices();
			Unfreeze();
			_phase = Phase.Idle;
			_revealPending = false;
			if (_subscribed != null)
				_subscribed.OnRevealChances = (ChanceCardTask.ChanceDelegate)System.Delegate.Remove(
					_subscribed.OnRevealChances, new ChanceCardTask.ChanceDelegate(OnReveal));
			_subscribed = null;
			_cards = null;
			_count = 0;
		}
	}
}
