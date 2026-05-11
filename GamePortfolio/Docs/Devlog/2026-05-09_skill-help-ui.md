# Devlog - Skill Help UI

Date: 2026-05-09

## What changed

- Added a generated `Skill Help Text` area to the battle test scene.
- `BattleManager` now writes beginner-readable help lines for:
  - Slash basic attack
  - Fire Bolt AP skill
  - Guard damage reduction
  - Enemy 3-turn strong attack pattern
- Updated the editor scene validator to check that `Skill Help Text` exists and is linked.
- Updated the battle logic auto-test to verify that help text explains the current actions and enemy pattern.
- Added a project `README.md` with overview, Unity menu steps, and portfolio talking points.

## Why this matters for portfolio

A turn-based battle UI should explain player choices clearly. This small help panel makes the prototype easier to understand during a portfolio review and shows attention to UX, not only battle logic.

## Manual Unity test steps

1. Open the project in Unity.
2. Run `Tools > Codex Tactics > Create Battle Test Scene`.
3. Run `Tools > Codex Tactics > Validate Battle Test Scene`.
4. Run `Tools > Codex Tactics > Run Battle Logic Auto Test`.
5. Press Play and confirm that the help text appears above the action buttons.

## External verification done

- Checked source files outside Unity.
- Added static/editor-test expectations before relying on the new UI behavior.
- Ran token checks for new serialized field names and help text strings.
- Ran brace/parenthesis balance checks for touched C# files.
