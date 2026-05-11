# Study - Rank-Scaled Rewards

Date: 2026-05-10

## Concept

A reward system can be introduced before a full economy exists by making it display-only and tied to result rank.

The important learning point is the dependency direction:

1. Battle events produce metrics.
2. Metrics produce a rank.
3. Rank produces a reward.
4. Result summary displays all of them together.

## Current reward table

| Rank | Reward |
| --- | --- |
| S | 150G |
| A | 120G |
| B | 100G |
| C / Defeat | 0G |

## Why not add inventory yet?

Gold is currently a result summary value only. That keeps scope controlled while still making the stage loop feel more complete. Inventory, shops, persistence, and save/load can be added later when the battle loop is stable.

## Implementation note

`BuildBattleResultData()` now stores the rank in a local variable and passes it into `BuildRewardGold(rank)`. This avoids recalculating or diverging reward logic from rank logic.
