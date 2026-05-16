# GamePortfolio - 2D Turn-Based RPG Prototype

Unity 2D 턴제 RPG 포트폴리오용 전투 프로토타입입니다. 현재 목표는 작은 전투 루프를 안정적으로 만들고, 포트폴리오에서 설명하기 좋은 시스템을 단계적으로 추가하는 것입니다.

현재 구현: 타이틀 → 스테이지 선택 → 전투 → 결과 → 재시작/다음 → 스테이지 선택 복귀의 완전한 게임 루프.  
3개 스테이지(각 2개 encounter), Stage 1만 플레이 가능, Stage 2/3은 순차 클리어 시 잠금 해제.

> 영문 README: [`README.EN.md`](README.EN.md)

## 시스템 요구사항

- **Unity:** 6000.4.6f1 (또는 호환 버전)
- **플랫폼:** Windows (WSL에서도 Unity Editor batchmode 테스트 가능)
- **저장공간:** ~100MB (프로젝트 + 라이브러리)

## 빠른 시작

1. Unity Hub에서 이 프로젝트 폴더를 추가/엽니다
2. Unity 상단 메뉴바에서 `Tools > Codex Tactics > Create Game Flow Scenes` 클릭
3. Assets/Scenes/에 TitleScene, StageSelectScene, BattleScene이 생성되었는지 확인
4. Build Settings에 세 씬이 등록되었는지 확인 (Scene In Build 체크)
5. 상단 가운데 `▶ Play` 버튼 클릭 → TitleScene부터 시작
6. "Start Game" 클릭 → StageSelectScene
7. Stage 1 카드 선택 후 "Start Battle" → BattleScene
8. 전투 플레이 후 Victory/Defeat → Stage Select로 복귀

> **주의:** UI가 바뀐 뒤에는 Unity에서 `Tools > Codex Tactics > Create Game Flow Scenes`를 다시 실행해 씬을 재생성하세요.

## 현재 구현된 핵심 기능

### 게임 흐름
- **타이틀 → 스테이지 선택 → 배틀 → 결과 → 재시작/다음/스테이지 선택 복귀**
  - `GameSceneFlow.cs`가 씬 간 내비게이션 관리
  - GameFlowSceneAutoBuilder로 Title/StageSelect 씬 자동 생성
  - Build Settings에 필요한 씬 자동 등록

### 스테이지 선택
- **Stage 1 (Slime Scout Route):** 항상 선택 가능
- **Stage 2 (Wolf Ambush):** Stage 1 클리어 시 잠금 해제
- **Stage 3 (Golem Depths):** 데이터 준비됨, 아직 잠김
- 3가지 시각 상태: 선택됨 (하이라이트) / 기본 / 잠김 (회색 + Locked 라벨)
- 스테이지 카드 클릭 → 설명 패널 갱신
- "Start Battle" 버튼은 유효한 선택 시에만 활성화

### 전투 시스템
- **플레이어:** HP (100), AP (3), 일반 공격 (Slash, 20), 화염 스킬 (Fire Bolt, 30)
- **적 패턴:** 일반/강공격 (3턴 주기), 약점 속성, Break 게이지
- **전략 요소:** AP 관리, Guard (피해 50% 감소), Burn 상태 이상 (턴당 피해), Weakness 보너스 피해 (+10)
- **Enemy Intent UI:** 다음 적 행동을 미리 표시
- **Stage UI:** Run Status, 현재 encounter, Objective, Progress 라벨
- **전투 로그:** Recent Actions 패널에 최근 행동 기록

### 결과 요약
- **Victory/Defeat** 시: 턴 수, HP/AP, 피해량/방어/스킬 사용 횟수, 랭크(S/A/B/C), 보상 골드, 누적 골드, 팁, 마지막 적 패턴 표시
- **Continue:** Victory 후 "Next Encounter" → 다음 encounter로 진행
- **Retry:** 현재 encounter 재시작
- **Stage Select:** 전투 후 스테이지 선택 화면으로 복귀

### 스테이지 데이터 (StageData)
| 스테이지 | 인덱스 | Normal | Boss |
|----------|--------|--------|------|
| Stage 1: Slime Scout Route | 0 | Slime Scout (80HP, Fire 약점) | Slime King (140HP, Fire 약점) |
| Stage 2: Wolf Ambush | 1 | Wolf Scout (100HP, Nature 약점) | Alpha Wolf (180HP, Nature 약점) |
| Stage 3: Golem Depths | 2 | Golem Sentry (120HP, Earth 약점) | Ancient Golem (220HP, Earth 약점) |

### 진행 상태 (ProgressState)
- 정적(static) 클래스로 씬 전환 간 상태 유지
- Stage 0 → 클리어 → Stage 1 잠금 해제 → 클리어 → Stage 2 잠금 해제
- Reset() 메서드로 테스트 시 초기화 가능
- 추후 저장/로드 시스템으로 대체 가능

## Unity 검증 메뉴

| 메뉴 | 설명 |
|------|------|
| `Tools > Codex Tactics > Create Game Flow Scenes` | Title/StageSelect/Battle 씬 생성/갱신 |
| `Tools > Codex Tactics > Validate Game Flow Scenes` | 씬 구성 + UI 연결 + 버튼 리스너 검증 (36체크) |
| `Tools > Codex Tactics > Run Battle Logic Auto Test` | 전투 로직 + ProgressState + Stage 데이터 자동 테스트 (221체크) |

