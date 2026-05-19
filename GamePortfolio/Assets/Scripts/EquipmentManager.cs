using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Static manager for the player's equipment inventory and equipped slots.
/// Each slot (Weapon, Armor, Accessory) holds one equipped item.
/// Inventory is a list of unequipped gear.
/// </summary>
public static class EquipmentManager
{
    /// <summary>Currently equipped items by slot. Null = empty slot.</summary>
    public static Dictionary<EquipmentSlot, EquipmentData> Equipped { get; private set; } = new();

    /// <summary>Unequipped gear in inventory.</summary>
    public static List<EquipmentData> Inventory { get; private set; } = new();

    /// <summary>Reset to default state — all empty, no inventory.</summary>
    public static void Reset()
    {
        Equipped = new Dictionary<EquipmentSlot, EquipmentData>();
        Inventory = new List<EquipmentData>();
    }

    /// <summary>Give starter equipment for a fresh game.</summary>
    public static void GiveStarterGear()
    {
        Equipped[EquipmentSlot.Weapon]    = EquipmentData.StarterWeapon();
        Equipped[EquipmentSlot.Armor]     = EquipmentData.StarterArmor();
        Equipped[EquipmentSlot.Accessory] = EquipmentData.StarterAccessory();
    }

    /// <summary>Equip an item from inventory. If slot already occupied, swap to inventory.</summary>
    public static void Equip(EquipmentData item)
    {
        if (item == null) return;

        // Remove from inventory
        Inventory.Remove(item);

        // If slot already filled, put current into inventory
        if (Equipped.TryGetValue(item.slot, out EquipmentData current))
        {
            Inventory.Add(current);
        }

        Equipped[item.slot] = item;
    }

    /// <summary>Unequip the item in the given slot back to inventory.</summary>
    public static void Unequip(EquipmentSlot slot)
    {
        if (!Equipped.TryGetValue(slot, out EquipmentData current)) return;
        Inventory.Add(current);
        Equipped.Remove(slot);
    }

    /// <summary>Get the item equipped in the given slot, or null.</summary>
    public static EquipmentData GetEquipped(EquipmentSlot slot) =>
        Equipped.TryGetValue(slot, out EquipmentData item) ? item : null;

    /// <summary>Sum of all stat bonuses from equipped gear.</summary>
    public static int TotalAttackBonus =>
        Equipped.Values.Sum(e => e.attackBonus);

    public static int TotalHpBonus =>
        Equipped.Values.Sum(e => e.hpBonus);

    public static int TotalApBonus =>
        Equipped.Values.Sum(e => e.apBonus);

    public static int TotalDefenseBonus =>
        Equipped.Values.Sum(e => e.defenseBonus);

    /// <summary>Summary string for UI display.</summary>
    public static string BuildSummary()
    {
        var lines = new List<string>();
        foreach (EquipmentSlot slot in System.Enum.GetValues(typeof(EquipmentSlot)))
        {
            var item = GetEquipped(slot);
            if (item != null)
                lines.Add($"{item.slot}: {item.itemName} [{item.rarity}]");
            else
                lines.Add($"{slot}: Empty");
        }
        return string.Join("\n", lines);
    }

    /// <summary>Total stat summary line for stage select panel.</summary>
    public static string BuildStatSummary()
    {
        var parts = new List<string>();
        int atk = TotalAttackBonus;
        int hp = TotalHpBonus;
        int ap = TotalApBonus;
        int def = TotalDefenseBonus;
        if (atk > 0) parts.Add($"ATK +{atk}");
        if (hp > 0)  parts.Add($"HP +{hp}");
        if (ap > 0)  parts.Add($"AP +{ap}");
        if (def > 0) parts.Add($"DEF +{def}");
        return parts.Count > 0 ? string.Join("  |  ", parts) : "No equipment bonuses";
    }

    // ── Serialization support ──

    [System.Serializable]
    public struct SerializedState
    {
        public List<EquipmentData> equippedList;
        public List<EquipmentData> inventory;
    }

    public static SerializedState CaptureState()
    {
        return new SerializedState
        {
            equippedList = Equipped.Values.ToList(),
            inventory = new List<EquipmentData>(Inventory)
        };
    }

    public static void RestoreState(SerializedState state)
    {
        Reset();
        if (state.equippedList != null)
        {
            foreach (var item in state.equippedList)
            {
                if (item != null && !Equipped.ContainsKey(item.slot))
                    Equipped[item.slot] = item;
            }
        }
        Inventory = state.inventory ?? new List<EquipmentData>();
    }
}
