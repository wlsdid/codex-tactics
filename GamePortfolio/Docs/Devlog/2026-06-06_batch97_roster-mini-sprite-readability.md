# 2026-06-06 Batch 97 — Roster Mini-Sprite Readability Pass

## Goal
Improve only the left party roster and right enemy roster/card mini-sprite crop/edge readability for README-sized battle captures, while keeping the existing character sprites and battle loop unchanged.

## Changes
- Added darker portrait-chip backing, sprite shadows, crop-frame panels, and thin edge accents around party roster mini sprites.
- Slightly retuned party mini-sprite size/offset so the existing Paladin, Cleric, Archmage, Bard, and Ranger sprites separate from the dark roster card backgrounds in small captures.
- Added matching shadow/crop-frame/edge-accent treatment to enemy roster mini sprites so the generated right-side enemy card units keep silhouette contrast.
- Extended `Validate Battle Test Scene` expectations to require the new mini-sprite shadow/edge readability checks for both party and enemy roster entries.
- Regenerated `BattleScene.unity`, refreshed capture PNG/GIF/contact-sheet evidence, and visually checked the contact sheet for non-blank, non-stale roster readability.

## Verification Results
- Start/end git status checked; root `screenshots/` stayed untracked and was not committed.
- C# brace balance check: PASS.
- Unity `BattleSceneAutoBuilder.CreateBattleTestScene`: PASS.
- Unity `BattleSceneAutoBuilder.ValidateBattleTestScene`: PASS (`RESULT: PASS`).
- Unity `BattleAutoTestRunner.RunBattleLogicAutoTest`: PASS (`RESULT: PASS`).
- Unity `CaptureScreenshots.Run`: PASS (`Build succeeded`).
- Standalone `CaptureRunner.exe` refresh: PASS using `-force-d3d11` and a Windows capture output path.
- Capture PNG QA: PASS — 7 PNGs exist, are fresh, non-blank, and 1920x1080.
- README GIF rebuild: PASS — 960x540, 7 frames.
- Runtime storyboard rebuild: PASS — 960x540, 29 frames.
- Contact-sheet rebuild/visual QA: PASS — party mini sprites and the right-side enemy mini/card silhouette remain visible at thumbnail size, with no blank or stale capture issue.
- `git diff --check`: PASS.

## Notes
This pass intentionally avoids new mechanics or large UI structure changes. It is limited to generated roster mini-sprite crop/edge/shadow readability plus validator and documentation evidence.
