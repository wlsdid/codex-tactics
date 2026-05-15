# Devlog — 2026-05-15

## What I did
- Critically reviewed the entire codebase as a "비판자" (critic)
- Identified and fixed critical balance issue: Slime King boss `strongAttackEveryTurns` was 1 (every turn Royal Slam 36 damage) → changed to 3 (every 3rd turn)
- Extracted all hardcoded balance values into `BattleBalanceConfig` ScriptableObject
- Made `currentStageIndex` non-serialized (was persisting across editor plays)
- Removed misleading `[SerializeField]` from fields that get overwritten by stage data
- Added config-backed helper properties in `BattleManager` with seamless fallback to defaults
- Passed `balanceConfig` to `BattleResultEvaluator` for config-driven rank/pace thresholds
- Kept full backward compatibility — all existing scene binds and test expectations preserved

## Concepts learned
- `[NonSerialized]` prevents Unity from saving a field's value between editor sessions
- `ScriptableObject` with `[CreateAssetMenu]` creates a clean tuning surface for non-programmers
- `→` expression-bodied properties are great for config fallback chains

## Next goal
- Capture Unity Play Mode screenshots/GIFs now that the boss is playable
- Update portfolio showcase draft with config-driven balance documentation
