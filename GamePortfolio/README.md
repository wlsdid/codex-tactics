# GamePortfolio - 2D Turn-Based RPG Prototype

Unity 2D 턴제 RPG 포트폴리오용 전투 프로토타입입니다. 현재 목표는 작은 전투 루프를 안정적으로 만들고, 포트폴리오에서 설명하기 좋은 시스템을 단계적으로 추가하는 것입니다.

## 현재 구현된 핵심 기능

- 플레이어/적 HP 텍스트 표시(현재/최대치와 퍼센트)
- Stage UI: 현재 encounter를 `Stage 1-1: Slime Scout`처럼 표시하고, 별도 Objective 라벨로 `Objective: Defeat Slime Scout` 목표를 안내
- StageData/EnemyData 기반의 2단계 전투 구성: 일반 슬라임 encounter 후 슬라임 킹 boss encounter
- Battle Guide 조작 힌트 UI: Attack, Fire Skill/Burn, Guard, Enemy Intent, Continue, Retry 흐름 안내
- 플레이어/적 HP 바 슬라이더 표시
- 플레이어 AP 텍스트/바 슬라이더 표시(현재/최대치와 퍼센트) 및 턴 시작 시 AP 회복
- 플레이어 상태 UI: 평상시 `Status: Ready`, 방어 선택 후 `Status: Guarding`
- 기본 공격 `Slash`
- AP를 소비하는 화염 스킬 `Fire Bolt`
- 적 약점 속성 보너스 피해
- 화상 `Burn` 상태 이상과 턴당 피해
- 방어 `Guard`: 다음 적 공격 피해 감소
- 적 AI 패턴: `EnemyPatternData`에 일반 공격/강공격 이름, 피해량, 발동 턴을 모아 관리
- 일반 공격은 15 피해, 3번째 적 턴마다 강공격 `Heavy Slam`은 30 피해
- 다음 적 행동을 미리 알려주는 Enemy Intent UI
- `Recent Actions` 제목과 어두운 배경 패널이 있는 최근 행동 전투 로그
- 스킬/행동 도움말 UI
- Victory/Defeat 후 전투를 다시 시작하는 `Retry` 버튼
- Victory 후 다음 encounter로 넘어가는 `Continue` 버튼: Stage 1-1 일반 슬라임에서 Stage 1-2 슬라임 킹으로 진행, 마지막 승리 시 Final Clear 표시
- Victory/Defeat 후 결과/턴 수, 최종 HP/AP, 피해량, 선택 횟수, 클리어 속도/생존율, 전투 랭크/이번 보상/누적 골드, 플레이 팁, 마지막 적 패턴을 compact하게 보여주는 결과 요약 UI
- 결과 요약 UI 뒤에 반투명 배경 패널을 표시해 전투 로그와 구분
- Unity Editor 메뉴 기반 테스트 씬 생성/검증/전투 로직 자동 테스트

## Unity에서 실행하는 방법

1. Unity Hub에서 이 프로젝트를 엽니다.
2. Unity 상단 메뉴바에서 `Tools > Codex Tactics > Create Battle Test Scene` 클릭
3. 생성 완료 팝업 확인 후 `Assets/Scenes/BattleScene.unity`가 열려 있는지 확인
4. 상단 가운데 `▶ Play` 버튼 클릭
5. 상단의 `Battle Guide` 라벨에서 `Attack`, `Fire Skill`, `Guard`, `Enemy Intent`, `Continue`, `Retry` 흐름을 먼저 확인
6. 현재 스테이지 라벨이 `Stage 1-1: Slime Scout`로 표시되고 목표 라벨이 `Objective: Defeat Slime Scout`로 표시되는지 확인
7. `Attack`, `Fire Skill`, `Guard`, `End Turn` 버튼으로 전투 테스트
8. 공격/방어 후 플레이어/적 HP 바가 HP 텍스트와 함께 줄어들고, 괄호 안 퍼센트도 함께 바뀌는지 확인
9. `Guard`를 누르면 플레이어 쪽 상태가 `Status: Guarding`으로 바뀌고, 적 공격을 받은 뒤 `Status: Ready`로 돌아오는지 확인
10. 적 HP 아래 `Next Enemy: Normal Attack (15)` 또는 `Next Enemy: Heavy Slam (30)` 의도 표시가 갱신되는지 확인
11. `Fire Skill` 사용 후 플레이어 AP 바가 `3/3 (100%) -> 1/3 (33%)`처럼 줄어드는지 확인
12. 화면 하단 `Recent Actions` 패널에 플레이어 턴, Fire Skill, Guard, 적 공격 같은 최근 행동이 누적되는지 확인
13. Victory/Defeat 후 결과 요약 UI에 `Damage: dealt ..., taken ...` / `Choices: Guard ..., Skills ...` / `Pace: ... | Survival: ...` / `Rank: ... | Reward: ...G | Total Gold: ...G`처럼 묶인 compact 문구와 `Tip`이 함께 나타나고, 뒤에 어두운 패널이 표시되는지 확인
14. 첫 Victory 후 목표 라벨이 `Objective Complete: Stage 1-1: Slime Scout | Continue to next encounter`로 바뀌는지 확인
15. `Continue` 버튼을 눌러 `Stage 1-2: Slime King` boss encounter로 넘어가고 목표가 `Objective: Defeat Slime King`으로 갱신되는지 확인
16. boss Victory 후 메시지가 `Final Clear! Stage 1 completed.`로 바뀌고 목표 라벨이 `Objective Complete: Stage 1 cleared`로 바뀌며 `Continue`는 숨겨지는지 확인
17. 나타나는 `Retry` 버튼으로 현재 encounter 재시작 테스트
18. 재시작 후 결과 요약 UI가 사라지고 HP/AP 바와 플레이어 상태, 전투 로그가 초기 상태로 돌아오는지 확인

