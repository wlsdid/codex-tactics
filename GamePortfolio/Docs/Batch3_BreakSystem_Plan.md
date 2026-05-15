# Batch 3 Plan — Break / Weakness Tactical Depth

**Goal:** 전투가 단순 HP 깎기가 아니라, 약점 공격을 누적해 적을 Break시키고 그 타이밍에 큰 이득을 얻는 2D 전술 RPG식 핵심 재미를 만든다.

**Role Split:**
- codex5.5: 계획/리뷰/완료 기준 담당
- hemes-deep: Unity 구현/검증/수정 담당
- 포트폴리오 문서 작성 금지. 구현 결과와 검증 결과만 보고.

---

## 작업 전 필수 순서

```bash
git pull --ff-only
git status --short --branch
git diff --stat
```

현재 남아있는 `Docs/Batch2_CombatFeedback_Instructions.md`는 untracked 상태로 유지하거나 삭제하되, 이번 배치 커밋에는 포함하지 않는다.

---

## Batch 3 핵심 방향

이번 배치는 **Break 게이지/상태**만 한다. 애니메이션, 외부 에셋, 복잡한 이펙트는 하지 않는다.

플레이 경험 목표:
1. 적에게 약점 게이지가 보인다.
2. Fire Skill 같은 약점 공격을 맞히면 Break 게이지가 감소한다.
3. 게이지가 0이 되면 적이 `BREAK` 상태가 된다.
4. Break 상태에서는 다음 플레이어 공격 피해가 증가한다.
5. Break가 발생했다는 메시지/Impact/상태 텍스트가 명확히 보인다.

---

## 구현 범위

### 1. Enemy Break 데이터 추가

추천 위치:
- `Assets/Scripts/Data/CharacterData.cs`

추가할 개념:
- `maxBreakGauge`
- `currentBreakGauge`
- `isBroken`
- `brokenTurnsRemaining`

최소 정책:
- 적 기본 Break Gauge: `2`
- 약점 공격 성공 시 `currentBreakGauge -= 1`
- `currentBreakGauge <= 0`이면 Break 발생
- Break 지속: `1` 플레이어 공격 기회
- Break 중 받는 피해: `+50%` 또는 고정 배율 `1.5x`

주의:
- 플레이어 캐릭터에는 Break가 없어도 됨. 단, 구조상 `CharacterData`에 들어가도 테스트에서 문제 없게 기본값을 안전하게 둔다.

---

### 2. BattleManager 로직 추가

추천 위치:
- `Assets/Scripts/Battle/BattleManager.cs`

필요 로직:

#### 약점 공격 처리
- `CalculateSkillDamage()` 전후 흐름에서 약점 여부를 명확히 계산한다.
- 약점 공격이면 적 Break Gauge를 1 감소시킨다.
- Break Gauge가 0이 되면 `enemy.isBroken = true`.

#### Break 피해 보너스
- 적이 Broken 상태이고 플레이어가 공격하면 피해량 증가.
- 증가 피해는 자동 테스트가 예측 가능해야 한다.
- 추천 공식:
  - 기본 피해 계산
  - 약점 보너스 적용
  - 마지막에 Broken이면 `Mathf.RoundToInt(damage * 1.5f)`

#### Break 소모
- Broken 상태에서 플레이어 공격이 한 번 들어가면 Break를 소모한다.
- 소모 후:
  - `isBroken = false`
  - `currentBreakGauge = maxBreakGauge`

#### 메시지/Impact
- Break 게이지 감소 시 Impact 예:
  - `Impact: Fire Bolt dealt 40 damage | Weakness hit | Break 1/2 | Burn applied`
- Break 발생 시 Impact 예:
  - `Impact: Fire Bolt dealt 40 damage | Weakness hit | BREAK! | Burn applied`
- Break 보너스 공격 시 Impact 예:
  - `Impact: Slash dealt 30 damage | Break bonus consumed`

문구는 테스트 기대값과 반드시 일치시킨다.

---

### 3. BattleUI 표시 추가

추천 위치:
- `Assets/Scripts/Battle/BattleUI.cs`

추가 UI:
- `enemyBreakText`
- 가능하면 `enemyBreakSlider`

최소 표시:
- 일반 상태: `Break: 2/2`
- 약점 1회 후: `Break: 1/2`
- Break 상태: `Break: BROKEN`

Debug getter 추가:
- `DebugEnemyBreakText`
- Slider를 추가하면:
  - `DebugEnemyBreakBarValue`
  - `DebugEnemyBreakBarMaxValue`

---

### 4. BattleSceneAutoBuilder 갱신

추천 위치:
- `Assets/Editor/BattleSceneAutoBuilder.cs`

해야 할 것:
- Enemy Card 영역에 `Enemy Break Text` 생성
- 가능하면 Enemy HP 아래에 `Enemy Break Slider` 생성
- `BattleUI.enemyBreakText` 연결
- Slider 추가 시 `BattleUI.enemyBreakSlider` 연결
- Validate에 추가:
  - `Enemy Break text exists`
  - `Enemy Break text linked`
  - Slider 구현 시 `Enemy Break slider exists/linked`

주의:
- `.unity` 직접 편집 금지
- 반드시 `BattleSceneAutoBuilder.CreateBattleTestScene`으로 씬 재생성
- 재생성 후 `git diff --check` trailing whitespace 정리

---

## TDD / 검증 순서

