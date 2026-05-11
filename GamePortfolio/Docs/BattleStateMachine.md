# Battle State Machine

This prototype uses a small, readable state machine for the battle loop.

```text
Start
  -> PlayerTurn
      -> EnemyTurn
          -> PlayerTurn
      -> Victory
      -> Defeat
```

## State summary

## Start

Battle data is reset:

- Player HP/AP
- Enemy HP
- Player/Enemy HP bars
- Player AP bar
- Player status text
- Enemy status text
- Enemy intent text
- Skills
- Guard flag
- Enemy turn counter
- Damage dealt/taken counters
- Guard use counter
- Skills used counter
- Battle log
- Result summary UI is cleared/hidden
- Result summary panel is cleared/hidden

After setup, the battle immediately enters `PlayerTurn`.

## PlayerTurn

The player can choose one action:

- `Attack`: free physical damage
- `Fire Skill`: costs AP, deals Fire damage, applies Burn
- `Guard`: ends the turn and reduces the next enemy attack
- `End Turn`: skips action and lets the next turn recover AP

At the start of a player turn, AP recovers by `playerApRecoveryPerTurn`. HP text, HP bars, AP text, and the AP bar are refreshed whenever battle UI updates.

Using `Fire Skill` spends 2 AP immediately, so the AP text and AP bar change from full `3/3` to `1/3` before the enemy turn begins. Attack and Fire Skill both increase the skills used counter after AP payment succeeds. The damage dealt counter records the actual HP removed from the enemy, including weakness bonus damage.

Choosing `Guard` sets the player status text to `Status: Guarding` until the next enemy attack is resolved. After the guard damage reduction is consumed, it returns to `Status: Ready`. Each chosen Guard action increases the guard use counter once.

## EnemyTurn

The enemy turn resolves in this order:

1. Burn damage, if active
2. Enemy attack
3. Victory/Defeat check
4. Return to `PlayerTurn` if both sides are alive

The enemy has a simple pattern counter. Every 3rd enemy turn, it uses `Heavy Slam` for stronger damage. Each resolved enemy hit increases the damage taken counter by the actual HP removed from the player.

The Enemy Intent text previews the next enemy action from `enemyTurnCount + 1`:

- `Next Enemy: Normal Attack (15)` for ordinary turns
- `Next Enemy: Heavy Slam (30)` before every 3rd enemy turn
- `Next Enemy: Battle ended` after Victory/Defeat

## Victory / Defeat

- `Victory`: enemy HP reaches 0
- `Defeat`: player HP reaches 0

All action buttons are disabled when the battle ends.
The retry button is shown, and a compact result summary appears with:

`BattleResultData.cs` contains the values used by the summary. `BattleResultPresenter.cs` owns the final summary text formatting. `BattleManager` builds the data object, then passes it to the presenter. This keeps result metrics such as damage, Guard uses, Skills used, pace, rank, reward, result tip, and last enemy pattern grouped in one place while keeping display text in a separate class as the combat report grows.

- Result: Victory or Defeat
- Enemy turns resolved
- Player final HP/AP
- Enemy final HP
- Damage dealt / damage taken
- Guard uses
- Skills used
- Pace label (`Fast`, `Steady`, `Long`, or `Defeated`)
- Battle rank (`S`, `A`, `B`, or `C`)
- Reward gold (`150G` for S, `120G` for A, `100G` for B, `0G` for C/Defeat)
- Result tip, such as `Perfect clear!` or `Guard before Heavy Slam.`
- Last enemy pattern used, such as `Normal Attack` or `Heavy Slam`

The player status text changes to `Status: Battle ended` when the result state is reached. The result summary panel is shown with the summary text, then hidden again on Retry so the next battle starts cleanly. Pace is intentionally simple: fast Victory is `Fast`, medium Victory is `Steady`, longer Victory is `Long`, and Defeat is `Defeated`. Rank is intentionally simple: Defeat is `C`, a clean fast Victory is `S`, solid Victory is `A`, and slower or rougher Victory is `B`. Reward gold is scaled from that rank so the result summary connects performance to payout. The result tip gives one short next-action hint based on the rank and the last enemy pattern.

## Portfolio note

This is intentionally small and beginner-readable. It is a good base for later features such as multiple enemies, boss phases, retry buttons, stage rewards, or a ScriptableObject-based skill database.
