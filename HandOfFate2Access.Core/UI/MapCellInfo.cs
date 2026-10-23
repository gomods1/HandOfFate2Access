using System.Collections.Generic;
using HandOfFate2Access.Localization;

namespace HandOfFate2Access.UI {
	/// <summary>The directions from a map slot that lead to a visible neighbour with no wall between.</summary>
	public sealed class MapExits {
		public bool Up { get; }
		public bool Down { get; }
		public bool Left { get; }
		public bool Right { get; }

		public MapExits(bool up, bool down, bool left, bool right) {
			Up = up;
			Down = down;
			Left = left;
			Right = right;
		}
	}

	/// <summary>
	/// One map slot as extracted by the plugin: the text of its top card (null when the
	/// card lies face down), the cards stacked under it, its marks (what the board shows on
	/// it beyond text: a token or shard to win, a figure standing on it), and its state on
	/// the board. Plain data only; composed by <see cref="MapCellElement"/>.
	/// </summary>
	public sealed class MapCellInfo {
		public IList<string> Top { get; }
		public IList<IList<string>> Attached { get; }
		public bool Here { get; }
		public bool Complete { get; }
		public bool Reachable { get; }
		public bool IsExit { get; }
		public MapExits Exits { get; }
		public IList<string> Marks { get; }

		public MapCellInfo(IList<string> top, IList<IList<string>> attached, bool here, bool complete,
				bool reachable, bool isExit, MapExits exits, IList<string> marks = null) {
			Top = top;
			Attached = attached ?? new List<IList<string>>();
			Marks = marks ?? new List<string>();
			Here = here;
			Complete = complete;
			Reachable = reachable;
			IsExit = isExit;
			Exits = exits;
		}
	}

	/// <summary>
	/// A map slot's readout: the card's name (or "face down card"), what is attached to
	/// it, its marks (a token to win, a figure on it), the single state word (here, completed, reachable), whether it is the level's
	/// exit, and the directions that continue from it, so the board's shape is learned by
	/// walking it. A card offered as the player's next move (the map's own selection) is
	/// read from the player instead: the direction it lies in first, and not its own
	/// paths, which would read as directions to press ("right, face down card, exit").
	/// The cards' full text, paths included, is in the details.
	/// </summary>
	public sealed class MapCellElement : UIElement {
		private readonly MapCellInfo _info;
		private readonly string _move;

		/// <param name="move">The direction word from the player's card when this card is
		/// offered as a move; null for a survey read.</param>
		public MapCellElement(MapCellInfo info, string move = null) {
			_info = info;
			_move = move;
		}

		public override Message Describe() {
			var message = new Message().Add(_move).Add(Name(_info.Top));
			foreach (IList<string> attached in _info.Attached) {
				string name = Name(attached);
				if (name.Length > 0)
					message.Add(Strings.MapSlotAttached + " " + name);
			}
			message.AddRange(_info.Marks);
			return AddState(message, _move == null);
		}

		public override List<string> Details() {
			var message = new Message();
			if (_info.Top == null) message.Add(Strings.CardFaceDown);
			else message.AddRange(_info.Top);
			foreach (IList<string> attached in _info.Attached) {
				string text = attached == null ? Strings.CardFaceDown : new Message().AddRange(attached).Resolve();
				if (text.Length > 0)
					message.Add(Strings.MapSlotAttached + " " + text);
			}
			message.AddRange(_info.Marks);
			return AddState(message, true).Items();
		}

		// A card's name is its first readable line.
		private static string Name(IList<string> lines) {
			if (lines == null) return Strings.CardFaceDown;
			foreach (string line in lines) {
				string name = new Message().Add(line).Resolve();
				if (name.Length > 0) return name;
			}
			return "";
		}

		private Message AddState(Message message, bool withExits) {
			if (_info.Here)
				message.Add(Strings.MapCellHere);
			else if (_info.Complete)
				message.Add(Strings.MapCellComplete);
			else if (_info.Reachable && _move == null)
				message.Add(Strings.MapCellReachable);
			if (_info.IsExit)
				message.Add(Strings.MapCellExit);
			if (withExits) message.Add(ExitsText(_info.Exits));
			return message;
		}

		public static string ExitsText(MapExits e) {
			if (e == null) return "";
			var dirs = new List<string>();
			if (e.Up) dirs.Add(Strings.DirUp);
			if (e.Down) dirs.Add(Strings.DirDown);
			if (e.Left) dirs.Add(Strings.DirLeft);
			if (e.Right) dirs.Add(Strings.DirRight);
			if (dirs.Count == 0) return Strings.MapNoExits;
			return Strings.MapExits + " " + string.Join(", ", dirs.ToArray());
		}
	}
}
