# Day04 - 기본 공격과 화염 스킬 버튼 분리

## 오늘 한 일

- `BattleManager`에 두 번째 스킬 데이터인 `fireSkill`을 추가했다.
- Unity Inspector에서 조절할 수 있는 화염 스킬 설정값을 추가했다.
  - `Fire Skill Name`
  - `Fire Skill Power`
  - `Fire Skill Ap Cost`
  - `Fire Skill Element`
- 화염 스킬용 `fireSkillButton` 연결 자리를 추가했다.
- 버튼 함수가 중복되지 않도록 `UsePlayerSkill(SkillData skill)` 공통 함수를 만들었다.
- 버튼 상태 갱신을 `UpdateActionButtons()`로 분리했다.
- 화염 스킬은 `Burn` 상태이상 후보 메시지를 표시하도록 했다.

## 왜 이 작업을 했나?

AP 시스템은 여러 스킬이 있을 때 의미가 커집니다. 기본 공격만 있으면 AP를 쓸 이유가 약하기 때문에, AP 2를 소모하는 강한 화염 스킬을 추가했습니다.

단, 아직 상태이상 전체 시스템까지 한 번에 만들지는 않았습니다. 초보자가 흐름을 따라가기 쉽도록 이번에는 다음까지만 구현했습니다.

1. 스킬 버튼 2개
2. AP 비용 차이
3. 약점 속성 재사용
4. 상태이상은 메시지 후보로만 표시

## 현재 전투 흐름

1. 플레이어 턴 시작
2. AP 회복
3. AP가 충분한 버튼만 활성화
4. 기본 공격 또는 화염 스킬 클릭
5. `UsePlayerSkill()`에서 AP 소모, 데미지 계산, 승리 체크
6. 적이 살아 있으면 적 턴으로 이동

## Unity에서 연결할 것

- 기존 `Attack Button`: 기본 공격 버튼
- 새 `Fire Skill Button`: 화염 스킬 버튼
- `Player Ap Text`: AP 표시 TextMeshPro
- `Enemy Weakness`: `Fire`로 두면 화염 스킬 약점 보너스 확인 가능

## 배운 점

- 기능이 늘어날 때 코드를 복사하기보다 공통 함수를 만들면 유지보수가 쉬워진다.
- `SkillData` 같은 class를 매개변수로 넘기면 여러 스킬을 같은 흐름에서 처리할 수 있다.
- 상태이상처럼 큰 기능은 먼저 데이터와 메시지 자리만 만들어두고, 나중에 작은 단위로 실제 효과를 붙일 수 있다.

## 다음 목표

- 턴 종료 버튼 추가: 공격하지 않고 턴을 넘겨 AP를 모으기
- Burn 상태이상 실제 효과 초안: 적 턴 시작 또는 종료 때 3 피해
- UI 가이드 문서 보강: 버튼 2개 배치 예시와 Inspector 연결 체크리스트
