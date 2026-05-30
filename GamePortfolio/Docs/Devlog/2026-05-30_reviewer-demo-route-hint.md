# Devlog — 2026-05-30 — Batch 87: Reviewer Demo Route Hint

## Goal

Make the current demo easier for a portfolio reviewer to judge without requiring a newly recorded Windows MP4.

## Changes

- Added an in-game `Demo Route` hint to the generated battle scene:
  `Click Hero -> Fire -> Guard -> Result -> Retry`.
- Added validator coverage so `Validate Battle Test Scene` checks the route panel/text and keeps its raycast behavior optimized.
- Added `Docs/ReviewerQuickCheck.md`, a concise under-60-second reviewer checklist.
- Linked the checklist from `README.md`.

## Manual test path

1. Open `BattleScene` or enter battle from `TitleScene`.
2. Read the new in-game route hint near the battle center.
3. Click Hero.
4. Use Fire, then Guard when enemy intent is dangerous.
5. Confirm Result/Retry is visible at the end.

## Portfolio value

This removes guesswork for reviewers: even before a true runtime MP4 is recorded, the playable sequence is visible in-game and documented in the README checklist.
