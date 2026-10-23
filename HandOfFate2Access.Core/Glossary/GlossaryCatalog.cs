using System.Collections.Generic;
using HandOfFate2Access.Audio;
using HandOfFate2Access.Combat;
using HandOfFate2Access.Gambit;
using HandOfFate2Access.Localization;

namespace HandOfFate2Access.Glossary {
	/// <summary>
	/// Every sound the mod plays, with a one-line meaning, for the mod menu's sound
	/// examples: the fight cues in the order a new player meets them, then the chance
	/// cards and the other minigames. Activating an entry plays the sound at its canonical
	/// parameters (centred, full loudness, scaled by the player's volume for its family);
	/// the wall tones and the recharge chime play their sides in turn, because their pan
	/// is their meaning, and the shuffle plays a short moving round of the card instruments.
	/// </summary>
	public static class GlossaryCatalog {
		public const string ProjectileKey = "glossary_projectile";
		public const string ProjectileReflectedKey = "glossary_projectile_reflected";
		public const float ProjectileDemoSeconds = 1f;
		public const float LoopHoldSeconds = 1f;
		private const float RechargeHoldSeconds = 1.6f;
		private const float CardHoldSeconds = 0.6f;
		private const int ShuffleDemoRounds = 4;

		/// <summary>The clip key a minigame outcome voice is registered under for the demo.</summary>
		public static string OutcomeKey(OutcomeKind kind) => "glossary_outcome_" + (int)kind;

		public static readonly GlossaryEntry[] CombatEntries = {
			OneShot(() => Strings.GlossaryBlock, AttackCueComposer.BlockKey, AttackCueComposer.MaxVolume, SoundCategory.AttackWarnings),
			OneShot(() => Strings.GlossaryDodge, AttackCueComposer.DodgeKey, AttackCueComposer.MaxVolume, SoundCategory.AttackWarnings),
			OneShot(() => Strings.GlossaryArmourAbsorbed, ArmourCueComposer.AbsorbedKey, ArmourCueComposer.Volume, SoundCategory.Armour),
			OneShot(() => Strings.GlossaryArmourCrack, ArmourCueComposer.CrackKey, ArmourCueComposer.Volume, SoundCategory.Armour),
			OneShot(() => Strings.GlossaryArmourBroken, ArmourCueComposer.BrokenKey, ArmourCueComposer.Volume, SoundCategory.Armour),
			OneShot(() => Strings.GlossaryArmourRestored, ArmourCueComposer.RestoredKey, ArmourCueComposer.Volume, SoundCategory.Armour),
			new GlossaryEntry(() => Strings.GlossaryEnemyStep, EnemySteps()),
			OneShot(() => Strings.GlossaryEnemyReach, EnemyPresenceTracker.ReachKey, 0.85f, SoundCategory.EnemyMovement),
			OneShot(() => Strings.GlossaryEnemyPing, EnemyPingSynth.Key, EnemyPingComposer.MaxVolume, SoundCategory.Locator),
			OneShot(() => Strings.GlossaryFinisher, FinisherBeaconComposer.Key, FinisherBeaconComposer.MaxVolume, SoundCategory.Locator),
			OneShot(() => Strings.GlossaryFinisherReady, FinisherBeaconComposer.ReadyKey, FinisherBeaconComposer.MaxVolume, SoundCategory.Locator),
			new GlossaryEntry(() => Strings.GlossaryWallTones, new[] {
				WallStep(WallSide.Right),
				WallStep(WallSide.Left),
				WallStep(WallSide.Above),
				WallStep(WallSide.Below),
			}),
			OneShot(() => Strings.GlossaryWallCollision, WallToneComposer.CollisionKey, 1f, SoundCategory.Walls),
			OneShot(() => Strings.GlossaryProjectile, ProjectileKey, ProjectileSonifier.MaxVolume, SoundCategory.Projectiles),
			OneShot(() => Strings.GlossaryProjectileReflected, ProjectileReflectedKey, ProjectileSonifier.MaxVolume, SoundCategory.Projectiles),
			OneShot(() => Strings.GlossaryZonePrimed, ZoneSynth.PrimedKey, ZoneSonifier.MaxVolume, SoundCategory.Hazards),
			OneShot(() => Strings.GlossaryZoneArming, ZoneSynth.ArmingKey, ZoneSonifier.MaxVolume, SoundCategory.Hazards),
			OneShot(() => Strings.GlossaryZoneActive, ZoneSynth.ActiveKey, ZoneSonifier.MaxVolume, SoundCategory.Hazards),
			OneShot(() => Strings.GlossaryZoneInside, ZoneSynth.InsideKey, 1f, SoundCategory.Hazards),
			new GlossaryEntry(() => Strings.GlossaryRecharge, new[] {
				new GlossaryStep(RechargeCueComposer.Key, RechargeCueComposer.Weapon, false, RechargeHoldSeconds, SoundCategory.Chimes),
				new GlossaryStep(RechargeCueComposer.Key, RechargeCueComposer.Artifact, false, 0f, SoundCategory.Chimes),
			}),
		};

