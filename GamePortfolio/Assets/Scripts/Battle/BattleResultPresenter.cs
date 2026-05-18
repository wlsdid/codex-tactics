public static class BattleResultPresenter
{
    public static string BuildSummaryText(BattleResultData data)
    {
        string resultLine = $"Result: {data.resultLabel} | Turns: {data.enemyTurns}\n";
        string hpLine = $"{data.playerName}: HP {data.playerCurrentHp}/{data.playerMaxHp}";
        if (data.playerMaxAp > 0)
            hpLine += $", AP {data.playerCurrentAp}/{data.playerMaxAp}";
        hpLine += "\n";
        string enemyHp = $"{data.enemyName}: HP {data.enemyCurrentHp}/{data.enemyMaxHp}\n";
        string statsLine = $"Damage: dealt {data.damageDealt}, taken {data.damageTaken}\n" +
                           $"Choices: Guard {data.guardUses}, Skills {data.skillsUsed}\n";
        string paceLine = $"Pace: {data.paceLabel} | Survival: {data.survivalLabel}\n";
        string rankLine = $"Rank: {data.rank} | Reward: {data.rewardGold}G | Total Gold: {data.totalGold}G\n";
        string tipLine = $"Tip: {data.resultTip}\n";
        string patternLine = $"Last enemy pattern: {data.lastEnemyPattern}";
        return resultLine + hpLine + enemyHp + statsLine + paceLine + rankLine + tipLine + patternLine;
    }
}
