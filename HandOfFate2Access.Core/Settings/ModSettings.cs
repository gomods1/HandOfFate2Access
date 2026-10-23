using HandOfFate2Access.Audio;
using HandOfFate2Access.Localization;

namespace HandOfFate2Access.Settings {
	/// <summary>
	/// Every setting the player can change, in the order the mod menu lists them. The
	/// plugin stores each in BepInEx's config file (BepInEx\config\com.handoffate2access.cfg)
	/// under its section and key, loads them at startup and saves each change the menu
	/// makes; the features read the live values here.
	/// </summary>
	public static class ModSettings {
		private const string Sounds = "Sounds";
		private const string Speech = "Speech";
		private const string Camera = "Camera";

		public static readonly Setting Volume = new Setting(Sounds, "Volume",
			"Overall volume of every sound the mod plays, 0 to 100.",
			SettingKind.Percent, 100, () => Strings.SettingVolume, () => Strings.SettingVolumeHelp);
		public static readonly Setting AttackVolume = new Setting(Sounds, "AttackWarnings",
			"Volume of the attack warnings (clang: counter, crack: dodge), 0 to 100.",
			SettingKind.Percent, 100, () => Strings.SettingAttackVolume, () => Strings.SettingAttackVolumeHelp);
		public static readonly Setting ArmourVolume = new Setting(Sounds, "Armour",
			"Volume of the enemy armour sounds (a hit glancing off, Bash cracking it, broken, growing back), 0 to 100.",
			SettingKind.Percent, 100, () => Strings.SettingArmourVolume, () => Strings.SettingArmourVolumeHelp);
		public static readonly Setting WallVolume = new Setting(Sounds, "WallTones",
			"Volume of the wall tones and the bump when walking into a wall, 0 to 100.",
			SettingKind.Percent, 100, () => Strings.SettingWallVolume, () => Strings.SettingWallVolumeHelp);
		public static readonly Setting ProjectileVolume = new Setting(Sounds, "Projectiles",
			"Volume of projectiles in flight, 0 to 100.",
			SettingKind.Percent, 100, () => Strings.SettingProjectileVolume, () => Strings.SettingProjectileVolumeHelp);
		public static readonly Setting HazardVolume = new Setting(Sounds, "Hazards",
			"Volume of traps and hazard areas, 0 to 100.",
			SettingKind.Percent, 100, () => Strings.SettingHazardVolume, () => Strings.SettingHazardVolumeHelp);
		public static readonly Setting EnemyMovementVolume = new Setting(Sounds, "EnemyMovement",
			"Volume of the nearest enemies' footsteps and the knock when one comes within reach, 0 to 100.",
			SettingKind.Percent, 100, () => Strings.SettingEnemyVolume, () => Strings.SettingEnemyVolumeHelp);
		public static readonly Setting LocatorVolume = new Setting(Sounds, "Locator",
			"Volume of the enemy locator ping, the exit and loot beacons and the finish beacons, 0 to 100.",
			SettingKind.Percent, 100, () => Strings.SettingLocatorVolume, () => Strings.SettingLocatorVolumeHelp);
		public static readonly Setting ChimeVolume = new Setting(Sounds, "AbilityChimes",
			"Volume of the chime when a weapon or artefact ability is ready, 0 to 100.",
			SettingKind.Percent, 100, () => Strings.SettingChimeVolume, () => Strings.SettingChimeVolumeHelp);
		public static readonly Setting MinigameVolume = new Setting(Sounds, "Minigames",
			"Volume of the chance cards, pendulum and wheel sounds, 0 to 100.",
			SettingKind.Percent, 100, () => Strings.SettingMinigameVolume, () => Strings.SettingMinigameVolumeHelp);

		public static readonly Setting LockCamera = new Setting(Camera, "LockCamera",
			"Ignore the camera keys and right stick in fights, so the camera keeps the arena's angle (true or false).",
			SettingKind.Toggle, 1, () => Strings.SettingLockCamera, () => Strings.SettingLockCameraHelp);
		public static readonly Setting ListenAtPlayer = new Setting(Camera, "ListenAtPlayer",
			"Hear the game's own sounds from the player's character instead of from toward the camera (true or false).",
			SettingKind.Toggle, 1, () => Strings.SettingListenAtPlayer, () => Strings.SettingListenAtPlayerHelp);

		public static readonly Setting VoiceLines = new Setting(Speech, "VoiceLines",
			"Read the dealer's and other characters' spoken lines (true or false).",
			SettingKind.Toggle, 1, () => Strings.SettingVoiceLines, () => Strings.SettingVoiceLinesHelp);
		public static readonly Setting ResourceChanges = new Setting(Speech, "ResourceChanges",
			"Announce changes to health, food, gold and fame (true or false).",
			SettingKind.Toggle, 1, () => Strings.SettingResourceChanges, () => Strings.SettingResourceChangesHelp);
		public static readonly Setting ActionKeys = new Setting(Speech, "ActionKeys",
			"Announce the F1 to F3 actions when they appear (true or false).",
			SettingKind.Toggle, 1, () => Strings.SettingActionKeys, () => Strings.SettingActionKeysHelp);
		public static readonly Setting ChanceCardHints = new Setting(Speech, "ChanceCardHints",
			"Explain each step of the chance cards as it happens (true or false).",
			SettingKind.Toggle, 1, () => Strings.SettingChanceHints, () => Strings.SettingChanceHintsHelp);
		public static readonly Setting GoblinWarnings = new Setting(Speech, "GoblinWarnings",
			"On the map, say when a goblin will step onto a card right after the player moves there (true or false).",
			SettingKind.Toggle, 1, () => Strings.SettingGoblinWarnings, () => Strings.SettingGoblinWarningsHelp);

		public static readonly Setting[] All = {
			Volume, AttackVolume, ArmourVolume, EnemyMovementVolume, WallVolume, ProjectileVolume, HazardVolume, LocatorVolume, ChimeVolume,
			MinigameVolume,
			LockCamera, ListenAtPlayer,
			VoiceLines, ResourceChanges, ActionKeys, ChanceCardHints, GoblinWarnings,
		};

		/// <summary>The volume setting of a sound family; null for interface sounds.</summary>
		public static Setting VolumeOf(SoundCategory category) {
			switch (category) {
				case SoundCategory.AttackWarnings: return AttackVolume;
				case SoundCategory.Walls: return WallVolume;
				case SoundCategory.Projectiles: return ProjectileVolume;
				case SoundCategory.Hazards: return HazardVolume;
				case SoundCategory.Locator: return LocatorVolume;
				case SoundCategory.EnemyMovement: return EnemyMovementVolume;
				case SoundCategory.Chimes: return ChimeVolume;
				case SoundCategory.Minigames: return MinigameVolume;
				case SoundCategory.Armour: return ArmourVolume;
				default: return null;
			}
		}

		/// <summary>The gain a sound of this family plays at: the overall volume times its own.</summary>
		public static float Gain(SoundCategory category) {
			Setting own = VolumeOf(category);
			return Volume.Fraction * (own != null ? own.Fraction : 1f);
		}

		/// <summary>Every setting back to its default (tests).</summary>
		internal static void ResetForTest() {
			foreach (Setting s in All) s.Value = s.Default;
		}
	}
}
