# Day03 - AP 시스템 초안 추가

## 오늘 한 일

- `CharacterData`에 AP 데이터를 추가했다.
  - `maxAp`
  - `currentAp`
- AP 관련 함수를 추가했다.
  - `HasEnoughAp()`
  - `SpendAp()`
  - `RecoverAp()`
- `BattleManager`에서 플레이어 턴 시작 시 AP를 회복하도록 했다.
- 공격 버튼을 누를 때 스킬 AP 비용을 먼저 검사하도록 했다.
- AP 표시용 `playerApText` 연결 자리를 추가했다.

## 왜 이 작업을 했나?

Codex Tactics의 핵심 예정 시스템 중 하나가 **AP 기반 스킬 사용**이기 때문입니다.

처음부터 복잡한 스킬창, 여러 캐릭터, 턴 종료 버튼을 한 번에 만들면 초보자가 이해하기 어렵습니다. 그래서 이번에는 기존 공격 버튼 구조를 유지하면서, AP 데이터와 검사 흐름만 작게 붙였습니다.

## 현재 규칙

- 플레이어는 최대 AP를 가진다.
- 전투 시작 시 AP가 최대치로 시작한다.
- 플레이어 턴이 시작될 때 AP를 회복한다.
- 스킬 사용 전 AP가 충분한지 검사한다.
- 부족하면 공격하지 않고 메시지만 보여준다.

## Unity에서 연결할 것

BattleManager가 붙은 오브젝트에서 Inspector를 확인합니다.

- `Player Max Ap`: 3 추천
- `Player Ap Recovery Per Turn`: 1 추천
- `Basic Skill Ap Cost`: 0 추천
- `Player Ap Text`: AP 표시용 TextMeshPro 텍스트 연결

`Player Ap Text`를 연결하지 않아도 전투 로직은 동작하도록 `null` 검사를 넣어두었습니다.

## 배운 점

- 데이터 class에 숫자를 추가하면 전투 규칙을 확장하기 쉬워진다.
- `bool` 함수는 "가능한가?"를 검사할 때 유용하다.
- AP 소모처럼 실패할 수 있는 행동은 함수가 성공/실패를 알려주면 안전하다.

## 다음 목표

- 기본 공격 외에 AP를 쓰는 강한 스킬 버튼 추가
- AP 부족 시 버튼 비활성화 또는 메시지 개선
- 턴 종료 버튼을 만들어 AP를 모으는 선택지 추가
