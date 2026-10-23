using System;

namespace HandOfFate2Access.Audio {
	/// <summary>The outcome a minigame position would give, from worst to best.</summary>
	public enum OutcomeKind {
		HugeFailure,
		Failure,
		Neutral,
		Success,
		HugeSuccess,
	}

	/// <summary>
	/// The sound vocabulary of the timing minigames (pendulum, wheel): one voice per
	/// outcome, ordered so better outcomes are higher and brighter. Huge failure is a low
	/// buzz, failure a dull low hum, neutral (a non-chance card) a plain mid tone, success
	/// a clear high tone, huge success a bright fifth with a fast shimmer. Loops are one
	/// second of integer-hertz partials, so they repeat seamlessly; ticks are short
	/// enveloped blips of the same timbres.
	/// </summary>
	public static class OutcomeTones {
		private struct Voice {
			public float[] Freqs;
			public float[] Amps;
			public float Gain;
			public float TremoloHz;
			public float TremoloDepth;
		}

		private static Voice VoiceFor(OutcomeKind kind) {
			switch (kind) {
				case OutcomeKind.HugeFailure:
					return new Voice { Freqs = new[] { 110f, 330f, 550f, 770f }, Amps = new[] { 1f, 0.5f, 0.3f, 0.2f }, Gain = 0.22f, TremoloHz = 0f };
				case OutcomeKind.Failure:
					return new Voice { Freqs = new[] { 196f, 392f }, Amps = new[] { 1f, 0.2f }, Gain = 0.22f, TremoloHz = 3f, TremoloDepth = 0.3f };
				case OutcomeKind.Success:
					return new Voice { Freqs = new[] { 659f, 1318f }, Amps = new[] { 1f, 0.25f }, Gain = 0.3f, TremoloHz = 0f };
				case OutcomeKind.HugeSuccess:
					return new Voice { Freqs = new[] { 880f, 1320f, 1760f }, Amps = new[] { 1f, 0.7f, 0.3f }, Gain = 0.3f, TremoloHz = 9f, TremoloDepth = 0.35f };
				default:
					return new Voice { Freqs = new[] { 392f }, Amps = new[] { 1f }, Gain = 0.25f, TremoloHz = 0f };
			}
		}

		/// <summary>One second of the outcome's sustained voice, seamless when looped.</summary>
		public static float[] Loop(OutcomeKind kind, int sampleRate) {
			Voice v = VoiceFor(kind);
			var pcm = new float[sampleRate];
			Render(v, pcm, sampleRate);
			return pcm;
		}

		/// <summary>A short blip of the outcome's voice (a wheel segment passing the pointer).</summary>
		public static float[] Tick(OutcomeKind kind, int sampleRate) {
			Voice v = VoiceFor(kind);
			v.TremoloHz = 0f;
			int n = sampleRate * 70 / 1000;
			var pcm = new float[n];
			Render(v, pcm, sampleRate);
			int attack = Math.Max(1, sampleRate / 500);
			for (int i = 0; i < n; i++) {
				float env = i < attack ? i / (float)attack : (float)Math.Exp(-6.0 * (i - attack) / (n - attack));
				pcm[i] *= env * 1.6f;
			}
			return pcm;
		}

		private static void Render(Voice v, float[] pcm, int sampleRate) {
			float ampSum = 0f;
			foreach (float a in v.Amps) ampSum += a;
			for (int i = 0; i < pcm.Length; i++) {
				double t = i / (double)sampleRate;
				double s = 0;
				for (int p = 0; p < v.Freqs.Length; p++)
					s += v.Amps[p] * Math.Sin(2 * Math.PI * v.Freqs[p] * t);
				s /= ampSum;
				if (v.TremoloHz > 0f)
					s *= 1 - v.TremoloDepth * 0.5 * (1 + Math.Sin(2 * Math.PI * v.TremoloHz * t));
				pcm[i] = (float)(s * v.Gain);
			}
		}
	}
}
