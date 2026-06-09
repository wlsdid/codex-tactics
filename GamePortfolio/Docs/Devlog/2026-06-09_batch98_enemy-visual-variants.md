# 2026-06-09 — Batch 98: Enemy Visual Variants

## Goal
Reduce repeated enemy silhouettes in portfolio captures by connecting existing extracted enemy reference sprites to the runtime battle UI per encounter.

## What changed
- Added `EnemyVisualVariant` to `EnemyData`.
- Assigned variants in `StageData` presets: Goblin, Skeleton, Orc, Lich, Golem, and Dark Knight.
- Updated `BattleManager` to pass the current encounter variant into `BattleUI` when a battle starts.
- Updated `BattleUI` so enemy portrait, central standee, and enemy roster mini sprites use the current encounter's reference sprite when available.
- Updated `BattleSceneAutoBuilder` to wire all extracted enemy reference sprites into the generated BattleScene.
- Extended validator and editor auto-test coverage for variant reference links and StageData variant assignments.

## Portfolio value
This is a small visual-readability pass, not a new combat system. The battle loop remains the same, but screenshots/GIFs can now show different enemy silhouettes across encounters and stages, which makes the demo feel less repetitive.

## Manual QA checklist
1. Open Unity project: `C:\Users\jywls\Desktop\game_portfolio\GamePortfolio`.
2. Run `Tools > Codex Tactics > Create Battle Test Scene`.
3. Run `Tools > Codex Tactics > Validate Battle Test Scene` and confirm PASS.
4. Run `Tools > Codex Tactics > Run Battle Logic Auto Test` and confirm PASS.
5. Play BattleScene and confirm:
   - Stage 1 normal starts with a Goblin-style enemy.
   - Pressing Continue after victory swaps the boss encounter to Skeleton-style enemy visuals.
   - Enemy portrait, central standee, and right roster mini sprite all match the encounter style.

## Notes
- This pass intentionally reuses existing `Assets/Art/ReferenceSprites/reference_*_full.png` files.
- No new large art-generation pipeline or combat system was added.
