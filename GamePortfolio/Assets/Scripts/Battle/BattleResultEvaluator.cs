public static class BattleResultEvaluator
{
    public static string BuildRank(BattleState resultState, int enemyTurnCount, int totalDamageTaken, BattleBalanceConfig config = null)
    {
        if (resultState != BattleState.Victory)
        {
            return "C";
        }

        int maxTurnsS = config != null ? config.sRankMaxTurns : 1;
        int maxDamageS = config != null ? config.sRankMaxDamageTaken : 0;
        int maxTurnsA = config != null ? config.aRankMaxTurns : 3;
        int maxDamageA = config != null ? config.aRankMaxDamageTaken : 30;

        if (enemyTurnCount <= maxTurnsS && totalDamageTaken <= maxDamageS)
        {
            return "S";
        }

        if (enemyTurnCount <= maxTurnsA && totalDamageTaken <= maxDamageA)
        {
            return "A";
        }

        return "B";
    }

    public static string BuildPaceLabel(BattleState resultState, int enemyTurnCount, BattleBalanceConfig config = null)
    {
        if (resultState != BattleState.Victory)
        {
            return "Defeated";
        }

        int fastMax = config != null ? config.fastPaceMaxTurns : 1;
        int steadyMax = config != null ? config.steadyPaceMaxTurns : 3;

        if (enemyTurnCount <= fastMax)
        {
            return "Fast";
        }

        if (enemyTurnCount <= steadyMax)
        {
            return "Steady";
        }

        return "Long";
    }

    public static string BuildSurvivalLabel(int currentHp, int maxHp)
    {
        if (maxHp <= 0)
        {
            return "0%";
        }

        int clampedCurrentHp = UnityEngine.Mathf.Clamp(currentHp, 0, maxHp);
        int survivalPercent = UnityEngine.Mathf.RoundToInt((float)clampedCurrentHp / maxHp * 100f);
        return survivalPercent + "%";
    }

    public static int BuildRewardGold(string rank, int sRankRewardGold, int aRankRewardGold, int bRankRewardGold, int defeatRewardGold)
    {
        switch (rank)
        {
            case "S":
                return sRankRewardGold;
            case "A":
                return aRankRewardGold;
            case "B":
                return bRankRewardGold;
            default:
                return defeatRewardGold;
        }
    }

    public static string BuildResultTip(string rank, string lastEnemyPattern, string strongAttackName)
    {
        if (rank == "S")
        {
            return "Perfect clear!";
        }

        if (lastEnemyPattern == strongAttackName)
        {
            return "Guard before Heavy Slam.";
        }

        if (rank == "A" || rank == "B")
        {
            return "Take less damage for a higher rank.";
        }

        return "Use Fire Skill to finish the Slime faster.";
    }

    public static string BuildLastEnemyPatternLabel(int enemyTurnCount, EnemyPatternData enemyPattern)
    {
        if (enemyTurnCount <= 0)
        {
            return "None";
        }

        if (enemyPattern != null && enemyPattern.IsStrongAttackTurn(enemyTurnCount))
        {
            return enemyPattern.strongAttackName;
        }

        return "Normal Attack";
    }
}
