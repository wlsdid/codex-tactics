# Batch 100 — Right Rail Clearance Polish

## Goal

Keep the Brown Dust 2-inspired tactical HUD proof points, but reduce the visual crowding around the enemy side of the battlefield in README/contact-sheet scale captures.

## Changes

- Shifted the right-side reference skill cards farther to the right and reduced their width/icon/text sizes.
- Slimmed the contextual `Reference Skill Detail Panel` and `Enemy Intent Card Panel` so they no longer push into the central battlefield.
- Updated Battle Scene validator thresholds to match the slimmer panel dimensions.
- Regenerated `BattleScene.unity`, the standalone capture runner, battle PNGs, README GIF, runtime storyboard GIF, and contact sheet.

## Verification

- C# delimiter static check: PASS.
- `BattleSceneAutoBuilder.CreateBattleTestScene`: PASS.
- `BattleSceneAutoBuilder.ValidateBattleTestScene`: PASS.
- `BattleAutoTestRunner.RunBattleLogicAutoTest`: PASS.
- `CaptureScreenshots.Run`: PASS (`Build succeeded`).
- Standalone `CaptureRunner.exe -capture -force-d3d11`: PASS; all battle PNGs regenerated at 1920x1080.
- `build_readme_gif.py`: PASS; `codex_tactics_battle_loop.gif` regenerated at 960x540.
- `build_runtime_motion_storyboard.py`: PASS; `codex_tactics_runtime_motion_storyboard.gif` regenerated at 960x540.
- `capture_contact_sheet.png`: PASS; regenerated and visually inspected.

## Visual QA

The enemy character and center battlefield remain readable in the updated contact sheet. The right rail still communicates Revenge / Blessed Shield / Holy Light and enemy intent, but with less intrusion into the combat stage. No black/blank capture frames were detected.
