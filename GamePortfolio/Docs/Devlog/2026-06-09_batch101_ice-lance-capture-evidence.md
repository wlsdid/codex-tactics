# Batch 101 — Ice Lance Capture Evidence

## Goal

Add a second skill-feedback proof point to the portfolio capture loop so the README GIF shows more than Fire/Burn before Guard/Result.

## Changes

- Added a deterministic portfolio capture progress setup inside `ScreenshotCaptureJob` so standalone capture runs temporarily unlock Fire/Ice/Earth without depending on the user's local save file.
- Inserted a new `03_ice_lance_stun.png` capture between Fire Skill and Guard.
- Updated README GIF and runtime storyboard builders to include the Ice Lance/Stun frame.
- Added `build_capture_contact_sheet.py` so the contact sheet can be regenerated reproducibly instead of relying on ad-hoc Python snippets.
- Updated README and showcase checklist to mark Ice Lance/Stun GIF evidence complete.

## Verification

- C# delimiter static check: PASS.
- Python compile checks for capture scripts: PASS.
- `BattleAutoTestRunner.RunBattleLogicAutoTest`: PASS.
- `CaptureScreenshots.Run`: PASS (`Build succeeded`).
- Standalone `CaptureRunner.exe -capture -force-d3d11`: PASS.
- `03_ice_lance_stun.png`: PASS, 1920x1080, non-blank.
- `build_readme_gif.py`: PASS, 8-frame 960x540 GIF.
- `build_runtime_motion_storyboard.py`: PASS, 34-frame 960x540 GIF.
- `build_capture_contact_sheet.py`: PASS, 8-capture contact sheet.
- Contact sheet visual QA: PASS — all 8 frames are readable; no black/white capture failures.
