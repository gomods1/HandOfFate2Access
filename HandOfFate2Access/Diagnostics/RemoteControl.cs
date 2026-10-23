using System;
using System.IO;
using HandOfFate2Access.Util;

namespace HandOfFate2Access.Diagnostics {
	/// <summary>
	/// Playtest-only remote input: lets the test harness drive the game while its window
	/// cannot take keyboard focus. Active only when <c>playtest-remote.enabled</c> exists
	/// in the plugin folder (the harness creates it; the release never ships it). Each
	/// frame the new lines of <c>playtest-remote.txt</c> are replayed:
	/// <list type="bullet">
	/// <item><c>nav Up|Down|Left|Right</c>: a directional UI input, through
	/// <c>UIInputManager.OnNavigate</c> exactly as a key press reaches the game.</item>
	/// <item><c>button Select|Cancel|F1|F2|F3|...</c>: a UI button, through
	/// <c>UIInputManager.OnInputButton</c>.</item>
	/// <item><c>kill</c>: ends the current fight by killing every enemy (the harness
	/// cannot fight without focus).</item>
	/// <item><c>mod status|locator|objectives</c>: one of the mod's own keys.</item>
	/// </list>
	/// </summary>
	internal sealed class RemoteControl {
		private readonly string _flag;
		private readonly string _commands;
		private long _offset;
		private bool _active;

		public Action Status;
		public Action Locator;
		public Action Objectives;

		public RemoteControl(string pluginDir) {
			_flag = Path.Combine(pluginDir, "playtest-remote.enabled");
			_commands = Path.Combine(pluginDir, "playtest-remote.txt");
		}

		public void Pump() {
			bool active = File.Exists(_flag);
			if (active != _active) {
				_active = active;
				Log.Info("playtest remote " + (active ? "enabled" : "disabled"));
				_offset = File.Exists(_commands) ? new FileInfo(_commands).Length : 0;
			}
			if (!_active || !File.Exists(_commands)) return;
			long length = new FileInfo(_commands).Length;
			if (length < _offset) _offset = 0;
			if (length == _offset) return;
			string text;
			using (var stream = new FileStream(_commands, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
				stream.Seek(_offset, SeekOrigin.Begin);
				var reader = new StreamReader(stream);
				text = reader.ReadToEnd();
			}
			int end = text.LastIndexOf('\n');
			if (end < 0) return;
			_offset += System.Text.Encoding.UTF8.GetByteCount(text.Substring(0, end + 1));
			foreach (string raw in text.Substring(0, end).Split('\n')) {
				string line = raw.Trim();
				if (line.Length > 0) Run(line);
			}
		}

		// Input the game sees as coming from the keyboard (a mouse-class Select would only
		// act on what the pointer is over).
		private static readonly System.Reflection.MethodInfo InputBy =
			HarmonyLib.AccessTools.Method(typeof(Defiant.InputManager), "OnInputBy");

		private void Run(string line) {
			Log.Info("remote: " + line);
			if (Defiant.InputManager.Instance != null && Defiant.InputManager.ActiveDeviceClass != InControl.InputDeviceClass.Keyboard)
				InputBy.Invoke(Defiant.InputManager.Instance, new object[] { InControl.InputDeviceClass.Keyboard });
			string[] parts = line.Split(' ');
			string arg = parts.Length > 1 ? parts[1] : null;
			try {
				switch (parts[0]) {
					case "nav":
						UIInputManager.OnNavigate?.Invoke((InputButton)Enum.Parse(typeof(InputButton), arg, true));
						break;
					case "button":
						UIInputManager.OnInputButton?.Invoke((InputButton)Enum.Parse(typeof(InputButton), arg, true));
						break;
					case "kill":
						KillEnemies();
						break;
					case "mod":
						if (arg == "status") Status?.Invoke();
						else if (arg == "locator") Locator?.Invoke();
						else if (arg == "objectives") Objectives?.Invoke();
						break;
					default:
						Log.Warn("remote: unknown command '" + line + "'");
						break;
				}
			} catch (Exception ex) {
				Log.Error("remote command '" + line + "' failed: " + ex);
			}
		}

		private static void KillEnemies() {
			CombatManager manager = CombatManager.Instance;
			if (manager == null) return;
			foreach (Targetable target in manager.Targets.ToArray()) {
				if (target == null || target.Team != TeamType.Enemy || target.Destroyable == null || target.Destroyable.IsDead) continue;
				target.Destroyable.ForceKill(null);
			}
		}
	}
}
