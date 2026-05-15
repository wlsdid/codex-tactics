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

    [Header("Rewards")]
    public int sRankRewardGold = 150;
    public int aRankRewardGold = 120;
    public int bRankRewardGold = 100;
    public int defeatRewardGold = 0;

    [Header("Battle Log")]
    public int maxBattleLogEntries = 6;
}
