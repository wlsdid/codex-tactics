# Devlog - Result Summary Tip

Date: 2026-05-10

## Goal

Add a short post-battle recommendation line to the result summary so the player receives feedback, not just statistics.

## Changes

- Added auto-test expectations for result summary tips:
  - S-rank Victory: `Tip: Perfect clear!`
  - Defeat after Heavy Slam: `Tip: Guard before Heavy Slam.`
- Added `resultTip` to `BattleResultData`.
- Added `BuildResultTip(rank, lastEnemyPattern)` in `BattleManager`.
- Updated the result summary text to show `Tip: ...` between reward and last enemy pattern.
- Updated README, battle state docs, balance notes, and manual validation docs.

## Why this helps the portfolio

The result screen now acts like a small feedback loop. It tells the player what happened, what they earned, and one simple improvement hint for the next attempt.

## Verification

Unity Editor was not run in this environment. Static source/document checks and `git diff --check` should be used before Unity manual validation.
