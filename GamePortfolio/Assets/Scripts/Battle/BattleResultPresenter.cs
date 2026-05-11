public static class BattleResultPresenter
{
    public static string BuildSummaryText(BattleResultData data)
    {
        return $"Result: {data.resultLabel} | Turns: {data.enemyTurns}\n" +
               $"{data.playerName}: HP {data.playerCurrentHp}/{data.playerMaxHp}, AP {data.playerCurrentAp}/{data.playerMaxAp}\n" +
               $"{data.enemyName}: HP {data.enemyCurrentHp}/{data.enemyMaxHp}\n" +
               $"Damage: dealt {data.damageDealt}, taken {data.damageTaken}\n" +
               $"Choices: Guard {data.guardUses}, Skills {data.skillsUsed}\n" +
               $"Pace: {data.paceLabel} | Survival: {data.survivalLabel}\n" +
               $"Rank: {data.rank} | Reward: {data.rewardGold}G | Total Gold: {data.totalGold}G\n" +
               $"Tip: {data.resultTip}\n" +
               $"Last enemy pattern: {data.lastEnemyPattern}";
    }
}
