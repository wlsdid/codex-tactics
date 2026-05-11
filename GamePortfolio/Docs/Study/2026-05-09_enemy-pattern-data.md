# Study Note — Serializable Enemy Pattern Data

## Concept learned
A small `[System.Serializable]` class can group related Inspector values without creating a ScriptableObject yet.

```csharp
[System.Serializable]
public class EnemyPatternData
{
    public int normalAttackDamage = 15;
    public string strongAttackName = "Heavy Slam";
    public int strongAttackDamage = 30;
    public int strongAttackEveryTurns = 3;
}
```

## Why it was needed
The enemy AI already had a normal attack and a strong attack, but the values were spread across `BattleManager`. Grouping them makes the pattern easier to read, tune, and explain.

## Where it was applied
- `Assets/Scripts/Data/EnemyPatternData.cs` stores normal/strong attack values.
- `BattleManager.ResolveEnemyAttack()` uses `GetDamageForTurn()` and `IsStrongAttackTurn()`.
- `BattleManager.UpdateSkillHelpText()` uses `BuildPatternHelpText()` so the player can see the enemy pattern.

## Reflection
This is a safe beginner step toward data-driven battle design. Later, this can grow into multiple enemy patterns or ScriptableObject assets, but the current version stays simple enough to understand and test.
