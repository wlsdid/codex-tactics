# 09 Next Autonomous Tasks

이 문서는 자동 실행 작업이 매번 무엇을 했고, 다음에 어떤 작은 목표를 진행하면 좋은지 누적 기록하는 공간이다.

## 2026-05-08 04:33 KST

### 이번 실행에서 완료

- `CharacterData`에 현재 상태이상과 남은 턴을 저장하는 값 추가.
- `ApplyStatusEffect()`, `HasStatusEffect()`, `ReduceStatusTurn()` 함수 추가.
- Fire Skill 사용 시 적에게 `Burn`을 2턴 적용하도록 연결.
- 적 턴 시작 시 Burn 고정 피해 3을 먼저 처리하고, Burn 피해로 적이 죽으면 승리 처리.
- 선택 UI 연결 칸 `enemyStatusText` 추가: 적 상태와 남은 턴 표시용.
- Burn 상태이상 공부 노트와 개발일지 추가.
- README 현재 개발 단계에 Burn 상태이상 초안을 반영.

### 다음 추천 작업

1. **Unity Inspector 연결 체크리스트 보강**
   - `Enemy Status Text`를 어떤 TextMeshPro 오브젝트에 연결할지 단계별로 작성
   - Attack / Fire Skill / End Turn 버튼 테스트 순서 정리
   - Burn 피해가 적 턴 시작에 적용되는지 확인하는 수동 테스트 표 작성

2. **상태이상 구조 개선 후보 문서화**
   - 지금처럼 캐릭터당 상태이상 1개만 둘지 결정
   - 여러 상태를 동시에 저장하려면 `List`가 왜 필요한지 초보자용으로 설명

3. **간단한 적 AI 초안**
   - 아직 적은 매번 기본 공격만 함
   - HP가 낮을 때 다른 메시지 또는 강공격 패턴 후보 정리

### 주의

- 아직 Unity Editor에서 실제 씬 연결 검증은 하지 않았다.
- 자동 작업에서는 씬/프리팹 파일을 억지로 만들지 않고 C#과 문서만 확장한다.

## 2026-05-08 03:30 KST

### 이번 실행에서 완료

- `BattleManager`에 `endTurnButton` UI 연결 자리를 추가.
- `OnClickEndTurnButton()`은 버튼 입력만 받고, 실제 규칙은 `EndPlayerTurn()`에서 처리하도록 함수 분리.
- 턴 종료 시 모든 행동 버튼을 비활성화하고 적 턴 코루틴으로 넘어가도록 구현.
- 다음 플레이어 턴 시작 시 기존 AP 회복과 버튼 갱신 흐름을 재사용하도록 유지.
- End Turn 버튼 공부 노트와 개발일지 추가.
- README 현재 개발 단계에 전투 버튼 흐름 초안을 반영.

### 다음 추천 작업

1. **Burn 상태이상 실제 효과 1차 초안**
   - Fire Skill 사용 시 적에게 Burn을 기록
   - 적 턴 시작 또는 종료 시 고정 피해 3 적용
   - 메시지에 Burn 피해를 따로 표시

2. **상태 표시 UI 연결 가이드**
   - Enemy Status Text 연결 칸 추가 후보 정리
   - Burn, Poison 같은 상태를 문자열로 보여주는 방법 문서화

3. **Unity Editor 수동 테스트 체크리스트 보강**
   - Attack / Fire Skill / End Turn 버튼 연결 순서 작성
   - AP 부족, 승리, 패배 상황별 확인 항목 추가

### 주의

- 아직 Unity Editor에서 실제 씬 연결 검증은 하지 않았다.
- 자동 작업에서는 씬/프리팹 파일을 억지로 만들지 않고 C#과 문서만 확장한다.

## 2026-05-08 02:26 KST

### 이번 실행에서 완료

- `BattleManager`에 두 번째 스킬인 `fireSkill` 추가.
- 화염 스킬 Inspector 설정값 추가: 이름, 위력, AP 비용, 속성.
- `fireSkillButton` UI 연결 자리를 추가하되, 연결하지 않아도 오류가 나지 않도록 `null` 검사 유지.
- 기본 공격과 화염 스킬이 같은 전투 처리 흐름을 쓰도록 `UsePlayerSkill(SkillData skill)` 공통 함수 추가.
- AP가 부족한 스킬 버튼은 비활성화되도록 `UpdateActionButtons()` 추가.
- 화염 스킬은 `Burn` 상태이상 후보 메시지를 표시하도록 연결.
- 스킬 버튼 확장 공부 노트와 개발일지 추가.

