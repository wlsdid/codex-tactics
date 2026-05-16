# Devlog — 2026-05-16: Lightning Strike (4th skill)

## What I did

Added **Lightning Strike** as the 4th player skill.

### Lightning Strike stats
- **Element:** Lightning
- **AP cost:** 3 (highest cost — full AP bar)
- **Power:** 40 (highest base damage)
- **Status:** None (pure damage)
- **Config:** configurable via `BattleBalanceConfig`

### Complete skill set
| Skill | Element | AP | Power | Status | Role |
|-------|---------|----|-------|--------|------|
| Slash | Physical | 0 | 20 | None | Free, reliable |
| Ice Lance | Ice | 1 | 25 | Stun | Delay enemy |
| Fire Bolt | Fire | 2 | 30 | Burn | Damage-over-time |
| Lightning Strike | Lightning | 3 | 40 | None | Burst damage |

### UI layout
- Row 1: Attack, Fire Skill, Guard, End Turn
- Row 2: Ice Lance, Lightning Strike

### Tests added
- Lightning Strike AP cost (3 AP → empty bar)
- Neutral damage (40 to Fire Slime)
- Damage tracking
- Skill usage counter
- Skill help text includes Lightning Strike

### Old doc cleanup
- Removed stale `Batch2_CombatFeedback_Instructions.md`