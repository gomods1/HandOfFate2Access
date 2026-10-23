namespace HandOfFate2Access.Audio {
	/// <summary>
	/// The family a sound belongs to, for its volume setting. Every voice carries one;
	/// the engine scales the voice's volume by the mod's overall volume times its
	/// family's volume (<see cref="Settings.ModSettings.Gain"/>) each time it is
	/// placed, so a setting change reaches a playing loop on its next update.
	/// Interface sounds (the menu's own examples of nothing in particular) follow the
	/// overall volume alone.
	/// </summary>
	public enum SoundCategory {
		Interface,
		AttackWarnings,
		Walls,
		Projectiles,
		Hazards,
		Locator,
		EnemyMovement,
		Chimes,
		Minigames,
		Armour,
	}
}
