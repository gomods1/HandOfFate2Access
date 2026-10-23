namespace HandOfFate2Access.UI {
	/// <summary>
	/// A focused control as the mod understands it, built by the plugin's
	/// ProxyFactory from extracted (Unity-free) data. Describe() composes the line
	/// spoken when focus lands on it. Subclasses hold only plain data, never a live
	/// Unity component, so their composition is unit-tested off-engine.
	/// </summary>
	public abstract class UIElement {
		public abstract Message Describe();

		/// <summary>
		/// The value part of the readout alone, for re-announcing an in-place value
		/// change (a settings row whose value moves on left/right while focus stays).
		/// Null when the element has no separable value.
		/// </summary>
		public virtual Message DescribeValue() {
			return null;
		}

		/// <summary>
		/// Everything there is to know about the control, one line per entry, for the
		/// details review buffer (Ctrl+Up). By default the readout's own parts, each on
		/// its own line; elements that speak a summary on focus list the rest here.
		/// </summary>
		public virtual System.Collections.Generic.List<string> Details() {
			return Describe().Items();
		}
	}

	/// <summary>
	/// A card: focus speaks its summary (name, type, main stats, a shop price); the
	/// details buffer holds every line the game's info panel shows, each trait as one
	/// line with its requirement and rules text, then whatever the summary adds that the
	/// card itself does not print (the price, the item it would replace).
	/// </summary>
	public sealed class CardElement : UIElement {
		private readonly string[] _summary;
		private readonly string[] _details;

		public CardElement(string[] summary, string[] details) {
			_summary = summary ?? new string[0];
			_details = details ?? new string[0];
		}

		public override Message Describe() {
			return new Message().AddRange(_summary);
		}

		public override System.Collections.Generic.List<string> Details() {
			return new Message().AddRange(_details).AddRange(_summary).Items();
		}
	}

	/// <summary>
	/// A control that speaks a short summary on focus and keeps the rest for the
	/// details buffer (an add-on pack: its name and state on focus, its description and
	/// contents in the details).
	/// </summary>
	public sealed class DetailedElement : UIElement {
		private readonly string[] _summary;
		private readonly string[] _details;

		public DetailedElement(string[] summary, string[] details) {
			_summary = summary ?? new string[0];
			_details = details ?? new string[0];
		}

		public override Message Describe() {
			return new Message().AddRange(_summary);
		}

		public override System.Collections.Generic.List<string> Details() {
			return new Message().AddRange(_details).Items();
		}
	}

	/// <summary>
	/// An encounter option button: the choice text, its utility line (the cost or
	/// requirement the game prints beside it), and whether it is greyed out. The
	/// game's "next page" arrow has no text; it reads as Continue.
	/// </summary>
	public sealed class ChoiceElement : UIElement {
		private readonly string _text;
		private readonly string _utility;
		private readonly bool _disabled;

		public ChoiceElement(string text, string utility, bool disabled) {
			_text = text;
			_utility = utility;
			_disabled = disabled;
		}

		public override Message Describe() {
			var message = new Message().Add(_text);
			if (message.Resolve().Length == 0)
				message.Add(Localization.Strings.Continue);
			message.Add(_utility);
			if (_disabled)
				message.Add(Localization.Strings.OptionUnavailable);
			return message;
		}
	}

	/// <summary>
	/// A settings row: its name, its current value, then the game's description of the
	/// setting. The value is separable, so a value changed in place (left/right on the
	/// row) re-speaks just the new value.
	/// </summary>
	public sealed class SettingElement : UIElement {
		private readonly string _title;
		private readonly string _value;
		private readonly string _description;

		public SettingElement(string title, string value, string description) {
			_title = title;
			_value = value;
			_description = description;
		}

		public override Message Describe() {
			return new Message().Add(_title).Add(_value).Add(_description);
		}

		public override Message DescribeValue() {
			return new Message().Add(_value);
		}
	}

	/// <summary>
	/// The generic fallback for any control without a targeted proxy: read every
	/// label, markup stripped, empties dropped, duplicates collapsed. When a control
	/// carries no usable label text, its object name is the only identification.
	/// </summary>
	public sealed class GenericElement : UIElement {
		private readonly string _name;
		private readonly string[] _labels;

		public GenericElement(string name, string[] labels) {
			_name = name;
			_labels = labels ?? new string[0];
		}

		public override Message Describe() {
			var labels = new Message().AddRange(_labels);
			if (labels.Resolve().Length == 0)
				return new Message().Add(_name);
			return labels;
		}
	}
}
