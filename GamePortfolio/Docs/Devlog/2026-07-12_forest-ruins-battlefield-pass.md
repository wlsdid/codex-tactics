# Forest Ruins Battlefield Pass (2026-07-12)

## Changes

- Added low-contrast forest-ruins terrain props: ground ridge, left pillar, right obelisk, moss accents, and a fallen slab.
- Removed rectangular capture-only lighting, landing-tile, rim-bar, blade-bar, and slash overlays that competed with the pixel standees.
- Kept shadows, base rings, contact glows, forest silhouette, and compact tactical HUD as the readable core.

## Verification

- Battle scene create/validate: PASS.
- Battle logic auto test: PASS.
- Standalone 1920x1080 capture sequence and README/runtime GIF assets refreshed.
- `git diff --check`: PASS.

## Follow-up

The scene now reads cleaner, but the party roster still has more information density than a commercial reference. The next visual batch should simplify roster rows and use stronger authored background art rather than additional UI decoration.
