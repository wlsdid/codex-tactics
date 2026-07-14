# Combat Focus Layout (2026-07-14)

## Goal

Correct the battle presentation using `Docs/Captures/02_fire_skill_burn.png` as evidence: preserve the existing 1v1 combat anchors and central 3v3 visual formation, while preventing HUD rails and clipped skill details from competing with the encounter.

## Changes

- Rebuilt `BattleSceneAutoBuilder` around a combat-focus hierarchy: compact, lower-contrast party/enemy rails leave the central 3v3 formation dominant.
- Replaced roster HP/MP bar density with compact numeric summaries; no source character sprite pixels or embedded platform artifacts were changed.
- Consolidated title, run state, message, stage, objective, and progress into separated top lanes above the formation.
- Moved bottom resource/status chips into the visible command strip and added a bounded current-skill detail region.
- Enlarged the runtime Fire Bolt command preview and suppressed the long reference-help text while that preview is active, preventing its final line from overlapping the current skill card. Clearing the preview restores the reference help.
- Kept character-select command visibility, buttons, serialized links, central Hero/enemy feedback anchors, TMP typography, and 1v1 battle logic unchanged.
- Regenerated `BattleScene.unity`, refreshed all standalone CaptureRunner evidence, and removed trailing whitespace from modified generated YAML.

## Verification

- `BattleSceneAutoBuilder.ValidateBattleTestScene`: `RESULT: PASS`.
- `BattleAutoTestRunner.RunBattleLogicAutoTest`: `RESULT: PASS`.
- `CaptureScreenshots.Run`: Windows standalone build succeeded.
- CaptureRunner standalone execution completed with exit code 0.
- Visual QA of `02_fire_skill_burn.png`: the central 3v3 reads first; side rails are secondary; top HUD lanes do not overlap; Fire Bolt title, AP cost, power, weakness, and Burn effect are fully visible without the former help-text collision.
- `git diff --check`: PASS.

## Scope note

This batch intentionally does not alter the colored platform artifacts embedded inside the supplied character source sprites. That requires new transparent art and remains outside this layout-only pass.
