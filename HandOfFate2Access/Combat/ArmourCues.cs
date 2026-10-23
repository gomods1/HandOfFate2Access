using System;
using System.Collections.Generic;
using System.IO;
using HandOfFate2Access.Audio;
using HandOfFate2Access.Speech;
using HandOfFate2Access.Util;
using UnityEngine;

namespace HandOfFate2Access.Combat {
	/// <summary>
	/// An enemy's armour, by ear. The game's armour is a <c>Protection</c> on the enemy:
	/// it takes damage only from Bash hits (hit ids containing "flurry"), absorbs every
	/// other blow, breaks at zero and can mend or regrow (the Terror of Westwend regrows
	/// it whole). The game draws this as the enemy's armour model and a button hint over
	/// it; here each event plays its sample from the enemy (Core
	/// <see cref="ArmourCueComposer"/>), and three are said: the first glancing blow on an
	/// armour ("Armoured, use Bash, E"), "Armour broken", and "Armour restored" when broken
	/// armour comes back.
	///
	/// Events come from the game's static <c>Protection</c> delegates (raised from its hit
	/// handling and its regrowth); the handlers only record them, and the pump plays and
	/// speaks them, so nothing is said from inside the game's hit code.
	/// </summary>
	internal sealed class ArmourCues {
		private struct Pending {
			public ArmourEvent Event;
			public Protection Armour;
		}

		private bool _soundsReady;
		private readonly List<Pending> _pending = new List<Pending>();
		private readonly HashSet<Protection> _warned = new HashSet<Protection>();
		private readonly HashSet<Protection> _damaged = new HashSet<Protection>();
		private readonly HashSet<Protection> _broken = new HashSet<Protection>();
		private readonly HashSet<Protection> _brokeThisPump = new HashSet<Protection>();
		private bool _inFight;

		public void Initialize(string pluginDir) {
			if (AudioEngine.IsAvailable) {
				string soundsDir = Path.Combine(pluginDir, "sounds");
				int loaded = 0;
				foreach (string key in ArmourCueComposer.Keys)
					if (LoadSample(soundsDir, key)) loaded++;
				_soundsReady = loaded == ArmourCueComposer.Keys.Length;
			} else {
				Log.Warn("audio backend unavailable; armour events are spoken only");
			}
			Protection.OnProtectionDamageAbsorbed = (Protection.ProtectionDelegate)Delegate.Combine(
				Protection.OnProtectionDamageAbsorbed, new Protection.ProtectionDelegate(OnAbsorbed));
			Protection.OnProtectionDamaged = (Protection.OnDamageDelegate)Delegate.Combine(
				Protection.OnProtectionDamaged, new Protection.OnDamageDelegate(OnDamaged));
			Protection.OnProtectionBroken = (Protection.BrokenDelegate)Delegate.Combine(
				Protection.OnProtectionBroken, new Protection.BrokenDelegate(OnBroken));
			Protection.OnProtectionRegen = (Protection.OnDamageDelegate)Delegate.Combine(
				Protection.OnProtectionRegen, new Protection.OnDamageDelegate(OnRegen));
			Log.Info("armour cues subscribed" + (_soundsReady ? "" : " (some sounds missing)"));
		}

		private static bool LoadSample(string soundsDir, string key) {
			string path = Path.Combine(soundsDir, key + ".wav");
			try {
				WavAudio.Decode(File.ReadAllBytes(path), out float[] pcm, out int channels, out int sampleRate);
				AudioEngine.Register(key, pcm, channels, sampleRate);
				return true;
			} catch (Exception ex) {
				Log.Error("armour sample '" + key + "' failed to load from " + path + ": " + ex);
				return false;
			}
		}

		// --- Game delegates: record only. ---

		private void OnAbsorbed(Protection armour) => Record(ArmourEvent.Absorbed, armour);

		private void OnDamaged(Protection armour, int amount, bool modified) => Record(ArmourEvent.Cracked, armour);

		private void OnBroken(ICombatEffectContainer container, Protection armour) => Record(ArmourEvent.Broken, armour);

		// The game raises this for a mending step and for regrowth whole; the pump tells them apart.
		private void OnRegen(Protection armour, int amount, bool modified) => Record(ArmourEvent.Mending, armour);

		private void Record(ArmourEvent e, Protection armour) {
			if (!_inFight || armour == null) return;
			_pending.Add(new Pending { Event = e, Armour = armour });
		}

		// --- Pump ---

		public void Pump() {
			if (!CombatGate.IsLive || !CombatFrame.TryGet(out CombatFrame frame)) {
				if (_inFight) {
					_inFight = false;
					_pending.Clear();
					_warned.Clear();
					_damaged.Clear();
					_broken.Clear();
				}
				return;
			}
			_inFight = true;
			if (_pending.Count == 0) return;

			// A breaking blow raises "damaged" then "broken": the break alone is heard.
			_brokeThisPump.Clear();
			foreach (Pending p in _pending)
				if (p.Event == ArmourEvent.Broken) _brokeThisPump.Add(p.Armour);

			foreach (Pending p in _pending) {
				Protection armour = p.Armour;
				if (armour == null || !IsEnemy(armour)) continue;
				ArmourEvent e = p.Event;
				switch (e) {
					case ArmourEvent.Absorbed:
						if (_warned.Add(armour)) {
							Help.CombatKeys keys = Help.CombatKeyNames.Read();
							SpeechPipeline.SpeakEvent(Localization.Strings.ArmourUseBash(keys != null ? keys.BashKey : null));
						}
						break;
					case ArmourEvent.Cracked:
						if (_brokeThisPump.Contains(armour)) continue;
						_damaged.Add(armour);
						break;
					case ArmourEvent.Broken:
						_broken.Add(armour);
						_damaged.Remove(armour);
						SpeechPipeline.SpeakEvent(Localization.Strings.ArmourBroken);
						break;
					case ArmourEvent.Mending:
						if (_broken.Remove(armour)) {
							e = ArmourEvent.Restored;
							SpeechPipeline.SpeakEvent(Localization.Strings.ArmourRestored);
						} else if (!_damaged.Contains(armour)) {
							continue;
						}
						if (armour.Health >= armour.InitialHealth) _damaged.Remove(armour);
						break;
				}
				Log.Info("armour " + e + ": " + armour.name + " " + armour.Health + "/" + armour.InitialHealth);
				Play(e, armour, frame);
			}
			_pending.Clear();
		}

		private void Play(ArmourEvent e, Protection armour, CombatFrame frame) {
			if (!_soundsReady) return;
			frame.Project(armour.transform.position, out float right, out float forward);
			AudioEngine.PlayOneShot(ArmourCueComposer.KeyFor(e), ArmourCueComposer.Compose(e, right, forward), SoundCategory.Armour);
		}

		private static bool IsEnemy(Protection armour) {
			Controller controller = armour.Controller;
			Targetable target = controller != null ? controller.Targetable : null;
			return target != null && target.Team == TeamType.Enemy;
		}
	}
}
