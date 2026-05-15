# Study Note — 2026-05-15

## Concept: Config-Driven Balance Design

### Why it was needed
The codebase had ~20 hardcoded magic numbers scattered across `BattleManager` (player HP, AP, skill power, guard %, burn damage, rank thresholds, rewards). Tuning required editing `.cs` code, and there was no single place to review all balance values.

### Where it was applied
- Created `BattleBalanceConfig.cs` — a `ScriptableObject` with `[CreateAssetMenu]` that exposes all balance constants in Unity Inspector
- Added config-backed helper properties to `BattleManager` using `=>` expression-bodied syntax with fallback to default values
- Updated `BattleResultEvaluator` to accept optional config parameter for rank/pace thresholds

### Example code
```csharp
// BattleBalanceConfig.cs (Inspector-editable)
[CreateAssetMenu(fileName = "BattleBalanceConfig", menuName = "Codex Tactics/Battle Balance Config")]
public class BattleBalanceConfig : ScriptableObject
{
    public int playerMaxHp = 100;
    public int sRankMaxTurns = 1;
    public int guardDamageReductionPercent = 50;
    // ... 20+ configurable values
}

// BattleManager.cs (fallback chain)
[SerializeField] private BattleBalanceConfig balanceConfig;
private int ConfigGuardReductionPercent => balanceConfig != null
    ? balanceConfig.guardDamageReductionPercent
    : 50;
```

### Reflection
Using `ScriptableObject` for balance values was straightforward and already the "Unity way." Adding config-backed properties with `=>` syntax and null-coalescing fallbacks meant zero risk of breaking the existing scene — if the Inspector field is unassigned, the old defaults are used. The next step is creating a `.asset` file from the menu `Assets > Create > Codex Tactics > Battle Balance Config`.
