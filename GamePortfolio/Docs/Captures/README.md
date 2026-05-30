# Captures

Unity screenshots, README gameplay GIF, runtime-motion storyboard GIF, and contact sheets for the portfolio README. Batch 82 adds a reproducible Pillow-based GIF builder that turns the current capture PNGs into `codex_tactics_battle_loop.gif` without reopening Unity. Batch 84 adds a motion-focused storyboard builder for action/VFX review when true runtime video capture is not available.

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
build_readme_gif.py
build_runtime_motion_storyboard.py
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
```

`build_runtime_motion_storyboard.py` validates the action-heavy source frames, writes a 960x540 GIF, writes a preview sheet, checks frame count, and rejects output that is suspiciously tiny or too large for README use.
