# Devlog - Skills Used Summary

## Goal

Make the battle result summary show how many offensive skill actions the player used.

This is a small portfolio-visible improvement because the combat report now records both:

- defensive choice count: `Guard uses`
- offensive action count: `Skills used`

## What changed

- Added TDD-style expectations in `BattleAutoTestRunner` first:
  - Fire Skill increases `DebugSkillsUsedCount` to `1`.
  - Restart and Retry reset the counter to `0`.
  - Victory/Defeat summaries include `Skills used: ...`.
- Added `skillsUsedCount` to `BattleManager`.
- Reset the counter in `StartBattle()`.
- Incremented the counter after a player skill successfully spends AP.
- Added `Skills used` to the result summary line next to `Guard uses`.
- Updated README, battle state machine docs, balance table, and manual validation guide.

## Why it helps

`Damage dealt` says what happened numerically, but `Skills used` says how active the player was with offensive turns. This gives the result screen another simple data point for later balancing and portfolio explanation.

## Unity manual validation

After Unity recompiles:

1. Run `Tools > Codex Tactics > Create Battle Test Scene`.
2. Run `Tools > Codex Tactics > Validate Battle Test Scene`.
3. Run `Tools > Codex Tactics > Run Battle Logic Auto Test`.
4. Play manually and confirm the result summary includes `Skills used`.

## Outside-Unity verification

Current environment cannot run Unity Editor, so verification is limited to static checks and `git diff --check`.
