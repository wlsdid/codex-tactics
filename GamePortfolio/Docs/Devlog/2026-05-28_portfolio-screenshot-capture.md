# 2026-05-28 — Batch 74: Portfolio Screenshot Capture

## What changed

- Extended the automated capture runner so it captures the full vertical-slice flow:
  - Title screen
  - Stage Select screen
  - Battle start
  - Fire skill / Burn feedback
  - Guard status
  - Result summary
  - Retry reset
- Built and ran the Windows standalone capture runner from WSL with graphics enabled.
- Copied the generated screenshots into `Docs/Captures/`.
- Created `Docs/Captures/capture_contact_sheet.png` for quick README/portfolio review.
- Updated README capture links.
- During visual QA, Stage Select still had button/description overlap, so the generated Stage Select layout was tightened and re-captured.

## Verification

- Unity `CreateGameFlowScenes`: PASS.
- Unity `CreateBattleTestScene`: PASS.
- `ValidateGameFlowScenes`: PASS / `RESULT: PASS`.
- `ValidateBattleTestScene`: PASS / `RESULT: PASS`.
- `BattleAutoTestRunner.RunBattleLogicAutoTest`: PASS / `RESULT: PASS`.
- Standalone capture runner saved all expected screenshots.
- Visual QA of the contact sheet: no blank captures, no severe Stage Select overlap, and no obvious missing-glyph boxes.

## Captured files

- `Docs/Captures/00_title_scene.png`
- `Docs/Captures/00_stage_select_scene.png`
- `Docs/Captures/01_battle_start.png`
- `Docs/Captures/02_fire_skill_burn.png`
- `Docs/Captures/03_guard_status.png`
- `Docs/Captures/04_result_summary_rank.png`
- `Docs/Captures/05_retry_reset.png`
- `Docs/Captures/capture_contact_sheet.png`

## Next recommended task

Use these captures to start a polished portfolio showcase page, then later add a short GIF once the user wants motion evidence.
