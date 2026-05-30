# 2026-05-30 — Batch 84: Runtime Motion Storyboard

## Goal

Improve runtime motion capture readiness without taking on the risk of a large true video-capture implementation. The target was a small, reproducible fallback that turns existing standalone-runner screenshots into a motion-focused GIF and validates the result.

## Changed

- Added `Docs/Captures/build_runtime_motion_storyboard.py`.
- The script selects the capture frames most likely to show action readability: Battle Start, Fire/Burn, Guard, Result, and Retry.
- It adds small pan/zoom beats so the current capture set can communicate runtime flow better than a still contact sheet.
- It writes `Docs/Captures/codex_tactics_runtime_motion_storyboard.gif` and `Docs/Captures/codex_tactics_runtime_motion_storyboard_preview.png`.
- Updated `Docs/Captures/README.md` with rebuild commands and README embed snippets.
- Updated `Docs/ManualValidationAndCaptureGuide.md` with storyboard fallback steps and the standalone-runner capture path.
- Updated `Docs/09_Next_Autonomous_Tasks.md` with Batch 84 status and next capture recommendations.

## Verification

- `python3 Docs/Captures/build_runtime_motion_storyboard.py`: PASS.
- Motion storyboard validation: PASS — source PNGs exist, sources are at least 960x540, GIF output is 960x540 with 29 frames, preview sheet is 1200x135, and GIF file size is 2,471,988 bytes.
- Preview visual QA: PASS — the sheet is not blank/corrupt and shows Battle Start, Fire/Burn, Guard, Result, and Retry beats.
- `python3 -m py_compile Docs/Captures/build_readme_gif.py Docs/Captures/build_runtime_motion_storyboard.py`: PASS.
- `git diff --check`: PASS.
- Unity compile was not run because this batch changed only capture docs, Python tooling, and generated media.

## Notes

This is a verified fallback, not a substitute for true runtime video. The next capture-focused batch should record or convert a real standalone-runner motion clip, then compare it against this storyboard to decide which asset belongs in the README.
