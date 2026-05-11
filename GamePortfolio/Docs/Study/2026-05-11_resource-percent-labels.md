# Study Note - 2026-05-11 Resource Percent Labels

## Concept learned

A UI label can combine an exact value and a calculated percentage to make resource state easier to read.

## Where it was applied

`BattleManager` now builds resource labels through one helper:

```csharp
private string BuildResourceText(string label, int currentValue, int maxValue)
{
    int percentage = maxValue > 0 ? Mathf.RoundToInt((float)currentValue / maxValue * 100f) : 0;
    return $"{label}: {currentValue}/{maxValue} ({percentage}%)";
}
```

This is used for:

- Player HP
- Player AP
- Enemy HP

## Why a helper method is useful

Before this change, each label had its own string format. A helper keeps the display rule in one place, so future resource labels can follow the same format without copying logic.

## Beginner C# points

- `(float)currentValue / maxValue` avoids integer division.
- `Mathf.RoundToInt(...)` converts the calculated percentage into a clean whole number for UI.
- `maxValue > 0 ? ... : ...` prevents division by zero.

## Portfolio note

This is a small UI polish step, but it shows attention to readability: values, bars, and percentages now communicate the same battle state together.
