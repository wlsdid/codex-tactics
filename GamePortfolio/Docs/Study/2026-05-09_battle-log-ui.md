# Study Note — Battle Log UI

## Concept learned
Keep a small list of recent battle messages and show it in a TextMeshPro UI text.

```csharp
battleLogSequence++;
battleLogEntries.Add($"{battleLogSequence}. {message}");

while (battleLogEntries.Count > MaxBattleLogEntries)
{
    battleLogEntries.RemoveAt(0);
}
```

## Why it was needed
The one-line message area changes quickly during a turn-based battle. A battle log helps players understand what just happened: AP recovery, Guard, enemy attacks, burn damage, and strong attacks.

## Where it was applied
- `BattleManager.UpdateUI(string message)` still updates the current message.
- The same message is also passed into `AddBattleLogEntry(message)`.
- `BattleSceneAutoBuilder` now creates and links `Battle Log Text`.
- `BattleAutoTestRunner` checks that log messages are recorded in order.

## Reflection
This is portfolio-useful because it shows UI/UX thinking, not only damage calculation. The player can now read the battle flow, and future mechanics like buffs, break, or boss patterns can reuse the same log.
