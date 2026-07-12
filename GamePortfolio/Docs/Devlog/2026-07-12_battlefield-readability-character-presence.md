# Battlefield Readability & Character Presence Pass (2026-07-12)

## Goal

Start the second visual-polish phase by improving the tactical battlefield's first read without adding gameplay systems.

## Changes

- Replaced the colliding long left-rail header with a compact `PARTY` heading and separate `5-UNIT SQUAD` chip.
- Reduced the size and opacity of the hero/enemy landing tiles so they frame the feet instead of competing with the sprites.
- Reduced the hero/enemy rectangular aura layers to near-transparent, smaller backing layers. Base rings, contact glows, shadows, rim lighting, and the pixel standees remain visible.
- Updated Battle Scene validator expectations to enforce the new restrained landing-tile and aura treatment.

## Verification

- `BattleSceneAutoBuilder.CreateBattleTestScene`: PASS.
- `BattleSceneAutoBuilder.ValidateBattleTestScene`: PASS.
- `BattleAutoTestRunner.RunBattleLogicAutoTest`: PASS.
- Standalone `CaptureRunner.exe` regenerated the battle/result PNG sequence.
- README GIF: 960x540, 8 frames.
- Runtime storyboard GIF: 960x540, 34 frames.
- `git diff --check`: PASS.

## Visual QA

- The Party header no longer collides with the squad-status label.
- The sprites retain readable grounding while the cyan/magenta rectangular overlays are less visually dominant.
- The full capture route remains non-blank and readable in the refreshed contact sheet.

## Next direction

This was a composition cleanup. The next high-impact art pass should replace the remaining abstract slab/spotlight geometry with authored forest ruins, terrain silhouettes, and scene props while preserving the compact tactical HUD.
