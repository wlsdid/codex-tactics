# Devlog — 2026-05-11 Battle Log Readability

## What changed

- Improved the existing battle log as a portfolio-visible UI polish pass.
- Added a `Recent Actions` heading and `No actions yet.` empty state in `BattleManager`.
- Updated the generated battle scene to include a dark Battle Log panel, title, and clearer text area.
- Extended scene validation and the battle logic auto test to check the improved battle log behavior.

## Why it matters

A viewer can now separate recent actions from the main message and result summary more easily. This makes Attack, Fire Skill, Burn, Guard, enemy attacks, Retry, Victory, and Defeat easier to follow during a short portfolio demo.

## Manual test focus

1. Rebuild the scene with `Tools > Codex Tactics > Create Battle Test Scene`.
2. Confirm the bottom log area has a `Recent Actions` title and dark panel.
3. Use Fire Skill, Guard, and enemy turns, then confirm recent actions appear in order.
4. Finish or retry the battle and confirm the log remains readable.

## Next

Capture a short Unity Play Mode GIF that shows Battle Guide, Recent Actions, Guard, and result summary together.
