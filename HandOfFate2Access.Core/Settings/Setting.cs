using System;
using HandOfFate2Access.Localization;

namespace HandOfFate2Access.Settings {
	public enum SettingKind {
		/// <summary>On or off (value 1 or 0).</summary>
		Toggle,
		/// <summary>A volume, 0 to 100 in steps of 10; 0 silences.</summary>
		Percent,
	}

	/// <summary>
	/// One player-facing setting: where it is stored in the config file (section and
	/// key, English and fixed, so a language switch never orphans a saved value), what
	/// the menu calls it and says about it (read at speech time, so they follow the
	/// language), and its value. The value is an int for both kinds: 0/1 for a toggle,
	/// a percentage for a volume. <see cref="Changed"/> fires on every real change,
	/// whoever made it (the menu, or the plugin loading the config file).
	/// </summary>
	public sealed class Setting {
		public const int PercentStep = 10;

		public readonly string Section;
		public readonly string Key;
		/// <summary>The config file's comment for the entry (English).</summary>
		public readonly string ConfigDescription;
		public readonly SettingKind Kind;
		public readonly int Default;
		private readonly Func<string> _name;
		private readonly Func<string> _description;
		private int _value;

		public event Action<Setting> Changed;

		public Setting(string section, string key, string configDescription, SettingKind kind, int defaultValue,
				Func<string> name, Func<string> description) {
			Section = section;
			Key = key;
			ConfigDescription = configDescription;
			Kind = kind;
			Default = Clamp(kind, defaultValue);
			_value = Default;
			_name = name;
			_description = description;
		}

		public string Name => _name();
		public string Description => _description();

		public int Value {
			get { return _value; }
			set {
				int v = Clamp(Kind, value);
				if (v == _value) return;
				_value = v;
				Changed?.Invoke(this);
			}
		}

		public bool On => _value != 0;

		/// <summary>The value as a gain, 0 to 1 (a toggle is 0 or 1).</summary>
		public float Fraction => Kind == SettingKind.Percent ? _value / 100f : (_value != 0 ? 1f : 0f);

		/// <summary>
		/// One step up (+1) or down (-1): a volume moves by ten and stops at its ends, a
		/// toggle flips whichever way. True when the value changed.
		/// </summary>
		public bool Step(int direction) {
			int before = _value;
			if (Kind == SettingKind.Toggle) Value = _value != 0 ? 0 : 1;
			else Value = _value + (direction >= 0 ? PercentStep : -PercentStep);
			return _value != before;
		}

		/// <summary>The value in words: "on", "80 percent".</summary>
		public string Spoken() {
			if (Kind == SettingKind.Toggle) return _value != 0 ? Strings.SettingOn : Strings.SettingOff;
			return _value == 0 ? Strings.SettingVolumeOff : Strings.Percent(_value);
		}

		private static int Clamp(SettingKind kind, int value) {
			if (kind == SettingKind.Toggle) return value != 0 ? 1 : 0;
			if (value < 0) return 0;
			if (value > 100) return 100;
			// Snap to the step, so a hand-edited 73 in the file reads and moves like 70.
			return (value + PercentStep / 2) / PercentStep * PercentStep;
		}
	}
}
