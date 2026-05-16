using System;
using System.Collections.Generic;

/// <summary>
/// Evaluates bonus conditions for stage encounters and computes extra rewards.
/// Bonuses encourage varied playstyles and add replay value.
/// </summary>
public static class StageBonusEvaluator
{
    public enum BonusType
    {
        NoDamage,       // Take 0 damage this encounter
        FastClear,      // Win within 2 enemy turns
        SkillMastery,   // Use 3+ different skills
        PerfectGuard,   // Guard at least 1 strong attack
        ItemFree        // Win without using items
    }

    private static readonly Dictionary<BonusType, string> BonusNames = new Dictionary<BonusType, string>
    {
        { BonusType.NoDamage, "Untouchable" },
        { BonusType.FastClear, "Swift Victory" },
        { BonusType.SkillMastery, "Jack of All Trades" },
        { BonusType.PerfectGuard, "Iron Wall" },
        { BonusType.ItemFree, "Pure Combat" }
    };

    private static readonly Dictionary<BonusType, string> BonusDescriptions = new Dictionary<BonusType, string>
    {
        { BonusType.NoDamage, "Defeat the enemy without taking any damage." },
        { BonusType.FastClear, "Win within 2 enemy turns." },
        { BonusType.SkillMastery, "Use 3 or more different skills in one encounter." },
        { BonusType.PerfectGuard, "Guard against at least one strong attack." },
        { BonusType.ItemFree, "Win without using any items." }
    };

    private static readonly Dictionary<BonusType, int> BonusGold = new Dictionary<BonusType, int>
    {
        { BonusType.NoDamage, 50 },
        { BonusType.FastClear, 30 },
        { BonusType.SkillMastery, 20 },
        { BonusType.PerfectGuard, 15 },
        { BonusType.ItemFree, 10 }
    };

    public static string GetBonusName(BonusType type) => BonusNames.GetValueOrDefault(type, type.ToString());
    public static string GetBonusDescription(BonusType type) => BonusDescriptions.GetValueOrDefault(type, "");
    public static int GetBonusGold(BonusType type) => BonusGold.GetValueOrDefault(type, 0);

    /// <summary>Evaluate all bonus conditions and return the list of earned bonuses with total extra gold.</summary>
    public static (List<BonusType> earned, int totalGold) Evaluate(
        int damageTaken,
        int enemyTurns,
        HashSet<string> skillsUsed,
        bool guardedStrongAttack,
        bool usedItems,
        int maxSkills)
    {
        var earned = new List<BonusType>();
        int totalGold = 0;

        if (damageTaken == 0) { earned.Add(BonusType.NoDamage); totalGold += GetBonusGold(BonusType.NoDamage); }
        if (enemyTurns <= 2) { earned.Add(BonusType.FastClear); totalGold += GetBonusGold(BonusType.FastClear); }
        if (skillsUsed.Count >= 3) { earned.Add(BonusType.SkillMastery); totalGold += GetBonusGold(BonusType.SkillMastery); }
        if (guardedStrongAttack) { earned.Add(BonusType.PerfectGuard); totalGold += GetBonusGold(BonusType.PerfectGuard); }
        if (!usedItems) { earned.Add(BonusType.ItemFree); totalGold += GetBonusGold(BonusType.ItemFree); }

        return (earned, totalGold);
    }
}
