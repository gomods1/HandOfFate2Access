using System.Reflection;
using HandOfFate2Access.Speech;
using HarmonyLib;
using UnityEngine;

namespace HandOfFate2Access.Screens {
	/// <summary>
	/// Speaks an encounter's narration as it appears: the page text (the story beat)
	/// and the announcement line under it (resource gains and losses, results). Each
	/// field is watched on its own, so a field that updates in a later frame is still
	/// spoken, and only once. The option buttons are read by the focus path.
	/// </summary>
	internal sealed class EncounterNarration {
		private static readonly FieldInfo PageTextField = AccessTools.Field(typeof(EncounterPanel), "m_pageText");
		private static readonly FieldInfo AnnouncementField = AccessTools.Field(typeof(EncounterPanel), "m_announcementText");

		private readonly PanelFinder<EncounterPanel> _panel = new PanelFinder<EncounterPanel>();
		private readonly LabelWatch _page = new LabelWatch(ignoreFade: true);
		private readonly LabelWatch _announcement = new LabelWatch(ignoreFade: true);

		// The encounter whose title was announced; a new encounter (or a sub-encounter
		// pushed on the game's encounter stack) announces its card's title first.
		private Encounter _announced;

		/// <summary>Speaks any new narration; true when something was spoken.</summary>
		public bool Pump() {
			bool titled = PumpTitle();
			EncounterPanel panel = _panel.Get();
			if (panel == null || !panel.gameObject.activeInHierarchy) {
				_page.Reset();
				_announcement.Reset();
				return titled;
			}
			bool spoke = titled;
			string page = _page.Poll((UILabel)PageTextField.GetValue(panel));
			if (page != null) {
				SpeechPipeline.SpeakEvent(page);
				spoke = true;
			}
			string announcement = _announcement.Poll((UILabel)AnnouncementField.GetValue(panel));
			if (announcement != null) {
				SpeechPipeline.SpeakEvent(announcement);
				spoke = true;
			}
			return spoke;
		}

		private bool PumpTitle() {
			Encounter encounter = Encounter.Instance;
			if (encounter == _announced) return false;
			_announced = encounter;
			if (encounter == null || encounter.Card == null) return false;
			string title = Focus.CardInfoReader.Title(encounter.Card);
			if (string.IsNullOrEmpty(title)) return false;
			SpeechPipeline.SpeakEvent(title);
			return true;
		}
	}

	/// <summary>
	/// Finds a scene-resident component the game wires only through serialized fields,
	/// rescanning at most twice a second while it is missing (destroyed objects compare
	/// equal to null, so a scene change triggers a fresh search).
	/// </summary>
	internal sealed class PanelFinder<T> where T : Component {
		private T _found;
		private float _nextSearch;

		public T Get() {
			if (_found != null) return _found;
			if (Time.unscaledTime < _nextSearch) return null;
			_nextSearch = Time.unscaledTime + 0.5f;
			_found = Object.FindObjectOfType<T>();
			return _found;
		}
	}
}
