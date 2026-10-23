using System.Reflection;
using HandOfFate2Access.Focus;
using HandOfFate2Access.Localization;
using HandOfFate2Access.Speech;
using HandOfFate2Access.UI;
using HarmonyLib;

namespace HandOfFate2Access.Minigames {
	/// <summary>
	/// Speaks the dice minigame. The game reports each state of the roll through
	/// <c>DiceRollStatus.Show(target, result, rollsRemaining)</c> (the panel that shows the
	/// target and the total); the postfix only flags it, and the pump speaks: before a roll
	/// the target and rolls left, after a roll the total, each die's value and whether the
	/// target was reached. The dice values are read live from the roller.
	/// </summary>
	internal sealed class DiceNarration {
		private static readonly FieldInfo RollsRemainingField = AccessTools.Field(typeof(DiceRollStatus), "m_rollsRemaining");

		public static DiceRollStatus Pending;
		public static int PendingTarget;
		public static int? PendingResult;

		// The game re-shows the panel when the rolls-left count drops as a throw begins
		// (the first throw with no total yet, a reroll still carrying the previous total
		// while the dice are in the air); that show is not a result and is skipped. A
		// count going up is a new game, spoken.
		private static int? _lastShownResult;
		private static int _lastShownRolls = -1;
		private string _lastLine;
		private float _lastLineAt;

		public static void ShowPostfix(DiceRollStatus __instance, int a_target, int? a_result, int a_rollsRemaining) {
			bool countOnly = a_result == _lastShownResult && _lastShownRolls >= 0 && a_rollsRemaining < _lastShownRolls;
			_lastShownResult = a_result;
			_lastShownRolls = a_rollsRemaining;
			if (countOnly) return;
			Pending = __instance;
			PendingTarget = a_target;
			PendingResult = a_result;
		}

		public bool Pump() {
			DiceRollStatus status = Pending;
			if (status == null) return false;
			Pending = null;
			var message = new Message();
			if (PendingResult.HasValue) {
				string faces = null;
				DiceRollTask task = DiceRollTask.RunningTask;
				if (task != null && task.Roller != null && task.Roller.ActiveDice.Count > 0) {
					var values = new System.Collections.Generic.List<string>();
					foreach (Die die in task.Roller.ActiveDice) values.Add(die.Value.ToString());
					faces = string.Join(", ", values.ToArray());
				}
				message.Add(string.Format(Strings.DiceResultFormat, PendingResult.Value));
				if (faces != null) message.Add(string.Format(Strings.DiceValuesFormat, faces));
				message.Add(PendingResult.Value >= PendingTarget ? Strings.DiceReached : Strings.DiceMissed);
			} else {
				message.Add(string.Format(Strings.DiceTargetFormat, PendingTarget));
			}
			message.Add(UiText.Read((UILabel)RollsRemainingField.GetValue(status)));
			// The game can show the same state twice in a row (the total and the
			// rolls-left count set one after the other); say it once.
			string line = message.Resolve();
			if (line == _lastLine && UnityEngine.Time.unscaledTime - _lastLineAt < 3f) return false;
			_lastLine = line;
			_lastLineAt = UnityEngine.Time.unscaledTime;
			SpeechPipeline.SpeakEvent(line);
			return true;
		}

		/// <summary>The readout for a focused die (choosing dice to reroll).</summary>
		public static UIElement Describe(Die die) {
			string text = string.Format(Strings.DieFormat, die.Value);
			return new GenericElement(text, die.Marked ? new[] { text, Strings.DieMarked } : new[] { text });
		}
	}
}