		public static readonly GlossaryEntry[] MinigameEntries = {
			Card(() => Strings.GlossaryCardHarp, 0),
			Card(() => Strings.GlossaryCardFlute, 1),
			Card(() => Strings.GlossaryCardHandpan, 2),
			Card(() => Strings.GlossaryCardMusicBox, 3),
			new GlossaryEntry(() => Strings.GlossaryShuffle, ShuffleDemo()),
			OneShot(() => Strings.GlossaryChanceHugeSuccess, GambitSounds.OutcomeKey(ChanceOutcome.HugeSuccess), 1f, SoundCategory.Minigames),
			OneShot(() => Strings.GlossaryChanceSuccess, GambitSounds.OutcomeKey(ChanceOutcome.Success), 1f, SoundCategory.Minigames),
			OneShot(() => Strings.GlossaryChanceFailure, GambitSounds.OutcomeKey(ChanceOutcome.Failure), 1f, SoundCategory.Minigames),
			OneShot(() => Strings.GlossaryChanceHugeFailure, GambitSounds.OutcomeKey(ChanceOutcome.HugeFailure), 1f, SoundCategory.Minigames),
			OneShot(() => Strings.GlossaryOutcomeHugeSuccess, OutcomeKey(OutcomeKind.HugeSuccess), 1f, SoundCategory.Minigames),
			OneShot(() => Strings.GlossaryOutcomeSuccess, OutcomeKey(OutcomeKind.Success), 1f, SoundCategory.Minigames),
			OneShot(() => Strings.GlossaryOutcomeNeutral, OutcomeKey(OutcomeKind.Neutral), 1f, SoundCategory.Minigames),
			OneShot(() => Strings.GlossaryOutcomeFailure, OutcomeKey(OutcomeKind.Failure), 1f, SoundCategory.Minigames),
			OneShot(() => Strings.GlossaryOutcomeHugeFailure, OutcomeKey(OutcomeKind.HugeFailure), 1f, SoundCategory.Minigames),
		};

		private static GlossaryEntry OneShot(System.Func<string> label, string clipKey, float volume, SoundCategory category) {
			return new GlossaryEntry(label, new[] {
				new GlossaryStep(clipKey, new SoundParams(0f, 1f, volume), false, 0f, category),
			});
		}

		// A card's instrument from where that card lies when four are shown, then again centred.
		private static GlossaryEntry Card(System.Func<string> label, int slot) {
			return new GlossaryEntry(label, new[] {
				new GlossaryStep(GambitSounds.CardKey(slot), new SoundParams(GambitLayout.SlotPan(slot, 4), 1f, 1f), false, CardHoldSeconds, SoundCategory.Minigames),
				new GlossaryStep(GambitSounds.CardKey(slot), new SoundParams(0f, 1f, 1f), false, 0f, SoundCategory.Minigames),
			});
		}

		// Four cards taking turns as in a shuffle: the harp travels from the far left to the
		// far right and the music box the other way, while the flute and handpan stay put.
		private static GlossaryStep[] ShuffleDemo() {
			var steps = new List<GlossaryStep>();
			for (int round = 0; round < ShuffleDemoRounds; round++) {
				float t = round / (float)(ShuffleDemoRounds - 1);
				for (int slot = 0; slot < 4; slot++) {
					float pan = GambitLayout.SlotPan(slot, 4);
					if (slot == 0) pan = -1f + 2f * t;
					else if (slot == 3) pan = 1f - 2f * t;
					bool last = round == ShuffleDemoRounds - 1 && slot == 3;
					steps.Add(new GlossaryStep(GambitSounds.CardKey(slot), new SoundParams(pan, 1f, 1f), false,
						last ? 0f : GambitLayout.TurnSeconds, SoundCategory.Minigames));
				}
			}
			return steps.ToArray();
		}

		// An enemy walking past in front of the player, from the left to the right.
		private static GlossaryStep[] EnemySteps() {
			const int count = 8;
			var steps = new GlossaryStep[count];
			for (int i = 0; i < count; i++) {
				float pan = -1f + 2f * i / (count - 1);
				steps[i] = new GlossaryStep(EnemyPresenceTracker.StepKey, new SoundParams(pan, 1f, 0.7f), false,
					i == count - 1 ? 0f : EnemyPresenceTracker.StepSeconds, SoundCategory.EnemyMovement);
			}
			return steps;
		}

		private static GlossaryStep WallStep(WallSide side) {
			return new GlossaryStep(
				WallToneComposer.KeyFor(side),
				new SoundParams(WallToneComposer.PanFor(side), 1f, WallToneComposer.MaxVolume),
				true, LoopHoldSeconds, SoundCategory.Walls);
		}
	}
}
