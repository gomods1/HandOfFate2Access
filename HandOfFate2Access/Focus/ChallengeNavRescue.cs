using HandOfFate2Access.Util;
using UnityEngine;

namespace HandOfFate2Access.Focus {
	/// <summary>
	/// The challenge map's Campfire section (character customisation) puts its single
	/// tab into the challenge row and hands the row the selection without hovering
	/// anything unless a controller is in use (<c>UIChallengeMapSection_Customisation</c>),
	/// so on a keyboard nothing has focus and no arrow does anything: a sighted player
	/// clicks the tab, a blind one is stuck. When the row's group holds the selection
	/// with nothing hovered for a few frames on a keyboard, its first tab is hovered,
	/// as the controller path does. Mouse play is left alone: the pointer resting off
	/// the icons is the same state and means nothing.
	/// </summary>
	internal sealed class ChallengeNavRescue {
		private const int EmptyFramesBeforeRescue = 10;

		private UIChallengeNav _nav;
		private float _nextScan;
		private int _emptyFrames;

		public void Pump() {
			if (Game.Instance == null || !(Game.Instance.ActiveGameState is GameState_ChallengeMap)) {
				_emptyFrames = 0;
				_nav = null;
				return;
			}
			if (_nav == null && Time.unscaledTime >= _nextScan) {
				_nextScan = Time.unscaledTime + 1f;
				_nav = Object.FindObjectOfType<UIChallengeNav>();
			}
			UISelectableGroup group = _nav != null ? _nav.SelectableGroup : null;
			UISelection selection = group != null ? group.Selection : null;
			if (selection == null || selection.SelectedGroup != group || selection.HoveredObject != null
					|| Defiant.InputManager.ActiveDeviceClass != InControl.InputDeviceClass.Keyboard) {
				_emptyFrames = 0;
				return;
			}
			if (++_emptyFrames < EmptyFramesBeforeRescue) return;
			_emptyFrames = 0;
			foreach (UINavTab tab in _nav.GetComponentsInChildren<UINavTab>(false)) {
				if (tab.Selectable == null || !tab.gameObject.activeInHierarchy) continue;
				if (UISelection.TryHover(tab.Selectable))
					Log.Info("challenge row had the selection with nothing focused; focused " + tab.name);
				else
					Log.Warn("challenge row had the selection with nothing focused; could not focus " + tab.name);
				return;
			}
		}
	}
}
