# GamePortfolio — Codex Tactics

Unity 2D 턴제 RPG 포트폴리오용 전투 수직 슬라이스입니다. 현재 Batch 55 기준으로 타이틀 화면, 스테이지 선택, 6개 스테이지/12개 encounter, 전투, 결과, 저장/진행도, 전장 기믹, UI/VFX/SFX, 자동 검증까지 포함합니다.

**최신 포트폴리오 정리:** [`Docs/Portfolio_CodexTactics_Review_and_Showcase_2026-05-18.txt`](Docs/Portfolio_CodexTactics_Review_and_Showcase_2026-05-18.txt)

**쇼케이스 초안:** [`Docs/PortfolioShowcaseDraft.md`](Docs/PortfolioShowcaseDraft.md)

**영문 README:** [`README.EN.md`](README.EN.md)

---

## 게임 루프

```text
Title Scene → Stage Select Scene → Battle Scene → Result / Continue / Retry → Stage Unlock / Save
```

- 6개 스테이지
- 각 스테이지 2개 encounter: 일반전 + 보스전
- 총 12개 전투 encounter
- 스테이지 클리어 시 다음 스테이지 및 스킬 진행도 해금

---

## 주요 시스템

### 전투 시스템

- **AP 기반 스킬 선택**
  - Slash: Physical / 0 AP
  - Ice Lance: Ice + Stun / 1 AP
  - Fire Bolt: Fire + Burn / 2 AP
  - Earth Wall: Earth + Shield / 2 AP
  - Lightning Strike: Lightning / 3 AP
- **속성/약점 시스템** — 약점 공격 시 데미지/Break 이점
- **상태이상** — Burn, Stun, Shield, Guard, Break, Enrage
- **아이템** — Potion, Hi-Potion, Ether, Full Ether
- **Auto Battle AI** — 아이템/가드/약점/스킬 우선순위 기반 자동 전투
- **결과 화면** — Rank, Pace, Damage, Guard/Skill/Item count, Gold/XP

### Stage Modifier / 전장 기믹

| Stage | Modifier | 실제 전투 효과 |
|---|---|---|
| Stage 1 | Tutorial Field | 안전한 학습용 전장 |
| Stage 2 | Pack Pressure | 적 강공격 주기 단축 |
| Stage 3 | Stoneguard | 적 Break gauge 증가 |
| Stage 4 | Storm Surge | 3번째 enemy turn마다 8 lightning hazard damage |
| Stage 5 | Void Drain | 2번째 enemy turn마다 AP 1 감소, AP 0이면 HP 5 damage |
| Stage 6 | Radiant Trial | 강공격 주기 단축 + Break gauge 증가 |

### UI / VFX / SFX

- Title screen star particles, fade-in, title float, button glow
- Stage Select auto-select, card pulse animation, modifier preview
- Battle HUD: HP/AP bars, status, enemy intent, Break gauge, stage progress, recent actions
- Element badge, impact text color coding, guard/status overlays
- Elemental projectile VFX, hit sparks, screen shake, damage/heal/buff popups
- AudioManager singleton with procedural fallback BGM/SFX

### 저장 & 진행

- JSON save/load
- Completed stages
- Player level / XP / total gold
- Difficulty mode
- Stage unlock state

---

## 기술 구조

| 파일/클래스 | 역할 |
|---|---|
| `BattleManager.cs` | 턴 상태 머신, 전투 로직, stage modifier 적용 |
| `BattleUI.cs` | 전투 UI 렌더링, 로그, 상태 표시, popup/VFX 연결 |
| `StageData.cs` / `EnemyData.cs` | 스테이지/적/기믹 데이터 정의 |
| `BattleBalanceConfig.cs` | HP/AP/데미지/보상 등 밸런스 값 관리 |
| `BattleResultEvaluator.cs` / `BattleResultPresenter.cs` | 결과 평가/표시 분리 |
| `StageSelectController.cs` | 스테이지 카드, 잠금/해금, modifier preview |
| `TitleManager.cs` | 타이틀 화면, 난이도, save/load/reset 진입 |
| `AudioManager.cs` | BGM/SFX 및 procedural fallback audio |
| `BattleAutoTestRunner.cs` | 전투 로직 자동 검증 |
| `BattleSceneAutoBuilder.cs` | 전투 씬 자동 생성/검증 |
| `GameFlowSceneAutoBuilder.cs` | Title/StageSelect/Battle flow 씬 자동 생성/검증 |

---

## 자동 검증

Unity Editor 메뉴:

- `Tools > Codex Tactics > Create Battle Test Scene`
- `Tools > Codex Tactics > Validate Battle Test Scene`
- `Tools > Codex Tactics > Run Battle Logic Auto Test`
- `Tools > Codex Tactics > Create Game Flow Scenes`
- `Tools > Codex Tactics > Validate Game Flow Scenes`

2026-05-18 검증 결과:

```text
git diff --check 9da4ab5..HEAD: PASS
BattleAutoTestRunner.RunBattleLogicAutoTest: PASS
GameFlowSceneAutoBuilder.ValidateGameFlowScenes: PASS
BattleSceneAutoBuilder.ValidateBattleTestScene: PASS
```

Unity batchmode에서 `This should not be called in batch mode.` 경고가 출력될 수 있지만, 현재 테스트는 exit code 0 및 `RESULT: PASS`로 정상 완료됩니다.

---

## 빠른 시작

1. Unity Hub에서 아래 폴더 열기:

```text
C:\Users\jywls\Desktop\game_portfolio\GamePortfolio
```

2. Unity 상단 메뉴에서 필요 시 씬 생성/검증:

```text
Tools > Codex Tactics > Create Game Flow Scenes
Tools > Codex Tactics > Validate Game Flow Scenes
Tools > Codex Tactics > Create Battle Test Scene
Tools > Codex Tactics > Validate Battle Test Scene
```

3. `TitleScene`부터 Play.

---

## 포트폴리오에서 강조할 점

- 완성된 수직 슬라이스: Title → Stage Select → Battle → Result → Save/Unlock
- 데이터 기반 설계: StageData / EnemyData / BattleBalanceConfig
- 실제 전투에 적용되는 Stage Modifier 시스템
- 자동 검증으로 UI/전투/씬 연결 회귀 방지
- UI/VFX/SFX를 통한 전투 피드백 강화
- C#과 Unity 학습을 실제 프로젝트 문제 해결에 적용한 과정

---

## 캡처 필요 목록

- Title screen
- Stage Select with modifier preview
- Battle HUD full view
- Fire/Burn, Ice/Stun, Break 발동 장면
- Stage 4 Storm Surge
- Stage 5 Void Drain
- Stage 6 Radiant Trial
- Result summary with rank/reward
