# Batch 4 Plan — Title → Stage Select → Battle Loop Vertical Slice

**Goal:** 전투 시스템만 있는 테스트 씬에서 벗어나, 플레이어가 실제 게임처럼 `Title → Stage Select → Battle → Result → Next/Retry` 흐름을 경험하게 만든다.

**Role Split**
- codex5.5: 계획/리뷰/완료 기준 담당
- hemes-deep: Unity 구현/검증/수정 담당
- 포폴 문서 작성 금지. 구현 결과와 검증 결과만 보고.

---

## 작업 전 필수 순서

```bash
git pull --ff-only
git status --short --branch
git diff --stat
```

주의:
- `Docs/Batch2_CombatFeedback_Instructions.md`는 아직 untracked 상태면 이번 배치에도 커밋 제외.
- 기존 Unity 설정/ProjectSettings는 건드리지 말 것.
- `.unity` 직접 편집 금지. 필요하면 Editor Builder로 생성.

---

## 왜 Batch 4인가

Batch 1~3에서 전투 화면/피드백/Break 핵심 메커닉이 생겼다.
이제 다음 전문성은 **게임으로서의 진입 플로우**다.

포트폴리오 관점에서 중요한 변화:
- 단일 테스트 전투가 아니라 실제 플레이 가능한 수직 슬라이스처럼 보임.
- BrownDust2풍 고급 2D RPG 느낌에 맞게 Title/Stage Select UI를 붙일 수 있음.
- 영상 촬영 시 `게임 시작 → 스테이지 선택 → 전투 → 결과` 흐름을 보여줄 수 있음.

---

## 구현 범위

이번 배치는 크게 만들지 말고, **가벼운 씬 플로우**만 구현한다.

### 1. Title Scene 생성

추천 씬:
- `Assets/Scenes/TitleScene.unity`

필요 UI:
- 게임 타이틀: `Codex Tactics` 또는 현재 프로젝트명
- 서브카피: `Tactical Break RPG Prototype`
- `Start Game` 버튼
- `Quit` 버튼은 선택. Web/Editor 환경에서 애매하면 생략 가능.

스타일:
- 어두운 판타지 RPG 배경 패널
- 금색/청색 포인트
- BrownDust2 느낌 참고: 복제 금지, 고급 전술 RPG 분위기만 참고

---

### 2. Stage Select Scene 생성

추천 씬:
- `Assets/Scenes/StageSelectScene.unity`

필요 UI:
- Header: `Select Stage`
- Stage Card 1: `Slime Scout Route`
- Stage Card 2: `Wolf Ambush` 또는 Locked 상태 카드
- 선택된 스테이지 설명 패널
- `Start Battle` 버튼
- `Back` 버튼

최소 기능:
- Stage 1 카드 클릭 또는 기본 선택 상태
- `Start Battle` 클릭 시 기존 `BattleScene` 로드
- Stage 2는 Locked 텍스트만 보여도 됨

---

### 3. Scene Flow Controller 추가

추천 파일:
- `Assets/Scripts/Flow/GameSceneFlow.cs`

역할:
- `LoadTitle()`
- `LoadStageSelect()`
- `LoadBattle()`
- `QuitGame()` optional

주의:
- `UnityEngine.SceneManagement.SceneManager.LoadScene(...)` 사용
- 씬 이름 상수화
- 버튼에서 연결 가능하게 public 메서드 제공

---

### 4. Editor Builder 추가

추천 파일:
- `Assets/Editor/GameFlowSceneAutoBuilder.cs`

메뉴:
- `Tools > Codex Tactics > Create Game Flow Scenes`
- `Tools > Codex Tactics > Validate Game Flow Scenes`

Builder가 할 것:
- TitleScene 생성/저장
- StageSelectScene 생성/저장
- Canvas/EventSystem/Camera 구성
- 버튼 OnClick에 `GameSceneFlow` 메서드 연결
- 기존 BattleScene은 건드리지 않거나, 필요하면 Build Settings 검증만 진행

Validate가 확인할 것:
- `TitleScene.unity` 존재
- `StageSelectScene.unity` 존재
- Title에 `Start Game Button` 존재
- Stage Select에 `Start Battle Button` 존재
- `GameSceneFlow` 컴포넌트 존재
- 버튼 OnClick 연결 존재

---

### 5. Build Settings / Scene List 처리

가능하면 Editor script에서 build settings에 아래 씬을 등록:

1. `Assets/Scenes/TitleScene.unity`
2. `Assets/Scenes/StageSelectScene.unity`
3. `Assets/Scenes/BattleScene.unity`

주의:
- ProjectSettings를 불필요하게 많이 바꾸지 말 것.
- BuildSettings 변경이 큰 diff를 만들면, 이번 배치에서는 Validate로 씬 존재만 확인해도 됨.

---

