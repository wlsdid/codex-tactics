# Devlog - Rank-Scaled Rewards

Date: 2026-05-10

## Goal

Make the result reward feel connected to battle performance instead of using one flat Victory reward.

## Changes

- Changed the clean Victory auto-test expectation from `Reward: 100G` to `Reward: 150G` for S rank.
- Replaced the single `victoryRewardGold` value with rank reward values:
  - S: `150G`
  - A: `120G`
  - B: `100G`
  - C/Defeat: `0G`
- Updated `BuildBattleResultData()` so it builds the rank once, then derives reward gold from that rank.
- Updated README, battle state docs, and balance docs to describe rank-based rewards.

## Why this helps the portfolio

This small change makes the result screen more game-like: better performance now produces a better reward. It also shows a clean relationship between battle metrics, rank calculation, and progression payout.

## Verification

Unity Editor was not run in this environment. Static checks and `git diff --check` should be run before Unity manual validation.
