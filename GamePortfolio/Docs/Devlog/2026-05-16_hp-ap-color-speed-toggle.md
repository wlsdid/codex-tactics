# Devlog — 2026-05-16: HP/AP Bar Colors + Speed Toggle

## What I did

Added dynamic color coding to HP and AP resource bars, plus a 2x speed toggle for faster battles.

### Color-coded HP bars

| Range | Color | Visual |
|-------|-------|--------|
| >60% | Green (#37B861) | Healthy |
| 30-60% | Yellow (#D9B82E) | Warning |
| <30% | Red (#D1383D) | Danger |

This applies to both player and enemy HP bars via `SetSliderColorByRatio()`.

### Color-coded AP bars

| Range | Color | Visual |
|-------|-------|--------|
| >66% | Blue (#428FFF) | Plentiful |
| 33-66% | Cyan (#42DCBF) | Moderate |
| <33% | Orange (#EB8F2E) | Low |

### Speed toggle (1x / 2x)

- New `speedToggleButton` in the command bar
- Toggles between `speedState=1` (1x) and `speedState=2` (2x)
- `WaitForBattleTick(seconds)` helper divides all animation delays by the speed multiplier
- All coroutine waits respect the toggle — enemy turn, burn tick, stun display, attack resolution

### Files changed

- `Assets/Scripts/Battle/BattleUI.cs` — `SetSliderColorByRatio()`, `UpdateSpeedLabel()`, `WaitForBattleTick()`
- `Assets/Scripts/Battle/BattleManager.cs` — `OnClickSpeedToggle()`, `speedState` field
- `Assets/Editor/BattleSceneAutoBuilder.cs` — Speed toggle button setup
- `Assets/Editor/BattleAutoTestRunner.cs` — Tests for speed toggle
