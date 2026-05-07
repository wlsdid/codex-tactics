# Study05 - 턴 종료 버튼과 함수 분리

## 오늘 배운 개념

이번 변경에서는 플레이어가 공격하지 않고 턴을 넘기는 **End Turn 버튼** 초안을 추가했다.

턴제 RPG에서는 항상 공격만 하는 것이 아니라, AP를 모으기 위해 일부러 행동을 쉬는 선택지도 필요하다. 그래서 `BattleManager`에 `endTurnButton`을 추가하고, 버튼을 누르면 적 턴으로 넘어가도록 만들었다.

## 핵심 코드 흐름

```csharp
public void OnClickEndTurnButton()
{
    EndPlayerTurn();
}
```

Unity 버튼은 보통 `public` 함수를 호출한다. 하지만 실제 로직을 버튼 함수 안에 전부 쓰면 코드가 점점 복잡해진다.

그래서 버튼 함수는 짧게 두고, 실제 처리는 아래 함수로 분리했다.

```csharp
private void EndPlayerTurn()
{
    if (currentState != BattleState.PlayerTurn)
    {
        return;
    }

    SetActionButtonsInteractable(false);
    UpdateUI("턴을 넘겼습니다.");
    StartCoroutine(EnemyTurnRoutine());
}
```

## C만 아는 입장에서 이해하기

C에서 함수를 나누는 이유와 비슷하다.

```c
void OnClickEndTurnButton()
{
    EndPlayerTurn();
}
```

- 버튼 입력을 받는 함수
- 실제 게임 규칙을 처리하는 함수

이렇게 역할을 나누면 나중에 키보드 단축키, AI 테스트, 튜토리얼에서도 `EndPlayerTurn()`만 재사용할 수 있다.

## 왜 버튼 비활성화가 필요한가?

턴을 넘긴 직후에도 버튼이 계속 눌리면, 적 턴 코루틴이 여러 번 실행될 수 있다.

그래서 턴을 넘기는 순간 모든 행동 버튼을 꺼 둔다.

```csharp
SetActionButtonsInteractable(false);
```

그리고 다음 플레이어 턴이 시작될 때 `UpdateActionButtons()`가 AP 상태에 맞게 버튼을 다시 켠다.

## Unity Inspector 연결 방법

1. Canvas 안에 `End Turn Button` 오브젝트를 만든다.
2. 버튼 안의 TextMeshPro 텍스트를 `End Turn` 또는 `턴 종료`로 바꾼다.
3. `BattleManager`가 붙어 있는 오브젝트를 선택한다.
4. Inspector의 `End Turn Button` 칸에 새 버튼 오브젝트를 드래그한다.
5. 코드에서 `onClick.AddListener()`를 사용하므로, Button 컴포넌트의 OnClick 목록을 직접 추가하지 않아도 된다.

## 지금 단계의 한계

- Unity Editor에서 실제 씬 연결은 아직 검증하지 않았다.
- 버튼 배치, 색상, 사운드 같은 UI 연출은 나중에 한다.
- 현재는 플레이어가 턴을 넘기면 바로 적이 공격한다.

## 다음에 연결하면 좋은 내용

- Burn 상태이상 실제 피해 적용
- 턴 종료 시 방어 자세 같은 효과 추가
- AP가 최대치일 때 턴 종료를 누르면 안내 메시지 표시
