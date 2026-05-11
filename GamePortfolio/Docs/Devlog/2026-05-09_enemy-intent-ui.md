# Devlog - 2026-05-09 Enemy Intent UI

## What changed

- Confirmed the status effect display was already present in the project.
- Added a small Enemy Intent text UI instead, so the battle screen previews the next enemy action.
- The generated scene now creates `Enemy Intent Text` under the enemy HP/status area.
- `BattleManager` updates the intent text during battle UI refreshes:
  - `Next Enemy: Normal Attack (15)` for ordinary turns
  - `Next Enemy: Heavy Slam (30)` before every 3rd enemy turn
  - `Next Enemy: Battle ended` after Victory/Defeat
- Scene validation now checks the intent text object and serialized linkage.
- Battle logic auto-test now checks initial intent, strong-attack preview, and Retry reset.

## Why it helps the portfolio

Enemy intent is a common turn-based tactics UX pattern. It makes Guard more meaningful because the player can see when a stronger enemy action is coming and choose defense intentionally.

## Manual Unity check

1. Run `Tools > Codex Tactics > Create Battle Test Scene`.
2. Run `Tools > Codex Tactics > Validate Battle Test Scene`.
3. Run `Tools > Codex Tactics > Run Battle Logic Auto Test`.
4. Press Play and confirm the enemy side shows `Next Enemy: Normal Attack (15)` at battle start.
5. After two enemy attacks, confirm it previews `Next Enemy: Heavy Slam (30)` before the third enemy attack.

## Notes

Unity Editor/Play Mode was not run in this environment. Only static source verification was performed here.
