using UnityEngine;

/// <summary>
/// Central balance configuration for the battle system.
/// All magic numbers are collected here for easy tuning.
/// </summary>
[CreateAssetMenu(fileName = "BattleBalanceConfig", menuName = "Codex Tactics/Battle Balance Config")]
public class BattleBalanceConfig : ScriptableObject
{
    [Header("Player Stats")]
    public int playerMaxHp = 100;
    public int playerAttack = 20;
    public int playerMaxAp = 3;
    public int playerApRecoveryPerTurn = 1;

    [Header("Basic Skill (Slash)")]
    public int basicSkillPower = 20;
    public int basicSkillApCost = 0;

    [Header("Fire Skill (Fire Bolt)")]
    public int fireSkillPower = 30;
    public int fireSkillApCost = 2;

    [Header("Ice Skill (Ice Lance)")]
    public int iceSkillPower = 25;
    public int iceSkillApCost = 1;
    public int stunTurnDuration = 1;

    [Header("Lightning Skill (Lightning Strike)")]
    public int lightningSkillPower = 40;
    public int lightningSkillApCost = 3;

    [Header("Battle Speed")]
    [Range(1.0f, 3.0f)]
    [Tooltip("Multiplier for battle animation speed (1 = normal, 2 = 2x)")]
    public float battleSpeedMultiplier = 1.0f;

    [Header("Status Effects")]
    public int burnDamagePerTurn = 3;
    public int burnTurnDuration = 2;

    [Header("Guard")]
    [Range(0, 100)]
    public int guardDamageReductionPercent = 50;

    [Header("Rank Thresholds")]
    [Tooltip("S rank requires fewer or equal enemy turns")]
    public int sRankMaxTurns = 1;
    [Tooltip("S rank requires less than or equal damage taken")]
    public int sRankMaxDamageTaken = 0;
    [Tooltip("A rank requires fewer or equal enemy turns")]
    public int aRankMaxTurns = 3;
    [Tooltip("A rank requires less than or equal damage taken")]
    public int aRankMaxDamageTaken = 30;

    [Header("Pace Thresholds")]
    public int fastPaceMaxTurns = 1;
    public int steadyPaceMaxTurns = 3;

    [Header("Element Weakness")]
    [Range(1.0f, 3.0f)]
    [Tooltip("Damage multiplier when skill element matches enemy weakness")]
    public float weaknessDamageMultiplier = 1.5f;
    [Range(0.1f, 1.0f)]
    [Tooltip("Damage multiplier when no relationship (neutral hit)")]
    public float neutralDamageMultiplier = 1.0f;

    [Header("Rewards")]
    public int sRankRewardGold = 150;
    public int aRankRewardGold = 120;
    public int bRankRewardGold = 100;
    public int defeatRewardGold = 0;

    [Header("Battle Log")]
    public int maxBattleLogEntries = 6;
}
