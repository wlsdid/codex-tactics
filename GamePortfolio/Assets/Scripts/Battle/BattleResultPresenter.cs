public static class BattleResultPresenter
{
    public static string BuildSummaryText(BattleResultData data)
    {
        if (data.resultLabel == "Victory")
        {
            string header = "⚔ VICTORY ⚔\n" +
                            $"{data.enemyName} Defeated!\n\n";
            string goldLine = $"Gold Earned: {data.rewardGold}G\n" +
                              $"Total Gold: {data.totalGold}G\n\n";
            string statsLine = $"HP Remaining: {data.playerCurrentHp}/{data.playerMaxHp}\n" +
                               $"Turns: {data.enemyTurns} | Rank: {data.rank}\n" +
                               $"Dealt: {data.damageDealt} | Taken: {data.damageTaken}\n" +
                               $"Guard: {data.guardUses} | Skills: {data.skillsUsed}\n\n";
            string tipLine = $"Tip: {data.resultTip}";
            return header + goldLine + statsLine + tipLine;
        }
        else
        {
            string header = "💀 DEFEAT 💀\n" +
                            $"{data.enemyName} was too strong...\n\n";
            string statsLine = $"HP: {data.playerCurrentHp}/{data.playerMaxHp}\n" +
                               $"Turns: {data.enemyTurns}\n" +
                               $"Dealt: {data.damageDealt} | Taken: {data.damageTaken}\n\n";
            string tipLine = $"Tip: {data.resultTip}\n\n";
            string actionLine = "Retry or return to Stage Select";
            return header + statsLine + tipLine + actionLine;
        }
    }
}
