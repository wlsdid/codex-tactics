# 2026-05-28 — Batch 79 Mature Pixel Density Pass

## 목표

기존 치비 픽셀 캐릭터가 화면에서 너무 크고 유아틱하게 보이는 문제를 줄이기 위해, 도트 수는 늘리고 실제 전투 화면 점유 크기는 줄인 성숙한 tactical RPG 스타일로 재작업했다.

## 변경

- `Assets/Art/Generated/generate_chibi_pixel_standees.py` 재작성
  - `SCALE`을 3에서 2로 낮춰 최종 화면 확대감을 줄임.
  - 원본 캔버스 해상도를 키워 내부 도트 정보량을 늘림.
  - 큰 동그란 눈 대신 작은 눈매/각진 머리/긴 몸통/긴 다리 비율로 조정.
  - 색감을 더 낮은 채도와 금속/가죽 중심으로 조정해 덜 장난감처럼 보이게 함.
- 메인 스탠디 4종 재생성
  - `chibi_hero_original.png`: 256x320, 더 긴 검/갑옷/망토 실루엣.
  - `chibi_enemy_original.png`: 272x320, 보스형 왕관/뿔/날개 유지하되 몸통 비율 개선.
  - `chibi_ally_guardian.png`: 208x264, 방패 정체성 강화.
  - `chibi_enemy_raider.png`: 208x264, 도끼 실루엣 강화.
- `BattleSceneAutoBuilder.cs` 조정
  - 메인 Hero/Enemy 스탠디 표시 크기 축소.
  - roster 미니 스프라이트 표시 크기도 축소.
  - validator 문구와 크기 기준을 high-density mature pixel 기준으로 갱신.

## 검증

```text
BattleSceneAutoBuilder.CreateBattleTestScene: PASS
BattleSceneAutoBuilder.ValidateBattleTestScene: RESULT: PASS
BattleAutoTestRunner.RunBattleLogicAutoTest: RESULT: PASS
CaptureScreenshots.Run: PASS
CaptureRunner.exe portfolio capture: PASS
Contact sheet visual QA: PASS
```

## 시각 QA

- 최신 `Docs/Captures/capture_contact_sheet.png` 기준으로 battle start / fire / guard / result / retry 캡처가 모두 검은 화면이나 흰 화면 없이 정상 출력된다.
- 전장 유닛은 이전보다 작아졌고, 머리-몸 비율이 덜 유아틱하며, 픽셀 내부 정보량은 증가했다.
- roster의 작은 칩에서도 Hero/Ally/Enemy 정체성이 유지된다.

## 다음 후보

1. Fire/Ice/Lightning/Guard 타격 VFX와 screen shake를 더 강하게 만든다.
2. 스탠디에 idle bob / hit reaction을 추가해 정지 이미지를 덜 뻣뻣하게 만든다.
3. 최신 캡처 파이프라인으로 README용 8~15초 gameplay GIF를 만든다.
