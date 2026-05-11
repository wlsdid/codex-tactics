# Study - Stage Reward Summary

Date: 2026-05-10

## Concept

A result screen becomes more useful when it includes both performance metrics and progression hooks.

- Performance metrics answer: how well did the player do?
- Reward metrics answer: what did the player gain?

## Current design

The prototype keeps the reward rule intentionally simple:

- Victory: `100G`
- Defeat: `0G`

This is enough to communicate the RPG loop without adding inventory, shops, save data, or economy complexity too early.

## Implementation note

`BattleResultData` is a good place to collect reward information because the result summary already gathers final HP/AP, damage totals, skill counters, rank, and enemy pattern data there. Adding `rewardGold` keeps the formatting code straightforward while preserving room for future expansion.

## Future options

- Scale reward by rank, for example `S = 150G`, `A = 120G`, `B = 100G`.
- Add item drops after the battle loop is stable.
- Move reward rules into a separate stage data object if multiple stages are added.
