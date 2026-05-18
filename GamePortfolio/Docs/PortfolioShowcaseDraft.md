# Codex Tactics — Portfolio Showcase Draft

> Updated: 2026-05-18 — current main `14ebcd2`, covers Batch 55 scope: 6 stages, 12 encounters, stage modifiers, battle UI/VFX/SFX polish, items, result/progress systems, and automated validation.

## 1. Game overview

**Codex Tactics** is a Unity 2D turn-based RPG vertical slice. It demonstrates a complete playable loop from title screen to stage selection, tactical battle, result summary, rewards, save/progress tracking, and stage unlocks.

The project focuses on portfolio-visible systems: AP-based skill choices, elemental weaknesses, status effects, Break gauge, items, auto-battle AI, stage-specific battlefield modifiers, UI/VFX/SFX feedback, and Unity Editor validation tools.

## 2. Play loop

```text
Title Scene → Stage Select → Battle → Result / Continue / Retry → Stage Unlock / Save
```

- 6 stages
- 2 encounters per stage: normal + boss
- 12 total battle encounters
- Stage clear unlocks the next stage and supports progression/save state

## 3. Current stage structure

| Stage | Modifier | Normal enemy | Boss enemy | Gameplay purpose |
|---|---|---|---|---|
| Stage 1 | Tutorial Field | Slime Scout | Slime King | Learn basic battle flow safely |
| Stage 2 | Pack Pressure | Wolf Scout | Alpha Wolf | Faster enemy strong attacks |
| Stage 3 | Stoneguard | Golem Sentry | Ancient Golem | Reinforced Break gauge |
| Stage 4 | Storm Surge | Storm Hawk | Thunder Phoenix | Periodic lightning hazard every 3 enemy turns |
| Stage 5 | Void Drain | Shadow Wraith | Shadow Lord | AP drain every 2 enemy turns; HP damage if AP is empty |
| Stage 6 | Radiant Trial | Light Warden | Holy Sentinel | Final trial: faster strong attacks + reinforced Break gauge |

## 4. Core battle systems

### AP-based skills

| Skill | AP | Element | Power | Effect |
|---|---:|---|---:|---|
| Slash | 0 | Physical | 20 | Reliable basic attack |
| Ice Lance | 1 | Ice | 25 | Applies Stun |
| Fire Bolt | 2 | Fire | 30 | Applies Burn |
| Earth Wall | 2 | Earth | 22 | Applies Shield |
| Lightning Strike | 3 | Lightning | 40 | High-damage burst |

Latest balance direction:
- Player max AP: 3
- Player turn AP recovery: 2
- Shield amount: 20
- Burn damage: 5 per tick

### Element and weakness system

- Enemies have weakness elements.
- Weakness hits improve damage and reduce Break gauge.
- UI shows element information and impact feedback.

### Status and defensive mechanics

- **Burn**: damage over time.
- **Stun**: enemy skips a turn.
- **Shield**: absorbs incoming damage.
- **Guard**: reduces next enemy attack.
- **Break**: weakness pressure depletes gauge; broken targets take bonus damage.

### Items

- Potion: restores 30 HP.
- Hi-Potion: restores 60 HP.
- Ether: restores 2 AP.
- Full Ether: restores AP up to max.

### Auto Battle AI

Auto Battle uses a simple priority-based decision tree:

```text
Guard / Item → Weakness skill → Lightning → Ice → Earth → Fire → Basic attack
```

This helps test and demonstrate battles quickly.

## 5. Technical architecture

| Area | Main files | Responsibility |
|---|---|---|
| Battle flow | `BattleManager.cs` | Turn state, player/enemy actions, stage modifiers, battle result trigger |
| UI rendering | `BattleUI.cs` | HP/AP bars, status, logs, impact text, result panel, overlays |
| Stage data | `StageData.cs`, `EnemyData.cs`, `EnemyPatternData.cs` | Data-driven encounters, modifiers, enemy patterns |
| Skills/items | `SkillData.cs`, `ItemData.cs` | Skill and item definitions |
| Balance | `BattleBalanceConfig.cs` | Tunable HP/AP/damage/reward/config values |
| Results | `BattleResultData.cs`, `BattleResultEvaluator.cs`, `BattleResultPresenter.cs` | Rank, pace, rewards, summary text |
| Flow | `TitleManager.cs`, `StageSelectController.cs`, `GameSceneFlow.cs` | Title, stage select, scene navigation |
| Save/progress | `ProgressState.cs`, `SaveManager.cs` | Unlocks, completed stages, level/XP/gold persistence |
| Audio/VFX | `AudioManager.cs`, `SkillProjectile.cs`, `ScreenShake.cs`, `DamagePopup.cs` | Feedback, procedural fallback SFX, projectile/hit effects |
| Editor tools | `BattleAutoTestRunner.cs`, `BattleSceneAutoBuilder.cs`, `GameFlowSceneAutoBuilder.cs` | Automated scene generation and validation |

