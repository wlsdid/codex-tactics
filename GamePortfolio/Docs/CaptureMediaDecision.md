# Capture Media Decision — Batch 85

Date: 2026-05-30

## Goal

Choose the safest portfolio media path after validating the current standalone capture runner and existing GIF builders.

## What was verified this batch

The existing standalone Windows capture runner was run from WSL with explicit output routing back to the project capture folder:

```bash
cd /mnt/c/Users/jywls/Desktop/game_portfolio/GamePortfolio
"/mnt/c/Users/jywls/Desktop/game_portfolio/GamePortfolio/Builds/CaptureBuild/CaptureRunner.exe" \
  -screen-width 1920 \
  -screen-height 1080 \
  -capture \
  -captureOutputDir "C:/Users/jywls/Desktop/game_portfolio/GamePortfolio/Docs/Captures" \
  -logFile "C:/Users/jywls/Desktop/game_portfolio/GamePortfolio/capture_runner_batch85.log"
```

Result: PASS. The runner refreshed the deterministic PNG sequence in `Docs/Captures/` without creating large raw video files.

Verified capture PNGs:

| File | Resolution target | Purpose |
| --- | --- | --- |
| `00_title_scene.png` | 1920x1080 | Title screen / first impression |
| `00_stage_select_scene.png` | 1920x1080 | Stage selection and modifier preview |
| `01_battle_start.png` | 1920x1080 | Battle HUD start state |
| `02_fire_skill_burn.png` | 1920x1080 | Fire/Burn feedback |
| `03_guard_status.png` | 1920x1080 | Guard feedback |
| `04_result_summary_rank.png` | 1920x1080 | Result summary, rank, reward |
| `05_retry_reset.png` | 1920x1080 | Retry reset state |

## Media options compared

| Option | Current status | Best use | Pros | Limits / risk | Decision |
| --- | --- | --- | --- | --- | --- |
| README gameplay GIF: `codex_tactics_battle_loop.gif` | Verified existing asset | Main README embed | Small, reviewer-friendly, directly shows the vertical slice flow | 7 key frames only; not true real-time motion | Keep as primary README media |
| Runtime-motion storyboard GIF: `codex_tactics_runtime_motion_storyboard.gif` | Verified existing asset | Supplemental motion/VFX evidence | 29 frames with pan/zoom beats; communicates impact, guard, result, retry without raw video | Built from still captures; should be labelled as storyboard/fallback | Keep as secondary evidence |
| True runtime MP4/GIF from live gameplay | Not generated this batch | Future portfolio/showcase video | Best proof of animation timing, camera feel, VFX, and interaction pacing | Requires Windows recorder/OBS/Game Bar or a new in-engine video recorder; raw files can become large and noisy for git | Do not commit until recorded, trimmed, compressed, and size-checked |

## Current portfolio decision

Use the README gameplay GIF as the primary embedded asset and the runtime-motion storyboard as supporting evidence. Do not add a raw MP4 to git yet. A true runtime MP4/GIF is useful only after it passes the acceptance criteria below.

## Future true MP4/GIF acceptance criteria

Before a true runtime clip replaces or joins the README GIF, it should meet all of these:

1. Length: 8-15 seconds.
2. Resolution: 1280x720 or 1920x1080 source; README GIF export may be 960x540.
3. Content: show Title or Stage Select briefly, selected-ally command UI, Fire/Burn or Guard feedback, and result/retry evidence.
4. File size: keep committed GIF under 5 MB if possible; keep MP4 out of git unless intentionally small and reviewed.
5. Readability: no black/blank frames, no obvious text overlap, no missing-glyph boxes.
6. Reproducibility: document the exact recorder and conversion commands.
7. Repository hygiene: do not commit raw OBS/Game Bar captures; commit only compressed showcase media and docs.

## Recommended future command path

If Windows Game Bar or OBS records a short source clip outside git, place it temporarily outside the repository or under an ignored folder, then convert a small GIF with ffmpeg:

```bash
# Example only; replace SOURCE.mp4 with the recorded clip path.
ffmpeg -y -i SOURCE.mp4 \
  -vf "fps=12,scale=960:-1:flags=lanczos,split[s0][s1];[s0]palettegen=max_colors=96[p];[s1][p]paletteuse=dither=bayer" \
  Docs/Captures/codex_tactics_runtime_clip.gif
```

Validation command:

```bash
python3 - <<'PY'
from pathlib import Path
from PIL import Image
p = Path('Docs/Captures/codex_tactics_runtime_clip.gif')
im = Image.open(p)
print(p, im.size, getattr(im, 'n_frames', 1), p.stat().st_size)
PY
```

Only embed `codex_tactics_runtime_clip.gif` after the clip passes the acceptance criteria.
