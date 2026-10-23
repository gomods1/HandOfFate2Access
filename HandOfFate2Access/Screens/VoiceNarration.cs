using System;
using System.Reflection;
using System.Text.RegularExpressions;
using HandOfFate2Access.Speech;
using HandOfFate2Access.Util;
using HarmonyLib;
using UnityEngine;

namespace HandOfFate2Access.Screens {
	/// <summary>
	/// Speaks every voiced line (the dealer, companions, characters) in full, read off
	/// the clip the game's voice manager is playing: the clip's subtitle text, whole,
	/// before the game would split it into timed two-line chunks. Polled from the
	/// voice manager rather than hooked on the subtitle display because the game only
	/// draws subtitles when the player turned them on or the text and voice-over
	/// languages differ; an English player with subtitles off would otherwise hear
	/// nothing of the dealer. A line is spoken once its audio is actually playing: the
	/// clip's trigger asset loads asynchronously and until it does the voice clip still
	/// holds the asset of its previous play, which must not be read again.
	/// </summary>
	internal sealed class VoiceNarration {
		// Timed lines carry "[sX.XeY.Y]" cue markers; the pilcrow marks a line break.
		private static readonly Regex TimingTag = new Regex(@"\[s[\d.]*e[\d.]*\]");

		private static readonly FieldInfo ClipTriggers = AccessTools.Field(typeof(VoiceClip), "m_currentClip");
		private static readonly FieldInfo ExplicitTriggers = AccessTools.Field(typeof(VoiceClipExplicit), "m_currentClip");

		private IVoiceClip _lastClip;
		private VoiceClipTriggers _lastTriggers;
		private int _lastPlayFrame = -1;

		/// <summary>True when a line was spoken this frame.</summary>
		public bool Pump() {
			Voice voice = Voice.Instance;
			IVoiceClip clip = voice != null ? voice.CurrentClip : null;
			if (clip == null) {
				_lastClip = null;
				_lastTriggers = null;
				_lastPlayFrame = -1;
				return false;
			}
			VoiceClipTriggers triggers = TriggersOf(clip);
			int playFrame = voice.ActivePlayFrame;
			if (triggers == null) return false;
			if (clip == _lastClip && triggers == _lastTriggers && playFrame == _lastPlayFrame) return false;
			// The line belongs to this play only once its own audio is sounding.
			AudioSource audio = clip.Audio;
			if (audio == null || !audio.isPlaying || audio.clip != triggers.Audio) return false;
			_lastClip = clip;
			_lastTriggers = triggers;
			_lastPlayFrame = playFrame;
			if (triggers.Subtitle == null || !triggers.Subtitle.HasKey) return false;
			string value;
			try {
				value = triggers.Subtitle.GetValue(triggers.StringLibrary);
			} catch (Exception ex) {
				Log.Error("voice line read failed: " + ex);
				return false;
			}
			if (string.IsNullOrEmpty(value)) return false;
			// The spoken lines setting off: the line is still taken (so turning it back on
			// does not read a stale one), just not spoken.
			if (!Settings.ModSettings.VoiceLines.On) return false;
			SpeechPipeline.SpeakEvent(TimingTag.Replace(value, " ").Replace("¶", ""));
			return true;
		}

		private static VoiceClipTriggers TriggersOf(IVoiceClip clip) {
			FieldInfo field = clip is VoiceClip ? ClipTriggers : clip is VoiceClipExplicit ? ExplicitTriggers : null;
			if (field == null) {
				Log.Warn("voice clip of unknown type " + clip.GetType().Name + "; its line is not read");
				return null;
			}
			return field.GetValue(clip) as VoiceClipTriggers;
		}
	}
}
