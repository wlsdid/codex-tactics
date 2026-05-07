# Study06 - Burn 상태이상 1차 구현

## 오늘 배운 개념

이번 작업에서는 `enum`으로 만든 `StatusEffectType.Burn`을 실제 전투 흐름에 조금 연결했다.

이전에는 Fire Skill을 쓰면 메시지에만 `Burn` 후보가 보였다. 이제는 적 캐릭터가 실제로 화상 상태를 기억하고, 적 턴 시작에 고정 피해를 받는다.

## C를 아는 사람 기준으로 이해하기

C에서 구조체에 값을 저장하듯이, C# class에도 상태를 저장할 변수를 둘 수 있다.

```csharp
public StatusEffectType currentStatusEffect;
public int statusTurnsRemaining;
```

- `currentStatusEffect`: 지금 걸린 상태이상이 무엇인지
- `statusTurnsRemaining`: 몇 턴 남았는지

`StatusEffectType`은 숫자 대신 이름을 쓰게 해주는 enum이다.

```csharp
StatusEffectType.None
StatusEffectType.Burn
```

이렇게 쓰면 `0`, `1` 같은 숫자를 외우지 않아도 되어서 실수를 줄일 수 있다.

## 이번에 추가한 함수

### 1. 상태가 있는지 확인

```csharp
public bool HasStatusEffect(StatusEffectType statusEffect)
{
    return currentStatusEffect == statusEffect && statusTurnsRemaining > 0;
}
```

뜻:

- 현재 상태이상이 `Burn`인지 확인한다.
- 남은 턴이 1 이상일 때만 실제 상태이상으로 인정한다.

### 2. 상태이상 적용

```csharp
public void ApplyStatusEffect(StatusEffectType statusEffect, int turns)
{
    currentStatusEffect = statusEffect;
    statusTurnsRemaining = turns;
}
```

뜻:

- 스킬이 Burn을 가지고 있으면 적에게 Burn을 저장한다.
- 이번 초안에서는 같은 상태이상을 다시 걸면 남은 턴을 새 값으로 덮어쓴다.

### 3. 상태 턴 감소

```csharp
public void ReduceStatusTurn()
```

뜻:

- 상태이상 피해가 한 번 적용된 뒤 남은 턴을 1 줄인다.
- 0턴이 되면 `None`으로 되돌린다.

## BattleManager에서의 흐름

Fire Skill 사용:

1. AP를 검사한다.
2. 데미지를 준다.
3. 스킬의 상태이상이 Burn이면 적에게 Burn을 적용한다.
4. 적 턴 코루틴으로 넘어간다.

적 턴 시작:

1. 적에게 Burn이 있는지 확인한다.
2. 있으면 고정 피해 3을 준다.
3. 남은 Burn 턴을 1 줄인다.
4. Burn 피해로 적이 죽으면 바로 승리 처리한다.
5. 살아 있으면 적 기본 공격을 진행한다.

## 왜 작은 초안으로 만들었나?

상태이상 시스템을 처음부터 완성하려고 하면 다음 내용이 한꺼번에 필요해진다.

- 여러 상태이상 동시 적용
- 독/화상/기절 각각의 규칙
- 상태 아이콘 UI
- 저항 확률
- 보스 면역

초보 단계에서는 너무 복잡해질 수 있으므로, 지금은 **Burn 하나가 실제로 적용되는 최소 구조**만 만들었다.

## Unity Inspector에서 새로 보이는 값

`BattleManager`에 아래 값이 추가된다.

- `Burn Damage Per Turn`: 화상 1회 피해량, 기본값 3
- `Burn Turn Duration`: 화상 지속 턴, 기본값 2
- `Enemy Status Text`: 적 상태 표시용 TextMeshPro UI 연결 칸

`Enemy Status Text`는 연결하지 않아도 오류가 나지 않도록 `null` 검사를 넣었다.

## 다음에 확장할 수 있는 방향

- Burn 외에 Poison도 같은 방식으로 추가하기
- 상태이상을 한 개가 아니라 리스트로 여러 개 저장하기
- Enemy Status Text를 아이콘 UI로 바꾸기
- 보스는 Burn 지속 턴을 짧게 만드는 저항 규칙 추가하기
