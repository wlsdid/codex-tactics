# Study Note - Run Status Header

## Concept learned

A small UI label can separate high-level run state from detailed objective text.

## Why it was needed

The stage label tells which encounter is active, and the objective label tells what to defeat. The new run status label tells the viewer whether the whole stage run is active, needs retry, is waiting for Continue, or is fully complete.

## Where it was applied

- `BattleManager.BuildRunStatusText()` builds the top-level run status.
- `BattleSceneAutoBuilder` creates and links `Run Status Text`.
- `BattleAutoTestRunner` verifies start, defeat, first Victory, and final clear statuses.

## Example

```text
Run Status: Stage 1 In Progress
Run Status: Encounter Clear - Continue
Run Status: Retry Current Encounter
Run Status: Stage 1 Complete
```

## Reflection

This keeps the UI beginner-readable and makes screenshots clearer for portfolio review.
