# Study Note - Stage Progress UI

## Concept learned

A UI can be more readable when objective text and progress text have separate responsibilities.

## Why it was needed

The battle scene now has two encounters. `Stage Text` names the current encounter, `Stage Objective Text` says what to defeat, and the new `Stage Progress Text` shows how far the player is through the stage.

## Where it was applied

- `BattleManager.BuildStageProgressText()` builds the progress string.
- `BattleSceneAutoBuilder` creates and links `Stage Progress Text`.
- `BattleAutoTestRunner` checks active, encounter clear, retry-needed, and final-clear progress states.

## Example

```text
Progress: Encounter 1/2 | Active
Progress: Encounter 1/2 | Encounter Clear
Progress: Encounter 2/2 | Stage Clear
```

## Reflection

This is a small UI change, but it helps a portfolio viewer understand the vertical slice without reading code or documentation first.
