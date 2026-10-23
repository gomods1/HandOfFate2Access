using System;
using System.Collections.Generic;

namespace HandOfFate2Access.Review {
	/// <summary>Where a review step stopped: on an item, or against an edge.</summary>
	public enum ReviewEdge {
		None,
		Top,
		End,
		Empty,
	}

	/// <summary>One review step: the item reached, or the edge it hit (Text null).</summary>
	public struct ReviewStep {
		public string Text;
		public ReviewEdge Edge;

		public static ReviewStep At(string text) => new ReviewStep { Text = text, Edge = ReviewEdge.None };
		public static ReviewStep Against(ReviewEdge edge) => new ReviewStep { Edge = edge };
	}

	/// <summary>
	/// A named list of lines the player reads one at a time with Ctrl+Up / Ctrl+Down:
	/// the focused thing's details, the events heard so far, the run's status. Items are
	/// in review order (index 0 first: the first detail line, or the newest event). The
	/// cursor starts before the top, so the first step forward (Ctrl+Up) reads item 0. No wrap: at
	/// either end the step reports the edge instead of moving.
	///
	/// A buffer with a <see cref="Refresher"/> rebuilds its items from live state on
	/// every read; a null result empties it (nothing to read here now).
	/// </summary>
	public sealed class ReviewBuffer {
		private readonly Func<string> _name;
		private readonly List<string> _items = new List<string>();
		private readonly int _maxItems;
		private int _cursor = -1;

		/// <summary>Rebuilds the items on every read; null when there is nothing.</summary>
		public Func<IList<string>> Refresher;

		/// <param name="name">The buffer's spoken name, read at speech time (it follows the language).</param>
		/// <param name="maxItems">Cap for <see cref="Add"/>; 0 for none.</param>
		public ReviewBuffer(Func<string> name, int maxItems = 0) {
			_name = name;
			_maxItems = maxItems;
		}

		public string Name => _name();

		public int Cursor => _cursor;

		public int Count {
			get {
				Refresh();
				return _items.Count;
			}
		}

		/// <summary>The current items, in review order.</summary>
		public IList<string> Items {
			get {
				Refresh();
				return _items.AsReadOnly();
			}
		}

		/// <summary>
		/// Replaces the items. The cursor goes back before the top only when the content
		/// changed, so re-reading the same focus keeps the player's place.
		/// </summary>
		public void Set(IList<string> items) {
			if (Same(items)) return;
			_items.Clear();
			if (items != null)
				foreach (string item in items)
					if (!string.IsNullOrEmpty(item)) _items.Add(item);
			_cursor = -1;
		}

		/// <summary>
		/// Puts an item on top (the newest event). A player partway through stays on the
		/// item they were on; past the cap the oldest item drops off.
		/// </summary>
		public void Add(string item) {
			if (string.IsNullOrEmpty(item)) return;
			_items.Insert(0, item);
			if (_cursor >= 0) _cursor++;
			if (_maxItems > 0 && _items.Count > _maxItems) {
				_items.RemoveAt(_items.Count - 1);
				if (_cursor >= _items.Count) _cursor = _items.Count - 1;
			}
		}

		public void Clear() {
			_items.Clear();
			_cursor = -1;
		}

		/// <summary>Back before the top: the next step forward reads item 0.</summary>
		public void ResetCursor() => _cursor = -1;

		/// <summary>The next item down the list (further detail, an older event).</summary>
		public ReviewStep Deeper() {
			Refresh();
			if (_items.Count == 0) return ReviewStep.Against(ReviewEdge.Empty);
			if (_cursor >= _items.Count - 1) return ReviewStep.Against(ReviewEdge.End);
			_cursor++;
			return ReviewStep.At(_items[_cursor]);
		}

		/// <summary>The previous item, back toward the top; past item 0 the cursor rests before the top.</summary>
		public ReviewStep TowardTop() {
			Refresh();
			if (_items.Count == 0) return ReviewStep.Against(ReviewEdge.Empty);
			if (_cursor <= 0) {
				_cursor = -1;
				return ReviewStep.Against(ReviewEdge.Top);
			}
			_cursor--;
			return ReviewStep.At(_items[_cursor]);
		}

		private void Refresh() {
			if (Refresher == null) return;
			IList<string> fresh = Refresher();
			if (fresh == null) {
				Clear();
				return;
			}
			Set(fresh);
		}

		private bool Same(IList<string> items) {
			int count = 0;
			if (items != null)
				foreach (string item in items)
					if (!string.IsNullOrEmpty(item)) {
						if (count >= _items.Count || _items[count] != item) return false;
						count++;
					}
			return count == _items.Count;
		}
	}
}
