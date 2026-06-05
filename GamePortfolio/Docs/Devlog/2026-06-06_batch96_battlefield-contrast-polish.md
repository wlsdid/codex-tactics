# 2026-06-06 Batch 96 — Battlefield Contrast Polish

## Goal
Improve only the central BattleScene battlefield readability for README-sized thumbnails without returning to bright debug colors or adding new combat features.

## Changes
- Darkened and widened the central stage floor plate so the character silhouettes separate from the forest background.
- Slightly increased tactical tile size/alpha while keeping the cyan-green grid restrained and non-debug-like.
- Tuned the distant forest, moonlight, fog, and horizon alpha values so the stage keeps depth but does not wash out the standees.
- Increased hero/enemy base ring, grounding shadow, aura, blade, and crown readability with small alpha/size changes only.
- Extended `Validate Battle Test Scene` expectations to check restrained contrast alpha ranges for tiles, rings, shadows, and subtle auras.

## Verification Results
- Start/end git status checked; root `screenshots/` stayed untracked and was not committed.
- C# brace balance check: PASS.
- Unity `BattleSceneAutoBuilder.CreateBattleTestScene`: PASS.
- Unity `BattleSceneAutoBuilder.ValidateBattleTestScene`: PASS (`RESULT: PASS`).
- Unity `BattleAutoTestRunner.RunBattleLogicAutoTest`: PASS (`RESULT: PASS`).
- Unity `CaptureScreenshots.Run`: PASS (`Build succeeded`).
- Standalone `CaptureRunner.exe` refresh: PASS.
- Capture PNG QA: PASS — 7 PNGs exist, are fresh, non-blank, and 1920x1080.
- README GIF rebuild: PASS — 960x540, 7 frames.
- Runtime storyboard rebuild: PASS — 960x540, 29 frames.
- Contact-sheet rebuild/visual QA: PASS — central character silhouettes, base rings, and tactical tiles are clearer at thumbnail size while remaining restrained rather than debug-bright.
- `git diff --check`: PASS.

## Notes
This pass intentionally avoids new UI/features and limits the change to central battlefield contrast/readability controls plus validation/docs evidence.
