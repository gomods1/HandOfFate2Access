using System.Collections.Generic;
using HandOfFate2Access.Localization;
using HandOfFate2Access.UI;

namespace HandOfFate2Access.ModMenu {
	/// <summary>
	/// Where the player is in the accessibility mod menu, and what each key says. The
	/// plugin turns key presses into these calls and speaks what they return; this only
	/// decides the position and the words, so the menu's behaviour is testable off-engine.
	///
	/// Up and Down move through the current list (a menu wraps; a list of text lines stops
	/// at its ends), Home and End jump to them, Enter goes into a section (its title, hint
	/// and first entry are said) or carries out an entry, Left and Right change a setting,
	/// Back returns to the section above and, at the top, closes the menu.
	/// </summary>
	public sealed class MenuNavigator {
		private sealed class Frame {
			public MenuItem Owner;
			public IList<MenuItem> Items;
			public int Index;
		}

		private readonly List<Frame> _stack = new List<Frame>();

		/// <summary>The entry the player is on; null when the menu is closed or its list is empty.</summary>
		public MenuItem Current {
			get {
				if (_stack.Count == 0) return null;
				Frame top = _stack[_stack.Count - 1];
				return top.Items.Count > 0 ? top.Items[top.Index] : null;
			}
		}

		public bool IsOpen => _stack.Count > 0;

		/// <summary>Opens at the top of <paramref name="root"/>: its title, hint and first entry.</summary>
		public string Open(MenuItem root) {
			_stack.Clear();
			return Push(root);
		}

		public void Close() => _stack.Clear();

		/// <summary>One entry up (-1) or down (+1).</summary>
		public string Move(int delta) {
			Frame top = Top();
			if (top == null) return null;
			if (top.Items.Count == 0) return Strings.ReviewEmpty(top.Owner.Label);
			int index = top.Index + delta;
			if (index < 0 || index >= top.Items.Count) {
				if (!top.Owner.Wraps) return index < 0 ? Strings.ReviewTop : Strings.ReviewEnd;
				index = (index % top.Items.Count + top.Items.Count) % top.Items.Count;
			}
			top.Index = index;
			return Current.Spoken;
		}

		/// <summary>The first (false) or last (true) entry of the current list.</summary>
		public string Jump(bool toEnd) {
			Frame top = Top();
			if (top == null || top.Items.Count == 0) return null;
			top.Index = toEnd ? top.Items.Count - 1 : 0;
			return Current.Spoken;
		}

		/// <summary>
		/// Enter on the current entry: a section is entered, a setting steps (a toggle flips),
		/// an action is handed back in <paramref name="activated"/> for the caller to carry
		/// out (it returns nothing to say; the action speaks or plays for itself).
		/// </summary>
		public string Enter(out MenuItem activated) {
			activated = null;
			MenuItem item = Current;
			if (item == null) return null;
			if (item.Children != null) return Push(item);
			if (item.Setting != null) {
				item.Setting.Step(1);
				return item.Setting.Spoken();
			}
			if (item.Activate != null) activated = item;
			return null;
		}

		/// <summary>Left (-1) or Right (+1) on a setting: its new value (or the same value at
		/// an end, so the key is never silent); null on anything else.</summary>
		public string Adjust(int direction) {
			MenuItem item = Current;
			if (item == null || item.Setting == null) return null;
			item.Setting.Step(direction);
			return item.Setting.Spoken();
		}

		/// <summary>Back to the section above, saying the entry the player returns to;
		/// <paramref name="closed"/> when it was the top and the menu is now closed.</summary>
		public string Back(out bool closed) {
			closed = false;
			if (_stack.Count <= 1) {
				_stack.Clear();
				closed = true;
				return null;
			}
			_stack.RemoveAt(_stack.Count - 1);
			return Current != null ? Current.Spoken : null;
		}

		private string Push(MenuItem owner) {
			IList<MenuItem> items = owner.Children != null ? owner.Children() : null;
			if (items == null) items = new List<MenuItem>();
			_stack.Add(new Frame { Owner = owner, Items = items, Index = 0 });
			var message = new Message().Add(owner.Label);
			if (owner.Hint != null) message.Add(owner.Hint());
			message.Add(items.Count > 0 ? items[0].Spoken : Strings.ReviewEmpty(owner.Label));
			return Sentences(message.Items());
		}

		// The parts as sentences: a part that already ends in punctuation keeps its own.
		private static string Sentences(List<string> parts) {
			var text = new System.Text.StringBuilder();
			for (int i = 0; i < parts.Count; i++) {
				string part = parts[i];
				text.Append(part);
				if (i == parts.Count - 1) break;
				char last = part[part.Length - 1];
				text.Append(last == '.' || last == '!' || last == '?' || last == ':' || last == '。' ? " " : ". ");
			}
			return text.ToString();
		}

		private Frame Top() => _stack.Count > 0 ? _stack[_stack.Count - 1] : null;
	}
}
