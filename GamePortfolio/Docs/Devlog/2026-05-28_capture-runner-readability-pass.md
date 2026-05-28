# 2026-05-28 — Batch 77 Capture Runner Readability Pass

## 목표

Batch 76에서 치비 픽셀 스탠디는 적용됐지만, standalone capture가 `Builds/CaptureBuild/CaptureRunner_Data/Docs/Captures` 아래에 검은 화면으로 남는 문제가 있었다. README/포트폴리오에 바로 쓸 수 있도록 자동 캡처 루프를 안정화했다.

## 원인

- standalone player에서 `Application.dataPath`를 기준으로 저장하면 프로젝트 `Docs/Captures`가 아니라 빌드 산출물의 `CaptureRunner_Data/Docs/Captures`로 저장됐다.
- `ReadPixels`가 일반 coroutine tick에서 호출되어 `not inside drawing frame` 경고가 발생할 수 있었다.

## 변경

- `ScreenshotCaptureJob`에 `-captureOutputDir` 인자와 `CODEX_TACTICS_CAPTURE_DIR` 환경변수 지원 추가.
- 기본 fallback도 standalone `_Data` 폴더가 아니라 프로젝트 `Docs/Captures`로 되돌아가도록 보정.
- 모든 캡처를 `WaitForEndOfFrame` 이후 `ReadPixels` 하도록 변경.
- `CaptureScreenshots.Run`이 `_capture_args.txt`를 생성해 외부 실행 시 올바른 출력 폴더를 알 수 있게 했다.
- `Docs/Captures/capture_contact_sheet.png`를 최신 치비 픽셀 전투 화면 기준으로 재생성했다.

## 검증

```text
CaptureScreenshots.Run build: PASS
CaptureRunner.exe -capture -captureOutputDir Docs/Captures: PASS
PNG non-black/stat check: PASS
Contact sheet visual QA: PASS
BattleSceneAutoBuilder.ValidateBattleTestScene: RESULT: PASS
BattleAutoTestRunner.RunBattleLogicAutoTest: RESULT: PASS
GameFlowSceneAutoBuilder.ValidateGameFlowScenes: RESULT: PASS
```

## 포트폴리오 메모

- 최신 README contact sheet에서 Title, Stage Select, Battle Start, Fire/Burn, Guard, Result, Retry 흐름이 모두 보인다.
- 치비 픽셀 Hero/Enemy가 전투 화면 중앙에 안정적으로 찍히므로 다음 README/GIF 작업의 기준 이미지로 사용할 수 있다.

## 다음 후보

1. 파티원/적군 구분을 위한 치비 픽셀 베리에이션 2~3종 추가.
2. Fire/Ice/Lightning/Guard 타격 연출과 화면 흔들림을 GIF용으로 더 크게 강화.
3. README용 8~15초 GIF 생성.
