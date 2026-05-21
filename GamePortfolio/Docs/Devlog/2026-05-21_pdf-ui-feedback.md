# 2026-05-21 PDF UI Feedback Pass

## What changed

- Read the PDF feedback and converted it into a small implementation pass focused on battle UI readability.
- Made the `Recent Actions` battle log start collapsed so it no longer covers the battle screen by default.
- Added a dedicated `Log` toggle button in the command area. Pressing it opens the log; pressing `Hide Log` closes it.
- Moved the expanded battle log upward and kept it hidden until requested to reduce overlap with command buttons.
- Updated the generated BattleScene validator so it checks that the log panel/title/text start collapsed and that the toggle button is wired.
- Added Battle Logic Auto Test coverage for the collapsed default and open/close toggle behavior.

## Why this matters for the portfolio

The feedback said the log was blocking the screen, UI elements overlapped, and the current presentation did not match the desired polished 2D RPG direction. This pass keeps the existing battle systems but improves usability and makes the UI easier to present in screenshots or a short gameplay clip.

## Manual check

1. In Unity, run `Tools > Codex Tactics > Create Battle Test Scene`.
2. Run `Tools > Codex Tactics > Validate Battle Test Scene`.
3. Press Play.
4. Confirm the battle log is hidden at battle start.
5. Press `Log`; confirm `Recent Actions` appears without blocking the command buttons.
6. Press `Hide Log`; confirm the battle log collapses again.
