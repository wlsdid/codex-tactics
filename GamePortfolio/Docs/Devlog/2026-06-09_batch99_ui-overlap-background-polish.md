# Batch 99 — UI Overlap Cleanup & Background UI Polish

## Goal
Clean up overlapping battle HUD elements and make the background UI read more like a polished tactical RPG presentation layer instead of stacked debug/reference widgets.

## Changes
- Removed the duplicate bottom-right `BATTLE START` reference CTA from the runtime battle HUD so it no longer competes with Retry/Continue/action commands.
- Replaced duplicate top-right reference labels (`AUTO`, `x2`, `II`) with non-text decorative accents, leaving only the real runtime buttons visible.
- Shortened the capture rehearsal chip runtime copy from long instructional strings to compact `SHOT n/5` labels.
- Reduced and repositioned the demo route/capture chips so they sit as faint reviewer evidence rather than covering the center field.
- Added a darker battlefield color-grade overlay and softened divider/floor alpha so the forest/tactical grid reads as a unified professional backdrop.
- Tightened the hidden skill/intent reference panels to avoid occupying excessive bottom-right space when command overlays appear.

## Portfolio Value
This pass addresses visible UI stacking in README/capture screenshots: duplicated controls are removed, temporary capture guidance is compact, and the battlefield background has a more commercial dark-fantasy tone while preserving the automated validation path.

## Verification
- C# brace balance check: PASS.
- Unity `BattleSceneAutoBuilder.CreateBattleTestScene`: PASS.
- Unity `BattleSceneAutoBuilder.ValidateBattleTestScene`: PASS (`RESULT: PASS`).
- Unity `BattleAutoTestRunner.RunBattleLogicAutoTest`: PASS (`RESULT: PASS`).
- Unity `CaptureScreenshots.Run`: PASS (`Build succeeded`).
- Standalone `CaptureRunner.exe -capture -force-d3d11`: PASS; all 7 capture PNGs are 1920x1080 and non-blank.
- README GIF/runtime storyboard/contact sheet rebuild: PASS.
- Contact-sheet visual QA: PASS — no blank frames; duplicate bottom-right CTA is removed and the battlefield background reads darker/more professional. Remaining risk: right-side skill/intention information is still intentionally dense.
- `git diff --check`: PASS.
