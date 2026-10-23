using BepInEx.Configuration;
using HandOfFate2Access.Util;

namespace HandOfFate2Access.Settings {
	/// <summary>
	/// Keeps <see cref="ModSettings"/> in BepInEx's config file
	/// (BepInEx\config\com.handoffate2access.cfg): each setting is bound under its section
	/// and key (a toggle as true/false, a volume as 0 to 100), the saved value is loaded at
	/// startup, a change made in the mod menu is written back at once (BepInEx saves on
	/// every set), and an edit of the file that BepInEx reloads reaches the live setting.
	/// </summary>
	internal static class SettingsStore {
		public static void Bind(ConfigFile config) {
			foreach (Setting setting in ModSettings.All) {
				Setting s = setting;
				try {
					if (s.Kind == SettingKind.Toggle) {
						ConfigEntry<bool> entry = config.Bind(s.Section, s.Key, s.Default != 0, s.ConfigDescription);
						s.Value = entry.Value ? 1 : 0;
						entry.SettingChanged += (sender, args) => s.Value = entry.Value ? 1 : 0;
						s.Changed += changed => { if (entry.Value != changed.On) entry.Value = changed.On; };
					} else {
						ConfigEntry<int> entry = config.Bind(s.Section, s.Key, s.Default,
							new ConfigDescription(s.ConfigDescription, new AcceptableValueRange<int>(0, 100)));
						s.Value = entry.Value;
						entry.SettingChanged += (sender, args) => s.Value = entry.Value;
						s.Changed += changed => { if (entry.Value != changed.Value) entry.Value = changed.Value; };
					}
				} catch (System.Exception ex) {
					Log.Error("setting " + s.Section + "." + s.Key + " could not be bound to the config file; it keeps its default: " + ex);
				}
			}
			Log.Info("settings loaded from " + config.ConfigFilePath);
		}
	}
}
