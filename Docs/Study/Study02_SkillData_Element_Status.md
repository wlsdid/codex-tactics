# Study 02 - SkillData, ElementType, StatusEffectType

## 오늘 배운 개념

- `class`로 스킬 데이터를 묶기
- `enum`으로 속성과 상태이상을 안전하게 표현하기
- 작은 계산 함수로 데미지 규칙 분리하기

## 왜 필요했는가

턴제 RPG는 캐릭터가 기본 공격만 하는 단계에서 곧 여러 스킬을 쓰는 단계로 확장된다. 이때 스킬 이름, 위력, AP 비용, 속성, 상태이상을 각각 따로 변수로 만들면 관리가 어려워진다.

그래서 이번에는 `SkillData`라는 class를 만들고, 속성/상태이상은 문자열이 아니라 enum으로 관리하도록 했다.

## 프로젝트에 적용한 파일

```text
Assets/Scripts/Data/SkillData.cs
Assets/Scripts/Data/ElementType.cs
Assets/Scripts/Data/StatusEffectType.cs
Assets/Scripts/Data/CharacterData.cs
Assets/Scripts/Battle/BattleManager.cs
```

## SkillData 구조

```csharp
public class SkillData
{
    public string skillName;
    public int power;
    public int apCost;
    public ElementType elementType;
    public StatusEffectType statusEffectType;
}
```

C로 비유하면 여러 변수를 따로 넘기는 대신, 관련 값들을 하나의 구조체처럼 묶어 쓰는 느낌이다. C#에서는 class를 사용하면 이런 데이터를 더 쉽게 확장할 수 있다.

## enum을 쓰는 이유

```csharp
public enum ElementType
{
    None,
    Physical,
    Fire,
    Ice,
    Lightning
}
```

`"Fire"`, `"fire"`, `"Fier"`처럼 문자열을 직접 쓰면 오타가 생길 수 있다. enum을 쓰면 Unity Inspector와 코드에서 정해진 선택지만 고를 수 있어서 실수를 줄일 수 있다.

## 이번에 추가된 전투 규칙

`BattleManager`에 `CalculateSkillDamage()` 함수를 추가했다.

```csharp
private int CalculateSkillDamage(CharacterData target, SkillData skill)
{
    int damage = skill.power;

    if (skill.elementType != ElementType.None && skill.elementType == target.weaknessElement)
    {
        damage += 10;
    }

    return damage;
}
```

아직 완성된 약점/Break 시스템은 아니지만, 나중에 다음처럼 확장하기 좋은 첫 단계다.

- 약점 공격 시 데미지 증가
- 약점 공격 누적 시 Break 발생
- 상태이상 적용
- AP를 소비하는 강한 스킬 추가

## Unity Inspector에서 확인할 것

BattleManager가 붙은 오브젝트에서 아래 값을 바꿔보면 된다.

- `Enemy Weakness`: 적 약점 속성
- `Basic Skill Name`: 기본 스킬 이름
- `Basic Skill Power`: 기본 스킬 위력
- `Basic Skill Element`: 기본 스킬 속성

예를 들어 `Enemy Weakness = Fire`, `Basic Skill Element = Fire`로 맞추면 기본 데미지보다 10 높은 피해가 들어간다.

## 느낀 점

처음부터 복잡한 스킬 시스템을 만들기보다, 데이터 구조와 데미지 계산 함수를 먼저 분리해 두면 이후 AP, 상태이상, Break를 조금씩 추가하기 쉬워진다.
