# Devlog — 2026-05-30 — Capture Media Decision

## Goal

Complete Batch 85 by checking whether the existing runtime capture path can safely support portfolio media without adding large unreviewed binaries.

## What changed

- Reran the existing standalone `CaptureRunner.exe` from WSL with `-captureOutputDir` set to the project `Docs/Captures` folder.
- Confirmed the current runner refreshes the deterministic PNG capture sequence, but does not record true MP4/GIF video by itself.
- Added `Docs/CaptureMediaDecision.md` to compare:
  - primary README gameplay GIF,
  - runtime-motion storyboard GIF,
  - future true runtime MP4/GIF.
- Updated capture docs, the manual validation guide, the README, the portfolio draft, and the autonomous task log with the current decision.

## Decision

Keep `Docs/Captures/codex_tactics_battle_loop.gif` as the primary README media. Keep `Docs/Captures/codex_tactics_runtime_motion_storyboard.gif` as secondary motion/VFX evidence. Do not commit a raw MP4 yet.

A true runtime clip should be recorded later through Windows Game Bar/OBS or a future in-engine recorder, then trimmed, converted, size-checked, and visually inspected before being added to the repository.

## Verification

```text
CaptureRunner.exe -capture -captureOutputDir Docs/Captures: PASS
Capture PNG validation: PASS — 7 expected PNGs, 1920x1080, non-trivial file sizes
Existing GIF validation: PASS — README GIF 960x540 / 7 frames / 482,572 bytes
Existing GIF validation: PASS — runtime storyboard GIF 960x540 / 29 frames / 2,471,988 bytes
ffmpeg availability: PASS — /usr/bin/ffmpeg
Markdown link/file existence check: PASS
python3 -m py_compile Docs/Captures/build_readme_gif.py Docs/Captures/build_runtime_motion_storyboard.py: PASS
git diff --check: PASS
Unity compile: not rerun by scope — docs/media-decision batch only, no C# or scene changes
```

## Next

Record a short true runtime clip with Windows Game Bar/OBS, convert it to a small GIF with ffmpeg, and compare it against the accepted README GIF/storyboard evidence before deciding whether to embed it.
