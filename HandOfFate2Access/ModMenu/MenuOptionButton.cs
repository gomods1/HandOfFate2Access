using System.Reflection;
using HandOfFate2Access.Localization;
using HandOfFate2Access.Util;
using HarmonyLib;
using UnityEngine;

namespace HandOfFate2Access.ModMenu {
	/// <summary>
	/// Adds the "Accessibility mod" option at the bottom of one of the game's button lists
	/// (the main menu, the pause menu), so any device reaches it like any other option.
	/// The option is a clone of one of the list's own buttons with its localization,
	/// mouse-click component and links stripped and its label set from the mod's strings,
	/// added to the list's selectable group and spliced into the up/down chain below the
	/// last active button, keeping the list's wrap-around. Every pump re-checks the splice,
	/// since the game shows, hides and appends buttons after the list is built (the main
	/// menu's news feed, buttons hidden on a new profile); a rebuilt list (a scene change)
	/// is injected again. Choosing it requests the mod menu.
	/// </summary>
	internal sealed class MenuOptionButton {
		private static readonly FieldInfo TitleField = AccessTools.Field(typeof(SpriteLabelButton), "m_titleLabel");
		private static readonly FieldInfo DescriptionField = AccessTools.Field(typeof(SpriteLabelButton), "m_descriptionLabel");

		private readonly string _where;
		private Object _owner;
		private UISelectable _button;
		private UISelectable _above;

		/// <param name="where">The list's name, for the log.</param>
		public MenuOptionButton(string where) {
			_where = where;
		}

		/// <summary>
		/// Keeps the option in the list of <paramref name="owner"/> (the menu component):
		/// <paramref name="template"/> is the button cloned, <paramref name="group"/> the
		/// list's selectable group.
		/// </summary>
		public void Pump(Object owner, UISelectable template, UISelectableGroup group) {
			if (owner == null || template == null || group == null) return;
			if (owner != _owner || _button == null) {
				_owner = owner;
				_above = null;
				_button = Inject(template, group);
				if (_button == null) return;
			}
			Splice();
			Refresh();
		}

		private UISelectable Inject(UISelectable template, UISelectableGroup group) {
			Transform grid = template.transform.parent;
			GameObject clone = Object.Instantiate(template.gameObject, grid, false);
			clone.name = "HoF2Access.ModMenu";
			clone.SetActive(true);
			foreach (UILocalize localize in clone.GetComponentsInChildren<UILocalize>(true))
				Object.Destroy(localize);
			foreach (UIButton button in clone.GetComponentsInChildren<UIButton>(true))
				Object.Destroy(button);
			UISelectable selectable = clone.GetComponent<UISelectable>();
			if (selectable == null) {
				Log.Error("mod menu option not added to the " + _where + ": its template has no UISelectable");
				Object.Destroy(clone);
				return null;
			}
			selectable.enabled = true;
			selectable.selectOnUp = null;
			selectable.selectOnDown = null;
			selectable.selectOnLeft = null;
			selectable.selectOnRight = null;
			// The group overwrites a new child's actions with its own, so ours goes after.
			group.Add(selectable);
			selectable.SelectAction = s => ModMenuState.RequestOpen();
			Log.Info("mod menu option added to the " + _where);
			return selectable;
		}

		// Below the bottom-most active button; whatever that button led down to (the first,
		// when the list wraps) now follows the option, and up from there reaches it.
		private void Splice() {
			Transform grid = _button.transform.parent;
			UISelectable last = LastActive(grid, _button);
			if (last == null || (last == _above && last.selectOnDown == _button)) return;
			if (_above != null && _above.selectOnDown == _button) _above.selectOnDown = _button.selectOnDown;
			UISelectable wrapTo = last.selectOnDown == _button ? _button.selectOnDown : last.selectOnDown;
			last.selectOnDown = _button;
			_button.selectOnUp = last;
			_button.selectOnDown = wrapTo;
			if (wrapTo != null && wrapTo.selectOnUp == last) wrapTo.selectOnUp = _button;
			_above = last;
			_button.transform.SetAsLastSibling();
			UIGrid layout = grid.GetComponent<UIGrid>();
			if (layout != null) layout.Reposition();
			Log.Info("mod menu option placed below '" + last.name + "' in the " + _where);
		}

		// The bottom-most active button in the list (grid child order is display order).
		private static UISelectable LastActive(Transform grid, UISelectable exclude) {
			UISelectable last = null;
			for (int i = 0; i < grid.childCount; i++) {
				Transform child = grid.GetChild(i);
				if (!child.gameObject.activeSelf) continue;
				UISelectable s = child.GetComponent<UISelectable>();
				if (s != null && s != exclude) last = s;
			}
			return last;
		}

		// The pause menu's buttons carry a title and a description; other lists' buttons a
		// single label (the first one takes the title, any others are cleared).
		private void Refresh() {
			SpriteLabelButton labels = _button.GetComponent<SpriteLabelButton>();
			if (labels != null) {
				Set((UILabel)TitleField.GetValue(labels), Strings.ModMenuTitle);
				Set((UILabel)DescriptionField.GetValue(labels), Strings.ModMenuDescription);
				return;
			}
			bool first = true;
			foreach (UILabel label in _button.GetComponentsInChildren<UILabel>(true)) {
				Set(label, first ? Strings.ModMenuTitle : "");
				first = false;
			}
		}

		private static void Set(UILabel label, string text) {
			if (label != null && label.text != text) label.text = text;
		}
	}
}
