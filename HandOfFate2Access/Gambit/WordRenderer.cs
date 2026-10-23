using System.Collections.Generic;
using HandOfFate2Access.Speech;
using HandOfFate2Access.Util;

namespace HandOfFate2Access.Gambit {
	/// <summary>
	/// Renders short words (the chance outcomes the game prints on its cards) to PCM
	/// through the SAPI shim, so they can be panned to a card's position and sequenced
	/// against its identity tone. The screen reader cannot do either (it returns no
	/// audio). Rendered buffers are kept per word text, so a language change simply
	/// renders the new words.
	/// </summary>
	public sealed class WordRenderer {
		private readonly SapiRenderer _renderer = new SapiRenderer();
		private readonly Dictionary<string, float[]> _pcm = new Dictionary<string, float[]>();
		private readonly Dictionary<string, int> _rates = new Dictionary<string, int>();
		private bool _available;

		public bool IsAvailable => _available;

		public bool Initialize() {
			_available = _renderer.Initialize();
			if (!_available)
				Log.Warn("gambit word speech unavailable; SAPI renderer did not initialize");
			return _available;
		}

		// SAPI parses text starting with '<' as SAPI XML; the optional voice element picks
		// an installed voice for the game's language and falls back to the default
		// voice when none is installed, so the word is pronounced in its own language.
		private static string WithLanguage(string word) {
			string escaped = word.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");
			string lcid = LanguageId();
			return lcid == null ? escaped : "<voice optional=\"Language=" + lcid + "\">" + escaped + "</voice>";
		}

		private static string LanguageId() {
			LanguageManager manager = LanguageManager.Instance;
			switch (manager != null ? manager.CurrentLanguage : null) {
				case "en": return "409";
				case "fr": return "40c";
				case "de": return "407";
				case "es": return "c0a;40a";
				case "es-lat": return "80a;c0a";
				case "ru": return "419";
				case "ja": return "411";
				case "ko": return "412";
				case "it": return "410";
				case "pt-br": return "416";
				case "zh": return "804";
				case "pl": return "415";
				default: return null;
			}
		}

		public void Shutdown() {
			_renderer.Shutdown();
			_available = false;
		}

		/// <summary>The rendered word, rendering it on first use. False when SAPI failed (logged).</summary>
		public bool TryGet(string word, out float[] pcm, out int sampleRate) {
			pcm = null;
			sampleRate = 0;
			if (!_available || string.IsNullOrEmpty(word)) return false;
			if (!_pcm.TryGetValue(word, out pcm)) {
				if (!_renderer.Render(WithLanguage(word), out pcm, out int channels, out int rate))
					return false;
				_pcm[word] = pcm;
				_rates[word] = rate;
			}
			sampleRate = _rates[word];
			return pcm != null && pcm.Length > 0;
		}
	}
}
