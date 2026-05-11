# Devlog - 2026-05-11 Resource Percent Labels

## What I changed

- Improved the existing HP/AP resource UI without adding another result metric.
- Player HP, Player AP, and Enemy HP labels now show both exact values and percentages.
- Examples:
  - `Hero HP: 100/100 (100%)`
  - `AP: 1/3 (33%)`
  - `Slime HP: 40/80 (50%)`

## Why this helps the portfolio

Percent labels make the battle state easier to read in screenshots and GIFs. A viewer can quickly understand how much HP/AP remains even when the bar length is hard to judge.

## Files touched

- `Assets/Scripts/Battle/BattleManager.cs`
- `Assets/Editor/BattleSceneAutoBuilder.cs`
- `Assets/Editor/BattleAutoTestRunner.cs`
- `README.md`
- `Docs/ManualValidationAndCaptureGuide.md`
- `Docs/ManualUnityValidationChecklist.md`
- `Docs/09_Next_Autonomous_Tasks.md`

## Manual test focus

1. Start battle and confirm all resource labels show `(100%)`.
2. Use `Fire Skill` and confirm AP becomes `1/3 (33%)` and Slime HP becomes `40/80 (50%)`.
3. Use `Guard` and confirm Hero HP becomes `93/100 (93%)` after the first guarded hit.
4. Finish or retry the battle and confirm resource labels reset cleanly.
