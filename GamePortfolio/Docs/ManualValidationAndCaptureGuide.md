# Manual Validation and Capture Guide

This guide is for recording proof that the Unity battle prototype works in the Editor. Use it after the latest code changes are compiled in Unity.

## 1. Rebuild the generated test scene

1. Open the project in Unity Hub.
2. In the Unity top menu, click `Tools > Codex Tactics > Create Battle Test Scene`.
3. Confirm the completion popup.
4. Check that `Assets/Scenes/BattleScene.unity` is open.

Why this matters: generated scene objects do not update automatically when scripts change. Re-run the builder after UI fields or buttons are added.

## 2. Run editor validation menus

Run these menus before recording:

1. `Tools > Codex Tactics > Validate Battle Test Scene`
2. `Tools > Codex Tactics > Run Battle Logic Auto Test`

Expected result:

- Both dialogs should show a pass/success result.
- If a menu is missing, wait for Unity compilation to finish and check the Console for compile errors.

## 3. Manual play checklist

Press the top-center `Play` button, then check this sequence:

| Step | Action | What to capture |
| --- | --- | --- |
| 1 | Start battle | Stage label `Stage 1-1: Slime Scout`, objective `Objective: Defeat Slime Scout`, Battle Guide with Attack/Fire Skill/Burn/Guard/Enemy Intent/Continue/Final Clear/Retry hints, full HP/AP bars, `100%` resource labels, `Status: Ready`, `Next Enemy: Normal Attack (15)`, and the `Recent Actions` log panel |
| 2 | Click `Fire Skill` | AP decreases from `3/3 (100%)` to `1/3 (33%)`, enemy HP shows `40/80 (50%)`, enemy gets Burn, and the action appears in the `Recent Actions` log |
| 3 | Restart or continue test | Prepare a Guard example |
| 4 | Click `Guard` | Player status changes to `Status: Guarding` |
| 5 | Resolve enemy attack | Guarded damage appears and status returns to `Status: Ready` |
| 6 | Reach Victory or Defeat | Result panel appears with compact grouped lines: `Damage: dealt ..., taken ...`, `Choices: Guard ..., Skills ...`, `Pace: ... | Survival: ...`, `Rank: ... | Reward: ...G | Total Gold: ...G`, plus `Tip` and last enemy pattern; first Victory objective becomes `Objective Complete: Stage 1-1: Slime Scout | Continue to Stage 1-2: Slime King` and the message names the next encounter |
| 7 | Click `Continue` after first Victory | Stage changes to `Stage 1-2: Slime King`, objective becomes `Objective: Defeat Slime King`, boss HP starts at `140/140 (100%)`, and enemy intent previews `Royal Slam (36)` |
| 8 | Clear the boss | Final Clear message tells the player to review Total Gold, objective becomes `Objective Complete: Stage 1 cleared | Final Clear`, and Continue is hidden |
| 9 | Click `Retry` | Result panel disappears and the current encounter HP/AP/status/objective reset |

## 4. Screenshot list for README

Capture these still images if GIF recording is not ready yet:

1. `01_battle_start.png` - initial battle UI with the stage label, Battle Guide hint label, and `Recent Actions` log panel
2. `02_fire_skill_burn.png` - Fire Skill / Burn state
3. `03_guard_status.png` - Guarding status before enemy attack
4. `04_result_summary_rank.png` - result summary with panel, compact Damage/Choices/Pace+Survival/Rank+Reward+Total Gold lines, and Tip
5. `05_continue_boss_stage.png` - Stage 1-2 Slime King boss encounter after Continue
6. `06_retry_reset.png` - restarted clean state

Suggested folder:

```text
Docs/Captures/
```

The folder is kept in the repository with `Docs/Captures/.gitkeep`, and `Docs/Captures/README.md` lists the recommended capture filenames.

## 5. GIF recording plan

A short GIF is better than a long video for README.

Recommended sequence length: 8-15 seconds.

Suggested GIF content:

1. Start in the generated battle scene.
2. Use `Fire Skill` once.
3. Show enemy intent/status update.
4. Use `Guard` once.
5. Win the first encounter and press `Continue`.
6. Show the Stage 1-2 Slime King boss start.
7. End on final result summary if possible.

Suggested filename:

```text
Docs/Captures/codex_tactics_battle_loop.gif
```

## 6. Windows capture tools

Simple options:

- Windows Game Bar: `Win + G`
- OBS Studio if installed
- ScreenToGif for short GIF capture

If recording through Windows Game Bar, save the video first, then convert a short clip to GIF later.

## 7. README embed example

After a capture file exists, add one of these to `README.md`.

Screenshot:

```md
![Battle result summary](Docs/Captures/04_result_summary_rank.png)
```

GIF:

```md
![Codex Tactics battle loop](Docs/Captures/codex_tactics_battle_loop.gif)
```

## 8. What not to claim yet

Do not write that Unity Play Mode passed until the project was actually run in Unity. Until then, use this wording:

> Static checks passed outside Unity. Unity manual validation is pending.
