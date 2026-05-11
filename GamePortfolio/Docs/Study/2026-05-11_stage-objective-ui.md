# Study Note - 2026-05-11 - Stage Objective UI

## Concept

A stage-based game UI should separate two pieces of information:

1. Where the player is: `Stage 1-1: Slime Scout`
2. What the player should do: `Objective: Defeat Slime Scout`

The first is identity/progress. The second is the current task.

## Project application

`StageData` already stored `stageName` and `encounterName`. This update added a small method that builds objective text from the same data:

```csharp
public string BuildObjectiveText()
{
    return $"Objective: Defeat {encounterName}";
}
```

`BattleManager` then decides whether the objective is active, complete, failed, or final clear based on the current `BattleState` and whether another stage exists.

## Why this is useful

- The player can immediately understand the current goal.
- Screenshots become easier to explain in a portfolio.
- The stage flow now communicates progress before adding bigger systems like a title screen, stage select, or rewards carryover.

## Unity UI lesson

Adding a serialized UI field is not enough. The generated scene builder and validator also need to know about it:

- Create the text object in `BattleSceneAutoBuilder`.
- Assign it to `BattleManager` through `SerializedObject`.
- Validate that the object exists, contains expected wording, and is linked.
- Add battle logic auto-test expectations for runtime text changes.

## Takeaway

Small UI labels can be real game-facing improvements when they clarify player intent. For a vertical slice, clear state/progress communication is often more valuable than adding another hidden metric.
