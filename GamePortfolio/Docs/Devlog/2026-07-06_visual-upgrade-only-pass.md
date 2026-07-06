# 2026-07-06 Visual Upgrade Only Pass

## Goal

Shift the next batch away from new mechanics and toward commercial first-impression polish. The user specifically asked whether UI had been sufficiently verified, so this pass treats the current problem as visual quality rather than feature count.

## Changed

- Added a battle-only commercial composition layer in `BattleSceneAutoBuilder`:
  - cinematic top/bottom letterbox panels,
  - restrained inner gold frame,
  - low-alpha field bloom,
  - angled premium landing tiles for hero/enemy,
  - center composition rule line.
- Added validator coverage for those new battlefield composition objects so the scene builder cannot silently regress them.
- Added a Stage Select premium preview layer in `GameFlowSceneAutoBuilder`:
  - compact route/map preview panel,
  - reward chips for Gold and XP,
  - field modifier chip.
- Added Game Flow validator checks for the new Stage Select visual elements.

## Why

The previous implementation was functionally verified, but the first screenshot still risked reading as a Unity feature demo. This batch adds visual framing and reward/map presentation elements that are closer to a commercial tactical RPG flow.

## Manual QA target

After Unity scene regeneration and capture refresh, compare the contact sheet against these criteria:

1. Battle scene should read as characters standing on a finished battlefield, not labels floating over a test canvas.
2. Stage Select should communicate route, rewards, and modifier at a glance.
3. Debug/reviewer proof labels must remain secondary to the actual game view.
4. If the new overlays make the screen too dark or crowded, reduce opacity before calling it portfolio-ready.