## 6. Problem-solving highlights

### 6.1 Stage Select vs Battle data consistency

Problem: Stage Select preview text and actual Battle logic could drift if each screen maintained separate modifier/reward data.

Solution:
- `StageData` became the single source for modifier metadata.
- Stage Select uses `StageData.BuildModifierSummaryText()`.
- Battle uses the same `StageData.stageModifier` to apply real combat effects.
- Auto-tests verify both metadata and actual runtime behavior.

### 6.2 Stage modifiers became real combat mechanics

The project moved from descriptive stage labels to functional battlefield rules:

- Pack Pressure changes enemy strong attack cadence.
- Stoneguard changes Break gauge.
- Storm Surge deals periodic hazard damage.
- Void Drain drains AP or damages HP when AP is empty.
- Radiant Trial combines pressure and Break difficulty.

### 6.3 Regression prevention through Unity batchmode tests

Instead of relying only on manual Play Mode checks, the project includes Editor validation methods that can run in batchmode:

- Battle logic auto-test
- Battle scene wiring validator
- Title/StageSelect/Battle flow validator
- `git diff --check` whitespace verification

## 7. Validation status on 2026-05-18

Reviewed at HEAD: `14ebcd2 Batch 55: Fix fallback config values - AP recovery 2, shield 20, burn 5`

Passed checks:

```text
git diff --check 9da4ab5..HEAD: PASS
BattleAutoTestRunner.RunBattleLogicAutoTest: PASS
GameFlowSceneAutoBuilder.ValidateGameFlowScenes: PASS
BattleSceneAutoBuilder.ValidateBattleTestScene: PASS
```

Note: Unity batchmode still prints a benign `This should not be called in batch mode.` warning from editor dialog usage, but the validation methods return exit code 0 and `RESULT: PASS`.

## 8. Review notes

### Looks good

- Full playable vertical slice exists.
- Stage 2~6 modifiers are implemented and tested.
- Battle UI, VFX, SFX, stage flow, result flow, and save/progress systems are connected.
- Data-driven structure is strong for portfolio explanation.
- Automated validation gives concrete evidence of reliability.

### Recommended follow-up polish

1. Prevent repeated overlay pulse coroutines in `BattleUI`.
2. Replace repeated `FindObjectOfType<Canvas>()` in projectile hit spark creation with cached/passed references.
3. Update the root README to match Batch 55 / 6-stage scope.
4. Capture fresh screenshots/GIFs for Stage Select, Stage 4~6 modifiers, and Result Summary.

## 9. Screenshots/GIF checklist

- [ ] Title screen with star particles and start button glow
- [ ] Stage Select with unlocked/locked cards and modifier text
- [ ] Battle HUD showing HP/AP bars, enemy element badge, and battle log
- [ ] Fire Bolt + Burn feedback
- [ ] Ice Lance + Stun feedback
- [ ] Break gauge depletion and Break bonus
- [ ] Stage 4 Storm Surge activation
- [ ] Stage 5 Void Drain AP drain / AP-empty HP damage
- [ ] Stage 6 Radiant Trial start and Break/strong-attack pressure
- [ ] Result summary with rank/reward/metrics

## 10. Short portfolio description

Codex Tactics is a Unity 2D turn-based RPG prototype built as a portfolio vertical slice. It includes a complete flow from title screen to stage selection, tactical battle, result summary, stage progression, and save data. The battle system uses AP-based skills, elemental weaknesses, status effects, Break gauge, items, auto-battle AI, and stage-specific battlefield modifiers. I also built Unity Editor automation to generate and validate scenes, allowing every new combat mechanic to be tested through batchmode checks.

## 11. Korean presentation summary

Codex Tactics는 Unity 2D 기반 턴제 RPG 포트폴리오 프로젝트입니다. 단순 전투 데모가 아니라 타이틀, 스테이지 선택, 전투, 결과, 저장/진행도까지 이어지는 수직 슬라이스를 목표로 만들었습니다. 전투는 AP 기반 스킬, 속성 약점, 상태이상, Break, 아이템, 자동 전투, 스테이지별 전장 기믹으로 구성되어 있습니다. 특히 StageData를 중심으로 스테이지 표시와 실제 전투 로직을 연결했고, Unity Editor 자동 검증을 통해 기능 추가 후에도 전투 로직과 UI 연결이 깨지지 않도록 관리했습니다.
