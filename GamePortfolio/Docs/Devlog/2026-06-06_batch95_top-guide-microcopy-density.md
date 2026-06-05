# 2026-06-06 — Batch 95 Top Guide Microcopy Density Pass

## 목표

README 축소 이미지에서 상단 설명 UI가 중앙 전장과 캐릭터 시선을 빼앗지 않도록 Battle HUD의 top guide / run status / small reviewer chips를 더 짧고 낮은 밀도로 정리했다.

## 변경

- `BattleSceneAutoBuilder`의 상단 패널 높이와 텍스트 박스를 줄였다.
- 긴 문구를 compact ASCII microcopy로 교체했다.
  - `Break -> flank.`
  - `Push = +25% HP dmg.`
  - `Grid / intent`
  - `Cost 3 / Chain`
  - `HERO > FIRE > GUARD > WIN`
  - `1/5 CLICK HERO`
- `BattleUI` 런타임 stage/run labels도 짧은 표기로 맞췄다.
  - `Run: Active`, `Run: Clear -> Next`, `Run: Retry`, `Run: Stage Clear`
  - `Goal: Defeat ...`, `Enc 1/2 | Active`
- `BattleSceneAutoBuilder` validator와 `BattleAutoTestRunner` 기대치를 새 compact string에 맞춰 갱신했다.
- `BattleScene.unity`를 batchmode로 재생성했다.
- CaptureRunner를 다시 빌드/실행해 `Docs/Captures/` PNG, README GIF, runtime storyboard GIF, contact sheet를 갱신했다.

## 검증

- 시작 상태: `## main...origin/main`, root `screenshots/`만 미추적.
- C# brace balance check: PASS.
- Unity `BattleSceneAutoBuilder.CreateBattleTestScene`: PASS.
- Unity `BattleSceneAutoBuilder.ValidateBattleTestScene`: PASS (`RESULT: PASS`).
- Unity `BattleAutoTestRunner.RunBattleLogicAutoTest`: PASS (`RESULT: PASS`).
- Unity `CaptureScreenshots.Run`: PASS (`Build succeeded`).
- Standalone `CaptureRunner.exe` PNG refresh: PASS — battle PNGs 1920x1080, refreshed at 2026-06-06 00:16 KST.
- README GIF rebuild: PASS — 960x540, 7 frames.
- Runtime storyboard rebuild: PASS — 960x540, 29 frames.
- `capture_contact_sheet.png`: PASS — 1024x694, refreshed at 2026-06-06 00:17 KST.
- Contact-sheet visual QA: PASS — captures are non-blank, top guide text is now small, and the central battlefield/characters remain readable at README thumbnail size.

## 다음 추천

상단/중앙 밀도는 충분히 줄었으므로 다음 배치는 캐릭터 standee와 roster mini-sprite의 배경/투명도 가장자리를 더 정리해 캡처 품질을 올리는 것이 좋다.
