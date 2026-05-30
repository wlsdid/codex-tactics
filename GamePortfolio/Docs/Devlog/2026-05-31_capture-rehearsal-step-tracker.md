# 2026-05-31 — Batch 88: Capture Rehearsal Step Tracker

## What changed

- Added an in-game `Capture Rehearsal` prompt for the short portfolio recording route.
- The prompt advances through five visible steps:
  1. Click Hero.
  2. Choose Fire Bolt.
  3. Wait for enemy turn, click Hero again, then choose Guard.
  4. Let battle reach Result, then press Retry.
  5. Confirm Retry reset.
- Kept the selected-character contextual command flow: action commands still appear only after the ally is selected.
- Added validator and auto-test checks so the helper stays wired in generated scenes.

## Why this helps the portfolio

A reviewer or recorder no longer has to memorize the 8-15 second route. The game itself shows the next capture step while preserving the tactical UI behavior.

## Manual check

1. Open `BattleScene`.
2. Press Play.
3. Confirm the top-center prompt starts with `Capture Rehearsal 1/5: Click Hero`.
4. Click Hero, use Fire, wait for the next player turn, click Hero again, use Guard.
5. Reach Result and press Retry.
6. Confirm the prompt says the retry reset is complete.
