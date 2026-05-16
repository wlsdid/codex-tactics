using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Defines a usable battle item: name, description, effect type, and potency.
/// </summary>
[System.Serializable]
public class ItemData
{
    public string itemName = "Potion";
    public string description = "Restore 30 HP.";
    public ItemEffectType effectType = ItemEffectType.HealHp;
    public int effectValue = 30;
    public int quantity = 1;

    public string BuildItemHelpLine()
    {
        string effectDesc = effectType switch
        {
            ItemEffectType.HealHp => $"Restore {effectValue} HP",
            ItemEffectType.RestoreAp => $"Restore {effectValue} AP",
            ItemEffectType.Cleanse => "Cleanse all status effects",
            _ => description
        };
        return $"{itemName}: {effectDesc} (x{quantity})";
    }
}

public enum ItemEffectType
{
    HealHp,
    RestoreAp,
    Cleanse
}
