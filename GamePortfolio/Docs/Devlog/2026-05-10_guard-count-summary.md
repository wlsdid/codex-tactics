# Devlog - 2026-05-10 Guard Count Summary

## What changed

- Added a `guardUseCount` combat report counter to `BattleManager`.
- The counter increments once when the player chooses `Guard`.
- The counter resets on `StartBattle()` and Retry.
- Result summary now includes:
  - `Guard uses: ...`
- Battle logic auto-test now checks:
  - Guard count starts at 0 after restart
  - choosing Guard increments the count to 1
  - Defeat summary includes `Guard uses: 0` in a no-guard defeat path
  - Retry resets the count to 0

## Why this matters

The result summary is becoming a small combat report, not just an end-state label. Showing Guard uses helps explain how defensive choices affected the battle and gives the portfolio prototype another visible UX/detail-oriented feature.

## Verification

Unity Play Mode was not run in this environment. Static verification covered source tokens, bracket balance, whitespace, and `git diff --check`.
