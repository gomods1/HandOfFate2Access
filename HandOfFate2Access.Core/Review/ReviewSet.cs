using System.Collections.Generic;
using HandOfFate2Access.Localization;

namespace HandOfFate2Access.Review {
	/// <summary>
	/// The review buffers in their fixed order, and the one the review keys act on.
	/// Ctrl+Up / Ctrl+Down step through the current buffer; Ctrl+Left / Ctrl+Right switch
	/// buffers, skipping empty ones and wrapping, and announce "Name, N items". When focus
	/// moves to something, its details buffer is made current with the cursor before the
	/// top, so the first Ctrl+Up after focusing anything reads that thing's first detail.
	/// Every method returns the line to speak.
	/// </summary>
	public sealed class ReviewSet {
		private readonly List<ReviewBuffer> _buffers;
		private int _current;

		public ReviewSet(params ReviewBuffer[] buffers) {
			_buffers = new List<ReviewBuffer>(buffers);
		}

		public ReviewBuffer Current => _buffers[_current];

		/// <summary>Makes a buffer current, its cursor before the top.</summary>
		public void MakeCurrent(ReviewBuffer buffer) {
			int index = _buffers.IndexOf(buffer);
			if (index < 0) return;
			_current = index;
			buffer.ResetCursor();
		}

		/// <summary>Ctrl+Up: the next item of the current buffer.</summary>
		public string Deeper() => Speak(Current.Deeper());

		/// <summary>Ctrl+Down: the previous item of the current buffer.</summary>
		public string TowardTop() => Speak(Current.TowardTop());

		/// <summary>Ctrl+Right (+1) / Ctrl+Left (-1): the next buffer with something in it.</summary>
		public string Switch(int direction) {
			int n = _buffers.Count;
			for (int i = 1; i <= n; i++) {
				int index = ((_current + direction * i) % n + n) % n;
				ReviewBuffer buffer = _buffers[index];
				int count = buffer.Count;
				if (count == 0) continue;
				_current = index;
				buffer.ResetCursor();
				return Strings.ReviewBufferHeading(buffer.Name, count);
			}
			return Strings.ReviewNothing;
		}

		private string Speak(ReviewStep step) {
			switch (step.Edge) {
				case ReviewEdge.Top: return Strings.ReviewTop;
				case ReviewEdge.End: return Strings.ReviewEnd;
				case ReviewEdge.Empty: return Strings.ReviewEmpty(Current.Name);
				default: return step.Text;
			}
		}
	}
}
