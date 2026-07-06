# Codex Tactics — Portfolio Showcase Draft

> Updated: 2026-05-31 — current target: short, high-quality commercial-looking portfolio demo with professionalized generated scene presentation. Covers Batch 89 scope plus prior GIF evidence: working standalone capture pipeline, chibi pixel-art inspired battle standees, premium battle presentation pass, generated scene validation, portfolio captures, 6 stages, 12 encounters, stage modifiers, battle UI/VFX/SFX polish, items, result/progress systems, automated validation, and reviewer-facing GIF explanation notes.

## 1. Game overview

**Codex Tactics** is a Unity 2D turn-based RPG vertical slice. It demonstrates a complete playable loop from title screen to stage selection, tactical battle, result summary, rewards, save/progress tracking, and stage unlocks.

The project focuses on portfolio-visible systems and presentation: AP-based skill choices, elemental weaknesses, status effects, Break gauge, items, auto-battle AI, stage-specific battlefield modifiers, premium tactical battle UI, original generated PNG chibi pixel standees, UI/VFX/SFX feedback, and Unity Editor validation tools.

Latest showcase direction:

> A short 5-10 minute vertical slice that looks closer to a polished commercial indie tactics demo than a test scene. Scope can stay small; first impression, UI hierarchy, battle readability, and documented process are the priority.

Latest visual polish pass:

- Title Scene now uses a dark premium frame, gold dividers, battlefield silhouette, party/enemy silhouettes, and a concise reviewer pitch line.
- Stage Select cards now include thumbnail-style art frames, element-colored accents, ground strips, and locked-card dim overlays.
- Battle Scene now has layered battlefield depth: distant forest silhouette, moonlight beam, foreground fog, rear horizon line, and unit base rings.
- Validators were extended so these presentation elements are automatically checked after regeneration.

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

## 5.1 Presentation direction

The current polish target is not a long RPG campaign. It is a compact portfolio demo that should look deliberate in screenshots and short videos.

Current commercial-look improvements:

- Dark tactical RPG panel hierarchy: top status, side unit cards, center battlefield, command bar.
- Generated battle backdrop, tactical grid, formation markers, floor glow, gold dividers, and vignette shadows.
- Original generated PNG hero/enemy chibi pixel standees in the battlefield so screenshots show characters, not only UI boxes.
- Improved Batch 78 pixel-art pass: clearer hero face/armor/sword silhouette, boss horn/crown/wing identity, and roster mini sprites for party/enemy readability.
- Reference-driven art direction: large heads, small bodies, crisp dark outlines, limited fantasy palettes, readable weapon/accessory silhouettes, and soft oval shadows. The implementation adapts the feel as original generated art instead of copying commercial/reference sprites.
- Premium command header and AP badge to make the skill area feel like a designed game interface.
- Stage Select showcase frame and fresh README capture contact sheet.

Next visual priorities:

1. Expand the chibi pixel standee system into a small original party/enemy roster with palette swaps and class silhouettes.
2. Improve skill impact timing and hit feedback so short GIFs feel satisfying.
3. Tighten Stage Select thumbnails and stage-card art direction.
4. Record a 8-15 second gameplay GIF for the README.

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

## 6.4 README GIF evidence — Batch 83

The current README GIF should be presented as portfolio evidence, not only as decoration. It demonstrates the connected vertical slice in a short review-friendly loop:

```text
Title -> Stage Select -> Battle HUD -> Fire/Burn feedback -> Guard feedback -> Result summary -> Retry reset
```

Frame-by-frame reviewer notes are maintained in [`Docs/ShowcaseGifEvidence.md`](ShowcaseGifEvidence.md).

What the GIF proves concretely:

- **Flow completion:** the project no longer relies on a single isolated battle test scene; it has title, stage selection, combat, result, and retry states.
- **Tactical UI direction:** party/enemy rosters, central battlefield, command area, recent actions, and enemy intent are visually separated for screenshot/GIF readability.
- **Data-driven design:** stage previews and battle behavior use `StageData`/`EnemyData`; skills use `SkillData`; result ranks/rewards/tips are handled through result data/evaluator/presenter classes.
- **Technical polish loop:** hit feedback, idle bob, hit reaction, and result metrics were chosen because they improve a short portfolio capture, not because they add unnecessary scope.
- **Verification path:** reviewers can run Unity `Tools > Codex Tactics` validators and rebuild the GIF with `python3 Docs/Captures/build_readme_gif.py`.

## 6.5 Capture media decision and runtime MP4 conversion — Batch 85/86

Batch 85 reran the existing standalone capture runner and confirmed it safely refreshes the deterministic PNG sequence in `Docs/Captures/`, but it does not produce a true MP4 by itself.