## TDD / 검증 순서

### Step 1 — RED 검증 추가

새 Editor 검증 메서드를 먼저 만든다:

```csharp
[MenuItem("Tools/Codex Tactics/Validate Game Flow Scenes")]
public static void ValidateGameFlowScenes()
```

초기에는 Title/Stage 씬이 없으므로 FAIL 확인.

예상 체크:
- Title Scene exists
- Stage Select Scene exists
- Title Start button exists
- Stage Start Battle button exists
- GameSceneFlow component exists

RED 실행 예시:

```bash
'/mnt/c/Program Files/Unity/Hub/Editor/6000.4.6f1/Editor/Unity.exe' -batchmode -quit -projectPath 'C:\Users\jywls\Desktop\game_portfolio\GamePortfolio' -executeMethod GameFlowSceneAutoBuilder.ValidateGameFlowScenes -logFile 'C:\Users\jywls\AppData\Local\Temp\game_flow_batch4_red.log'
```

---

## 구현 순서

1. `GameSceneFlow.cs` 추가
2. `GameFlowSceneAutoBuilder.cs`에 Validate 먼저 작성
3. RED 실패 확인
4. Builder로 TitleScene / StageSelectScene 생성
5. 버튼 연결 구현
6. Validate PASS
7. 기존 Battle 검증도 회귀 테스트
8. `git diff --check` PASS
9. 리뷰 요청

---

## 필수 검증 명령

```bash
'/mnt/c/Program Files/Unity/Hub/Editor/6000.4.6f1/Editor/Unity.exe' -batchmode -quit -projectPath 'C:\Users\jywls\Desktop\game_portfolio\GamePortfolio' -executeMethod GameFlowSceneAutoBuilder.CreateGameFlowScenes -logFile 'C:\Users\jywls\AppData\Local\Temp\game_flow_create_batch4.log'
```

```bash
'/mnt/c/Program Files/Unity/Hub/Editor/6000.4.6f1/Editor/Unity.exe' -batchmode -quit -projectPath 'C:\Users\jywls\Desktop\game_portfolio\GamePortfolio' -executeMethod GameFlowSceneAutoBuilder.ValidateGameFlowScenes -logFile 'C:\Users\jywls\AppData\Local\Temp\game_flow_validate_batch4.log'
```

```bash
'/mnt/c/Program Files/Unity/Hub/Editor/6000.4.6f1/Editor/Unity.exe' -batchmode -quit -projectPath 'C:\Users\jywls\Desktop\game_portfolio\GamePortfolio' -executeMethod BattleAutoTestRunner.RunBattleLogicAutoTest -logFile 'C:\Users\jywls\AppData\Local\Temp\battle_logic_batch4_regression.log'
```

```bash
'/mnt/c/Program Files/Unity/Hub/Editor/6000.4.6f1/Editor/Unity.exe' -batchmode -quit -projectPath 'C:\Users\jywls\Desktop\game_portfolio\GamePortfolio' -executeMethod BattleSceneAutoBuilder.ValidateBattleTestScene -logFile 'C:\Users\jywls\AppData\Local\Temp\battle_scene_validate_batch4_regression.log'
```

```bash
git diff --check
```

---

## 완료 기준

Batch 4 완료 조건:

- `TitleScene.unity` 생성됨
- `StageSelectScene.unity` 생성됨
- Title의 `Start Game` 버튼이 Stage Select로 이동하도록 연결됨
- Stage Select의 `Start Battle` 버튼이 BattleScene으로 이동하도록 연결됨
- `ValidateGameFlowScenes`: `RESULT: PASS`
- 기존 `RunBattleLogicAutoTest`: `RESULT: PASS`
- 기존 `ValidateBattleTestScene`: `RESULT: PASS`
- `git diff --check`: PASS
- `Docs/Batch2_CombatFeedback_Instructions.md`는 커밋 제외

---

## 이번 배치에서 하지 말 것

- 저장/세이브 시스템
- 인벤토리/상점
- 캐릭터 성장
- 복잡한 스테이지 데이터 에디터
- 외부 에셋 import
- 포트폴리오 문서 작성

---

## 완료 보고 형식

```markdown
배치 4 구현 완료.

## 변경 파일
- ...

## 구현 내용
- TitleScene 생성
- StageSelectScene 생성
- GameSceneFlow 추가
- GameFlowSceneAutoBuilder 검증 추가

## 검증 결과
- CreateGameFlowScenes: PASS 또는 완료 로그
- ValidateGameFlowScenes: PASS
- RunBattleLogicAutoTest: PASS
- ValidateBattleTestScene: PASS
- git diff --check: PASS

## 커밋 전 주의사항
- Batch2 untracked 문서 제외 여부
- BuildSettings 변경 여부
- 남은 이슈
```

커밋은 codex5.5 리뷰 후 진행.
