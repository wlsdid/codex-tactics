# Enrage Threat Readout Pass

## Goal
Make the command window reflect actual combat danger when the enemy enters Enrage. A professional tactical UI should not keep showing base damage after a phase change.

## Changes
- Command threat preview now calculates incoming damage through the same difficulty/enrage path used by enemy attacks.
- Enraged enemies are explicitly labeled in the command window:
  - `Enemy next: ENRAGED ... dmg -> Guard ...`
- Guard preview now shows the attack name and enrage state, not just a raw damage number.
- Added an auto-test sequence that pushes Slime below the enrage threshold and verifies the selected-unit command window labels the boosted threat.

## Portfolio Value
This makes phase changes readable and reinforces the tactical loop: notice enemy state, evaluate incoming damage, and choose Guard/offense intentionally.

## Verification
- Static C# brace check: PASS
- Targeted enrage threat readout checks: PASS
- `git diff --check`: PASS
