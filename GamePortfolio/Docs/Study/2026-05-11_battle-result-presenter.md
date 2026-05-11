# Study Note - BattleResultPresenter Split

## Concept learned

A presenter class can separate **data** from **display formatting**.

## Why it was needed

The battle result summary kept gaining more fields:

- damage dealt / taken
- Guard uses
- Skills used
- rank
- reward gold
- result tip
- last enemy pattern

If all formatting stays inside `BattleManager`, the battle-flow script becomes harder to read. If all formatting stays inside the data struct, the data type starts doing two jobs.

## Where it was applied

- `BattleManager` still decides when the battle ends and gathers result values.
- `BattleResultData` stores the result values.
- `BattleResultPresenter` converts those values into the final multiline UI text.

## Example pattern

```csharp
BattleResultData data = BuildBattleResultData(resultState);
string summary = BattleResultPresenter.BuildSummaryText(data);
```

## Reflection

This is a small refactor, but it is useful for a portfolio because it shows responsibility separation. The visible result UI did not change, but the code is now easier to extend later if the result screen needs icons, localization, or a richer layout.
