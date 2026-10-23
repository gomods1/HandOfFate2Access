using System.Collections.Generic;
using UnityEngine;

namespace HandOfFate2Access.Focus {
	/// <summary>Shared helpers for pulling visible text off NGUI hierarchies.</summary>
	internal static class UiText {
		/// <summary>
		/// The text of every active, enabled, visible UILabel under <paramref name="root"/>
		/// (root included), in hierarchy order. Empty labels and fully transparent ones
		/// (hidden by alpha, which NGUI uses instead of deactivation) are skipped.
		/// </summary>
		public static List<string> Labels(GameObject root) {
			var result = new List<string>();
			if (root == null) return result;
			// Button prompts (Continue, Back) drawn as input hints read after the
			// content they act on, so a panel reads title, text, then its action.
			var prompts = new List<string>();
			foreach (UILabel label in root.GetComponentsInChildren<UILabel>(false)) {
				if (!IsVisible(label)) continue;
				string text = label.text;
				if (string.IsNullOrEmpty(text)) continue;
				if (label.GetComponentInParent<InputHintButton>() != null)
					prompts.Add(Localization.KeyNames.Localize(text));
				else
					result.Add(text);
			}
			result.AddRange(prompts);
			return result;
		}

		public static bool IsVisible(UIWidget widget) {
			return widget != null && widget.enabled && widget.gameObject.activeInHierarchy && widget.finalAlpha > 0.01f;
		}

		/// <summary>A label's text if it is visible, else null.</summary>
		public static string Read(UILabel label) {
			return IsVisible(label) ? label.text : null;
		}
	}
}
