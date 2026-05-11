# Study Note - Battle Rank Summary

## Concept

A battle rank is a compact reward signal. It summarizes performance without asking the player to read every number.

## Current rule

```csharp
private string BuildBattleRank(BattleState resultState)
{
    if (resultState != BattleState.Victory)
    {
        return "C";
    }

    if (enemyTurnCount <= 1 && totalDamageTaken == 0)
    {
        return "S";
    }

    if (enemyTurnCount <= 3 && totalDamageTaken <= 30)
    {
        return "A";
    }

    return "B";
}
```

## Design note

The formula is intentionally simple for now. It uses data the battle already tracks: result, enemy turn count, and damage taken. That keeps the feature readable while still making the result screen feel more game-like.

## Future improvement

Later, the rank can account for damage dealt, Guard uses, remaining AP, or stage-specific target turns.
