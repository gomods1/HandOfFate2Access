using System.Collections.Generic;
using HandOfFate2Access.Localization;
using HandOfFate2Access.UI;
using UnityEngine;

namespace HandOfFate2Access.Focus {
	/// <summary>
	/// An inventory slot under focus (the inventory's top level, where each slot is a card
	/// pile): the slot first, then what is equipped and how many other items could go in
	/// it ("Weapon slot, currently Soldier's Sword, 2 other items to equip"). The cards'
	/// stats are not spoken here; every card in the slot is in the details buffer
	/// (Control plus up arrow). The modifier slot (blessings, curses and the like) says how
	/// many cards it holds.
	/// </summary>
	internal static class InventorySlotReader {
		private static readonly Equipment.EquipmentType[] Types = {
			Equipment.EquipmentType.Weapon, Equipment.EquipmentType.Shield, Equipment.EquipmentType.Artifact,
			Equipment.EquipmentType.Armour, Equipment.EquipmentType.Helm, Equipment.EquipmentType.Ring };

		/// <summary>The slot's readout, or null when <paramref name="go"/> is not an inventory slot.</summary>
		public static UIElement Describe(GameObject go) {
			CardContainer container = go.GetComponent<CardContainer>();
			UIInventorySlot slot = container != null ? go.GetComponentInParent<UIInventorySlot>() : null;
			if (slot == null || slot.Container != container) return null;
			UIInventory inventory = go.GetComponentInParent<UIInventory>();
			Player player = Player.Instance;
			if (inventory == null || player == null || player.InventoryData == null) return null;

			var details = new List<string>();
			foreach (Card held in container.Cards)
				if (held != null && !held.Flipped) details.AddRange(CardInfoReader.ReadParts(held).All());

			var summary = new List<string>();
			if (slot == inventory.ModifierSlot) {
				summary.Add(Strings.InventoryModifierSlot);
				summary.Add(container.Cards.Count == 0 ? Strings.SlotEmpty : Strings.InventoryCards(container.Cards.Count));
				return new CardElement(summary.ToArray(), details.ToArray());
			}
			foreach (Equipment.EquipmentType type in Types) {
				if (inventory.GetEquipmentSlot(type) != slot) continue;
				summary.Add(Strings.InventorySlot(SlotName(type)));
				EquipmentCard equipped = Inventory.GetEquippedCard(player.InventoryData, type);
				summary.Add(equipped != null ? Strings.InventoryCurrent(CardInfoReader.ShortName(equipped)) : Strings.SlotEmpty);
				int others = 0;
				foreach (Card held in container.Cards)
					if (held != null && held != equipped) others++;
				if (others > 0) summary.Add(Strings.InventoryOthers(others));
				return new CardElement(summary.ToArray(), details.ToArray());
			}
			return null;
		}

		public static string SlotName(Equipment.EquipmentType type) {
			switch (type) {
				case Equipment.EquipmentType.Weapon: return Strings.SlotWeapon;
				case Equipment.EquipmentType.Shield: return Strings.SlotShield;
				case Equipment.EquipmentType.Artifact: return Strings.SlotArtifact;
				case Equipment.EquipmentType.Armour: return Strings.SlotArmour;
				case Equipment.EquipmentType.Helm: return Strings.SlotHelm;
				default: return Strings.SlotRing;
			}
		}
	}
}
