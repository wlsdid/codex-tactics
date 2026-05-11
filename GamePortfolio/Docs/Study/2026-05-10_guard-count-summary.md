# Study Note - Guard Count Summary

## Concept

A combat report can include both outcomes and player choices. Damage dealt/taken shows the numeric result, while Guard uses records a defensive decision the player made.

## Implementation idea

The guard counter is intentionally simple:

```csharp
private int guardUseCount;
public int DebugGuardUseCount => guardUseCount;
```

It resets with the rest of the battle state:

```csharp
guardUseCount = 0;
```

It increments when the player chooses Guard, not when damage is later received:

```csharp
playerIsGuarding = true;
guardUseCount++;
```

This distinction matters because the counter describes a player action. Damage taken still describes the result of the enemy hit.

## Test lesson

The test checks both the internal debug value and the result summary text. That catches two classes of mistakes:

1. the counter is not updated/reset correctly
2. the counter exists but is not visible to the player in the summary
