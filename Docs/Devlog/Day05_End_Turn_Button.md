# Day05 - End Turn 버튼 초안

## 오늘 한 일

- `BattleManager`에 `endTurnButton` 연결 자리를 추가했다.
- 플레이어가 공격하지 않고 턴을 넘길 수 있도록 `OnClickEndTurnButton()`을 추가했다.
- 실제 턴 종료 처리는 `EndPlayerTurn()` 함수로 분리했다.
- 턴을 넘기는 순간 공격/스킬/턴 종료 버튼이 모두 비활성화되도록 했다.
- 다음 플레이어 턴에는 기존 `UpdateActionButtons()` 흐름으로 버튼 상태가 다시 갱신된다.

## 왜 필요한가?

AP 기반 전투에서는 강한 스킬을 쓰기 위해 AP를 모으는 선택지가 필요하다.

예를 들어 현재 화염 스킬은 AP 2를 소모한다. 플레이어가 AP가 부족할 때 기본 공격만 하는 구조라면 전략성이 약하다. 턴 종료 버튼이 있으면 플레이어는 잠시 공격을 포기하고 다음 턴의 강한 행동을 준비할 수 있다.

## 구현 메모

```csharp
[SerializeField] private Button endTurnButton;
```

Unity Inspector에서 버튼을 연결할 수 있도록 `SerializeField`를 사용했다.

```csharp
if (endTurnButton != null)
{
    endTurnButton.onClick.AddListener(OnClickEndTurnButton);
}
```

버튼이 아직 씬에 없어도 오류가 나지 않도록 `null` 검사를 유지했다.

## 테스트 체크리스트

Unity Editor에서 확인할 내용:

- [ ] End Turn 버튼을 누르면 플레이어 턴이 끝나는가?
- [ ] 적이 한 번만 공격하는가?
- [ ] 적 공격 후 다시 플레이어 턴으로 돌아오는가?
- [ ] 다음 플레이어 턴 시작 시 AP가 1 회복되는가?
- [ ] AP가 충분하면 Fire Skill 버튼이 다시 활성화되는가?

## 다음 작업 후보

1. Burn 상태이상 실제 효과 구현
2. 적 HP 옆에 Burn 상태 표시용 텍스트 추가
3. README에 현재 전투 프로토타입 조작 흐름 정리
