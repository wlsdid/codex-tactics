# Study Note - 2026-05-12 Display-only Total Gold

## 배운 점

작은 RPG 진행감은 반드시 큰 시스템에서만 나오지 않는다. 이번에는 인벤토리, 상점, 저장 시스템 없이도 `Reward`와 `Total Gold`를 함께 보여줘서 encounter 사이의 연결감을 만들었다.

## 코드 구조 메모

- `BattleResultEvaluator`는 랭크와 이번 보상 계산을 담당한다.
- `BattleManager`는 현재 전투가 끝났을 때 보상을 한 번만 누적한다.
- `BattleResultData`는 표시용 데이터를 담는다.
- `BattleResultPresenter`는 데이터를 UI 문장으로 포맷한다.

이렇게 나누면 나중에 실제 골드 사용처가 생겨도 result text 포맷과 전투 흐름을 한꺼번에 바꾸지 않아도 된다.

## 중복 지급 방지

결과 요약이 여러 번 만들어질 수 있는 상황을 대비해 `currentBattleRewardClaimed` 플래그를 두었다. 또한 이미 보상을 받은 stage index를 기록해 Victory 후 Retry를 반복해도 같은 encounter 보상이 계속 누적되지 않게 했다.

## 초보자용 설계 원칙

- 숫자는 한 줄에 모아 읽기 쉽게 표시한다.
- 새 시스템을 추가할 때 저장/상점/아이템까지 한 번에 넓히지 않는다.
- 자동 테스트에는 플레이어가 실제로 확인할 문구(`Total Gold`)를 포함한다.
