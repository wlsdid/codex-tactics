# Study03 - AP 시스템 1차 초안

## 오늘 배운 내용

AP(Action Point)는 턴제 RPG에서 플레이어가 한 턴에 어떤 행동을 할 수 있는지 제한하는 숫자입니다.

예시:

- 기본 공격: AP 0
- 강한 스킬: AP 2
- 회복 스킬: AP 1

이번 단계에서는 아직 여러 스킬 버튼을 만들지 않고, **기존 공격 버튼에 AP 비용 검사만 붙이는 작은 확장**을 했습니다.

## C# 개념: bool 함수

`CharacterData`에 아래처럼 `true` 또는 `false`를 돌려주는 함수를 추가했습니다.

```csharp
public bool HasEnoughAp(int apCost)
{
    return currentAp >= apCost;
}
```

뜻:

- 현재 AP가 스킬 비용보다 크거나 같으면 `true`
- 부족하면 `false`

C 언어의 조건문과 비슷하게 생각하면 됩니다.

```c
if (currentAp >= apCost)
{
    return true;
}
```

C#에서는 위 조건을 바로 `return currentAp >= apCost;`로 줄여 쓸 수 있습니다.

## C# 개념: 함수가 성공/실패를 알려주기

`SpendAp()`는 AP를 실제로 소모하고, 성공 여부를 알려줍니다.

```csharp
public bool SpendAp(int apCost)
{
    if (!HasEnoughAp(apCost))
    {
        return false;
    }

    currentAp -= apCost;
    return true;
}
```

포인트:

1. AP가 부족하면 아무것도 빼지 않고 `false`
2. 충분하면 AP를 빼고 `true`

이렇게 만들면 `BattleManager`에서 안전하게 사용할 수 있습니다.

## Unity에서 확인할 Inspector 값

`BattleManager`에 추가된 값:

- `Player Max Ap`: 플레이어 최대 AP
- `Player Ap Recovery Per Turn`: 플레이어 턴 시작 시 회복 AP
- `Basic Skill Ap Cost`: 기본 공격/스킬의 AP 비용
- `Player Ap Text`: AP를 표시할 TextMeshPro UI 연결 자리

초반 테스트 추천값:

```text
Player Max Ap = 3
Player Ap Recovery Per Turn = 1
Basic Skill Ap Cost = 0
```

기본 공격은 AP 0으로 두면 전투가 막히지 않습니다. 나중에 별도 스킬 버튼을 만들 때 AP 1~2짜리 스킬을 추가하면 됩니다.

## 이번 코드가 포트폴리오에 의미 있는 이유

단순히 데미지만 주는 전투에서 한 단계 발전해, 스킬 사용을 자원으로 관리하는 구조가 생겼습니다.

포트폴리오 설명에 이렇게 쓸 수 있습니다.

> 스킬을 무제한으로 쓰면 전략성이 약해져서 AP 자원을 추가했다. 캐릭터 데이터에 현재 AP와 최대 AP를 넣고, 스킬 사용 전 비용을 검사하도록 구현했다.

## 다음에 이어서 할 일

1. 스킬 버튼을 2개로 늘리기: 기본 공격 / 화염 스킬
2. AP 2를 쓰는 강한 스킬 추가
3. UI에 AP 부족 메시지 표시 개선
4. 턴 종료 버튼을 추가해 AP를 모으는 선택지 만들기
