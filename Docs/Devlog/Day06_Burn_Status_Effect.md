# Day06 - Burn 상태이상 1차 초안

## 오늘 한 일

Fire Skill에 붙어 있던 `Burn` 상태이상을 실제 전투 흐름에 연결했다.

기존에는 Fire Skill을 사용해도 메시지에 "추가 효과 후보: Burn"만 표시되었다. 이번 작업 이후에는 적 캐릭터가 Burn 상태를 저장하고, 적 턴 시작 시 고정 피해를 받는다.

## 변경한 파일

- `Assets/Scripts/Data/CharacterData.cs`
- `Assets/Scripts/Battle/BattleManager.cs`
- `Docs/Study/Study06_Burn_Status_Effect.md`
- `Docs/Devlog/Day06_Burn_Status_Effect.md`
- `Docs/09_Next_Autonomous_Tasks.md`
- `README.md`

## 구현 내용

### CharacterData

적 또는 아군이 현재 상태이상을 기억할 수 있도록 아래 값을 추가했다.

- `currentStatusEffect`
- `statusTurnsRemaining`

그리고 상태이상 처리를 위한 작은 함수를 추가했다.

- `HasStatusEffect()`
- `ApplyStatusEffect()`
- `ReduceStatusTurn()`

### BattleManager

`BattleManager`에는 Burn 규칙과 UI 연결 칸을 추가했다.

- `burnDamagePerTurn = 3`
- `burnTurnDuration = 2`
- `enemyStatusText`

Fire Skill이 적에게 맞으면 Burn을 적용한다.

적 턴이 시작되면 Burn 피해를 먼저 처리한다. Burn 피해로 적 HP가 0이 되면 적의 공격을 진행하지 않고 승리 처리한다.

## 포트폴리오 관점에서 의미

이번 작업은 작은 기능이지만 포트폴리오에서 설명하기 좋다.

> 스킬 데이터에 들어 있던 상태이상 타입을 실제 전투 상태와 연결했고, 턴 시작 타이밍에 지속 피해를 처리하는 구조를 만들었다.

이 설명은 단순히 버튼을 누르면 데미지만 주는 게임보다 한 단계 더 시스템적으로 보인다.

## 아직 하지 않은 것

- Unity Editor에서 실제 TextMeshPro 연결 테스트는 하지 않았다.
- 여러 상태이상을 동시에 저장하는 구조는 만들지 않았다.
- Burn 아이콘이나 애니메이션은 추가하지 않았다.
- Poison, Stun 같은 다른 상태이상 규칙은 아직 구현하지 않았다.

## 다음 목표

1. Unity Inspector 연결 체크리스트 문서 보강
   - `Enemy Status Text` 연결 방법
   - Fire Skill 버튼 테스트 순서
   - Burn 피해 확인 방법

2. 상태이상 구조 개선 후보 정리
   - 한 캐릭터가 여러 상태이상을 동시에 가질지 결정
   - 지금처럼 1개만 유지할지 결정

3. 간단한 적 AI 초안
   - 적 HP가 낮으면 공격 메시지를 다르게 표시
   - 나중에 회복/강공격 패턴으로 확장
