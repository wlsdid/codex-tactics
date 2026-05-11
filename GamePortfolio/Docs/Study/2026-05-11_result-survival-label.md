# Study Note - Result Survival Label

Date: 2026-05-11

## Concept learned

A result screen can show both raw numbers and interpreted labels. Raw HP like `40/100` is exact, while `Survival: 40%` is easier to compare between runs.

## Why it was needed

The prototype already tracked final HP, damage taken, Pace, Rank, Reward, and Tip. Survival adds a defensive performance summary that is easy to understand in a portfolio screenshot.

## Where it was applied

- `BattleResultData` stores `survivalLabel` with the other result values.
- `BattleResultEvaluator.BuildSurvivalLabel(...)` converts current HP and max HP into a clamped whole-percent label.
- `BattleResultPresenter.BuildSummaryText(...)` prints the final UI line.
- `BattleAutoTestRunner` verifies the label for Defeat, Victory, evaluator logic, and presenter formatting.

## Example

```text
Hero HP: 40/100 | AP: 3/3
Pace: Defeated
Survival: 40%
Rank: C
```

## Reflection

Keeping the percentage calculation inside `BattleResultEvaluator` follows the current structure: `BattleManager` gathers battle state, the evaluator computes result labels, and the presenter formats readable text.
