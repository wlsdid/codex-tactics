# Skill Help UI Notes

Date: 2026-05-09

## Goal

Add a simple help/tooltip area that explains what each battle action does without requiring custom tooltip popups yet.

## Applied Unity/C# concepts

- A `TMP_Text` field can be serialized in `BattleManager` and linked by an editor builder script.
- Help text can be generated from existing skill values so UI explanations stay close to gameplay data.
- Editor validation can check object references before entering Play Mode.

## Current help lines

- `Slash`: power, AP cost, element, and no-cost role.
- `Fire Bolt`: power, AP cost, element, Burn effect, and weakness role.
- `Guard`: damage reduction percentage.
- Enemy pattern: every 3rd enemy turn uses `Heavy Slam`.

## Next improvement idea

Later, convert this into true hover/click tooltips using `EventTrigger` or Unity UI pointer interfaces. For now, always-visible help text is safer and easier to test for a beginner portfolio prototype.
