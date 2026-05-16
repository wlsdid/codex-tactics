# Devlog — 2026-05-16: Auto-Battle AI

## What I did

Added an auto-battle toggle that lets the AI play the game. Useful for demo mode and balance testing.

### Toggle button

- New `autoBattleButton` in the command bar
- Toggle via `OnClickAutoBattleToggle()`
- Visual indicator: `Auto: ON` / `Auto: OFF`
- When toggled ON during player turn, immediately executes an action

### AI decision tree (priority order)

```
1. Guard if incoming strong attack (highest priority)
2. Use weakness-element skill if Break gauge remains
3. Use Lightning Strike (3 AP, 40 power, burst)
4. Use Ice Lance (1 AP, Stun)
5. Use Fire Bolt (2 AP, 30 power, Burn)
6. Default: basic attack (0 AP, 20 power)
```

### Priority logic explained

1. **Guard priority** — If the enemy's next turn is a strong attack, the AI guards to reduce damage by 50%. This is the highest priority because surviving is more important than dealing damage.

2. **Weakness priority** — If the AI can hit the enemy's elemental weakness and the Break gauge isn't empty, it uses that skill. Breaking the enemy (reducing Break to 0) grants a 1.5x damage bonus on the next hit.

3. **Burst priority** — Lightning Strike is used first among skills because it deals the most raw damage (40 power). Then Ice Lance (stun, cheap), then Fire Bolt (burn, moderate cost).

4. **Default** — Basic attack is always available and costs nothing.

### Key detail

- `ExecuteAutoAction()` only runs during `BattleState.PlayerTurn`
- Already-dead enemies are handled before the decision tree
- AP availability is checked before each skill (`player.HasEnoughAp()`)

### Files changed

- `Assets/Scripts/Battle/BattleManager.cs` — `ExecuteAutoAction()`, `GetWeaknessSkill()`, `OnClickAutoBattleToggle()`
- `Assets/Scripts/Battle/BattleUI.cs` — `SetAutoBattleIndicator()`, `autoBattleButton`
- `Assets/Editor/BattleSceneAutoBuilder.cs` — Auto-battle button setup
- `Assets/Editor/BattleAutoTestRunner.cs` — Tests for auto-battle toggle
