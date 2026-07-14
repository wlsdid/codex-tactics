# Tactical Requiem Battle Layout Redesign — 2026-07-14

## Goal

Rebuilt the BattleScene information architecture around a wide central tactical field without copying reference-game art, copy, icons, or layout.

## Delivered

- Central battlefield now presents the live Hero and live Enemy anchors with two original-art visual supports per side for a readable 3v3 formation.
- Narrow left ally rail now uses three compact portrait/status rows; no long MP bars or oversized character cards.
- Narrow right enemy rail retains only the live enemy portrait, HP, status, intent, and break state.
- Top lane is reduced to stage, current-turn messaging, and compact encounter queue information; run/help/progress proof chips and control buttons are visually removed.
- Bottom HUD is bounded above the 1080 capture edge. It keeps selected-unit/AP context, three contextual commands (Attack, Fire, Guard), resource state, and short skill detail.
- Replaced BattleScene presentation sprite references with Tactical Requiem generated original assets. The pre-existing 1v1 mechanics and Hero/Enemy interaction anchors remain intact.
- Suppressed legacy rectangular prop/proof overlays from captures and added validator assertions for compact rails, central formation, top-lane consolidation, and bounded command/detail areas.

## Verification

- Regenerated `Assets/Scenes/BattleScene.unity` in Unity 6000.4.6f1.
- Passed BattleScene validator, Game Flow validator, and Battle Logic Auto Test.
- `CaptureScreenshots.Run` rebuilt `Builds/CaptureBuild/CaptureRunner.exe` successfully.
- Ran CaptureRunner at 1920×1080; refreshed `Docs/Captures/01_battle_start.png` and `Docs/Captures/02_fire_skill_burn.png` are nonblank 1920×1080 captures.
- Visual QA confirmed central 3v3 readability, compact side rails, visible Fire detail, and no lower-edge command-strip clipping.