Current showcase choice:
- Use `Docs/Captures/codex_tactics_battle_loop.gif` as the primary README/portfolio media because it is compact and directly communicates the vertical slice flow.
- Use `Docs/Captures/codex_tactics_runtime_motion_storyboard.gif` as secondary motion/VFX evidence, clearly labelled as a storyboard fallback.
- Do not commit raw MP4 yet. A true runtime clip should be recorded through Windows Game Bar/OBS or a future in-engine recorder, then trimmed, converted, and checked against the acceptance criteria in [`Docs/CaptureMediaDecision.md`](CaptureMediaDecision.md).

Batch 86 adds the reproducible conversion step for that future source clip:

```bash
python3 Docs/Captures/convert_runtime_clip.py "/mnt/c/Users/jywls/Videos/Captures/YOUR_RUNTIME_CLIP.mp4"
```

The converter creates `codex_tactics_runtime_clip.gif` plus `codex_tactics_runtime_clip_preview.png`, using 12 seconds, 960px width, 12 fps, a 96-color palette, and a 5 MB size cap by default. It validates `ffmpeg`/`ffprobe`, source duration/dimensions, GIF width/frame count/file size, and preview output before the media is considered portfolio-ready.

## 7. Validation status on 2026-07-06

Current presentation focus:
- The battle screen is shifting from feature-proof layout toward commercial screenshot readability.
- Latest builder pass enlarges central hero/enemy standees, adds cinematic battlefield lighting, removes debug-looking header suffixes, and softens reviewer/capture chips.
- Unity scene/capture refresh is pending because the current Windows Unity batch run is blocked by license activation (`No valid Unity Editor license found`). Re-run validation after Unity is signed in/activated.

Most recent completed checks:

```text
C# brace/parenthesis balance check: PASS
Git whitespace check (`git diff --check`): PASS
Unity CreateBattleTestScene: BLOCKED by license activation on 2026-07-06
```

Previous fully regenerated capture set remains documented in earlier batches; do not treat screenshots as reflecting the 2026-07-06 battle-builder polish until Unity scene generation and capture refresh pass.

## 8. Review notes

### Looks good

- Full playable vertical slice exists.
- Stage 2~6 modifiers are implemented and tested.
- Battle UI, VFX, SFX, stage flow, result flow, and save/progress systems are connected.
- Data-driven structure is strong for portfolio explanation.
- Automated validation gives concrete evidence of reliability.

### Recommended follow-up polish

1. Add 2-3 more original chibi pixel variants for party/enemy roster readability.
2. Add stronger skill impact anticipation/hit/recovery feedback.
3. Capture a true runtime motion GIF/MP4 after the next camera/VFX pass.
4. Keep README, GIF evidence notes, and devlog updated after every visual polish batch.

## 9. Screenshots/GIF checklist

- [x] Title screen with star particles and start button glow
- [x] Stage Select with unlocked/locked cards and modifier text
- [x] Battle HUD showing HP/AP bars, enemy element badge, and tactical layout
- [x] Fire Bolt + Burn feedback
- [x] Ice Lance + Stun feedback GIF
- [ ] Break gauge depletion and Break bonus
- [ ] Stage 4 Storm Surge activation
- [ ] Stage 5 Void Drain AP drain / AP-empty HP damage
- [ ] Stage 6 Radiant Trial start and Break/strong-attack pressure
- [x] Result summary with rank/reward/metrics
- [x] 8-15 second polished gameplay GIF

## 10. Short portfolio description

Codex Tactics is a Unity 2D turn-based RPG prototype built as a portfolio vertical slice. It includes a complete flow from title screen to stage selection, tactical battle, result summary, stage progression, and save data. The battle system uses AP-based skills, elemental weaknesses, status effects, Break gauge, items, auto-battle AI, and stage-specific battlefield modifiers. I also built Unity Editor automation to generate and validate scenes, allowing every new combat mechanic to be tested through batchmode checks.

## 11. Korean presentation summary

Codex Tactics는 Unity 2D 기반 턴제 RPG 포트폴리오 프로젝트입니다. 단순 전투 데모가 아니라 타이틀, 스테이지 선택, 전투, 결과, 저장/진행도까지 이어지는 수직 슬라이스를 목표로 만들었습니다. 전투는 AP 기반 스킬, 속성 약점, 상태이상, Break, 아이템, 자동 전투, 스테이지별 전장 기믹으로 구성되어 있습니다. 최근에는 브라운더스트2처럼 캐릭터성이 살아있는 전투 화면을 목표로, 레퍼런스의 치비 픽셀 감성을 직접 복제하지 않고 큰 머리/작은 몸/진한 외곽선/제한 팔레트 기반의 오리지널 절차형 스탠디로 재해석했습니다. 특히 StageData를 중심으로 스테이지 표시와 실제 전투 로직을 연결했고, Unity Editor 자동 검증을 통해 기능 추가 후에도 전투 로직과 UI 연결이 깨지지 않도록 관리했습니다.
