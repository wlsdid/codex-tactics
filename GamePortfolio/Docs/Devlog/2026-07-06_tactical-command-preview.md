# Tactical Command Preview Pass

## Goal
Make the battle command layer feel more professional instead of like a raw button strip. When the player clicks the allied unit, the command panel now immediately explains the best tactical read, AP state, Break state, and the next enemy threat.

## Changes
- Replaced the generic selected-unit prompt with a tactical command readout:
  - current AP
  - enemy Break gauge
  - recommended command
  - next enemy attack and guarded damage estimate
- The recommendation prioritizes:
  1. Guard when a strong enemy attack is incoming
  2. Weakness skill for Break pressure
  3. Lightning burst when affordable
  4. Basic/Guard tempo management fallback
- Fixed Guard preview math to match real combat integer damage reduction.
- Added an auto-test assertion for the selected Hero command window.

## Portfolio Value
This improves perceived game quality because the UI now communicates decision-making like a commercial tactical RPG: the player sees intent, resource state, and a suggested tactical option before committing.

## Verification
- Static C# brace check: PASS
- `git diff --check`: PASS
