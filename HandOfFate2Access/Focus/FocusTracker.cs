using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace HandOfFate2Access.Focus {
	/// <summary>
	/// Reads the game's live focus. Hand of Fate 2 keeps one UISelection per
	/// category (Main, Menu, Dialogue, Inventory, Navigation) in UISelectionManager;
	/// "focus" is the hovered UISelectable of the selection on top of the manager's
	/// active stack (the one receiving input), falling back to the highest-priority
	/// selection that hovers anything. Polled every frame from the pump: polling
	/// catches every source of a focus change (keys, mouse, the game's own default
	/// selection) without hooking each one.
	/// </summary>
	internal static class FocusTracker {
		private static GameObject _last;
		private static bool _forceRefresh;
		private static readonly FieldInfo InstanceField = AccessTools.Field(typeof(UISelectionManager), "m_instance");

		/// <summary>The selection on top of the game's active stack (the inventory's while it
		/// is open over the map), or null.</summary>
		public static UISelection TopSelection() {
			UISelectionManager manager = (UISelectionManager)InstanceField.GetValue(null);
			if (manager == null || manager.ActiveStack.Count == 0) return null;
			return manager.ActiveStack.Peek();
		}

		public static UISelectable CurrentSelectable() {
			UISelectionManager manager = (UISelectionManager)InstanceField.GetValue(null);
			if (manager == null) return null;
			UISelection[] selections = UISelectionManager.Selections;
			if (selections == null) return null;
			if (manager != null && manager.ActiveStack.Count > 0) {
				UISelection top = manager.ActiveStack.Peek();
				if (top != null && top.HoveredObject != null)
					return top.HoveredObject;
			}
			for (int i = selections.Length - 1; i >= 0; i--) {
				UISelection s = selections[i];
				if (s != null && s.HoveredObject != null)
					return s.HoveredObject;
			}
			return null;
		}

		public static GameObject Current() {
			UISelectable s = CurrentSelectable();
			return s == null ? null : s.gameObject;
		}

		/// <summary>The frame focus last changed (or was refreshed).</summary>
		public static int LastChangeFrame { get; private set; } = -100;

		/// <summary>Re-announce the current focus on the next poll even if unchanged.</summary>
		public static void Refresh() => _forceRefresh = true;

		/// <summary>
		/// The newly focused object, if focus changed since the last call (or a refresh
		/// was requested). Null focus (nothing hovered) is not reported as a change.
		/// </summary>
		public static bool TryConsume(out GameObject go) {
			go = Current();
			if (go == null) {
				_last = null;
				return false;
			}
			if (go == _last && !_forceRefresh) return false;
			_forceRefresh = false;
			_last = go;
			LastChangeFrame = Time.frameCount;
			return true;
		}
	}
}
