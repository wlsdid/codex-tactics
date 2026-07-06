# Strong-Attack Guard Readout Regression

## Goal
Lock down the tactical-command behavior that makes the battle UI feel less amateur: when an enemy heavy attack is coming, the command window must tell the player to Guard before pushing damage.

## Changes
- Extended the battle auto-test around the third-turn Heavy Slam setup.
- The test now clicks Hero while the enemy intent is `Heavy Slam` and verifies the command window says:
  - `Recommended: Guard first`
  - the incoming heavy attack name
  - the guarded damage estimate `30 dmg -> Guard 15`

## Portfolio Value
This protects the core tactical loop: read enemy intent, choose defense/offense, then commit. It is a small but important marker of a professional tactical RPG UI.

## Verification
- Static C# brace check: PASS
- Targeted tactical-preview text checks: PASS
- `git diff --check`: PASS
