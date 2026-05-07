# 전투 시스템 설계

## 전투 흐름

```text
BattleStart
→ PlayerTurn
→ PlayerAction
→ EnemyTurn
→ EnemyAction
→ CheckBattleEnd
→ Victory / Defeat
```

## 1차 프로토타입 규칙

- 플레이어와 적은 각각 HP와 공격력을 가진다.
- 플레이어가 공격 버튼을 누르면 적 HP가 감소한다.
- 적이 살아있으면 적 턴으로 넘어간다.
- 적은 자동으로 플레이어를 공격한다.
- 플레이어 HP가 0 이하이면 패배.
- 적 HP가 0 이하이면 승리.

## 2차 확장 예정

### AP 시스템

- 매 플레이어 턴마다 AP 3 획득
- 기본 공격: 1 AP
- 스킬: 2 AP
- 궁극기: 3 AP

### Break 시스템

- 적의 약점 속성으로 공격하면 Break Gauge가 감소
- Break Gauge가 0이 되면 적은 1턴 동안 행동 불가
- Break 상태에서는 받는 피해 증가

### 상태이상

- Poison: 턴 종료 시 피해
- Stun: 1턴 행동 불가
- Shield: 피해 흡수
- DefenseDown: 받는 피해 증가
