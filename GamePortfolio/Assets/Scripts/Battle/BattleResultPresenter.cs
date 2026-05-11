public static class BattleResultPresenter
{
    public static string BuildSummaryText(BattleResultData data)
    {
        return $"Result: {data.resultLabel}\n" +
               $"Enemy turns: {data.enemyTurns}\n" +
               $"{data.playerName} HP: {data.playerCurrentHp}/{data.playerMaxHp} | AP: {data.playerCurrentAp}/{data.playerMaxAp}\n" +
               $"{data.enemyName} HP: {data.enemyCurrentHp}/{data.enemyMaxHp}\n" +
               $"Damage dealt: {data.damageDealt} | Damage taken: {data.damageTaken}\n" +
               $"Guard uses: {data.guardUses} | Skills used: {data.skillsUsed}\n" +
               $"Rank: {data.rank}\n" +
               $"Reward: {data.rewardGold}G\n" +
               $"Tip: {data.resultTip}\n" +
               $"Last enemy pattern: {data.lastEnemyPattern}";
    }
}
