# 2026-05-28 — Batch 78 Chibi Pixel Art Improvement

## 목표

전투 화면의 Hero/Enemy 픽셀 스탠디가 더 명확한 캐릭터 실루엣을 갖도록 개선하고, 좌우 roster에도 작은 픽셀 초상화를 넣어 UI 전체의 아트 일관성을 높였다.

## 변경

- `Assets/Art/Generated/chibi_hero_original.png` 재생성
  - 큰 머리/작은 몸 비율 강화
  - 얼굴, 눈, 머리카락, 갑옷, 검 실루엣을 더 뚜렷하게 개선
  - 진한 외곽선과 제한 팔레트 유지
- `Assets/Art/Generated/chibi_enemy_original.png` 재생성
  - 보스형 적 실루엣, 왕관, 뿔, 날개, 눈 강조
  - 보라/금색 계열로 적 정체성 강화
- 추가 베리에이션 생성
  - `chibi_ally_guardian.png`
  - `chibi_enemy_raider.png`
- `BattleSceneAutoBuilder` 개선
  - party/enemy roster chip에 미니 픽셀 스프라이트 배치
  - validator가 메인 스탠디와 roster 미니 스프라이트의 Sprite 연결까지 확인하도록 확장
- `ScreenshotCaptureJob` 조정
  - Guard 캡처가 screen flash 중 찍히지 않도록 대기 시간 보정

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

- Hero/Enemy가 이전보다 더 큰 얼굴, 외곽선, 무기/왕관/날개 실루엣을 가진다.
- roster의 party/enemy slot에도 작은 픽셀 얼굴이 들어가 전투 UI가 덜 테스트 화면처럼 보인다.
- `03_guard_status.png`의 흰 화면 capture 문제가 해결됐다.

## 다음 후보

1. Fire/Ice/Lightning/Guard 타격 연출을 GIF용으로 더 과장한다.
2. 캐릭터 idle 흔들림 또는 hit reaction을 추가한다.
3. README용 8~15초 GIF를 만든다.
