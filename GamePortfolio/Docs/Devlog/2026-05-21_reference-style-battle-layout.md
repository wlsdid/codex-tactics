# 2026-05-21 Reference-Style Battle Layout Pass

## Goal

Improve the generated battle scene layout after screenshot review. The previous capture had too many large center panels and overlapping text. The new target is closer to the provided tactical RPG reference: a slim top mission HUD, left party roster, open center battlefield, right enemy roster, and compact bottom command controls.

## Changes

- Reworked `BattleSceneAutoBuilder` layout into a reference-inspired battle HUD:
  - Top mission/status strip.
  - Left vertical party card and party roster slots.
  - Open center stage with tactical grid tiles, field shadow panels, firefly glow accents, and a skill action arc.
  - Right enemy roster slots with compact enemy HP/status/intent information.
  - Bottom-right command cluster and battle-start style primary action button.
- Reduced font sizes and panel sizes to avoid the previous screenshot's UI overlap.
- Kept skill-help copy compact and moved top controls into the header so the enemy roster and command area stay readable.
- Kept everything procedural/code-generated; no external UI images or game assets were imported.
- Updated scene validator checks for the new roster/grid/action-arc elements.
- Replaced emoji button prefixes in `BattleUI` with ASCII labels to reduce TMP missing-glyph warnings in automated captures.

## Verification Plan

1. Regenerate BattleScene via `BattleSceneAutoBuilder.CreateBattleTestScene`.
2. Run `BattleSceneAutoBuilder.ValidateBattleTestScene`.
3. Run `BattleAutoTestRunner.RunBattleLogicAutoTest`.
4. Build/run the screenshot capture player and inspect the newest generated screenshots.

## Notes

This is a layout-direction pass, not a direct copy of the reference image. The reference informs composition and readability while the project keeps its own procedural placeholder style.
