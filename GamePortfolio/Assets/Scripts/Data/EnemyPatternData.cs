using UnityEngine;

[System.Serializable]
public class EnemyPatternData
{
    [Header("Normal Attack")]
    public string normalAttackMessageVerb = "attacks";
    public int normalAttackDamage = 15;

    [Header("Strong Attack")]
    public string strongAttackName = "Heavy Slam";
    public int strongAttackDamage = 30;
    public int strongAttackEveryTurns = 3;

    public bool IsStrongAttackTurn(int enemyTurnNumber)
    {
        return strongAttackEveryTurns > 0 && enemyTurnNumber % strongAttackEveryTurns == 0;
    }

    public int GetDamageForTurn(int enemyTurnNumber)
    {
        if (IsStrongAttackTurn(enemyTurnNumber))
        {
            return strongAttackDamage;
        }

        return normalAttackDamage;
    }

    public string BuildPatternHelpText()
    {
        return $"Enemy pattern: Normal attack: {normalAttackDamage} damage. {strongAttackName}: {strongAttackDamage} damage every {strongAttackEveryTurns}rd enemy turn.";
    }
}
