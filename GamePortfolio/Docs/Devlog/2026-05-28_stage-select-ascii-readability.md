# 2026-05-28 — Batch 72: Stage Select ASCII Readability

## What changed

- Replaced Stage Select emoji/status glyphs with ASCII-safe labels to reduce TMP missing-glyph boxes in captures.
- Converted stage element markers from emoji to compact tags such as `FIRE`, `NAT`, `EARTH`, `LIT`, `DARK`, and `LIGHT`.
- Converted stage difficulty markers from star glyphs to `D1`, `D2`, and `D3`.
- Updated the generated Stage Select scene text and validator expectations so auto-generated scenes match runtime labels.

## Why this matters for the portfolio

The battle UI had already been cleaned up for screenshot readability. This pass applies the same standard to the Stage Select screen so portfolio captures and videos look more stable across TextMeshPro font setups.

## Manual check steps

1. In Unity, run `Tools > Codex Tactics > Create Game Flow Scenes`.
2. Run `Tools > Codex Tactics > Validate Game Flow Scenes` and confirm `RESULT: PASS`.
3. Open `StageSelectScene` and verify:
   - Stage 1 status reads `NEXT`.
   - Locked cards read `LOCKED`.
   - Element/difficulty line uses tags like `FIRE D1`, not emoji/star glyphs.
   - Description panel uses `Slime -> Slime King` and `Status: NEXT - Click Start Battle`.

## Next recommended task

After validation, capture Title/Stage Select/Battle screenshots and start preparing README-ready showcase images if the screens read well.