### Step 1 — RED 테스트 추가

수정 파일:
- `Assets/Editor/BattleAutoTestRunner.cs`

추가 검증 예시:

1. 전투 시작 시:
```csharp
AppendCheck(ref passed, ref report, "Enemy Break starts full", battleManager.DebugEnemyBreakText == "Break: 2/2");
```

2. Fire Skill 1회 후:
```csharp
AppendCheck(ref passed, ref report, "Weakness hit reduces Break gauge", battleManager.DebugEnemyBreakText == "Break: 1/2");
AppendCheck(ref passed, ref report, "Impact text includes Break gauge loss", battleManager.DebugImpactText == "Impact: Fire Bolt dealt 40 damage | Weakness hit | Break 1/2 | Burn applied");
```

3. 테스트용으로 AP 회복/턴 흐름을 이용해 Fire Skill을 한 번 더 사용하거나, 테스트 전용 helper를 추가해서 두 번째 약점 공격을 검증:
```csharp
AppendCheck(ref passed, ref report, "Second weakness hit triggers Break", battleManager.DebugEnemyBreakText == "Break: BROKEN");
```

4. Break 상태에서 Slash 공격 후:
```csharp
AppendCheck(ref passed, ref report, "Break bonus increases Slash damage", battleManager.DebugImpactText == "Impact: Slash dealt 30 damage | Break bonus consumed");
AppendCheck(ref passed, ref report, "Break resets after bonus attack", battleManager.DebugEnemyBreakText == "Break: 2/2");
```

RED 확인:
```bash
'/mnt/c/Program Files/Unity/Hub/Editor/6000.4.6f1/Editor/Unity.exe' -batchmode -quit -projectPath 'C:\Users\jywls\Desktop\game_portfolio\GamePortfolio' -executeMethod BattleAutoTestRunner.RunBattleLogicAutoTest -logFile 'C:\Users\jywls\AppData\Local\Temp\battle_logic_batch3_red.log'
```

예상: `DebugEnemyBreakText` 미구현 등으로 FAIL 또는 compiler error.

---

## 구현 순서

1. `BattleAutoTestRunner.cs`에 RED 기대값 추가
2. RED 실패 확인
3. `CharacterData.cs`에 Break 상태 필드/메서드 추가
4. `BattleUI.cs`에 Enemy Break 표시/Debug getter 추가
5. `BattleManager.cs`에 약점 → Break gauge 감소 → Broken → 피해 보너스 → Reset 흐름 구현
6. `BattleSceneAutoBuilder.cs`에 Enemy Break UI 생성/연결/Validate 추가
7. `CreateBattleTestScene` 실행
8. 검증 전체 실행
9. trailing whitespace 정리
10. 최종 보고 후 리뷰 요청

---

## 필수 검증 명령

```bash
'/mnt/c/Program Files/Unity/Hub/Editor/6000.4.6f1/Editor/Unity.exe' -batchmode -quit -projectPath 'C:\Users\jywls\Desktop\game_portfolio\GamePortfolio' -executeMethod BattleAutoTestRunner.RunBattleLogicAutoTest -logFile 'C:\Users\jywls\AppData\Local\Temp\battle_logic_batch3.log'
```

```bash
'/mnt/c/Program Files/Unity/Hub/Editor/6000.4.6f1/Editor/Unity.exe' -batchmode -quit -projectPath 'C:\Users\jywls\Desktop\game_portfolio\GamePortfolio' -executeMethod BattleSceneAutoBuilder.CreateBattleTestScene -logFile 'C:\Users\jywls\AppData\Local\Temp\battle_scene_create_batch3.log'
```

```bash
'/mnt/c/Program Files/Unity/Hub/Editor/6000.4.6f1/Editor/Unity.exe' -batchmode -quit -projectPath 'C:\Users\jywls\Desktop\game_portfolio\GamePortfolio' -executeMethod BattleSceneAutoBuilder.ValidateBattleTestScene -logFile 'C:\Users\jywls\AppData\Local\Temp\battle_scene_validate_batch3.log'
```

```bash
git diff --check
```

---

## 완료 기준

Batch 3는 아래가 모두 만족되면 완료:

- 전투 시작 시 Enemy Break가 `2/2`로 표시된다.
- 약점 공격 1회 후 `1/2`로 감소한다.
- 약점 공격 2회 후 `BROKEN` 상태가 표시된다.
- Broken 상태에서 다음 플레이어 공격 피해가 증가한다.
- Break 보너스 공격 후 Break Gauge가 `2/2`로 리셋된다.
- Impact Text가 Break 감소/발생/소모를 명확히 보여준다.
- `RunBattleLogicAutoTest`: `RESULT: PASS`
- `ValidateBattleTestScene`: `RESULT: PASS`
- `git diff --check`: PASS
- `Docs/Batch2_CombatFeedback_Instructions.md`는 커밋 제외

---

## 보고 형식

완료 후 deep은 아래 형식으로 보고:

```markdown
배치 3 구현 완료.

## 변경 파일
- ...

## 구현 내용
- ...

## 검증 결과
- RunBattleLogicAutoTest: PASS
- CreateBattleTestScene: PASS 또는 완료 로그
- ValidateBattleTestScene: PASS
- git diff --check: PASS

## 커밋 전 주의사항
- untracked 문서 제외 여부
- 남은 이슈
```

커밋은 codex5.5 리뷰 후 진행.
