# Devlog - 2026-05-10 Balance Table

## What changed

- Added `Docs/BalanceTable.md`.
- Recorded current prototype values for:
  - Hero HP/AP
  - Slime HP
  - Slash / Fire Skill / Guard / End Turn
  - Burn duration and damage
  - Normal Attack and Heavy Slam
  - Result summary metrics
  - Rank rules
- Added short notes explaining why each value exists and how it affects the battle loop.

## Why this matters

A portfolio project should show not only what was implemented, but also how design decisions were tracked. The balance table makes HP, AP, damage, enemy pattern, and rank threshold decisions easy to review later.

## Verification

Unity Play Mode was not run. This was a documentation-only update based on current source values in `BattleManager.cs` and `EnemyPatternData.cs`.
