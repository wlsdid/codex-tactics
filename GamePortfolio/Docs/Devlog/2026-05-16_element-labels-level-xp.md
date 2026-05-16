# Devlog — 2026-05-16: Element Labels + Player Level/XP

## What I did

Added element labels to enemy intent display and a player level/XP system that grants progression on stage clear.

### Element labels in enemy intent

Enemy intent now shows the element type of the incoming attack:

```
Before: Next Enemy: Normal Attack (15)
After:  Next Enemy: [Fire] Normal Attack (15)
       Next Enemy: [Lightning] Thunder Dive (40)
```

- Stored in `enemyElementLabel` field in BattleUI
- Set whenever an enemy's weakness element changes
- Physical/None elements show no prefix
- Helps players learn enemy elements and plan their skill usage

### Player Level/XP system

XP is awarded when all encounters in a stage are cleared (final encounter victory):

| Trigger | XP Gain |
|---------|---------|
| Stage 1 clear | 50 + (1 × 30) = 80 XP |
| Stage 2 clear | 50 + (2 × 30) = 110 XP |
| Stage 3 clear | 50 + (3 × 30) = 140 XP |
| Stage 4 clear | 50 + (4 × 30) = 170 XP |

Level-up formula:
- XP to next level: `xpToNextLevel` starts at 100, multiplies by 1.5× per level
- Max HP increase: +20 per level
- Level is displayed in the run status area as `Lv.1 | Run Status: ...`

### Level display

`SetLevelText()` prepends level info to the run status display:
```
Lv.1 | Run Status: Stage 1 In Progress
```

### Files changed

- `Assets/Scripts/Battle/BattleManager.cs` — `playerLevel`, `playerXp`, `xpToNextLevel`, level-up logic in `EndBattle()`
- `Assets/Scripts/Battle/BattleUI.cs` — `SetEnemyElementLabel()`, `SetLevelText()`, element label in intent
- `Assets/Editor/BattleAutoTestRunner.cs` — Level/XP tests
