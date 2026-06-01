# Devlog — 2026-06-01 Reference-Driven Battle UI Pass

## Goal

Adapt the battle screen toward the user's new UI reference while preserving the current selected-ally command flow.

## Changes

- Repositioned the ally roster click target so it aligns with the visible party card instead of sitting in the lower screen area.
- Removed the extra visible `Click Hero` label from the transparent roster click target; the capture prompt and bottom command hint now carry that guidance instead.
- Moved contextual battle commands into the bottom command strip as compact ASCII-safe buttons: `ATK`, `FIRE`, `ICE`, `LIT`, `EARTH`, `GUARD`, `END`, `ITEM`.
- Expanded the command bar into a wider reference-style selected-unit strip with a visible select-unit hint.
- Added right-side reference panels for enemy intent and selected skill detail.
- Saved the user's game-progress UI reference as `Docs/References/UI/2026-06-01_browndust2_progress_ui_reference.png`.
- Adopted the game-progress reference structure: tall left party roster, center preparation grid, right skill-card stack, bottom FEH-style status strip, portrait queue, turn dial, and large `BATTLE START` CTA.
- Hidden/offscreened legacy prep overlays that were cluttering the first capture while keeping runtime command buttons available after selecting an ally.
- Added a first visual-quality pass after comparing against the polished reference: generated a layered forest battle backdrop, softened the grid/material colors, upgraded right-side skill icons from flat squares to drawn PNG icons, and made the HUD panels more translucent.
- Extracted the user's provided character/enemy sheet into Unity-ready reference sprites under `Assets/Art/ReferenceSprites/`.
- Replaced the generated battlefield standees/roster chips with the provided Paladin, Cleric, Archmage, Goblin, Skeleton, and Dark Knight sprites.
- Added serialized runtime reference sprites so `BattleUI` no longer overwrites the provided Paladin/Goblin portraits with procedural placeholders.
- Added validator coverage for the new reference-style skill card and enemy intent card.

## Portfolio note

This pass does not copy the reference UI directly. It translates the hierarchy into the generated Unity scene:

- top HUD for stage/run info,
- left party roster,
- open center battlefield,
- right intent/skill info,
- bottom contextual command strip.

## Verification target

- Static C# brace/syntax smoke check: PASS.
- Unity `CreateBattleTestScene`: PASS.
- Unity `ValidateBattleTestScene`: PASS (`RESULT: PASS`).
- Unity `BattleAutoTestRunner.RunBattleLogicAutoTest`: PASS (`RESULT: PASS`).
- Capture refresh/contact-sheet visual QA: PASS after running the capture player with a Windows output path.
- `git diff --check`: PASS.