수동 검증/캡처 절차: `Docs/ManualValidationAndCaptureGuide.md`  
체크박스형 기록지: `Docs/ManualUnityValidationChecklist.md`

## 주요 코드 위치

| 폴더 | 파일 | 설명 |
|------|------|------|
| `Assets/Scripts/Flow/` | `GameSceneFlow.cs` | 씬 내비게이션 (Title/StageSelect/Battle) |
| | `StageSelectController.cs` | 스테이지 카드 선택/잠금/Start Battle |
| `Assets/Scripts/Battle/` | `BattleManager.cs` | 전투 상태 기계, 턴 흐름 |
| | `BattleUI.cs` | UI 렌더링 분리 |
| | `BattleResultData.cs` | 결과 요약 데이터 |
| | `BattleResultEvaluator.cs` | 랭크/보상/팁 평가 규칙 |
| | `BattleResultPresenter.cs` | 결과 요약 포맷/표시 문장 |
| | `BattleState.cs` | 전투 상태 enum |
| `Assets/Scripts/Data/` | `StageData.cs` | 스테이지/encounter 데이터 (3스테이지) |
| | `EnemyData.cs` | 적 데이터 |
| | `CharacterData.cs` | 캐릭터/적 공통 데이터 |
| | `SkillData.cs` | 스킬 데이터 |
| | `EnemyPatternData.cs` | 적 행동 패턴 (일반/강공격) |
| | `ElementType.cs` | 속성 enum (None/Physical/Fire/Ice/Lightning/Nature/Dark/Light/Earth) |
| `Assets/Scripts/` | `ProgressState.cs` | 진행 상태 (잠금 해제) |
| `Assets/Editor/` | `BattleSceneAutoBuilder.cs` | 배틀 씬 생성/검증 |
| | `BattleAutoTestRunner.cs` | 전투 로직 자동 테스트 (221체크) |
| | `GameFlowSceneAutoBuilder.cs` | Title/StageSelect 씬 생성/검증 (36체크) |
| `Docs/` | `BalanceTable.md` | 밸런싱 기록 |
| | `Captures/` | README용 스크린샷/GIF |

## 포트폴리오 설명 포인트

- **데이터 기반 아키텍처:** StageData/EnemyData/EnemyPatternData로 전투 값 분리, 새로운 스테이지나 적을 데이터만 추가해 확장 가능
- **명확한 상태 흐름:** Title → StageSelect → PlayerTurn → EnemyTurn → Victory/Defeat → Continue/Retry → StageSelect로 이어지는 완전한 게임 루프
- **진행 관리 시스템:** ProgressState 정적 클래스로 스테이지 잠금 해제 관리, Scene 전환 간 상태 유지
- **3계층 스테이지 확장:** 각 스테이지 2개의 encounter(Normal + Boss), 점진적 난이도, 플레이어가 이전 스테이지를 클리어해야 다음 스테이지 진입 가능
- **Stage Select UX:** 카드 기반 선택, 선택/기본/잠김 시각 상태, 설명 패널 연동, 잠금 해제 조건 직관적 표시
- **전략적 턴제 전투:** AP 관리, 속성 약점, Break 게이지, Guard, Burn 상태 이상, 적 패턴 예측
- **Enemy Intent UI:** 플레이어가 다음 적 행동을 보고 공격/방어/스킬을 선택하게 만드는 UX
- **Run Status + Objective + Progress UI:** 현재 진행 상태를 여러 각도에서 안내하는 계층적 UX
- **결과 요약 UI:** 전투 후 최종 상태, 누적 피해량, 선택 횟수, 랭크/보상/누적 골드를 compact하게 표시
- **Editor 자동화:** 씬 생성/검증 + 전투 로직 단위 테스트로 회귀 방지
- **역할 분리 아키텍처:** BattleManager(전투 상태) / BattleUI(표시) / BattleResultEvaluator(평가) / BattleResultPresenter(포맷)로 각 책임 분리
- **확장 가능한 설계:** ProgressState → 저장/로드, 새로운 ElementType 추가, Stage 3+ 데이터 추가 등 포트폴리오에서 "다음 단계"를 설명하기 쉬운 구조

## 수동 플레이 테스트

1. Unity Play Mode → TitleScene에서 Start
2. StageSelectScene에서 Stage 1 카드 선택 → 설명 확인 → Start Battle
3. 전투: Attack / Fire Skill / Guard / End Turn 조작
4. Victory 후 결과 확인 → **Next Encounter** 버튼으로 boss 진행
5. Boss Victory 후 Stage Select 버튼 → StageSelectScene 복귀
6. Stage 2: 처음에는 Locked → Stage 1을 한 번 더 클리어하면 잠금 해제
7. "Stage Select" 버튼 → BattleScene에서도 StageSelectScene으로 복귀 가능

## 주의

- 기존 씬은 코드 수정만으로 자동 갱신되지 않습니다. UI가 바뀐 뒤에는 Unity에서 `Tools > Codex Tactics > Create Game Flow Scenes`를 다시 실행하세요.
- ProgressState는 정적 클래스이므로 Play Mode 종료 시 초기화됩니다. 지속 저장이 필요하면 Save/Load 시스템을 추가하세요.
