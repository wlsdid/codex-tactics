# Battle UI / Sprite Reference Adoption Note — 2026-06-01

## Source files

- `Docs/References/UI/2026-06-01_character_enemy_sprite_reference.png`
- `Docs/References/UI/2026-06-01_browndust2_battle_ui_reference.png`

## Direction

Use the provided character sprites as the visual target for the current tactical RPG portfolio demo. Keep the existing character roles, but replace/align the generated standees with this style over time.

The UI reference should guide the next battle-screen pass, not be copied 1:1. The goal is a darker, more professional tactical RPG presentation with readable information zones.

## Character / enemy asset notes

Playable party candidates:

- Paladin: tank/frontline, shield + mace silhouette, strong readability at large and small sizes.
- Cleric: healer/support, staff and white/gold palette.
- Archmage: magic damage, purple/cyan glow, strong VFX identity.
- Rogue: assassin/fast striker, dark silhouette.
- Bard: support/buffer, green/gold palette.
- Ranger: ranged physical attacker, bow/cape silhouette.

Enemy candidates:

- Goblin: basic melee enemy.
- Skeleton: ranged/fragile enemy.
- Orc Berserker: bruiser/elite enemy.
- Lich: boss/mage enemy.
- Golem: tank enemy.
- Dark Knight: late-stage boss/elite.

Import/slicing constraints:

- The current reference image is a combined sheet with labels, guide grid, and partial cropped enemy row, so it should first be kept as a reference image.
- For production use, individual transparent PNGs or a cleaned sprite sheet are preferred.
- If slicing from this sheet, crop each unit manually or by bounding boxes, remove the background grid/text, and set Unity import settings to `Sprite (2D and UI)`, filter `Point`, compression `None`, and consistent pixels-per-unit.
- Small side/back-view sprites are useful later for motion/turn indicators, but the large front-facing standees are better for the first portfolio screenshot pass.

## Battle UI layout target

Reference hierarchy to adapt:

1. Top slim HUD
   - Stage/run label, resource/currency, objective text.
   - Small Auto / speed / pause controls on the right.
2. Left party roster
   - 3–5 compact character cards.
   - Portrait/standee crop, name, HP/MP or AP bars, status line.
3. Center battlefield
   - Dark forest/ruins background.
   - Isometric or diamond tactical grid.
   - Ally/enemy standees grounded with base rings and shadows.
   - Keep this area less cluttered than the side panels.
4. Right skill/intent panel
   - Enemy intent or selected skill cards.
   - Large icon square, title, short description.
5. Bottom command/status strip
   - Selected unit summary, turn indicator, compact action buttons, party thumbnails.
   - Avoid showing global Attack/Guard/Skill buttons permanently; keep contextual commands after selecting an ally.

## Optimization/readability rules for implementation

- Use concise English/ASCII runtime labels until a reliable Korean TMP font/icon pipeline is added.
- Reduce always-visible text. Put long skill explanations in a selected skill/intent card.
- Disable raycast targets on decorative images and non-interactive TMP labels.
- Update runtime UI text/bars only when values change.
- Validate generated scene regions and RectTransform bounds after every layout change.
- Capture a 1920x1080 screenshot/contact sheet and visually inspect it before calling the UI pass complete.

## Recommended next implementation batch

Batch 92 can become: **Reference-driven battle UI adoption pass**.

Scope:

1. Import these reference files into the project as documentation/reference assets.
2. Update `BattleSceneAutoBuilder` so the generated battle scene moves closer to the reference:
   - darker top HUD,
   - left party card stack,
   - cleaner center grid,
   - right intent/skill cards,
   - bottom selected-unit command strip.
3. Preserve current selected-ally contextual command behavior.
4. Add validator checks for all major UI regions.
5. Regenerate battle scene, run battle validation/auto-test, refresh captures, inspect contact sheet.

Acceptance criteria:

- Battle screen no longer reads as a generic test scene.
- Character standees remain the focus, with side panels supporting rather than covering them.
- `CreateBattleTestScene`, `ValidateBattleTestScene`, `RunBattleLogicAutoTest`, capture refresh, and `git diff --check` pass.
