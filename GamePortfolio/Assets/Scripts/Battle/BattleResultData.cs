public struct BattleResultData
{
    public string resultLabel;
    public int enemyTurns;
    public string playerName;
    public int playerCurrentHp;
    public int playerMaxHp;
    public int playerCurrentAp;
    public int playerMaxAp;
    public string enemyName;
    public int enemyCurrentHp;
    public int enemyMaxHp;
    public int damageDealt;
    public int damageTaken;
    public int guardUses;
    public int skillsUsed;
    public string rank;
    public int rewardGold;
    public string resultTip;
    public string lastEnemyPattern;

    public string BuildSummaryText()
    {
        return BattleResultPresenter.BuildSummaryText(this);
    }
}
