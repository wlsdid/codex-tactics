# 2026-05-30 — Batch 82: README Gameplay GIF

## Goal

Create a README-ready gameplay GIF from the existing capture pipeline output, without importing external assets or requiring a large Unity scene edit.

## Changed

- Added `Docs/Captures/build_readme_gif.py`, a small Pillow-based builder that converts the current capture PNG sequence into a 960x540 animated GIF.
- Generated `Docs/Captures/codex_tactics_battle_loop.gif` for the README showcase section.
- Generated `Docs/Captures/codex_tactics_battle_loop_preview.png` so the GIF frames can be quickly inspected as a contact sheet.
- Updated README and capture docs with the GIF path and stable rebuild command.

## Verification

- `python3 Docs/Captures/build_readme_gif.py`: PASS.
- GIF validation: 960x540, 7 frames, non-trivial file size.
- Preview visual QA: PASS; Title, Stage Select, Battle Start, Fire Skill, Guard, Result, and Retry frames are visible.
- No external copyrighted assets were added.

## Notes

This is a compact README evidence GIF built from deterministic captures. A future improvement could record a true in-engine motion GIF or MP4 after the next real-time camera/VFX polish pass.
