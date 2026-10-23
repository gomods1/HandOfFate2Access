using System;
using System.Collections.Generic;
using HandOfFate2Access.Settings;

namespace HandOfFate2Access.ModMenu {
	/// <summary>
	/// One entry of the accessibility mod menu. An entry is one of: a section (its
	/// <see cref="Children"/> are built when the player enters it, so a help topic reads
	/// the live key bindings), a line of text to read, an action (play a sound), or a
	/// setting (left and right change it). Labels are read through delegates so they
	/// follow a language switch.
	/// </summary>
	public sealed class MenuItem {
		private readonly Func<string> _label;

		/// <summary>The entries inside a section; null for anything else.</summary>
		public Func<IList<MenuItem>> Children;
		/// <summary>Said after a section's title when the player enters it.</summary>
		public Func<string> Hint;
		/// <summary>False for a list of text lines, which stops at its ends ("top", "end")
		/// instead of wrapping like a menu.</summary>
		public bool Wraps = true;
		/// <summary>What Enter does on an action entry.</summary>
		public Action Activate;
		/// <summary>The setting this entry changes; null for anything else.</summary>
		public Setting Setting;
		/// <summary>The entry's lines for the details review buffer; a setting's is its explanation.</summary>
		public Func<IList<string>> Details;

		public MenuItem(Func<string> label) {
			_label = label;
		}

		public string Label => _label();

		/// <summary>What is said when the player reaches the entry: a setting with its value.</summary>
		public string Spoken => Setting != null ? Label + ", " + Setting.Spoken() : Label;

		public IList<string> DetailLines() {
			if (Setting != null) return new List<string> { Setting.Description };
			return Details != null ? Details() : null;
		}

		public static MenuItem Section(Func<string> label, Func<string> hint, Func<IList<MenuItem>> children, bool wraps = true) {
			return new MenuItem(label) { Hint = hint, Children = children, Wraps = wraps };
		}

		public static MenuItem Text(string line) {
			return new MenuItem(() => line);
		}

		public static MenuItem Action(Func<string> label, Action activate) {
			return new MenuItem(label) { Activate = activate };
		}

		public static MenuItem ForSetting(Setting setting) {
			return new MenuItem(() => setting.Name) { Setting = setting };
		}

		/// <summary>A section whose entries are lines of text, built when entered.</summary>
		public static MenuItem Topic(Func<string> label, Func<IList<string>> lines) {
			return Section(label, null, () => {
				var items = new List<MenuItem>();
				foreach (string line in lines()) items.Add(Text(line));
				return items;
			}, wraps: false);
		}
	}
}
