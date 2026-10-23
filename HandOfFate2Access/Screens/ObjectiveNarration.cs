using System.Collections.Generic;
using System.Reflection;
using HandOfFate2Access.Focus;
using HandOfFate2Access.Localization;
using HandOfFate2Access.Speech;
using HandOfFate2Access.UI;
using HarmonyLib;
using UnityEngine;

namespace HandOfFate2Access.Screens {
	/// <summary>
	/// The challenge objectives the game lists in the corner of the table ("Find the
	/// thieves' leader"). Each <c>UIObjective</c> is a live label bound to an objective
	/// whose state (in progress, failed, complete) the game shows only by colour and a
	/// strikethrough. A new or reworded objective is spoken as it appears; a completed or
	/// failed one is spoken with its outcome. Edges are tracked per game objective, not per
	/// widget: the widgets hide and reappear whenever a menu covers the table, which is no
	/// change. <see cref="SpeakAll"/> answers the objectives key.
	/// </summary>
	internal sealed class ObjectiveNarration {
		private static readonly FieldInfo LabelField = AccessTools.Field(typeof(UIObjective), "m_label");
		private static readonly FieldInfo ObjectiveField = AccessTools.Field(typeof(UIObjective), "m_objective");

		private struct Seen {
			public string Text;
			public ObjectiveChannel.Objective.StateType State;
		}

		private readonly Dictionary<ObjectiveChannel.Objective, Seen> _seen =
			new Dictionary<ObjectiveChannel.Objective, Seen>();
		private UIObjective[] _widgets = new UIObjective[0];
		private float _nextScan;

		public bool Pump() {
			if (Time.unscaledTime >= _nextScan) {
				_nextScan = Time.unscaledTime + 0.5f;
				_widgets = Object.FindObjectsOfType<UIObjective>();
			}
			bool spoke = false;
			foreach (UIObjective widget in _widgets) {
				if (widget == null) continue;
				string text;
				ObjectiveChannel.Objective objective;
				if (!Read(widget, out text, out objective)) continue;
				Seen seen;
				bool known = _seen.TryGetValue(objective, out seen);
				if (!known || seen.Text != text || seen.State != objective.State) {
					SpeechPipeline.SpeakEvent(Describe(text, objective.State));
					spoke = true;
				}
				_seen[objective] = new Seen { Text = text, State = objective.State };
			}
			return spoke;
		}

		/// <summary>Every objective currently shown, with its state; or that there are none.</summary>
		public void SpeakAll() {
			var message = new Message();
			foreach (UIObjective widget in Object.FindObjectsOfType<UIObjective>()) {
				string text;
				ObjectiveChannel.Objective objective;
				if (Read(widget, out text, out objective))
					message.Add(Describe(text, objective.State));
			}
			string line = message.Resolve();
			SpeechPipeline.SpeakInterrupt(line.Length > 0 ? line : Strings.NoObjectives);
		}

		private static bool Read(UIObjective widget, out string text, out ObjectiveChannel.Objective objective) {
			text = UiText.Read((UILabel)LabelField.GetValue(widget));
			objective = (ObjectiveChannel.Objective)ObjectiveField.GetValue(widget);
			return !string.IsNullOrEmpty(text) && objective != null;
		}

		private static string Describe(string text, ObjectiveChannel.Objective.StateType state) {
			var message = new Message().Add(text);
			if (state == ObjectiveChannel.Objective.StateType.Complete)
				message.Add(Strings.ObjectiveComplete);
			else if (state == ObjectiveChannel.Objective.StateType.Failed)
				message.Add(Strings.ObjectiveFailed);
			return message.Resolve();
		}
	}
}
