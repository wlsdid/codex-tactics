# 2026-07-06 — Skill Cast Anticipation VFX Pass

## Goal

Continue without waiting for a Unity capture refresh by improving the runtime skill-feedback code that will be visible once the battle scene can be regenerated/captured again.

## Changed

- Added a short element-colored cast anticipation flare at the caster position before projectiles travel.
- Tuned the cast flare by element so Lightning reads fastest/largest, Fire is strong for GIF readability, and Ice stays smaller/slower.
- Extended `SkillProjectile.DebugImpactProfile` with a `Cast=` field so editor auto-tests can verify the new VFX profile without relying on visual inspection alone.
- Updated `BattleAutoTestRunner` expectations for Fire, Lightning, and Ice impact profiles.

## Why it matters

The battle screen already has mechanics, but professional-looking short gameplay clips need anticipation -> projectile -> hit -> recovery. This pass adds the anticipation beat, making skills easier to read in an 8-15 second portfolio GIF.

## Verification

- C# brace/parenthesis balance check: PASS.
- `git diff --check`: PASS.
- Unity runtime validation/capture still depends on restoring Windows Unity license activation.
