# Devlog — 2026-05-16: Stage 4 (Lightning element)

## What I did

Added **Stage 4: Storm Peaks** with Lightning element enemies.

### Stage 4 encounters
| Encounter | Enemy | HP | Element | Normal | Strong | Interval |
|-----------|-------|----|---------|--------|--------|----------|
| Stage 4-1 | Storm Hawk | 140 | Lightning | 22 swoop | 40 Thunder Dive | Every 3rd |
| Stage 4-2 | Thunder Phoenix | 250 | Lightning | 28 call lightning | 55 Skyfall | Every 3rd |

### Updated systems
- **`StageData.cs`** — Added `CreateStage4Normal()` and `CreateStage4Boss()`, updated `GetEncountersForStage()` for index 3
- **`ProgressState.cs`** — `TotalStages` from 3 → 4
- **`StageSelectController.cs`** — Added "Storm Peaks" name, description, and stage label
- **Tests** — Added 3 Stage 4 data validation checks, updated TotalStages expectation

### Full stage lineup
| Stage | Index | Element | Theme | Normal HP | Boss HP |
|-------|-------|---------|-------|-----------|---------|
| Stage 1 | 0 | Fire | Slime Scout Route | 80 | 140 |
| Stage 2 | 1 | Nature | Wolf Ambush | 100 | 180 |
| Stage 3 | 2 | Earth | Golem Depths | 120 | 220 |
| Stage 4 | 3 | Lightning | Storm Peaks | 140 | 250 |

The difficulty curve is consistent: each stage adds ~20 HP to normal enemies and ~40 HP to bosses.