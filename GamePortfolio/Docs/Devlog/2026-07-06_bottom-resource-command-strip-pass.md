# 2026-07-06 Bottom Resource Command Strip Pass

## Goal

Continue visual-only polish. The next weak area was the bottom resource/command region: HP/AP/status labels were functionally present, but they still read like loose debug text rather than part of a designed tactical-RPG HUD.

## Changed

- Added a dedicated bottom resource strip behind the player HP/AP/status cluster.
- Framed HP and AP as separate chips with color-coded side edges.
- Added strip top highlight, bottom depth, and a subtle separator to increase material hierarchy.
- Reduced HP/AP/status font sizes so the bottom area supports the battlefield instead of competing with it.
- Framed the “Select Hero to open commands” hint as a subdued command chip with a gold edge.
- Added BattleScene validation for the bottom resource strip, HP/AP chips, and command hint chip.

## Portfolio rationale

Commercial tactical RPG screenshots usually have dense bottom information, but it is grouped into clear materials and chips. This pass makes the bottom UI feel intentionally authored while preserving the existing runtime bindings and validator-required text tokens.

## Verification note

Static checks verify code structure and whitespace. Unity scene regeneration/capture QA is still blocked until the Windows Unity license is active.
