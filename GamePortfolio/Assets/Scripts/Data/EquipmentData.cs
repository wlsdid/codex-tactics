using System;
using UnityEngine;

/// <summary>Equipment slot type — same as BrownDust2's 3-slot gear system.</summary>
public enum EquipmentSlot
{
    Weapon,
    Armor,
    Accessory
}

/// <summary>Rarity with BrownDust2-style color mapping (white → green → blue → purple → gold).</summary>
public enum EquipmentRarity
{
    Common,     // White  — base stat
    Uncommon,   // Green  — 1.3x
    Rare,       // Blue   — 1.6x
    Epic,       // Purple — 2.0x
    Legendary   // Gold   — 2.5x
}

/// <summary>
/// Serializable equipment piece. Each piece has a slot, rarity, and stat bonuses.
/// Stat bonus = baseValue * rarityMultiplier.
/// </summary>
[System.Serializable]
public class EquipmentData
{
    public string itemName;
    public EquipmentSlot slot;
    public EquipmentRarity rarity;
    public int attackBonus;   // added to player attack
    public int hpBonus;       // added to max HP
    public int apBonus;       // added to max AP
    public int defenseBonus;  // damage reduction per hit

    private static readonly float[] RarityMultipliers = { 1.0f, 1.3f, 1.6f, 2.0f, 2.5f };
    public float RarityMultiplier => RarityMultipliers[(int)rarity];

    /// <summary>Rarity colour hex for UI display.</summary>
    public static Color RarityColor(EquipmentRarity r)
    {
        return r switch
        {
            EquipmentRarity.Common    => new Color(0.80f, 0.80f, 0.80f), // white
            EquipmentRarity.Uncommon  => new Color(0.30f, 0.85f, 0.35f), // green
            EquipmentRarity.Rare      => new Color(0.25f, 0.55f, 1.00f), // blue
            EquipmentRarity.Epic      => new Color(0.75f, 0.35f, 1.00f), // purple
            EquipmentRarity.Legendary => new Color(1.00f, 0.80f, 0.15f), // gold
            _ => Color.white
        };
    }

    /// <summary>Human-readable rarity label with colour tag for TMP.</summary>
    public string RarityLabel()
    {
        Color c = RarityColor(rarity);
        return $"<color=#{ColorUtility.ToHtmlStringRGB(c)}>{rarity}</color>";
    }

    /// <summary>Full tooltip line: "Weapon · Rare · ATK +12 HP +40"</summary>
    public string BuildTooltip()
    {
        var parts = new System.Collections.Generic.List<string>();
        if (attackBonus > 0)  parts.Add($"ATK +{attackBonus}");
        if (hpBonus > 0)      parts.Add($"HP +{hpBonus}");
        if (apBonus > 0)      parts.Add($"AP +{apBonus}");
        if (defenseBonus > 0) parts.Add($"DEF +{defenseBonus}");
        string stats = parts.Count > 0 ? string.Join("  ", parts) : "No stats";
        string rarStr = $"{RarityLabel()}";
        return $"{itemName} ← {slot} · {rarStr}\n{stats}";
    }

    // ── Factory ──

    /// <summary>Generate a random equipment piece of the given slot and step index.</summary>
    public static EquipmentData Generate(EquipmentSlot slot, int stepIndex)
    {
        // Higher step index → better rarity chance
        float roll = UnityEngine.Random.value;
        EquipmentRarity rarity = stepIndex switch
        {
            <= 1 => roll < 0.60f ? EquipmentRarity.Common
                  : roll < 0.90f ? EquipmentRarity.Uncommon
                  : EquipmentRarity.Rare,
            <= 3 => roll < 0.35f ? EquipmentRarity.Common
                  : roll < 0.65f ? EquipmentRarity.Uncommon
                  : roll < 0.85f ? EquipmentRarity.Rare
                  : roll < 0.95f ? EquipmentRarity.Epic
                  : EquipmentRarity.Legendary,
            _    => roll < 0.20f ? EquipmentRarity.Common
                  : roll < 0.45f ? EquipmentRarity.Uncommon
                  : roll < 0.70f ? EquipmentRarity.Rare
                  : roll < 0.90f ? EquipmentRarity.Epic
                  : EquipmentRarity.Legendary
        };
        return Generate(slot, rarity);
    }

    public static EquipmentData Generate(EquipmentSlot slot, EquipmentRarity rarity)
    {
        float mult = RarityMultipliers[(int)rarity];
        int atk = 0, hp = 0, ap = 0, def = 0;

        switch (slot)
        {
            case EquipmentSlot.Weapon:
                atk = Mathf.RoundToInt(12 * mult);
                hp  = Mathf.RoundToInt(20 * mult);
                break;
            case EquipmentSlot.Armor:
                hp  = Mathf.RoundToInt(50 * mult);
                def = Mathf.RoundToInt(4 * mult);
                break;
            case EquipmentSlot.Accessory:
                ap  = Mathf.RoundToInt(1 * mult);
                hp  = Mathf.RoundToInt(15 * mult);
                def = Mathf.RoundToInt(2 * mult);
                break;
        }

        string[] namePrefixes = { "Iron", "Steel", "Crystal", "Mystic", "Ancient" };
        string[] nameSuffixes = slot switch
        {
            EquipmentSlot.Weapon    => new[] { "Blade", "Staff", "Sword", "Axe", "Bow" },
            EquipmentSlot.Armor     => new[] { "Plate", "Mail", "Robe", "Vest", "Guard" },
            EquipmentSlot.Accessory => new[] { "Ring", "Amulet", "Bracelet", "Crown", "Orb" },
            _ => new[] { "Relic" }
        };
        string name = $"{namePrefixes[UnityEngine.Random.Range(0, namePrefixes.Length)]} {nameSuffixes[UnityEngine.Random.Range(0, nameSuffixes.Length)]}";

        return new EquipmentData
        {
            itemName = name,
            slot = slot,
            rarity = rarity,
            attackBonus = atk,
            hpBonus = hp,
            apBonus = ap,
            defenseBonus = def
        };
    }

    /// <summary>Starter weapons for new game.</summary>
    public static EquipmentData StarterWeapon() =>
        new EquipmentData { itemName = "Training Sword", slot = EquipmentSlot.Weapon, rarity = EquipmentRarity.Common, attackBonus = 5, hpBonus = 10, defenseBonus = 0, apBonus = 0 };

    public static EquipmentData StarterArmor() =>
        new EquipmentData { itemName = "Leather Vest", slot = EquipmentSlot.Armor, rarity = EquipmentRarity.Common, attackBonus = 0, hpBonus = 20, defenseBonus = 2, apBonus = 0 };

    public static EquipmentData StarterAccessory() =>
        new EquipmentData { itemName = "Copper Ring", slot = EquipmentSlot.Accessory, rarity = EquipmentRarity.Common, attackBonus = 0, hpBonus = 5, defenseBonus = 0, apBonus = 0 };
}
