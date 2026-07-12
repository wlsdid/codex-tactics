# Compact Five-Unit Party Roster (2026-07-12)

## Visual target

Reduce the left party roster footprint without changing combat mechanics, the five-unit party, or contextual selected-ally commands.

## Changes

- Reduced the generated player-card and roster-panel footprint.
- Compressed each roster row from 70px to 56px and reduced portrait, typography, HP/MP bar, and selected-rim dimensions coherently.
- Kept all five named units with HP/MP visible; removed nonessential level numerals that were clipping at the left capture edge.
- Repositioned the Party header, squad chip, primary portrait, and selectable Paladin row to avoid collision.
- Replaced the old broad minimum-size validation with an explicit compact-roster geometry assertion.

## Verification

- Unity `CreateBattleTestScene`: PASS.
- Unity `ValidateBattleTestScene`: `RESULT: PASS`.
- Unity `BattleAutoTestRunner.RunBattleLogicAutoTest`: `RESULT: PASS`.
- `CaptureScreenshots.Run` Windows build: PASS.
- Standalone 1920x1080 capture sequence: PASS (eight nonblank PNGs).
- README GIF, runtime-motion storyboard GIF, and contact sheet refreshed.
- Visual QA: the header and squad chip do not overlap, the five roster rows retain readable name/HP/MP data, and the central standees remain the primary battle read.

## Follow-up

Continue commercial visual polish with authored forest-ruins composition only; do not add mechanics or re-expand the left roster.
