# Devlog — 2026-05-16: Ice Lance + Stun Mechanic

## What I did

Added a 3rd player skill: **Ice Lance** (Ice element, 1 AP, applies Stun).

### Ice Lance stats
- **Element:** Ice
- **AP cost:** 1 (cheap tactical option vs Fire Bolt's 2 AP)
- **Power:** 25 (slightly less than Fire Bolt's 30)
- **Status:** Stun (enemy skips its next attack turn)
- **Config:** configurable via `BattleBalanceConfig` (`iceSkillPower`, `iceSkillApCost`, `stunTurnDuration`)

### Stun mechanic
When an enemy is Stunned:
- The enemy's turn is skipped entirely
- No attack is resolved
- `enemyTurnCount` is NOT incremented (so strong attack timing isn't affected)
- Status duration reduces by 1 each skipped turn

This means Stun is a **delay tactic** — it gives the player an extra turn without advancing the enemy's pattern cycle.

### Buttons expanded
- Command bar now has **5 interactive buttons**: Attack, Ice Lance, Fire Skill, Guard, End Turn
- Skill help text shows all 3 skills with their stats

### Tests added
- 7 new test checks for Ice Lance:
  - AP cost (1 AP consumed)
  - Neutral damage calculation (25 damage to Fire Slime)
  - Stun status display ("Stun (1 turns)")
  - Impact text showing Stun
  - Damage tracking
  - Skill usage counter
  - Skill help text includes Ice Lance info

## Files changed
- `Assets/Scripts/Data/BattleBalanceConfig.cs` — ice skill config fields
- `Assets/Scripts/Battle/BattleManager.cs` — iceSkill, Stun logic, button wiring
- `Assets/Scripts/Battle/BattleUI.cs` — iceSkillButton, all UI wiring
- `Assets/Editor/BattleSceneAutoBuilder.cs` — Ice Lance button + validation
- `Assets/Editor/BattleAutoTestRunner.cs` — tests for Ice Lance + Stun