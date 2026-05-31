# 2026-05-31 — Battle HUD Density Pass

## Goal

Improve README/contact-sheet readability by reducing Battle HUD clutter without removing the tactical RPG proof points reviewers need to see.

## Changes

- Reduced generated party roster from five side rows to three readable rows.
- Reduced generated enemy roster from five side rows to three readable rows.
- Shortened the capture rehearsal copy from verbose instructions to compact step labels.
- Tightened the route strip and top mission guide while preserving the `Hero -> Fire -> Guard -> Result -> Retry` proof.
- Regenerated `BattleScene.unity` through the scene builder.
- Refreshed battle PNG captures, contact sheet, README gameplay GIF, and runtime-motion storyboard GIF.

## Visual QA result

The battle frames now leave more negative space around the central characters and battlefield. The side panels still prove roster/party/enemy structure, but no longer dominate the thumbnail. Core portfolio evidence remains visible: HP/AP bars, enemy intent, Fire/Burn feedback, Guard state, Victory result, Retry reset, and command flow.

## Verification

- Static C# brace check: PASS.
- `git diff --check`: PASS.
- `BattleSceneAutoBuilder.CreateBattleTestScene`: PASS.
- `BattleSceneAutoBuilder.ValidateBattleTestScene`: PASS.
- `BattleAutoTestRunner.RunBattleLogicAutoTest`: PASS.
- `CaptureScreenshots.Run`: PASS.
- Standalone `CaptureRunner.exe` PNG refresh: PASS.
- `build_readme_gif.py`: PASS.
- `build_runtime_motion_storyboard.py`: PASS.
- Contact-sheet visual QA: PASS.

## Next recommended batch

If continuing visual polish, move back to Title/Stage Select illustrative art or prepare final portfolio submission notes. Further Battle HUD work should avoid adding more text unless it replaces existing text.
