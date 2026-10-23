using System.Text;
using HandOfFate2Access.Util;

namespace HandOfFate2Access.Diagnostics {
	/// <summary>
	/// Development aid: logs the active GameState and the open menu of every menu
	/// stack whenever that signature changes, so the screen flow can be mapped by
	/// playing. Gated by <see cref="Inspector.Enabled"/>.
	/// </summary>
	internal sealed class ScreenRecon {
		private string _last;

		public void Pump() {
			if (!Inspector.Enabled) return;
			var sb = new StringBuilder();
			Game game = Game.Instance;
			sb.Append("state=").Append(game != null && game.ActiveGameState != null ? game.ActiveGameState.GetType().Name : "none");
			MenuManager menus = MenuManager.Instance;
			if (menus != null) {
				foreach (MenuStack stack in menus.MenuStacks) {
					sb.Append(' ').Append(stack.Priority).Append('=');
					for (int i = 0; i < stack.Stack.Count; i++) {
						if (i > 0) sb.Append('>');
						sb.Append(stack.Stack[i] == null ? "null" : stack.Stack[i].name);
					}
				}
			}
			string sig = sb.ToString();
			if (sig == _last) return;
			_last = sig;
			Log.Info("recon " + sig);
		}
	}
}
