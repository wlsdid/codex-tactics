# Devlog — 2026-05-30 Hit Feedback VFX Pass

## Goal
Make the current battle loop feel better in a short portfolio GIF by strengthening visible hit feedback without adding a large new combat system.

## Changed
- Tuned `SkillProjectile` so each element has clearer projectile timing, trail padding, pulse, impact spark size, impact ring size, burst spark count, and screen shake strength.
- Added an impact ring at the hit point so Fire/Ice/Lightning/Earth attacks read better in screenshots and short captures.
- Added a small public debug profile for projectile feedback values so the editor auto-test can catch accidental regression.
- Extended `Run Battle Logic Auto Test` with checks for Fire, Lightning, and Ice feedback profiles.

## Manual test checklist
1. Unity 상단 메뉴 `Tools > Codex Tactics > Create Battle Test Scene` 실행.
2. `Tools > Codex Tactics > Run Battle Logic Auto Test`가 `RESULT: PASS`인지 확인.
3. Play 모드에서 Hero 클릭 후 Fire/Ice/Lightning을 눌러 hit ring, spark burst, screen shake가 보이는지 확인.
4. 짧은 GIF를 찍을 때는 Fire weakness hit 또는 Lightning hit 순간을 중심으로 캡처.

## Portfolio note
This is a polish batch, not a new mechanic. It improves the first impression of the already-working battle loop by making actions more readable and GIF-friendly.
