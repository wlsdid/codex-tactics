# Batch 63 Plan — UI SFX Feedback Pass

## Goal

Add small, visible/audio feedback to the existing Title, Stage Select, Battle, and Settings flow so the new volume settings feel testable instead of being only sliders.

This batch should stay beginner-readable and portfolio-safe: no new art packages, no large audio asset imports, and no unrelated combat-system expansion.

## Role Split

- **Codex/codex5.5:** planning, acceptance criteria, verification checklist, final review.
- **Deep/hemes-deep:** Unity implementation, scene/builder wiring, validation, implementation report.

## Current Repo State

- Repo root: `/mnt/c/Users/jywls/Desktop/game_portfolio`
- Unity project root: `/mnt/c/Users/jywls/Desktop/game_portfolio/GamePortfolio`
- Latest known commit: `d25a525 Batch 62: Settings/Options screen — persistent BGM/SFX volume sliders`
- Working tree was clean when this plan was written.

## Preflight

```bash
cd /mnt/c/Users/jywls/Desktop/game_portfolio
git pull --ff-only
git status --short --branch
git log --oneline -5
git diff --stat
```

If the working tree is not clean, stop and report before editing.

## Implementation Scope

### 1. AudioManager: add simple generated UI SFX

Likely file:

- `GamePortfolio/Assets/Scripts/AudioManager.cs`

Add beginner-readable public methods such as:

- `PlayButtonClick()` — short click/confirm sound
- `PlayBack()` — softer back/cancel sound
- `PlaySliderTick()` or `PlayPreview()` — short volume preview sound

Preferred approach:

- Reuse existing `AudioSource`/SFX volume path if present.
- Use small generated clips or existing generated clip pattern; do not import external assets.
- Avoid spawning new GameObjects per click.
- Respect `SettingsManager` SFX volume.
- If SFX volume is `0`, calls should be harmless/silent.

### 2. Wire click SFX to generated UI buttons

Likely files:

- `GamePortfolio/Assets/Scripts/TitleManager.cs`
- `GamePortfolio/Assets/Scripts/Flow/StageSelectController.cs`
- `GamePortfolio/Assets/Scripts/Battle/BattleUI.cs`
- `GamePortfolio/Assets/Scripts/SettingsController.cs`

Add click/back/confirm feedback to existing button handlers, not to every frame/update.

Minimum target buttons:

- Title: Start, Reset Progress, Difficulty, Settings
- Stage Select: stage card selection, Start Battle, Back
- Battle: skill buttons, Guard, Continue, Retry if these are centralized enough
- Settings: Back and slider preview/test sound

### 3. Settings screen preview behavior

When the SFX volume slider changes, play a tiny preview sound in a throttled way.

Pitfall:

- Do **not** play a sound every single slider value update if Unity fires many events rapidly.
- Add a small cooldown such as `0.08s`–`0.15s`, or provide a dedicated `Test SFX` button if that is simpler.

For beginner readability, a `Test SFX` button is acceptable and may be better than slider throttling if it keeps the code cleaner.

### 4. Scene builders / validators

If generated scenes create Settings/Title/Stage/Battle UI, update the relevant editor builder/validator scripts so newly generated scenes include any new button or linkage requirements.

Likely editor files:

- `GamePortfolio/Assets/Editor/GameFlowSceneAutoBuilder.cs`
- `GamePortfolio/Assets/Editor/SettingsSceneAutoBuilder.cs`
- `GamePortfolio/Assets/Editor/BattleSceneAutoBuilder.cs`

Do not hand-edit scene YAML unless unavoidable. Prefer builder scripts and regeneration.

### 5. Documentation

Update only concise implementation evidence, not polished portfolio prose.

Suggested files:

- `GamePortfolio/Docs/09_Next_Autonomous_Tasks.md`
- `GamePortfolio/Docs/Devlog/2026-05-21_ui-sfx-feedback.md`
- `GamePortfolio/Docs/Study/2026-05-21_ui-sfx-feedback.md`

Deep should write factual implementation notes only. Codex can later convert this into polished portfolio writing.

## RED / Acceptance Criteria

Before implementation, identify at least one validation expectation that currently fails or is missing. Good options:

- Settings scene validator expects a `Test SFX` button and fails before it exists.
- Game flow validator expects `AudioManager` to expose UI SFX methods.
- A lightweight editor test checks generated Settings scene contains BGM/SFX sliders plus a test SFX/back flow.

Acceptance criteria after implementation:

1. BGM volume slider still changes BGM volume.
2. SFX volume slider still persists through `SettingsManager` / PlayerPrefs.
3. Button clicks across Title/Stage/Battle can call SFX without null-reference errors.
4. Settings screen has a clear way to test SFX volume.
5. No external audio assets are required.
6. All previous battle/game-flow tests still pass.
7. Working tree diff contains only intended code/docs/scene-builder changes.

## Required Verification Commands

From WSL:

```bash
cd /mnt/c/Users/jywls/Desktop/game_portfolio

git diff --check

'/mnt/c/Program Files/Unity/Hub/Editor/6000.4.6f1/Editor/Unity.exe' \
  -batchmode -quit \
  -projectPath 'C:\Users\jywls\Desktop\game_portfolio\GamePortfolio' \
  -executeMethod GameFlowSceneAutoBuilder.ValidateGameFlowScenes \
  -logFile 'C:\Users\jywls\AppData\Local\Temp\batch63_game_flow_validate.log'

'/mnt/c/Program Files/Unity/Hub/Editor/6000.4.6f1/Editor/Unity.exe' \
  -batchmode -quit \
  -projectPath 'C:\Users\jywls\Desktop\game_portfolio\GamePortfolio' \
  -executeMethod BattleSceneAutoBuilder.ValidateBattleTestScene \
  -logFile 'C:\Users\jywls\AppData\Local\Temp\batch63_battle_scene_validate.log'

'/mnt/c/Program Files/Unity/Hub/Editor/6000.4.6f1/Editor/Unity.exe' \
  -batchmode -quit \
  -projectPath 'C:\Users\jywls\Desktop\game_portfolio\GamePortfolio' \
  -executeMethod BattleAutoTestRunner.RunBattleLogicAutoTest \
  -logFile 'C:\Users\jywls\AppData\Local\Temp\batch63_battle_logic.log'
```

Pass/fail evidence to report:

```bash
grep -E "RESULT: PASS|RESULT: FAIL|error CS|Exception" /mnt/c/Users/jywls/AppData/Local/Temp/batch63_*.log || true
git status --short --branch
git diff --stat
git diff --name-only
```

## Commit Rule

Deep should implement and verify, then request Codex review before commit/push unless the user explicitly says Deep may commit directly.

Suggested commit after Codex approval:

```bash
git add GamePortfolio/Assets GamePortfolio/Docs
git commit -m "Batch 63: Add UI SFX feedback across menus"
git push
```

## Report Template for Deep

```text
Batch 63 구현 보고
- 변경 요약:
- 수정 파일:
- 검증 결과:
  - git diff --check:
  - ValidateGameFlowScenes:
  - ValidateBattleTestScene:
  - RunBattleLogicAutoTest:
- 수동 확인 필요:
- 커밋/푸시 여부:
- Codex 리뷰 요청:
```
