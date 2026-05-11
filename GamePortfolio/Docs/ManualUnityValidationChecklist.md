# Manual Unity Validation Checklist

Use this checklist when you open the project in Unity and need a quick pass/fail record. It is intentionally checkbox-based so it can be copied into Notion, Discord, or a devlog.

Project scene:

```text
Assets/Scenes/BattleScene.unity
```

## 1. Setup

- [ ] Open `GamePortfolio` in Unity Hub.
- [ ] Wait until script compilation finishes.
- [ ] Confirm there are no red compile errors in the Unity Console.
- [ ] Run `Tools > Codex Tactics > Create Battle Test Scene`.
- [ ] Confirm the generated `BattleScene.unity` is open.

## 2. Editor menu validation

- [ ] Run `Tools > Codex Tactics > Validate Battle Test Scene`.
- [ ] Confirm the validation dialog reports success/pass.
- [ ] Run `Tools > Codex Tactics > Run Battle Logic Auto Test`.
- [ ] Confirm the battle logic test dialog reports `RESULT: PASS`.

## 3. Play Mode start state

Press the top-center Unity `▶ Play` button.

- [ ] Hero HP text starts at `Hero HP: 100/100`.
- [ ] Hero HP bar starts full.
- [ ] AP text starts at `AP: 3/3`.
- [ ] AP bar starts full.
- [ ] Slime HP text starts at `Slime HP: 80/80`.
- [ ] Slime HP bar starts full.
- [ ] Player status shows `Status: Ready`.
- [ ] Enemy status shows `Status: None`.
- [ ] Enemy intent shows `Next Enemy: Normal Attack (15)`.
- [ ] Retry button is hidden.
- [ ] Result summary panel is hidden.

## 4. Fire Skill / Burn check

- [ ] Click `Fire Skill`.
- [ ] AP decreases from `3/3` to `1/3`.
- [ ] Slime HP decreases by Fire weakness damage.
- [ ] Enemy status shows `Status: Burn (2 turns)`.
- [ ] Battle log records the Fire Skill action.

## 5. Guard check

Restart the battle if needed, then test Guard.

- [ ] Click `Guard`.
- [ ] Player status changes to `Status: Guarding`.
- [ ] Resolve the next enemy attack.
- [ ] Guarded normal attack deals reduced damage, expected `Hero HP: 93/100` for the first normal hit.
- [ ] Player status returns to `Status: Ready` after Guard is consumed.
- [ ] Battle log records the Guard action and guarded hit.

## 6. Enemy intent / heavy attack check

- [ ] Advance enemy turns until the preview shows `Next Enemy: Heavy Slam (30)`.
- [ ] Confirm the 3rd enemy turn uses `Heavy Slam`.
- [ ] Confirm the battle log records the strong attack.

## 7. Result summary check

Finish the battle by Victory or Defeat.

- [ ] Action buttons are disabled after the result.
- [ ] Retry button is visible and interactable.
- [ ] Result summary panel is visible behind the text.
- [ ] Result summary includes `Result: Victory | Turns:` or `Result: Defeat | Turns:`.
- [ ] Result summary includes final Hero HP/AP.
- [ ] Result summary includes final Slime HP.
- [ ] Result summary includes `Damage: dealt` and `taken`.
- [ ] Result summary includes `Choices: Guard` and `Skills`.
- [ ] Result summary includes `Pace` and `Survival` on the same line.
- [ ] Result summary includes `Rank` and `Reward` on the same line.
- [ ] Result summary includes `Tip`.
- [ ] Result summary includes `Last enemy pattern`.

## 8. Retry reset check

- [ ] Click `Retry`.
- [ ] Hero HP resets to `Hero HP: 100/100`.
- [ ] AP resets to `AP: 3/3`.
- [ ] Player status resets to `Status: Ready`.
- [ ] Enemy status resets to `Status: None`.
- [ ] Enemy intent resets to `Next Enemy: Normal Attack (15)`.
- [ ] Result summary text is cleared.
- [ ] Result summary panel is hidden.
- [ ] Retry button is hidden again.

## 9. Capture readiness

- [ ] Save screenshots or GIFs under `Docs/Captures/`.
- [ ] Capture at least one result summary screenshot.
- [ ] Capture one short GIF of Fire Skill, Guard, result summary, and Retry if possible.
- [ ] Do not claim Unity Play Mode passed in README/devlog until this checklist is actually completed.

## Validation record

Copy this block into the devlog after testing:

```text
Date:
Unity version:
Validator menu: PASS / FAIL
Battle logic auto test: PASS / FAIL
Manual Play Mode checklist: PASS / FAIL
Screenshots/GIF captured: YES / NO
Notes:
```
