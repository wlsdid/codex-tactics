# 2026-06-05 Compact Battle HUD Chips

## Goal

The battle screen still had reviewer/demo text sitting over the center field after the reference-driven UI pass. This batch reduces that prototype feel by turning the remaining route/capture/impact hints into compact HUD chips.

## Changes

- Moved the reviewer path from a long center overlay into a smaller chip near the battle-center header.
- Shortened the route copy to `PATH  HERO / FIRE / GUARD / RESULT / RETRY`.
- Shortened the capture prompt to `SHOT 1/5  CLICK HERO` and reduced its panel size.
- Made the impact label smaller and less dominant so the battlefield art has more room.
- Updated `Validate Battle Test Scene` thresholds so the compact chips are checked automatically.
- Regenerated `BattleScene.unity` from the scene builder.

## Verification

- `BattleSceneAutoBuilder.CreateBattleTestScene`: PASS.
- `BattleSceneAutoBuilder.ValidateBattleTestScene`: PASS / `RESULT: PASS`.
- `BattleAutoTestRunner.RunBattleLogicAutoTest`: PASS / `RESULT: PASS`.
- `git diff --check`: PASS after Unity YAML whitespace cleanup.

## Next

Refresh battle captures/contact sheet and visually QA whether the new chips stay readable at README size without bringing back center-field clutter.
