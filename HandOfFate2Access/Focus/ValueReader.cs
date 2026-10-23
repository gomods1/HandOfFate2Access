using System;
using HandOfFate2Access.UI;
using HandOfFate2Access.Util;
using UnityEngine;

namespace HandOfFate2Access.Focus {
	/// <summary>
	/// The controls whose value changes in place while focus stays on them: a settings
	/// row (left/right, a rebound key, a ticked box), a die in the reroll step (marked
	/// or not) and a map card's goblins (they move and turn after each player step). The
	/// focus pump polls the value and re-speaks it on change.
	/// </summary>
	internal static class ValueReader {
		/// <summary>The component that owns the value under <paramref name="go"/>, or null.</summary>
		public static Component Owner(GameObject go) {
			if (go == null) return null;
			MapLayoutSlot slot = go.GetComponent<MapLayoutSlot>();
			if (slot != null) return slot;
			PauseMenuControl row = go.GetComponentInParent<PauseMenuControl>();
			if (row != null) return row;
			return go.GetComponentInParent<Die>();
		}

		/// <summary>What to speak for the value now; empty when there is nothing to say;
		/// null when it cannot be read yet.</summary>
		public static string Read(GameObject go) {
			try {
				var slot = Owner(go) as MapLayoutSlot;
				if (slot != null) return Maps.MapBoardReader.GoblinValue(slot);
				var die = Owner(go) as Die;
				if (die != null) return Minigames.DiceNarration.Describe(die).Describe().Resolve();
				SettingElement setting = SettingsReader.TryRead(go);
				Message value = setting != null ? setting.DescribeValue() : null;
				return value != null ? value.Resolve() : null;
			} catch (Exception ex) {
				Log.Error("value read failed: " + ex);
				return null;
			}
		}
	}
}
