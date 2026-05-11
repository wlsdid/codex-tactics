# Devlog - Guard Action

## Goal
Add one more tactical choice to the battle prototype: Guard.

## What changed
- Added `Guard` action button.
- Guard ends the player turn like End Turn.
- The next enemy attack is reduced by 50%.
- Current enemy attack is 15, so Guard reduces it to 7 damage.
- Updated the battle scene auto-builder so the generated UI has 4 buttons:
  - Attack
  - Fire Skill
  - Guard
  - End Turn
- Updated the scene validator to check the Guard button too.
- Added an editor auto-test menu for the Guard logic.

## Test menu
Use this Unity menu:

```text
Tools > Codex Tactics > Run Battle Logic Auto Test
```

Expected result:

```text
RESULT: PASS
```

## Manual test steps
1. Run `Tools > Codex Tactics > Create Battle Test Scene`.
2. Press Play.
3. Click `Guard`.
4. Enemy attack should deal 7 damage instead of 15.

## Next recommended task
Add a simple enemy AI pattern, such as:
- Slime uses normal attack most turns.
- Slime uses stronger attack every 3rd turn.
- Later enemies can have different patterns.
