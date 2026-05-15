# Deep Coding Plan — 2026-05-16

Purpose: move the Unity portfolio from a functional prototype toward a more professional, BrownDust2-inspired tactical RPG presentation.

## Role split

- Deep: coding and Unity-side implementation only.
- Codex/Hermes: portfolio writing, README/showcase/devlog cleanup after code is reasonably complete.
- Do not spend Deep's time writing portfolio prose unless it is required for technical validation notes.

## Current repo state checked by Codex

- Repo root: `/mnt/c/Users/jywls/Desktop/game_portfolio`
- Unity root: `/mnt/c/Users/jywls/Desktop/game_portfolio/GamePortfolio`
- Recent work includes title scene, placeholder sprites, BattleUI extraction, stage flow, balance config, result metrics, and screenshot docs.
- Current git status has existing Unity settings changes and untracked files:
  - `GamePortfolio/Assets/Scripts/Battle/BattleUI.cs.meta`
  - `GamePortfolio/title_scene_log.txt`
  - Unity settings/profile assets modified
- Do not delete or overwrite these without checking diff first.

## Priority direction

The next phase should not stay trapped in “portfolio documentation.” Build the game like a small professional vertical slice:

1. More polished RPG battle presentation.
2. Better tactical combat decisions.
3. Clear stage/title flow.
4. Automated validation so progress is safe.

BrownDust2 reference direction: premium 2D tactical RPG feeling, dark elegant panels, strong character/enemy cards, readable command buttons, turn/intent clarity, reward/result polish. Do not copy assets; emulate layout quality and UX clarity with original placeholder art.

## Batch 1 — UI/UX polish pass

Goal: make the generated battle scene look less like a test scene.

Implement:
- A proper battle screen layout pass in `Assets/Editor/BattleSceneAutoBuilder.cs`.
- Dark fantasy panel hierarchy: top title/stage strip, left hero card, right enemy card, center action/result area, bottom command bar.
- Button visual states that feel like RPG commands: Attack, Fire, Guard, End Turn, Retry, Continue.
- Keep TextMeshPro strings English for early TMP glyph safety.
- Preserve existing BattleUI field hookups.

Update validation:
- `Tools > Codex Tactics > Validate Battle Test Scene` must check the new critical panels/buttons/texts.
- `Tools > Codex Tactics > Run Battle Logic Auto Test` must still pass.

## Batch 2 — Professional battle feedback

Goal: make attacks feel visible and understandable.

Implement small, safe feedback systems:
- Floating damage/heal text placeholder, or at minimum a dedicated `Last Action` impact line.
- Enemy intent stronger visual wording: Normal Attack / Strong Attack / Burning / Guard interaction.
- Result summary should remain readable, not overloaded.

Avoid:
- Big animation systems.
- Imported external assets.
- Complex inventory/shop systems yet.

## Batch 3 — One tactical depth feature

Goal: add one mechanic that makes the battle less linear.

Recommended feature: `Break` gauge.
- Enemy has a small Break gauge, e.g. 0/100.
- Fire/weakness attacks add more Break.
- At full Break, enemy skips or weakens the next attack once.
- Show Break in UI as text and, if easy, a slider.
- Add config values to `BattleBalanceConfig` rather than hardcoding everything.

Keep it beginner-readable and validate with editor auto-test.

## Batch 4 — Title-to-battle flow

Goal: make the vertical slice feel like a game, not isolated scenes.

Implement:
- Title screen Start button loads BattleScene.
- Battle final clear can return to title or show a clear state with next action.
- Validate scene names/build settings if possible.

## Required workflow for Deep

1. Start with:
   ```bash
   cd /mnt/c/Users/jywls/Desktop/game_portfolio
   git status --short --branch
   git diff --stat
   ```
2. Inspect existing code before editing.
3. Choose one batch only; complete and verify it before the next.
4. Prefer generated scene/editor automation over manually editing Unity scene YAML.
5. Run lightweight checks:
   - `git diff --check`
   - C# brace/final-newline/trailing-whitespace checks
   - Unity batchmode compile if available
   - scene validation menu method if available
   - battle logic auto-test menu method if available
6. Report back with:
   - changed files
   - verification results
   - what the user should click in Unity
   - what remains for Codex/Hermes to document

## Important instruction

When code reaches a stable visible milestone, tell Codex/Hermes to write the portfolio materials. Deep should continue coding; Codex/Hermes will handle Korean portfolio explanation, README/showcase/devlog polish, and submission-style writing.
