# Devlog - 2026-05-10 Damage Summary Counters

## What changed

- Added two battle counters to `BattleManager`:
  - `totalDamageDealt`
  - `totalDamageTaken`
- Player damage is counted when attacks or Burn remove enemy HP.
- Enemy damage is counted when normal, strong, or guarded attacks remove player HP.
- `StartBattle()` and Retry reset both counters to 0.
- The result summary now includes:
  - `Damage dealt: ...`
  - `Damage taken: ...`
- Battle logic auto-test now checks:
  - Fire Skill weakness damage adds 40 dealt damage
  - restarting battle resets both counters
  - Guarded hit records 7 taken damage
  - three enemy attacks record 60 taken damage
  - Defeat summary displays damage dealt/taken values

## Why it helps the portfolio

The result screen now communicates more than win/loss. It shows measurable battle performance, which is useful for balancing and for explaining combat outcomes in a portfolio video or README.

## Manual Unity check

1. Run `Tools > Codex Tactics > Create Battle Test Scene`.
2. Run `Tools > Codex Tactics > Validate Battle Test Scene`.
3. Run `Tools > Codex Tactics > Run Battle Logic Auto Test`.
4. Press Play and finish a battle.
5. Confirm the result summary includes `Damage dealt` and `Damage taken`.
6. Click `Retry` and confirm the next battle starts with counters reset.

## Notes

The counters track actual HP removed, not just requested damage. This avoids over-counting if a future attack deals more damage than the target's remaining HP.

Unity Editor/Play Mode was not run in this environment. Only source-level/static verification was performed here.
