# Devlog — 2026-05-16: Earth Wall + Shield System

## What I did

Added the 5th player skill: **Earth Wall** (Earth element, 2 AP, 22 power, applies Shield). This completes elemental coverage for Stage 3 (Earth Golems).

### Earth Wall stats

| Field | Value |
|-------|-------|
| Element | Earth |
| AP cost | 2 |
| Power | 22 |
| Status | None (player gets Shield instead) |
| Shield absorption | 15 damage (configurable) |
| Config | `earthSkillPower`, `earthSkillApCost`, `earthSkillShieldAmount` in `BattleBalanceConfig` |

### Shield mechanic

Unlike Burn/Stun which are applied to the enemy, **Shield** is a player-side defensive effect:

1. Using Earth Wall applies Shield (15 absorption) to the player
2. On the next enemy hit, Shield absorbs up to its amount
3. Remaining damage passes through (minimum 1)
4. Shield is consumed in one hit (one-use)
5. Shield text shows in the player status area

### Complete skill set (5 skills)

| Skill | Element | AP | Power | Effect | Role |
|-------|---------|----|-------|--------|------|
| Slash | Physical | 0 | 20 | None | Free, reliable |
| Ice Lance | Ice | 1 | 25 | Stun | Delay enemy |
| Earth Wall | Earth | 2 | 22 | Shield (15) | Defensive |
| Fire Bolt | Fire | 2 | 30 | Burn | Damage-over-time |
| Lightning Strike | Lightning | 3 | 40 | None | Burst damage |

### UI layout

- Row 1: Attack (-330), Fire Skill (-110), Guard (110), End Turn (330)
- Row 2: Ice Lance (-330), Lightning Strike (-110), **Earth Wall (110)**

### Auto-battle AI updated

Added Earth Wall to the AI decision tree:
- Priority 5 (between Ice Lance and Fire Bolt): use if shield inactive and HP < 60%
- `GetWeaknessSkill()` checks Earth element — Stage 3 (Golem Depths) weakness hit

### Shield UI

- New `playerShieldText` TMP_Text field in BattleUI
- Set via `SetPlayerShieldText(int amount)` — shows "Shield: 15" when active, clears otherwise
- Battle message: "Shield active! (15 damage absorbed next hit)."
- Impact text: " | Shield 15 applied" on Earth Wall use

### Files changed

- `Assets/Scripts/Data/BattleBalanceConfig.cs` — earth skill config
- `Assets/Scripts/Battle/BattleManager.cs` — earth skill, shield logic, auto-battle update
- `Assets/Scripts/Battle/BattleUI.cs` — earth button, shield text, all signature updates
- `Assets/Editor/BattleSceneAutoBuilder.cs` — Earth Wall button, validation
