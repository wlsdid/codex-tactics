# Selected Character Command UI

## Summary

Changed the battle command flow from always-visible command buttons to a contextual tactical-RPG style flow:

1. The player clicks the ally entry in the left party roster.
2. The selected ally is highlighted.
3. The command panel opens beside the party list with Attack, skills, Guard, Items, and End Turn.
4. Choosing an action resolves the existing battle action and closes the command UI.
5. The next player turn starts with the command UI hidden again.

## Implementation Notes

- `BattleUI` now owns a selected-character command panel, roster select button, selection highlight, and selected-unit label.
- `BattleManager.OnClickPlayerUnit()` opens the command menu only during `PlayerTurn`.
- Player-turn prompts now tell the player to click Hero first.
- Action execution paths call `HideCharacterCommandMenu()` so the contextual UI does not stay open after an action.
- `BattleSceneAutoBuilder` creates and wires the party roster select button, hidden highlight, and hidden command panel.
- `BattleSceneAutoBuilder.ValidateBattleTestScene()` now validates that command buttons start hidden until ally selection.
- `BattleAutoTestRunner` now verifies hidden-by-default state, click-to-open behavior, and action/guard close behavior.

## Validation

Batchmode validation was run from WSL against Unity `6000.4.6f1`:

- Compile/import batch run: passed with no `error CS` or script compiler errors.
- `BattleSceneAutoBuilder.CreateBattleTestScene`: exit code 0.
- `BattleSceneAutoBuilder.ValidateBattleTestScene`: `RESULT: PASS`.
- `BattleAutoTestRunner.RunBattleLogicAutoTest`: `RESULT: PASS`.

Manual Play Mode visual review is still recommended for final polish, but the automated scene and logic checks cover the requested default-hidden/click-open/action-close behavior.
