# 2026-07-06 — Battle Cinematic Density Pass

## Goal

Continue without asking for a feature choice: keep reducing prototype/debug impressions and make the battle screenshot read more like a commercial tactical RPG composition.

## Changed

- Added validator expectations for the new cinematic lighting layer so future Unity scene generation checks for hero/enemy spotlights, center clash glow, and floor highlight.
- Updated standee validation thresholds to match the newly enlarged hero/enemy characters.
- Compressed the right-side reference skill cards: smaller card height, smaller icons, softer alpha, and reduced typography size.
- Kept the change art/UI-only; no combat rules or save data changed.

## Verification

- C# brace/parenthesis balance check: PASS.
- `git diff --check`: PASS.
- Unity batch validation still requires Windows Unity license activation before scene regeneration/capture refresh can run.

## Next

Once Unity is activated, regenerate the battle scene and captures. If contact-sheet QA still looks crowded, continue with an art-only pass on side-panel density and stronger skill impact VFX.
