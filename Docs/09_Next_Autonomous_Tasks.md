# 09 Next Autonomous Tasks

이 문서는 자동 실행 작업이 매번 무엇을 했고, 다음에 어떤 작은 목표를 진행하면 좋은지 누적 기록하는 공간이다.

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
