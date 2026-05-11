# Study - Result Summary Tip

Date: 2026-05-10

## Concept

A result screen can teach the player without opening a separate tutorial window. One short recommendation line is enough to connect the outcome to the next decision.

## Current tip rules

- S rank: `Perfect clear!`
- Last enemy pattern was `Heavy Slam`: `Guard before Heavy Slam.`
- A/B rank: `Take less damage for a higher rank.`
- Fallback: `Use Fire Skill to finish the Slime faster.`

## Design note

The tip is intentionally short. A portfolio viewer should immediately understand the feedback system without reading a long tutorial.

## Implementation note

`BuildBattleResultData()` computes the rank and last enemy pattern once, then passes them into `BuildResultTip(rank, lastEnemyPattern)`. This keeps the result summary data grouped while avoiding duplicate rank/pattern logic.

## Future options

- Add localized Korean/English tip text later.
- Make tips data-driven if multiple enemies or bosses are added.
- Add a dedicated presenter class if result summary formatting grows further.
