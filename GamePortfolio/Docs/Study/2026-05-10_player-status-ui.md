# Study Note - Player Status UI

## Concept

A temporary battle state should be visible while it is active. `Guard` is not just a button click; it creates a short-lived state that affects the next enemy attack.

Without a status display, the player may wonder whether Guard is still active. A small text label solves that clearly.

## Applied in this project

`BattleManager` already had a `playerIsGuarding` flag. The new status text converts that internal boolean into readable UI text:

```csharp
if (playerIsGuarding)
{
    return "Status: Guarding";
}

return "Status: Ready";
```

When the enemy attack is resolved, the existing battle logic already consumes the guard flag:

```csharp
playerIsGuarding = false;
```

Because `UpdateUI()` refreshes the player status text, the UI automatically returns to `Status: Ready` after the guarded enemy attack.

## Why this is beginner-readable

This feature only needs:

1. one `TMP_Text` field,
2. one string-building helper,
3. one generated scene text object,
4. a few auto-test checks.

It is a good example of connecting gameplay state to UI without adding complex systems.

## Portfolio explanation

This is a small UX polish feature: the code does not only calculate Guard damage reduction, it also communicates the current defensive state to the player. That makes the prototype easier to test, record, and explain.