## Unity에서 검증하는 메뉴

- 테스트 씬 생성/갱신: `Tools > Codex Tactics > Create Battle Test Scene`
- 씬 구성 검증: `Tools > Codex Tactics > Validate Battle Test Scene`
- 전투 로직 자동 테스트: `Tools > Codex Tactics > Run Battle Logic Auto Test`
- 수동 검증/스크린샷/GIF 캡처 절차: `Docs/ManualValidationAndCaptureGuide.md`
- 체크박스형 Unity 수동 검증 기록지: `Docs/ManualUnityValidationChecklist.md`

## 주요 코드 위치

- 전투 흐름: `Assets/Scripts/Battle/BattleManager.cs`
- Stage encounter 데이터: `Assets/Scripts/Data/StageData.cs`
- 적 데이터: `Assets/Scripts/Data/EnemyData.cs`
- 결과 요약 데이터: `Assets/Scripts/Battle/BattleResultData.cs`
- 결과 평가 규칙: `Assets/Scripts/Battle/BattleResultEvaluator.cs`
- 결과 요약 포맷/표시 문장: `Assets/Scripts/Battle/BattleResultPresenter.cs`
- 전투 상태 enum: `Assets/Scripts/Battle/BattleState.cs`
- 캐릭터 데이터: `Assets/Scripts/Data/CharacterData.cs`
- 스킬 데이터: `Assets/Scripts/Data/SkillData.cs`
- 적 행동 패턴 데이터: `Assets/Scripts/Data/EnemyPatternData.cs`
- 밸런싱 기록: `Docs/BalanceTable.md`
- 포트폴리오 쇼케이스 초안: `Docs/PortfolioShowcaseDraft.md`
- 수동 검증/캡처 가이드: `Docs/ManualValidationAndCaptureGuide.md`
- Unity 수동 검증 체크리스트: `Docs/ManualUnityValidationChecklist.md`
- README용 스크린샷/GIF 저장 폴더: `Docs/Captures/`
- 테스트 씬 생성/검증: `Assets/Editor/BattleSceneAutoBuilder.cs`
- 전투 로직 자동 테스트: `Assets/Editor/BattleAutoTestRunner.cs`

## 포트폴리오 설명 포인트

- 데이터 기반으로 캐릭터/스킬/적 패턴/스테이지 encounter 값을 분리하려는 구조
- `PlayerTurn -> EnemyTurn -> Victory/Defeat -> Continue/Retry` 형태의 명확한 전투 상태 흐름
- Stage 1-1 일반 encounter에서 Stage 1-2 boss encounter로 이어지는 작은 vertical slice 진행 구조와 현재 목표/완료 상태를 분리해 보여주는 Objective UI
- AP, 상태 이상, 방어, 일반 공격/강공격 적 패턴 등 턴제 RPG의 기본 전략 요소
- Enemy Intent UI로 플레이어가 다음 적 공격을 보고 공격/방어 선택을 고민하게 만드는 UX
- Battle Guide 라벨로 처음 보는 플레이어가 Attack, Fire Skill/Burn, Guard, Enemy Intent, Continue, Retry 흐름을 바로 알 수 있게 만든 온보딩 UX
- Player Status UI로 `Guarding` 상태를 즉시 보여줘 방어가 적용 중인지 명확히 전달하는 UX
- HP/AP 텍스트에 현재/최대치와 퍼센트를 함께 표시하고 리소스 바를 같이 갱신해 전투 상태를 빠르게 읽을 수 있게 만든 UX
- `Recent Actions` 전투 로그 패널로 최근 행동 흐름을 별도 영역에서 따라갈 수 있게 만든 UX
- 결과 요약 UI로 플레이어가 전투 후 최종 상태, 누적 피해량, 방어/스킬 사용 횟수, 클리어 속도/생존율, 전투 랭크/이번 보상/누적 골드를 짧게 묶어 읽고 플레이 팁과 적 패턴을 되돌아볼 수 있는 UX
- `BattleResultData.cs`로 결과 요약 값을 모으고, `BattleResultEvaluator.cs`로 랭크/속도/생존율/보상/팁 규칙을 분리한 뒤, `BattleResultPresenter.cs`로 표시 문장 포맷을 분리해 결과 UI가 커져도 구조를 유지하기 쉽게 정리
- 결과 요약 패널 배경으로 전투 종료 정보가 battle log와 섞이지 않게 만든 UI 가독성
- Editor 자동화로 테스트 씬을 재생성하고 필수 UI 연결을 검증하는 방식
- `Docs/BalanceTable.md`로 HP/AP/피해량/적 패턴/랭크 기준의 의도를 기록하는 밸런싱 문서화
- `Docs/PortfolioShowcaseDraft.md`로 현재 구현 기능, 코드 구조, 검증 증거, 추후 캡처 대상을 포트폴리오 설명 초안으로 정리
- `Docs/ManualValidationAndCaptureGuide.md`로 Unity 수동 검증과 README용 스크린샷/GIF 확보 절차 문서화
- `Docs/ManualUnityValidationChecklist.md`로 Unity에서 직접 체크할 PASS/FAIL 기록지를 분리
- `Docs/Captures/` 폴더를 준비해 README용 스크린샷/GIF 파일 위치를 고정

## 주의

기존 씬은 코드 수정만으로 자동 갱신되지 않습니다. UI가 바뀐 뒤에는 Unity에서 `Tools > Codex Tactics > Create Battle Test Scene`을 다시 실행해 테스트 씬을 재생성하세요.
