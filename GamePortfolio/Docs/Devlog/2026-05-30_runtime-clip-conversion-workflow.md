# Devlog — 2026-05-30 — Runtime Clip Conversion Workflow

## Batch 86 goal

Create the next best verified asset for true runtime media capture: a reproducible ffmpeg-based workflow that converts a user-provided Windows Game Bar/OBS MP4 into a small portfolio GIF plus preview/contact sheet.

## What changed

- Added `Docs/Captures/convert_runtime_clip.py`.
- The script trims a runtime MP4, resizes to 960px wide by default, limits output to 12 fps, generates a palette-based GIF, and writes a preview/contact sheet.
- Default output paths:
  - `Docs/Captures/codex_tactics_runtime_clip.gif`
  - `Docs/Captures/codex_tactics_runtime_clip_preview.png`
- The script validates:
  - `ffmpeg` and `ffprobe` availability
  - input existence and `.mp4` extension
  - source video dimensions and duration
  - output GIF width, frame count, file size, and non-trivial preview output
- Updated capture docs and showcase notes so a future user-recorded MP4 has an exact command path.

## User command after recording

Hermes/WSL cannot press Windows Game Bar/OBS controls directly. After recording a short runtime MP4 on Windows, run this from WSL:

```bash
cd /mnt/c/Users/jywls/Desktop/game_portfolio/GamePortfolio
python3 Docs/Captures/convert_runtime_clip.py "/mnt/c/Users/jywls/Videos/Captures/YOUR_RUNTIME_CLIP.mp4"
```

For a longer recording, trim the best section:

```bash
python3 Docs/Captures/convert_runtime_clip.py "/mnt/c/Users/jywls/Videos/Captures/YOUR_RUNTIME_CLIP.mp4" \
  --start 2 \
  --duration 10
```

## Verification notes

No sample runtime MP4 existed in `Docs/Captures` or the project tree this batch, so the real project output GIF was not generated or committed. The script was verified through syntax, help text, missing-input behavior, ffmpeg/ffprobe availability, a temporary synthetic MP4 conversion smoke test outside the repo, documentation reference checks, and git whitespace checks. Unity compile was not needed because no C# or scene files changed.

## Next recommended task

Record one 8-15 second Windows runtime MP4, run `convert_runtime_clip.py`, inspect the generated preview/contact sheet, then decide whether `codex_tactics_runtime_clip.gif` should replace or supplement the current README gameplay GIF.
