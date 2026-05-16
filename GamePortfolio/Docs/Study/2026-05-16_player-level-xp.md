# Player Level/XP System: Progression Design (플레이어 레벨/XP 시스템: 성장 설계)

## Overview

The level/XP system gives the player a sense of progression across stages. Unlike traditional RPGs where XP is earned per kill, this system awards XP only when all encounters in a stage are cleared — rewarding completion, not grinding.

## Core mechanics

### XP gain

When the player defeats the final encounter of a stage (the boss encounter), they earn XP:

```
xpGained = 50 + (stageIndex + 1) × 30
```

| Stage | XP | Formula |
|-------|----|---------|
| Stage 1 (Fire) | 80 | 50 + 1×30 |
| Stage 2 (Nature) | 110 | 50 + 2×30 |
| Stage 3 (Earth) | 140 | 50 + 3×30 |
| Stage 4 (Lightning) | 170 | 50 + 4×30 |

### Level-up thresholds

```
xpToNextLevel starts at 100
On level-up: xpToNextLevel = Mathf.RoundToInt(xpToNextLevel × 1.5f)
```

| Level | XP to reach next | Total XP from start |
|-------|------------------|---------------------|
| 1 | 100 | 0 |
| 2 | 150 | 100 |
| 3 | 225 | 250 |
| 4 | 338 | 475 |
| 5 | 507 | 813 |

### Rewards per level

| Reward | Value |
|--------|-------|
| Max HP increase | +20 |
| Message | "Level Up! Now Level {N}. Max HP increased to {maxHp}." |

## Design rationale

### Why stage-clear only (not per-encounter)?
This avoids the "grind for XP" problem. The player is never forced to replay a stage just to level up. XP is a reward for *completing* a stage, encouraging forward progress. It also keeps the design simple — no need for per-encounter XP calculations.

### Why increasing XP curve?
Higher stages are harder, so they should give more XP. The formula `50 + (stageIndex + 1) × 30` means Stage 4 gives 170 XP vs Stage 1's 80 XP — a natural difficulty-aligned progression.

### Why 1.5x XP threshold growth?
Each level takes 50% more XP than the previous one. This creates a gentle curve that rewards the first few level-ups quickly (levels 1-2 after two stage clears) but slows down as the player advances. It's a soft cap that avoids infinite power scaling.

### Why +20 max HP per level?
+20 HP (20% of base 100) is meaningful but not game-breaking. After 5 levels, the player has 200 max HP — double the starting value. This allows tackling harder stages without making early stages trivial. The HP increase is noticed but doesn't break the balance curve.

### Why no AP or damage upgrades?
Keeping AP and damage fixed preserves skill balance. The progression system enhances *survivability*, not *offensive power*, which means the player still needs to use skills strategically. Leveling up makes mistakes more forgiving, not the game easier.

## Implementation notes

- XP and level reset between runs (no persistent save system yet)
- `rewardedStageIndexes` set prevents double-XP from replaying the same stage
- Level display appears in the run status area via `SetLevelText()`

## Future potential

- **Persistent progression** — save XP/level across sessions
- **Perk system** — choose bonuses on level-up (more AP, skill upgrade, etc.)
- **Hard mode** — start at level 1 with adjusted enemy scaling
- **XP from defeated enemies** — small XP per kill for multi-encounter stages

## Portfolio relevance

1. **Simple, fair progression** — rewards completion, not grinding
2. **Visible feedback** — level display + level-up message
3. **Balanced scaling** — 1.5x threshold, +20 HP, no power creep
4. **Designer-friendly** — XP formula can be tuned without touching code
