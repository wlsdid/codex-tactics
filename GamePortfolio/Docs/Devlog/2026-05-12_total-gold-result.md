# Devlog - 2026-05-12 Display-only Total Gold

## 작업 목표

Stage 1-1에서 얻은 보상이 Stage 1-2 결과 화면까지 이어지는 느낌을 주기 위해, 인벤토리나 상점 없이 결과 요약에 누적 골드만 표시하는 작은 개선을 진행했다.

## 구현 내용

- `BattleManager`에 `totalGoldEarned` 값을 추가했다.
- 결과 요약 생성 시 현재 전투의 `rewardGold`를 누적 골드에 더하고, `BattleResultData.totalGold`로 전달한다.
- 같은 전투 결과나 이미 클리어한 stage index에서 보상이 중복 지급되지 않도록 `currentBattleRewardClaimed`와 rewarded-stage 추적을 추가했다.
- `BattleResultPresenter`의 랭크/보상 줄을 `Rank: ... | Reward: ...G | Total Gold: ...G` 형태로 확장했다.
- 전투 로직 자동 테스트에서 Defeat, 첫 Victory, boss Victory, presenter 직접 포맷 검사를 업데이트했다.

## 포트폴리오 관점

이번 변경은 아직 경제 시스템을 추가하지 않고도 “스테이지 보상이 다음 전투로 이어진다”는 vertical slice 감각을 보여준다. 결과 요약 한 줄만 확장했기 때문에 UI 범위를 크게 늘리지 않고, 나중에 인벤토리/상점/스테이지 선택으로 연결할 수 있는 hook을 마련했다.

## 다음 작업 메모

- 실제 Unity Play Mode에서 첫 Victory와 Final Clear 결과 요약의 `Total Gold` 줄을 캡처한다.
- 캡처가 준비되면 README와 showcase draft에 이미지/GIF를 연결한다.
- 그 다음 기능은 더 많은 보상 수치보다 title/start screen 또는 캡처 정리를 우선한다.
