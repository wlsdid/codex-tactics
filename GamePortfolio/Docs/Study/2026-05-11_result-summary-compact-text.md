# Study Note - Compact UI Text for Result Summaries

Date: 2026-05-11

## Concept learned

When a UI screen has many values, readability can improve without adding new systems by grouping related information into compact lines.

## Where it was applied

`BattleResultPresenter.BuildSummaryText(...)` now groups related result values:

```text
Result: Victory | Turns: 0
Hero: HP 100/100, AP 3/3
Damage: dealt 80, taken 0
Choices: Guard 0, Skills 2
Pace: Fast | Survival: 100%
Rank: S | Reward: 150G
```

## Why it was needed

The result summary already included many portfolio-friendly metrics. Adding more could make the UI harder to read. Compact wording keeps the same information but makes the result panel easier to scan and capture.

## Code lesson

Because result text is isolated in `BattleResultPresenter.cs`, this UI wording change did not require changing battle flow logic. Only presenter expectations in `BattleAutoTestRunner.cs` needed to be updated.

## Reflection

Small UI text changes should still be tested. A result summary is part of gameplay feedback, so automated expectations should check important displayed strings.
