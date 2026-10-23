using System;
using UnityEngine;

namespace HandOfFate2Access.Focus {
	/// <summary>
	/// Tracks whether the current focus change was driven by the user pressing a key.
	/// Every UI input the game acts on is dispatched through UIInputManager's static
	/// OnInputButton / OnNavigate delegates; the mod subscribes to both and stamps the
	/// frame. A focus change seen on that frame or the next is user-driven; any other
	/// is the game landing focus on its own. Frame-stamped rather than a held flag so
	/// nothing can strand the state "on".
	/// </summary>
	internal static class NavigationState {
		private static int _inputFrame = -1;

		public static void Install() {
			UIInputManager.OnInputButton = (UIInputManager.InputButtonDelegate)Delegate.Combine(
				UIInputManager.OnInputButton, new UIInputManager.InputButtonDelegate(OnInput));
			UIInputManager.OnNavigate = (UIInputManager.InputButtonDelegate)Delegate.Combine(
				UIInputManager.OnNavigate, new UIInputManager.InputButtonDelegate(OnNavigate));
		}

		private static void OnInput(InputButton button) {
			Diagnostics.LatencyProbe.Mark("game button " + button);
			Mark();
			StampMouse();
		}

		private static void OnNavigate(InputButton button) {
			Diagnostics.LatencyProbe.Mark("game navigate " + button);
			Mark();
			StampMouse();
			LastNavigateFrame = Time.frameCount;
			LastNavigateButton = button;
		}

		// Where the mouse pointer was at the last key or button input.
		private static Vector3 _mouseAtInput;
		private static bool _inputSeen;

		private static void StampMouse() {
			_mouseAtInput = UnityEngine.Input.mousePosition;
			_inputSeen = true;
		}

		/// <summary>
		/// Whether the mouse pointer has stayed put since the last key or controller
		/// input. A pointer left resting over a scrolling list would otherwise take the
		/// focus whenever a row slides under it.
		/// </summary>
		public static bool MouseIsStale =>
			_inputSeen && (UnityEngine.Input.mousePosition - _mouseAtInput).sqrMagnitude < 4f;

		/// <summary>The frame of the last directional UI input (arrows, d-pad, stick).</summary>
		public static int LastNavigateFrame { get; private set; } = -100;

		/// <summary>The direction of the last directional UI input.</summary>
		public static InputButton LastNavigateButton { get; private set; }

		/// <summary>
		/// Stamps the frame when any key or controller button went down. Some screens
		/// (the card carousels) read input themselves instead of through
		/// UIInputManager, so its delegates alone miss them.
		/// </summary>
		public static void PollRawInput() {
			if (UnityEngine.Input.anyKeyDown) {
				Mark();
				return;
			}
			InControl.InputDevice device = InControl.InputManager.ActiveDevice;
			if (device != null && (device.AnyButtonWasPressed || device.DPad.WasPressed || device.LeftStick.WasPressed))
				Mark();
		}

		public static void Mark() => _inputFrame = Time.frameCount;

		/// <summary>Whether user input was dispatched this frame or the previous one.</summary>
		public static bool WasRecent => Time.frameCount - _inputFrame <= 1;
	}
}
