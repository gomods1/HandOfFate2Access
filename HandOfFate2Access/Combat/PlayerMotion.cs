using UnityEngine;

namespace HandOfFate2Access.Combat {
	/// <summary>
	/// Tracks whether the player is actually translating this frame, by measuring the
	/// combat controller's world displacement, and whether they are pushing into a wall:
	/// move input held while not translating. Hand of Fate 2 drives footsteps from
	/// animator curves with no patchable per-step event, so a blocked "step" is
	/// synthesized here at walking cadence while the push lasts; the collision cue turns
	/// those into directional bumps. Pumped once per frame from the update loop.
	///
	/// The last position is the only retained value, and only to form a per-frame speed;
	/// the position itself is re-read live every frame, so this holds no stale game state.
	/// </summary>
	internal static class PlayerMotion {
		// Speed, in world units per second, below which the player counts as stationary.
		// Free walk speed is about 10; a wall-blocked player is about 0, so a low cut
		// separates "blocked" from any real movement, including sliding along a wall.
		private const float MovingSpeed = 1f;

		private static Vector3 _lastPosition;
		private static bool _hasLast;
		private static bool _blockedStep;
		private static float _nextBlockedStep;

		// Move-stick magnitude above which the player counts as pushing, and the interval
		// between synthesized blocked steps (about one walking footstep).
		private const float PushThreshold = 0.5f;
		private const float BlockedStepInterval = 0.45f;

		public static bool IsMoving { get; private set; }

		/// <summary>Records a step taken walking into a wall. Set by the pump's blocked-step
		/// timer, drained once by the collision cue's pump.</summary>
		public static void NoteBlockedStep() => _blockedStep = true;

		/// <summary>Returns and clears the blocked-step flag.</summary>
		public static bool ConsumeBlockedStep() {
			bool v = _blockedStep;
			_blockedStep = false;
			return v;
		}

		public static void Pump() {
			PlayerController player = PlayerController.Instance;
			if (player == null) {
				_hasLast = false;
				IsMoving = false;
				return;
			}

			Vector3 position = player.transform.position;
			if (!_hasLast) {
				_lastPosition = position;
				_hasLast = true;
				IsMoving = false;
				return;
			}

			float dt = Time.deltaTime;
			float speed = dt > 0f ? Vector3.Distance(position, _lastPosition) / dt : 0f;
			_lastPosition = position;
			IsMoving = speed >= MovingSpeed;

			if (IsMoving || !CombatGate.IsLive || !Pushing()) {
				_nextBlockedStep = 0f;
				return;
			}
			if (Time.time >= _nextBlockedStep) {
				_nextBlockedStep = Time.time + BlockedStepInterval;
				NoteBlockedStep();
			}
		}

		private static bool Pushing() {
			Defiant.InputManager input = Defiant.InputManager.Instance;
			return input != null && input.GetAxis(InputAxis.Move).magnitude >= PushThreshold;
		}
	}
}
