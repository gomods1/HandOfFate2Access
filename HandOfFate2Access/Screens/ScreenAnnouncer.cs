using System.Collections.Generic;
using System.Reflection;
using HandOfFate2Access.Focus;
using HandOfFate2Access.Localization;
using HandOfFate2Access.Speech;
using HandOfFate2Access.UI;
using HandOfFate2Access.Util;
using HarmonyLib;
using UnityEngine;

namespace HandOfFate2Access.Screens {
	/// <summary>
	/// Speaks the name of the screen the player has just entered (main menu, map,
	/// challenge map, deck builder, inventory, pause, results), read off the game's menu
	/// stacks. The main menu adds the help key and the mod menu; the inventory how to
	/// browse it and the key that closes it (each slot reads its item, and the Status
	/// review buffer lists every slot, empty ones included).
	/// </summary>
	internal sealed class ScreenAnnouncer {
		private ScreenId _current = ScreenId.None;

		/// <summary>The screen last announced (None for encounters, minigames, combat).</summary>
		public ScreenId Current => _current;

		/// <summary>True when a screen name was spoken this frame (focus readouts queue after it).</summary>
		public bool Pump() {
			MenuManager menus = MenuManager.Instance;
			if (menus == null) return false;
			string primary = null, secondary = null;
			Menu primaryTop = null;
			foreach (MenuStack stack in menus.MenuStacks) {
				if (stack.Stack.Count == 0) continue;
				Menu top = stack.Stack[stack.Stack.Count - 1];
				string name = top != null ? top.name : null;
				string priority = stack.Priority.ToString();
				if (priority == "Primary") { primary = name; primaryTop = top; }
				else if (priority == "Secondary") secondary = name;
			}
			// The end of a run (death by wounds or starvation, a forfeit, a failed
			// challenge) is a menu of its own whose title and subtitle carry the game's
			// verdict; it is read when it comes up, before the results that follow.
			bool spokeEnd = PumpEndOfRun(primaryTop);
			ScreenId id = ScreenCatalog.Identify(primary, secondary);
			if (id == _current) return spokeEnd;
			_current = id;
			string text = ScreenCatalog.NameOf(id);
			if (string.IsNullOrEmpty(text)) return false;
			var message = new Message().Add(text);
			// The main menu names the help key; the inventory says how to move through it and
			// how to leave it (Backspace, the mod's second cancel key beside the game's Tab).
			if (id == ScreenId.MainMenu) message.Add(Strings.HelpHint);
			// The appearance screen has no way back, only F3 to finish (the game turns its
			// cancel off), and its F3 action is otherwise announced at most every few minutes.
			if (id == ScreenId.Customisation) message.Add(Strings.CustomisationFinish);
			if (id == ScreenId.Inventory) {
				message.Add(Strings.InventoryBrowse);
				message.Add(Strings.InventoryClose(Strings.KeyBackspace));
			}
			SpeechPipeline.SpeakEvent(message.Resolve());
			return true;
		}

		private static readonly FieldInfo EndTitleField = AccessTools.Field(typeof(EndRunMenu), "m_titleLabel");
		private static readonly FieldInfo EndSubtitleField = AccessTools.Field(typeof(EndRunMenu), "m_subtitleLabel");

		private Menu _endMenu;
		private EndRunMenu _endPending;
		private int _endPendingUntil;
		private const int EndRunRetryFrames = 240;

		// The end-of-run menu on top of the primary stack, read once per appearance:
		// its title ("You have died") and subtitle (the cause). The labels fade in a
		// moment after the menu is pushed, so an empty read is retried for a few seconds.
		private bool PumpEndOfRun(Menu top) {
			if (top != _endMenu) {
				_endMenu = top;
				_endPending = null;
				EndRunMenu end = top != null ? top.GetComponentInParent<EndRunMenu>() : null;
				if (end == null && top != null) {
					foreach (EndRunMenu candidate in Object.FindObjectsOfType<EndRunMenu>())
						if (candidate.Menu == top) { end = candidate; break; }
				}
				if (end == null) return false;
				_endPending = end;
				_endPendingUntil = Time.frameCount + EndRunRetryFrames;
			}
			if (_endPending == null) return false;
			// The labels keep an alpha of zero (the verdict is drawn by its own effect), so
			// their text is taken as set, not by visibility.
			var message = new Message()
				.Add(TextOf((UILabel)EndTitleField.GetValue(_endPending)))
				.Add(TextOf((UILabel)EndSubtitleField.GetValue(_endPending)));
			string text = message.Resolve();
			if (text.Length == 0) {
				if (Time.frameCount > _endPendingUntil) {
					Log.Warn("end-of-run menu " + top.name + " never showed a readable title: title "
						+ Describe((UILabel)EndTitleField.GetValue(_endPending)) + "; subtitle "
						+ Describe((UILabel)EndSubtitleField.GetValue(_endPending)));
					_endPending = null;
				}
				return false;
			}
			_endPending = null;
			SpeechPipeline.SpeakEventInterrupt(text);
			return true;
		}

		private static string TextOf(UILabel label) {
			return label != null && label.enabled && label.gameObject.activeInHierarchy ? label.text : null;
		}

		// Diagnostic: why a label read as nothing.
		private static string Describe(UILabel label) {
			if (label == null) return "null";
			return "'" + label.text + "' enabled=" + label.enabled + " active=" + label.gameObject.activeInHierarchy
				+ " alpha=" + label.finalAlpha.ToString("0.00");
		}
	}
}
