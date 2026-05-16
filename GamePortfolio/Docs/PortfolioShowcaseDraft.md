# Portfolio Showcase Draft

> Updated: 2026-05-16 — covers 4 stages, 4 skills, element system, Stun, Break

## Game overview

Codex Tactics is a turn-based tactical RPG vertical slice built in Unity. It demonstrates a complete battle loop with stage progression, elemental mechanics, multiple player skills, and automated validation.

## Current features

### 4 stages with increasing difficulty
| Stage | Enemies | Type | Normal HP | Boss HP |
|-------|---------|------|-----------|---------|
| Slime Scout Route | Slime Scout, Slime King | Fire | 80 | 140 |
| Wolf Ambush | Wolf Scout, Alpha Wolf | Nature | 100 | 180 |
| Golem Depths | Golem Sentry, Ancient Golem | Earth | 120 | 220 |
| Storm Peaks | Storm Hawk, Thunder Phoenix | Lightning | 140 | 250 |

### 4 player skills
| Skill | AP | Element | Power | Effect |
|-------|----|---------|-------|--------|
| Slash | 0 | Physical | 20 | — |
| Ice Lance | 1 | Ice | 25 | Stun (enemy skips turn) |
| Fire Bolt | 2 | Fire | 30 | Burn (3/turn for 2 turns) |
| Lightning Strike | 3 | Lightning | 40 | — |

### Element weakness system
- Configurable multiplier (1.5x default) via `BattleBalanceConfig`
- Visible impact text (`Weakness x1.5`, `Neutral x1.0`, `Physical`)
- Effects Break gauge on weakness hits

### Break & Stun mechanics
- **Break gauge** (2 points): weakness hits reduce gauge; at 0 → BROKEN, next attack deals 1.5x
- **Stun**: enemy skips its entire attack turn — a pure delay tactic

### Battle result evaluation
- Rank (S/A/B/C) based on turns taken and damage received
- Pace label (Fast/Steady/Long/Defeated)
- Survival percentage
- Reward gold scaled to rank
- Configurable thresholds via `BattleBalanceConfig`

### Full game flow
- Title screen → Stage Select → Battle → Result → Title
- Stage progression: clear all encounters → next stage unlocks
- Retry / Continue / Stage Select navigation

## Technical highlights

### Config-driven design
All balance values live in `BattleBalanceConfig` (ScriptableObject). Designers can tune without code changes: damage, HP, AP costs, weakness multipliers, rank thresholds, rewards.

### Clean separation
- `BattleManager` — state machine and turn flow
- `BattleUI` — all rendering (extracted from BattleManager)
- `StageData` / `EnemyData` — encounter definitions
- `BattleResultEvaluator` / `BattleResultPresenter` — result logic
- `BattleBalanceConfig` — all magic numbers

### Automated testing
- Scene validation (checks all critical UI elements exist)
- Battle logic auto-test (230+ regression checks)
- Unity batchmode compile: PASS
- Test coverage: skills, damage, status effects, elements, progress, stage data

### Generated scenes
`BattleSceneAutoBuilder` creates the complete battle scene from code — no manual prefab editing needed. This ensures the scene stays in sync with code changes.

## How to play

1. Open `Assets/Scenes/BattleScene.unity` (or `StageSelectScene.unity`)
2. Press Play
3. Click card to select stage → Start Battle
4. Choose actions: Attack (free), Ice Lance (1 AP, Stun), Fire Bolt (2 AP, Burn), Guard, End Turn
5. Defeat all encounters to complete the stage

## Screenshots needed
- [ ] Battle scene with all UI panels visible
- [ ] Fire Bolt dealing weakness damage with impact text
- [ ] Stun message ("is STUNNED!")
- [ ] Break gauge at max / depleted / BROKEN
- [ ] Stage select with locked/unlocked cards
- [ ] Result summary (Victory S rank)
- [ ] Result summary (Defeat)
