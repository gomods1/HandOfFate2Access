using HandOfFate2Access.Audio;

namespace HandOfFate2Access.Glossary {
	/// <summary>
	/// One playback step of a sound example: which clip, at what voice parameters, in
	/// which sound family (so the example plays at the player's volume for it), and for
	/// how long. A looping step (a clip authored as a continuous loop, like the wall
	/// tones) is stopped after <see cref="HoldSeconds"/>; a one-shot step plays itself
	/// out, and HoldSeconds is only the gap before the NEXT step starts (zero for a
	/// final step).
	/// </summary>
	public readonly struct GlossaryStep {
		public readonly string ClipKey;
		public readonly SoundParams Params;
		public readonly bool Loop;
		public readonly float HoldSeconds;
		public readonly SoundCategory Category;

		public GlossaryStep(string clipKey, SoundParams parameters, bool loop, float holdSeconds, SoundCategory category) {
			ClipKey = clipKey;
			Params = parameters;
			Loop = loop;
			HoldSeconds = holdSeconds;
			Category = category;
		}
	}

	/// <summary>
	/// One sound example: the line spoken as the player reaches it in the list (the
	/// sound's name, then what it means), and the demo steps played when they activate
	/// it. Most entries are a single step; the wall tones play their four sides in
	/// sequence, the shuffle a moving round of the card instruments.
	/// </summary>
	public sealed class GlossaryEntry {
		// The label is read through a delegate, not baked at construction: the
		// catalog's entries live for the whole session, and the spoken line must
		// follow a language switch.
		private readonly System.Func<string> _label;
		public string Label => _label();
		public readonly GlossaryStep[] Steps;

		public GlossaryEntry(System.Func<string> label, GlossaryStep[] steps) {
			_label = label;
			Steps = steps;
		}
	}
}
