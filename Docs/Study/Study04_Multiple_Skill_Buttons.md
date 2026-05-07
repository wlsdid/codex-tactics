# Study04 - 스킬 버튼을 2개로 늘리기

## 오늘 배운 내용

이번 단계에서는 기존 공격 버튼 하나만 있던 구조를 다음처럼 확장했습니다.

- 기본 공격 버튼: AP 0, 물리 속성
- 화염 스킬 버튼: AP 2, 화염 속성, Burn 상태이상 후보

중요한 점은 버튼마다 전투 코드를 전부 복사하지 않고, 공통 함수 `UsePlayerSkill()`을 만들었다는 것입니다.

## 왜 공통 함수가 필요한가?

처음에는 이렇게 만들기 쉽습니다.

```csharp
public void OnClickAttackButton()
{
    // 공격 처리 코드
}

public void OnClickFireSkillButton()
{
    // 거의 같은 공격 처리 코드
}
```

하지만 스킬이 3개, 4개로 늘어나면 같은 코드가 계속 반복됩니다. 나중에 버그를 고칠 때 모든 버튼 코드를 각각 수정해야 해서 실수하기 쉽습니다.

그래서 버튼 함수는 짧게 유지하고, 실제 처리는 하나의 함수로 모았습니다.

```csharp
public void OnClickAttackButton()
{
    UsePlayerSkill(basicAttackSkill);
}

public void OnClickFireSkillButton()
{
    UsePlayerSkill(fireSkill);
}
```

C 언어로 비유하면, `UsePlayerSkill()`은 매개변수로 어떤 스킬을 쓸지 받는 공통 함수입니다.

## C# 개념: class를 함수 매개변수로 넘기기

`SkillData`는 스킬 정보를 묶은 class입니다.

```csharp
private void UsePlayerSkill(SkillData skill)
{
    // skill.apCost
    // skill.power
    // skill.elementType
}
```

이렇게 하면 같은 함수 안에서 AP 비용, 데미지, 속성을 모두 `skill`에서 읽어올 수 있습니다.

## Unity Inspector에서 확인할 값

`BattleManager`에 새로 보이는 값:

- `Fire Skill Name`: Fire Bolt
- `Fire Skill Power`: 30
- `Fire Skill Ap Cost`: 2
- `Fire Skill Element`: Fire
- `Fire Skill Button`: 화염 스킬용 UI Button 연결 자리

테스트 추천값:

```text
Player Max Ap = 3
Player Ap Recovery Per Turn = 1
Basic Skill Ap Cost = 0
Fire Skill Ap Cost = 2
Enemy Weakness = Fire
```

이렇게 두면 화염 스킬이 적 약점을 찔러서 기본 위력 30 + 약점 보너스 10 = 40 피해가 됩니다.

## 버튼 연결 방법

1. Unity Hierarchy에서 화염 스킬용 Button을 새로 만듭니다.
2. Button 안의 TextMeshPro 텍스트를 `Fire Bolt` 또는 `화염 스킬`로 바꿉니다.
3. `BattleManager`가 붙은 오브젝트를 선택합니다.
4. Inspector의 `Fire Skill Button` 칸에 새 Button 오브젝트를 드래그합니다.
5. 코드에서 `AddListener(OnClickFireSkillButton)`를 등록하므로 Button의 OnClick 목록을 수동으로 추가하지 않아도 됩니다.

## 아직 구현하지 않은 것

이번 작업의 Burn은 실제 지속 피해가 아니라 **상태이상 후보 메시지**까지만 표시합니다.

실제 Burn 효과는 다음 단계에서 아래 데이터를 추가하면서 구현할 수 있습니다.

- 상태이상 지속 턴 수
- 턴 시작/종료 시 피해 처리
- 적 또는 플레이어가 가진 상태이상 목록

## 포트폴리오에 쓸 수 있는 설명

> 스킬 버튼이 늘어나도 BattleManager가 복잡해지지 않도록, 버튼별 함수는 입력만 받고 공통 함수 UsePlayerSkill()에서 AP 검사, 데미지 계산, 승리 판정을 처리하도록 리팩터링했다.
