# Captures

Unity screenshots, README gameplay GIF, runtime-motion storyboard GIF, runtime MP4 conversion helper, and contact sheets for the portfolio README. Batch 82 adds a reproducible Pillow-based GIF builder that turns the current capture PNGs into `codex_tactics_battle_loop.gif` without reopening Unity. Batch 84 adds a motion-focused storyboard builder for action/VFX review when true runtime video capture is not available. Batch 85 validates the standalone capture runner path and records the current media decision in `Docs/CaptureMediaDecision.md`. Batch 86 adds an ffmpeg-based conversion/validation workflow for a user-recorded runtime MP4. Batch 90 refreshes the generated PNG/GIF set after visual polish and fixes victory result summary readability. Batch 91 refreshes the battle captures again after reducing HUD density for README-size readability.

## Current captured files

```text
00_title_scene.png
00_stage_select_scene.png
01_battle_start.png
02_fire_skill_burn.png
03_guard_status.png
04_result_summary_rank.png
05_retry_reset.png
capture_contact_sheet.png
codex_tactics_battle_loop.gif
codex_tactics_battle_loop_preview.png
codex_tactics_runtime_motion_storyboard.gif
codex_tactics_runtime_motion_storyboard_preview.png
convert_runtime_clip.py
build_readme_gif.py
build_runtime_motion_storyboard.py
../CaptureMediaDecision.md
```

## README snippets

```md
![Codex Tactics gameplay GIF](Docs/Captures/codex_tactics_battle_loop.gif)
```

```md
![Codex Tactics capture sheet](Docs/Captures/capture_contact_sheet.png)
```

```md
![Stage Select](Docs/Captures/00_stage_select_scene.png)
```

```md
![Battle result summary](Docs/Captures/04_result_summary_rank.png)
```

```md
![Codex Tactics runtime motion storyboard](Docs/Captures/codex_tactics_runtime_motion_storyboard.gif)
```

Do not add large raw video files here. Keep short converted GIFs or compressed images only.

## Rebuild commands

From the Unity project root:

```bash
python3 Docs/Captures/build_readme_gif.py
python3 Docs/Captures/build_runtime_motion_storyboard.py
python3 Docs/Captures/convert_runtime_clip.py --help
```

`build_runtime_motion_storyboard.py` validates the action-heavy source frames, writes a 960x540 GIF, writes a preview sheet, checks frame count, and rejects output that is suspiciously tiny or too large for README use.

## Batch 86 runtime MP4 conversion workflow

Hermes/WSL cannot directly operate Windows Game Bar or OBS. After recording a short runtime MP4 on Windows, keep the raw MP4 outside git and convert it from WSL:

```bash
cd /mnt/c/Users/jywls/Desktop/game_portfolio/GamePortfolio
python3 Docs/Captures/convert_runtime_clip.py "/mnt/c/Users/jywls/Videos/Captures/YOUR_RUNTIME_CLIP.mp4"
```

Default conversion settings are portfolio-safe for an 8-15 second README clip:

- trim: start `0s`, duration `12s`
- output width: `960px` with aspect ratio preserved
- frame rate: `12 fps`
- palette: `96` colors
- size cap: `5 MB`
- outputs: `Docs/Captures/codex_tactics_runtime_clip.gif` and `Docs/Captures/codex_tactics_runtime_clip_preview.png`

Useful override example:

```bash
python3 Docs/Captures/convert_runtime_clip.py "/mnt/c/Users/jywls/Videos/Captures/YOUR_RUNTIME_CLIP.mp4" \
  --start 2 \
  --duration 10 \
  --fps 12 \
  --width 960
```

The script checks for `ffmpeg`/`ffprobe`, validates the input MP4, creates a palette-based GIF, creates a preview/contact sheet, validates GIF width/frame count/file size, and prints exact source/output metrics. Commit only the validated GIF/preview if they are portfolio-ready; do not commit raw recorder MP4 files.

## Batch 85 media decision

`../CaptureMediaDecision.md` records the current capture choice after rerunning the standalone capture runner: keep `codex_tactics_battle_loop.gif` as the primary README media, keep `codex_tactics_runtime_motion_storyboard.gif` as supplemental motion evidence, and postpone a true MP4/GIF until a short Windows Game Bar/OBS clip can be recorded, trimmed, compressed, and size-checked without committing raw video.


## Batch 90 refresh notes

The current PNGs, contact sheet, README gameplay GIF, and runtime-motion storyboard were regenerated from a freshly rebuilt standalone `CaptureRunner.exe`. Visual QA specifically checked that `04_result_summary_rank.png` reads as a Victory result instead of a red defeat-like overlay, and that the summary no longer uses oversized centered text that spills over the combatants.


## Batch 91 HUD density notes

The battle PNGs and GIFs were regenerated after a HUD density pass. The capture keeps the tactical proof points visible — HP/AP, enemy intent, Fire/Burn, Guard, Result, Retry — while reducing side roster rows and shortening route/capture prompts so the center battlefield and characters are clearer in README-sized images.
