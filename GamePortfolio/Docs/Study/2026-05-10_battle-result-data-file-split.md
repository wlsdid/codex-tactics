# Study - Splitting Result Data from BattleManager

Date: 2026-05-10

## Concept

When a manager class grows, one safe refactor is to move pure data and formatting logic into a small separate type.

Before the split:

- `BattleManager` controlled battle flow.
- `BattleManager` collected result values.
- `BattleManager` also contained the result summary formatting struct.

After the split:

- `BattleManager` still controls battle flow.
- `BattleManager` builds `BattleResultData`.
- `BattleResultData` formats the final summary text.

## Why this is safe

The visible result summary does not change. The refactor only moves where the formatting code lives.

## Portfolio explanation

This is a small example of separation of responsibilities:

- Gameplay system: `BattleManager`
- Result report model/formatter: `BattleResultData`

It keeps the beginner project readable while preparing for later features like result presenter UI, localization, or richer reward data.
