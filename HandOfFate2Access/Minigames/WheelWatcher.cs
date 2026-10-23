using HandOfFate2Access.Audio;
using HandOfFate2Access.Focus;
using HandOfFate2Access.Speech;
using HandOfFate2Access.UI;
using HandOfFate2Access.Util;

namespace HandOfFate2Access.Minigames {
	/// <summary>
	/// Makes the wheel minigame playable by ear. When the wheel's segments are laid out,
	/// they are spoken in order (the cards on the wheel). While it spins, a short tick in
	/// the passing segment's outcome voice plays each time a new segment reaches the
	/// pointer, so the player hears the good segments go by and presses confirm to start
	/// the wind-down. Where it lands is spoken when the spin completes.
	/// </summary>
	internal sealed class WheelWatcher {
		private bool _available;
		private int _rate;
		private float[][] _ticks;
		private WheelSpinTask _announcedTask;
		private int _lastIndex = -1;
		private bool _subscribed;
		private WheelOfFortune _wheel;
		private WheelOfFortune.CardSegment _landed;

		public void Initialize() {
			_available = AudioEngine.IsAvailable;
			if (!_available) return;
			_rate = AudioEngine.OutputSampleRate;
			_ticks = new float[5][];
			for (int i = 0; i < 5; i++)
				_ticks[i] = OutcomeTones.Tick((OutcomeKind)i, _rate);
		}

		public void Pump() {
			WheelSpinTask task = WheelSpinTask.RunningTask;
			if (task == null || DeckManager.Instance == null) {
				Unsubscribe();
				_announcedTask = null;
				_lastIndex = -1;
				return;
			}
			WheelOfFortune wheel = DeckManager.Instance.WheelSpinContainer.GetComponent<WheelOfFortune>();
			if (wheel != _wheel) {
				Unsubscribe();
				_wheel = wheel;
				_wheel.OnSpinComplete = (WheelOfFortune.SegmentDelegate)System.Delegate.Combine(_wheel.OnSpinComplete,
					new WheelOfFortune.SegmentDelegate(OnSpinComplete));
				_subscribed = true;
			}
			if (_announcedTask != task && wheel.Segments.Count > 0 && wheel.SpinContainer.Cards.Count == wheel.Segments.Count) {
				_announcedTask = task;
				var message = new Message();
				foreach (WheelOfFortune.CardSegment segment in wheel.Segments)
					message.Add(CardInfoReader.ShortName(segment.card));
				SpeechPipeline.SpeakEvent(message.Resolve());
			}
			if (_landed != null) {
				string title = CardInfoReader.ShortName(_landed.card);
				_landed = null;
				SpeechPipeline.SpeakEventInterrupt(title);
				Log.Info("wheel landed on " + title);
			}
			int index = wheel.GetCurrentIndex();
			if (index != _lastIndex && index >= 0) {
				_lastIndex = index;
				if (_available)
					AudioEngine.PlayBuffer(_ticks[(int)Outcomes.Kind(wheel.Segments[index].card)], _rate, false, new SoundParams(0f, 1f, 1f), SoundCategory.Minigames);
			}
		}

		private void OnSpinComplete(WheelOfFortune.CardSegment segment) => _landed = segment;

		private void Unsubscribe() {
			if (_subscribed && _wheel != null)
				_wheel.OnSpinComplete = (WheelOfFortune.SegmentDelegate)System.Delegate.Remove(_wheel.OnSpinComplete,
					new WheelOfFortune.SegmentDelegate(OnSpinComplete));
			_subscribed = false;
			_wheel = null;
		}
	}
}
