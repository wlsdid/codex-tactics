# Study Note — Enemy Turn Count AI

## Concept learned
Use an integer counter to track how many times the enemy has acted.

```csharp
enemyTurnCount++;
bool isStrongAttackTurn = enemyTurnCount % 3 == 0;
```

## Why it was needed
A portfolio battle system looks stronger when enemies do not repeat the exact same action forever. A simple 3-turn pattern is easy to understand and easy to balance.

## Where it was applied
- `BattleManager.ResolveEnemyAttack()` increments `enemyTurnCount`.
- Every 3rd enemy turn changes Slime's normal attack into `Heavy Slam`.
- Inspector fields control the timing, damage multiplier, and attack name.

## Reflection
This is a small step toward data-driven boss patterns. Later, this can become a list of enemy actions such as `Attack -> Attack -> Heavy Slam -> Guard Break`.
