# Devlog - 2026-05-12 Stage Progress UI

## What changed

- Added a visible Stage Progress label to the generated battle scene.
- The label shows encounter count and current result state, for example `Progress: Encounter 1/2 | Active`.
- Victory, Defeat, Continue, and Final Clear states now update the progress label.
- Updated the scene builder, scene validator, and battle logic auto-test expectations.

## Why

The stage objective text already described what to do, but the player also needed a quick way to see how far they are through the current vertical slice. A separate progress line makes the two-encounter structure easier to understand in screenshots and GIFs.

## Manual test focus

- Start: `Progress: Encounter 1/2 | Active`
- First Victory: `Progress: Encounter 1/2 | Encounter Clear`
- Boss start after Continue: `Progress: Encounter 2/2 | Active`
- Final Clear: `Progress: Encounter 2/2 | Stage Clear`
