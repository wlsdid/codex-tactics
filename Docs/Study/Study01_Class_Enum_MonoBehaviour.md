# Study 01 - class, enum, MonoBehaviour

## 오늘 배운 개념

- class
- enum
- MonoBehaviour

## 왜 필요했는가

턴제 RPG에서는 캐릭터의 이름, HP, 공격력 같은 데이터를 하나로 묶어야 한다. 또한 전투가 시작인지, 플레이어 턴인지, 적 턴인지 구분해야 한다.

## 프로젝트에 적용한 파일

```text
Assets/Scripts/Data/CharacterData.cs
Assets/Scripts/Battle/BattleState.cs
Assets/Scripts/Battle/BattleManager.cs
```

## class 적용

`CharacterData` class는 캐릭터 정보를 묶는다.

```csharp
public class CharacterData
{
    public string characterName;
    public int maxHp;
    public int currentHp;
    public int attackPower;
}
```

C에서 여러 변수를 따로 관리하는 대신, C#에서는 관련 데이터를 class로 묶어 관리할 수 있다.

## enum 적용

`BattleState` enum은 전투 상태를 명확하게 표현한다.

```csharp
public enum BattleState
{
    Start,
    PlayerTurn,
    EnemyTurn,
    Victory,
    Defeat
}
```

문자열로 `"PlayerTurn"`을 비교하는 것보다 오타 위험이 적고 코드가 읽기 쉽다.

## MonoBehaviour 적용

Unity에서 오브젝트에 붙는 스크립트는 보통 `MonoBehaviour`를 상속한다.

```csharp
public class BattleManager : MonoBehaviour
{
    private void Start()
    {
        StartBattle();
    }
}
```

`Start()`는 게임이 실행될 때 Unity가 자동으로 호출한다.

## 느낀 점

C#은 C보다 구조를 더 크게 묶어서 관리하는 느낌이다. 턴제 RPG처럼 캐릭터와 전투 상태가 많은 게임에서는 class와 enum이 코드를 정리하는 데 중요하다.
