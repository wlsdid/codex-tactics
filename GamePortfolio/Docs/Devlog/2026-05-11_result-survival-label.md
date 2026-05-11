# Devlog - Result Survival Label

Date: 2026-05-11

## Goal

Add one small portfolio-visible result summary metric that makes defensive performance easier to compare after a battle.

## What changed

- Added `survivalLabel` to `BattleResultData`.
- Added `BattleResultEvaluator.BuildSurvivalLabel(currentHp, maxHp)`.
- Updated `BattleResultPresenter` to print `Survival: ...%` between `Pace` and `Rank`.
- Updated the editor auto-test to check Defeat, Victory, evaluator output, and direct presenter formatting.
- Updated README, balance docs, portfolio showcase draft, and manual validation docs.

## Why it helps the portfolio

Damage taken already tells what happened, but `Survival: 40%` is faster to understand in a screenshot. It gives the result screen another simple RPG-style performance metric without adding inventory, save data, or a larger reward system.

## Verification

- Static source/document checks
- Unity batch compile
- Battle scene validation
- Battle logic auto test

## Next goal

Capture real Unity Play Mode screenshots/GIFs so the result summary can be shown directly in the README and portfolio showcase draft.
