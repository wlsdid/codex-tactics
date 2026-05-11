# Study Note - Enemy Intent UI

## Concept

Enemy Intent UI shows what the enemy is likely to do next before the player chooses an action.

In turn-based RPGs, this improves decision-making because the player can compare risk and reward:

- attack now for damage
- use a skill before the enemy acts
- guard before a strong attack
- skip/end turn to recover resources

## Applied in this project

`BattleManager` already tracks enemy turns with `enemyTurnCount` and chooses a strong attack every 3rd enemy turn through `EnemyPatternData`.

The new intent text uses the next enemy turn number:

```csharp
int nextEnemyTurn = enemyTurnCount + 1;
```

Then it checks whether that upcoming turn is a strong attack:

```csharp
if (enemyPattern.IsStrongAttackTurn(nextEnemyTurn))
{
    return $"Next Enemy: {enemyPattern.strongAttackName} ({enemyPattern.strongAttackDamage})";
}

return $"Next Enemy: Normal Attack ({enemyPattern.normalAttackDamage})";
```

## Why this is beginner-readable

The feature does not require animations, sprites, or complex AI. It only needs:

1. a `TMP_Text` field,
2. a small string-building function,
3. the existing enemy pattern data,
4. one extra generated scene text object.

## Portfolio explanation

This is a good UX-focused portfolio detail because it shows that battle systems are not only about damage calculation. The UI communicates game state clearly so the player can make tactical choices.