### 다음 추천 작업

1. **턴 종료 버튼 초안**
   - 공격하지 않고 턴을 넘겨 AP를 모으는 선택지 만들기
   - `endTurnButton`을 추가하고 플레이어 턴에서만 활성화
   - 초보자가 이해하기 쉽게 `EndPlayerTurn()` 함수로 분리

2. **Burn 상태이상 실제 효과 1차 초안**
   - 아직은 메시지만 표시됨
   - 적에게 Burn이 걸렸는지 bool 또는 작은 데이터로 기록
   - 적 턴 시작/종료 시 고정 피해 3 적용

3. **Unity UI 연결 가이드 보강**
   - Attack Button / Fire Skill Button / Player Ap Text 연결 체크리스트 작성
   - Button OnClick을 코드에서 자동 등록한다는 점 설명

### 주의

- 아직 Unity Editor에서 실제 씬 연결 검증은 하지 않았다.
- 자동 작업에서는 씬/프리팹 파일을 억지로 만들지 않고 C#과 문서만 확장한다.

## 2026-05-08 01:22 KST

### 이번 실행에서 완료

- `CharacterData`에 `maxAp`, `currentAp` 추가.
- AP 검사/소모/회복 함수 추가: `HasEnoughAp()`, `SpendAp()`, `RecoverAp()`.
- `BattleManager`에 플레이어 최대 AP, 턴 시작 AP 회복량, 기본 스킬 AP 비용 설정값 추가.
- 플레이어 턴 시작 시 AP를 회복하고, 공격 전 AP 비용을 검사하도록 확장.
- AP 표시용 `playerApText` UI 연결 자리를 추가하되, 연결하지 않아도 동작하도록 `null` 검사를 유지.
- AP 시스템 공부 노트와 개발일지 추가.

### 다음 추천 작업

1. **강한 스킬 버튼 추가**
   - 기본 공격은 AP 0 유지
   - 화염 스킬은 AP 2 소모로 추가
   - `SkillData`를 하나 더 만들어 같은 데미지 계산 흐름을 재사용

2. **Unity Inspector 연결 가이드 보강**
   - `Player Ap Text`를 어떤 TextMeshPro 오브젝트에 연결하는지 단계별 설명
   - 버튼 OnClick 연결법 정리

3. **턴 종료 버튼 초안**
   - 공격하지 않고 턴을 넘겨 AP를 모으는 선택지 만들기
   - 버튼이 늘어나도 `BattleManager`가 너무 복잡해지지 않게 작은 함수로 분리

### 주의

- 아직 Unity Editor에서 실제 씬 연결 검증은 하지 않았다.
- 자동 작업에서는 씬/프리팹 파일을 억지로 만들지 않고 C#과 문서만 확장한다.

## 2026-05-08 00:19 KST

### 이번 실행에서 완료

- `SkillData` 추가: 스킬 이름, 설명, 위력, AP 비용, 속성, 상태이상 타입을 묶는 데이터 class.
- `ElementType` 추가: Physical, Fire, Ice 등 속성 enum.
- `StatusEffectType` 추가: Poison, Burn, Stun 등 상태이상 enum.
- `CharacterData`에 `weaknessElement` 추가.
- `BattleManager` 기본 공격을 `SkillData` 기반으로 계산하도록 변경.
- 약점 속성과 스킬 속성이 같으면 데미지 +10이 되는 초안 규칙 추가.
- 초보자용 공부 문서와 개발일지 추가.

### 다음 추천 작업

1. **AP 시스템 1차 초안**
   - 플레이어 현재 AP / 최대 AP 추가
   - 기본 공격은 AP 0, 스킬은 AP 소모 방식으로 설계
   - 턴 시작 시 AP 회복 규칙 작성

2. **SkillData 문서 보강**
   - Unity Inspector에서 스킬 데이터를 어떻게 입력할지 예시 스크린샷 자리 만들기
   - 나중에 ScriptableObject로 바꿀 수 있다는 설명 추가

3. **BattleManager 리팩터링 후보 기록**
   - 데미지 계산을 별도 클래스로 빼기 전에, 현재는 함수 하나로 유지
   - 초보자가 읽기 쉬운 선에서만 확장

### 주의

- Unity Editor가 필요한 씬/프리팹 생성은 자동으로 무리하게 하지 않는다.
- 큰 구조 변경보다 작은 데이터 구조와 문서화를 우선한다.
