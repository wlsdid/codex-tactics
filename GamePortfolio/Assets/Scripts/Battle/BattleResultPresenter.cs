public static class BattleResultPresenter
{
    public static string BuildSummaryText(BattleResultData data)
    {
        string title = data.resultLabel.ToUpperInvariant();
        string summary = $"{title}\n" +
                         $"Party {data.survivors}/{data.partySize} Survived  |  HP {data.partyRemainingHp}\n" +
                         $"Turns {data.enemyTurns}\n" +
                         $"Damage Dealt {data.damageDealt}  |  Damage Taken {data.damageTaken}\n" +
                         $"Skills {data.skillsUsed}  |  Guards {data.guardUses}\n";
        if (title == "VICTORY")
            summary += $"Gold +{data.rewardGold}  |  XP +{data.rewardXp}\nRank {data.rank}";
        else
            summary += data.resultTip;
        return summary;
    }
}
