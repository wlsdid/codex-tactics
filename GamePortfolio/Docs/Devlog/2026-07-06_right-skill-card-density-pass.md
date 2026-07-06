# 2026-07-06 Right Skill Card Density Pass

## Goal

Continue visual-only polish. The next weakest screenshot area is the right-side skill/reference rail: it contains useful proof UI, but if it is too large or flat it competes with the central battlefield and makes the screen feel like a prototype overlay.

## Changed

- Compressed the right-side progress/reference skill cards from taller proof blocks into tighter commercial cards.
- Added authored icon-frame treatment:
  - element-tinted frame,
  - icon glow,
  - gold top edge,
  - card top highlight and bottom shade.
- Reduced skill-card label sizes so the central characters and battlefield remain the first visual read.
- Updated BattleScene validation to check compact card density and framed skill icons.

## Why

BrownDust2-style tactical UI relies on dense but polished side rails. The side UI should support the battlefield, not dominate it. This pass keeps the proof points while making the right rail less noisy and more authored.

## Verification note

Static checks verify source structure and validator coverage. Unity scene regeneration and contact-sheet QA are still required once the Windows Unity license is active.
