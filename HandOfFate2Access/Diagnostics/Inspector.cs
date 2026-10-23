using System.Reflection;
using System.Text;
using HandOfFate2Access.Util;
using UnityEngine;

namespace HandOfFate2Access.Diagnostics {
	/// <summary>
	/// Development aid: dumps a GameObject's path, the components on it and its
	/// ancestors, and the labels beneath it, so an unreadable control can be traced
	/// back to the decompiled source. Gated by <see cref="Enabled"/>.
	/// </summary>
	internal static class Inspector {
		public static bool Enabled = true;

		public static string PathOf(Transform t) {
			var sb = new StringBuilder(t.name);
			for (Transform p = t.parent; p != null; p = p.parent)
				sb.Insert(0, p.name + "/");
			return sb.ToString();
		}

		public static void Dump(string reason, GameObject go) {
			if (!Enabled || go == null) return;
			var sb = new StringBuilder();
			sb.Append("inspect (").Append(reason).Append(") ").Append(PathOf(go.transform));
			Transform t = go.transform;
			for (int depth = 0; t != null && depth < 6; depth++, t = t.parent) {
				sb.Append("\n  ").Append(depth).Append(' ').Append(t.name).Append(':');
				foreach (Component c in t.GetComponents<Component>())
					if (c != null) sb.Append(' ').Append(c.GetType().Name);
			}
			foreach (UILabel l in go.GetComponentsInChildren<UILabel>(true))
				sb.Append("\n  label ").Append(PathOf(l.transform).Replace(PathOf(go.transform), "~"))
					.Append(l.gameObject.activeInHierarchy ? "" : " [inactive]")
					.Append(" a=").Append(l.finalAlpha.ToString("0.00"))
					.Append(" '").Append(l.text).Append('\'');
			Log.Info(sb.ToString());
		}

		public static void DumpSelections() {
			var sb = new StringBuilder("selections:");
			var manager = (UISelectionManager)HarmonyLib.AccessTools.Field(typeof(UISelectionManager), "m_instance").GetValue(null);
			if (manager == null) { Log.Info("selections: no manager"); return; }
			foreach (UISelection s in UISelectionManager.Selections)
				sb.Append("\n  ").Append(s.Category).Append(" group=").Append(s.SelectedGroup == null ? "null" : s.SelectedGroup.name)
					.Append(" hover=").Append(s.HoveredObject == null ? "null" : PathOf(s.HoveredObject.transform))
					.Append(" nav=").Append(s.ControllerNavObject == null ? "null" : s.ControllerNavObject.name);
			UISelection nav = UISelectionManager.NavSelection;
			if (nav != null)
				sb.Append("\n  NAV group=").Append(nav.SelectedGroup == null ? "null" : nav.SelectedGroup.name)
					.Append(" hover=").Append(nav.HoveredObject == null ? "null" : nav.HoveredObject.name);
			sb.Append("\n  active stack:");
			foreach (UISelection s in manager.ActiveStack) sb.Append(' ').Append(s.Category);
			Log.Info(sb.ToString());
		}

		/// <summary>Dumps every slot of the map board: grid cell, visibility, cards, state and spices.</summary>
		public static void DumpMap() {
			Map map = Map.Instance;
			MapLayout layout = map != null ? map.MapLayout : null;
			if (layout == null) { Log.Info("map: no layout"); return; }
			var sb = new StringBuilder("map slots (entry=" + (layout.Entry != null ? layout.Entry.name : "null")
				+ " exit=" + (layout.ExitSlot != null ? layout.ExitSlot.name : "null") + "):");
			foreach (MapLayoutSlot slot in layout.Slots) {
				if (slot == null) continue;
				sb.Append("\n  ").Append(slot.name).Append(" grid=").Append(slot.GridPosition.x).Append(',').Append(slot.GridPosition.y)
					.Append(" hidden=").Append(slot.Hidden).Append(" cards=").Append(slot.Cards.Count)
					.Append(" top=").Append(slot.TopCard != null ? slot.TopCard.name : "none")
					.Append(" complete=").Append(slot.IsComplete).Append(" canMove=").Append(slot.CanMoveTo)
					.Append(" spiceBlocked=").Append(slot.SpiceBlocked);
				foreach (Spice spice in slot.Spices)
					if (spice != null)
						sb.Append(" spice[").Append(spice.name).Append(" id=").Append(spice.Id)
							.Append(" blocks=").Append(spice.BlocksPlayer).Append(" complete=").Append(spice.Complete)
							.Append(" model=").Append(spice.HasModel).Append(']');
			}
			Log.Info(sb.ToString());
		}

		/// <summary>Dumps the visible labels and sprites under the top menu of every stack.</summary>
		public static void DumpMenus() {
			MenuManager menus = MenuManager.Instance;
			if (menus == null) return;
			var roots = new System.Collections.Generic.List<Component>();
			foreach (MenuStack stack in menus.MenuStacks) {
				Menu top = stack.CurrentMenu();
				if (top != null) roots.Add(top);
			}
			foreach (NavBar bar in Object.FindObjectsOfType<NavBar>())
				if (bar.isActiveAndEnabled) roots.Add(bar);
			foreach (Component top in roots) {
				string stackName = top is NavBar ? "navbar" : "menu";
				var sb = new StringBuilder(stackName + " " + PathOf(top.transform));
				foreach (UIWidget w in top.GetComponentsInChildren<UIWidget>(false)) {
					if (!w.enabled || w.finalAlpha <= 0.01f) continue;
					var label = w as UILabel;
					var sprite = w as UISprite;
					if (label != null && !string.IsNullOrEmpty(label.text))
						sb.Append("\n  L ").Append(PathOf(w.transform).Replace(PathOf(top.transform), "~")).Append(" '").Append(label.text).Append('\'');
					else if (sprite != null)
						sb.Append("\n  S ").Append(PathOf(w.transform).Replace(PathOf(top.transform), "~")).Append(" sprite=").Append(sprite.spriteName);
				}
				Log.Info(sb.ToString());
			}
		}

		/// <summary>Dumps every instance field of a component (name = value type/value).</summary>
		public static void DumpFields(object component) {
			if (!Enabled || component == null) return;
			var sb = new StringBuilder("fields of " + component.GetType().FullName);
			for (System.Type type = component.GetType(); type != null && type != typeof(MonoBehaviour); type = type.BaseType)
				foreach (FieldInfo f in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly)) {
					object v;
					try { v = f.GetValue(component); } catch (System.Exception e) { v = "<" + e.GetType().Name + ">"; }
					sb.Append("\n  ").Append(f.Name).Append(" = ").Append(v == null ? "null" : v.ToString());
				}
			Log.Info(sb.ToString());
		}
	}
}
