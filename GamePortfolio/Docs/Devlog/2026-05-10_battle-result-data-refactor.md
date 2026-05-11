# Devlog - BattleResultData Summary Refactor

## Goal

Keep the result summary code readable as more combat report metrics are added.

## What changed

- Added a private `BattleResultData` struct inside `BattleManager`.
- Added `BuildBattleResultData()` to gather result values in one place.
- Kept the visible result summary format the same:
  - `Result`
  - `Enemy turns`
  - final Hero HP/AP
  - final Slime HP
  - `Damage dealt` / `Damage taken`
  - `Guard uses` / `Skills used`
  - `Rank`
  - `Last enemy pattern`
- Existing auto-test expectations still cover the result summary text.
- Updated README and battle state machine docs.

## Why it helps

The result summary has grown from a simple Victory/Defeat label into a compact combat report. Grouping values into `BattleResultData` makes future additions easier, such as rewards, stage clear time, or bonus objectives.

## Verification note

This was a structure-preserving refactor. Unity Editor was not run in this environment, so verification is limited to static token/brace checks and `git diff --check`.
