# Devlog - 2026-05-11 Battle Guide Hint

## What changed

- Added a small `Battle Guide Text` label to the generated battle test scene.
- The guide tells a first-time viewer what to try immediately:
  - Attack
  - Fire Skill / Burn
  - Guard before Heavy Slam
  - Watch Enemy Intent
  - Retry after result
- Updated the scene validator so the guide object and its key words are checked automatically.

## Why it matters

The prototype already had the mechanics, but a portfolio viewer could still miss the intended test path. A short guide label makes the scene easier to understand without adding more result metrics or larger systems.

## Manual test notes

1. Run `Tools > Codex Tactics > Create Battle Test Scene`.
2. Run `Tools > Codex Tactics > Validate Battle Test Scene`.
3. Press Play and confirm the guide is visible near the top of the UI.
4. Follow the suggested flow: Attack, Fire Skill, Guard, watch Enemy Intent, then Retry after the result.

## Next goal

Capture a real Unity Play Mode screenshot/GIF now that the start screen explains the intended controls more clearly.
