# Study Note - BattleResultEvaluator Split

## Concept

When one class starts doing too many jobs, split by responsibility.

Before this change, `BattleManager` ended the battle, gathered result values, decided the rank, decided the pace label, picked the reward, picked the tip, and then sent data to the presenter.

That still worked, but it made `BattleManager` less focused.

## Applied in this project

A new `BattleResultEvaluator` class now owns the result rules:

```csharp
string rank = BattleResultEvaluator.BuildRank(resultState, enemyTurnCount, totalDamageTaken);
string pace = BattleResultEvaluator.BuildPaceLabel(resultState, enemyTurnCount);
```

`BattleManager` still controls the battle flow, but the evaluation rules are easier to find and test.

## Portfolio explanation

This is a small example of separation of concerns:

- Battle flow code is separate from result evaluation rules.
- Result evaluation rules are separate from UI text formatting.
- The result screen can grow without making one giant manager class.
