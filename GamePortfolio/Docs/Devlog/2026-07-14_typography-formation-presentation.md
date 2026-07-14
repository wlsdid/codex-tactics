# Typography & 3v3 Formation Presentation (2026-07-14)

## Goal

Polish Tactical Requiem presentation without changing the existing 1v1 battle rules: make the central encounter read as two three-unit formations and make generated UI use the project-owned Korean-capable font.

## Changes

- Added the dynamic `NotoSansKR-Regular SDF` TMP asset generated from `Assets/Fonts/NotoSansKR-Regular.otf`, with the project TMP default asset retained as fallback.
- Applied that TMP asset whenever the generated Title, Stage Select, and Battle scenes are rebuilt; coverage setup includes Korean, English, and digits.
- Kept Hero and the live enemy as the only battle logic anchors. Added two visual supports per side to the central formation, with larger readable silhouettes and staggered idle motion.
- Routed selection, attack, hit, target-ring, and status feedback to the central Hero/enemy standees while preserving the existing portrait/UI feedback and all combat behavior.
- Removed the overlapping Stage Select strategic-chip strip; the lower stage detail panel and primary buttons now remain unobscured.
- Regenerated Title, Stage Select, and Battle scenes, refreshed the standalone CaptureRunner, and cleaned trailing whitespace in generated Unity YAML.

## Verification

- `GameFlowSceneAutoBuilder.ValidateGameFlowScenes`: `RESULT: PASS`.
- `BattleSceneAutoBuilder.ValidateBattleTestScene`: `RESULT: PASS`.
- `BattleAutoTestRunner.RunBattleLogicAutoTest`: `RESULT: PASS`.
- `CaptureScreenshots.Run`: Windows standalone build succeeded.
- `CaptureRunner.exe -capture -force-d3d11`: PASS — 8 nonblank 1920×1080 PNGs.
- Visual QA of Title, Stage Select, and battle burn captures: no missing-glyph boxes observed; the central 3v3 formation is legible and the removed Stage Select strip no longer overlaps its detail panel.
- `git diff --check`: PASS.

## Next direction

The current 3v3 formation is presentation-only by design. Any later art pass should replace the remaining sprite-embedded platform slabs only with authored transparent sprite variants; do not add units, mechanics, or new rectangular HUD overlays.
