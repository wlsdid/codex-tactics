# Study Note - Damage Summary Counters

## Concept

A result summary becomes more useful when it records performance numbers, not only Victory or Defeat.

For this prototype, two simple numbers are enough:

- damage dealt: how much enemy HP the player removed
- damage taken: how much player HP the enemy removed

## Applied in this project

`BattleManager` now stores counters during one battle:

```csharp
private int totalDamageDealt;
private int totalDamageTaken;
```

They are reset in `StartBattle()` so Retry starts a clean run:

```csharp
totalDamageDealt = 0;
totalDamageTaken = 0;
```

Instead of adding the raw requested damage, the code compares HP before and after damage:

```csharp
totalDamageDealt += Mathf.Max(0, enemyHpBeforeDamage - enemy.currentHp);
```

This is safer because a future attack might deal 100 damage to an enemy with only 5 HP left. In that case, the actual HP removed should be 5, not 100.

## Portfolio explanation

This feature is small but useful for balancing. The developer can now compare how much damage the player dealt and received in a test run, then adjust enemy HP, skill power, Guard reduction, or strong attack damage with clearer feedback.

## Next learning direction

Later, this can become a fuller combat report:

1. turns taken,
2. damage dealt/taken,
3. skills used,
4. Guard count,
5. grade/rank such as S/A/B.
