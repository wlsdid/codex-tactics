# Study Note - BattleResultData Refactor

As UI summaries grow, string-building methods can become hard to read.

## Before

The result summary method directly read many fields:

- battle state
- player HP/AP
- enemy HP
- damage counters
- Guard count
- Skills used count
- rank
- last enemy pattern

That works for a small prototype, but each new metric makes the method longer.

## Refactor idea

`BattleResultData` gathers the result values first, then formats them:

```csharp
return BuildBattleResultData(resultState).BuildSummaryText();
```

This separates two concerns:

1. Collect result data from the battle state.
2. Format the result data for UI text.

## Portfolio lesson

This is a small example of refactoring after features are working. The visible behavior stays the same, but the code becomes easier to extend for future systems like rewards, bonus objectives, or stage result grades.
