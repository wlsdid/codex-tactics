# Codex Tactics Balance Table

This document records the current prototype numbers and the design reason behind each value. It is meant to make future tuning decisions visible for portfolio review.

## Core Character Values

| Item | Current value | Why it exists |
| --- | ---: | --- |
| Hero max HP | 100 | Easy baseline for reading percent-like damage values. |
| Hero attack | 20 | Matches the basic Slash skill power. |
| Hero max AP | 3 | Small enough for clear choices between free and paid actions. |
| AP recovery per player turn | 1 | Encourages waiting or using basic actions between Fire Skill casts. |
| Slime max HP | 80 | Lets a few correct attacks end the test battle quickly. |

## Player Actions

| Action | Cost | Current effect | Balance note |
| --- | ---: | --- | --- |
| Slash / Attack | 0 AP | 20 physical damage | Reliable fallback action. |
| Fire Bolt / Fire Skill | 2 AP | 30 fire damage + 10 weakness bonus + Burn | Strong burst option that spends most AP. |
| Guard | 0 AP | Ends turn and reduces next enemy attack by 50% | Defensive choice that trades tempo for survival. |
| End Turn | 0 AP | Skips action | Simple AP/waiting baseline. |

## Status Effects

| Status | Current value | Balance note |
| --- | ---: | --- |
| Burn duration | 2 turns | Short enough to stay readable. |
| Burn damage | 3 per enemy turn | Small bonus damage; not the main win condition. |

## Enemy Pattern

| Enemy action | Current value | Timing | Balance note |
| --- | ---: | --- | --- |
| Normal Attack | 15 damage | Most enemy turns | Threatens the player over several turns. |
| Heavy Slam | 30 damage | Every 3rd enemy turn | Creates a predictable danger spike for Guard timing. |

## Result Summary Metrics

| Metric | Current behavior | Why it is useful |
| --- | --- | --- |
| Damage dealt | Tracks actual enemy HP removed | Avoids inflated overkill numbers. |
| Damage taken | Tracks actual hero HP removed | Helps explain defensive choices. |
| Guard uses | Counts chosen Guard actions | Shows player defensive behavior. |
| Skills used | Counts successful Attack / Fire Skill actions | Shows how actively the player spent offensive turns. |
| Pace | Fast / Steady / Long / Defeated from result and enemy turns | Gives a quick clear-speed label before the detailed rank. |
| Tip | Shows a short performance or counterplay hint | Turns the result screen into a small learning loop. |
| Last enemy pattern | Shows Normal Attack / Heavy Slam / None | Helps connect result to enemy AI pattern. |
| Summary evaluator | Computes pace, rank, reward, tip, and last pattern label | Keeps result rules separate from battle flow and text formatting. |
| Summary presenter | Formats `BattleResultData` into UI text | Keeps result display text separate from battle flow code. |
| Rank | S/A/B/C | Gives the result screen a game-like performance label. |
| Reward | S: 150G, A: 120G, B: 100G, C/Defeat: 0G | Turns the result screen into a portfolio-ready stage clear payout hook. |

## Rank Rules

| Rank | Current condition | Reward |
| --- | --- | --- |
| S | Victory, enemy turns <= 1, damage taken = 0 | 150G |
| A | Victory, enemy turns <= 3, damage taken <= 30 | 120G |
| B | Other Victory | 100G |
| C | Defeat | 0G |

## Pace Rules

| Pace | Current condition | Design note |
| --- | --- | --- |
| Fast | Victory, enemy turns <= 1 | Quick clear label for strong play. |
| Steady | Victory, enemy turns <= 3 | Normal clear pace for the prototype. |
| Long | Victory, enemy turns >= 4 | Signals a slower win without removing the Victory. |
| Defeated | Defeat | Separates failed runs from clear-speed labels. |

## Current Tuning Read

- Fire Skill is intentionally efficient because the enemy is weak to Fire: 30 base + 10 weakness bonus = 40 damage.
- Two Fire Skill hits can defeat the 80 HP Slime, but AP cost prevents immediate repeated casting without turns/recovery.
- Normal Attack damage of 15 means the hero survives several mistakes.
- Heavy Slam at 30 damage every 3rd enemy turn makes the Enemy Intent UI and Guard action meaningful.
- Guard reducing 15 to 7 demonstrates integer damage reduction and gives a clear visible benefit.

## Future Tuning Ideas

1. If battles feel too short, raise Slime HP from 80 to 100.
2. If Guard feels too strong, reduce damage reduction from 50% to 40%.
3. If Fire Skill dominates, lower weakness bonus from +10 to +5 or increase AP cost later.
4. If rank is too easy, require `enemyTurnCount == 0` for S after adding stronger player burst options.
