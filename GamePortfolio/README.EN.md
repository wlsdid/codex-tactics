# GamePortfolio — 2D Turn-Based RPG Prototype

> **A learn-by-building portfolio project**: A playable 2D turn-based battle system built in Unity, demonstrating data-driven design, clean code practices, scene flow architecture, and thorough documentation.

**Complete game loop:** Title → Stage Select → Battle → Result → Retry/Next → Stage Select return.  
3 stages (2 encounters each), sequential unlock on clear. Stage editor automation + 221 auto tests.

---

## What This Project Shows

### 🎮 Gameplay

**Full game loop:**
- **Title Screen** → "Start Game" button
- **Stage Select** — 3 stage cards with lock/unlock, descriptions, Start Battle
- **Battle** — turn-based combat with Attack, Fire Skill, Guard, End Turn
- **Result** — Victory/Defeat summary (rank S/A/B/C, rewards, tips)
- **Post-battle** — Retry, Continue to next encounter, or Stage Select return

**3 stages with progressive difficulty:**

| Stage | Normal | Boss |
|-------|--------|------|
| 1: Slime Scout Route | Slime Scout (80HP, Fire weak) | Slime King (140HP, Fire weak) |
| 2: Wolf Ambush | Wolf Scout (100HP, Nature weak) | Alpha Wolf (180HP, Nature weak) |
| 3: Golem Depths | Golem Sentry (120HP, Earth weak) | Ancient Golem (220HP, Earth weak) |

Only Stage 1 is unlocked initially. Clearing all encounters in a stage unlocks the next.

**Battle mechanics:** AP resource management, elemental weakness (+10 damage), Break gauge, Guard (50% damage reduction), Burn (damage-over-time), enemy pattern AI (heavy attack every 3rd turn), intent preview.

**UX highlights:**
- Stage card selection with selected/default/locked visual states
- Run Status, Objective, Progress labels for current stage awareness
- Enemy Intent preview before every enemy turn
- Recent Actions battle log panel
- Compact result summary (turns, HP/AP, damage, choices, rank, gold, tips)

### 🏗️ Architecture

```
Assets/Scripts/
├── Flow/                     # Scene navigation
│   ├── GameSceneFlow.cs      — Scene loader (Title/StageSelect/Battle)
│   └── StageSelectController — Card selection, lock/unlock, Start Battle
├── Battle/                   # Battle system
│   ├── BattleManager.cs      — State machine, turns, damage
│   ├── BattleUI.cs           — UI rendering (separated from logic)
│   ├── BattleResult*.cs      — Result data / evaluator / presenter
│   └── BattleState.cs        — State enum
├── Data/                     # All configurable values
│   ├── StageData.cs          — 3 stages, 6 encounters total
│   ├── ElementType.cs        — 9 elements (incl. Earth)
│   └── ... (EnemyData, CharacterData, SkillData, EnemyPatternData)
└── ProgressState.cs          — Static unlock/clear tracking
Assets/Editor/
├── GameFlowSceneAutoBuilder.cs — Title/StageSelect scene generator + validator (36 checks)
├── BattleSceneAutoBuilder.cs   — Battle scene generator + validator
└── BattleAutoTestRunner.cs     — Battle logic auto test (221 checks)
```

Key pattern: **separation of concerns** — BattleManager (logic) → BattleUI (display) → BattleResultEvaluator (rules) → BattleResultPresenter (formatting). Each has one responsibility.

### 🔓 Progress System

`ProgressState` static class tracks stage completion across scene loads:
- Stage 0 always unlocked
- Clearing all encounters in a stage → `ProgressState.MarkStageCompleted(stageIndex)` → next stage unlocks
- Static fields persist within a Unity play session (reset on Play Mode exit)
- Ready for Save/Load system integration

### 🧪 Testing & Validation

| Menu Command | What It Does |
|---|---|
| `Tools > Codex Tactics > Create Game Flow Scenes` | Regenerates Title + StageSelect + Battle scenes |
| `Tools > Codex Tactics > Validate Game Flow Scenes` | Validates scene structure + UI links + button listeners (36 checks) |
| `Tools > Codex Tactics > Run Battle Logic Auto Test` | Battle logic + ProgressState + stage data tests (221 checks) |

### 📝 Documentation Culture

Every feature batch is paired with:
- **Devlog** — what was done and how
- **Study notes** — what was learned and why

This practice makes the project a genuine portfolio of growth, not just a feature list.

---

## Quick Start

1. Open the project in Unity Hub
2. `Tools > Codex Tactics > Create Game Flow Scenes`
3. Verify 3 scenes are in Build Settings (Scene In Build checked)
4. Press **Play** → TitleScreen
5. Click "Start Game" → StageSelectScene
6. Select Stage 1 → "Start Battle" → fight!
7. After Victory: "Next Encounter" → boss fight → Stage Select return
8. Repeat Stage 1 → Stage 2 unlocks → select and battle Stage 2

---

## Project Layout

```
Assets/
├── Scenes/
│   ├── TitleScene.unity
│   ├── StageSelectScene.unity
│   └── BattleScene.unity
├── Scripts/
│   ├── Flow/
│   │   ├── GameSceneFlow.cs
│   │   └── StageSelectController.cs
│   ├── Battle/
│   │   ├── BattleManager.cs
│   │   ├── BattleUI.cs
│   │   ├── BattleResultData.cs
│   │   ├── BattleResultEvaluator.cs
│   │   └── BattleResultPresenter.cs
│   ├── Data/
│   │   ├── StageData.cs
│   │   ├── EnemyData.cs
│   │   ├── CharacterData.cs
│   │   ├── SkillData.cs
│   │   ├── EnemyPatternData.cs
│   │   ├── ElementType.cs
│   │   └── BattleBalanceConfig.cs
│   └── ProgressState.cs
├── Editor/
│   ├── GameFlowSceneAutoBuilder.cs
│   ├── BattleSceneAutoBuilder.cs
│   ├── BattleAutoTestRunner.cs
│   └── CreateBalanceConfigAsset.cs
Docs/
├── Captures/             — Screenshots & GIFs
├── Devlog/               — Per-batch dev notes
├── Study/                — Per-feature learning notes
├── BalanceTable.md       — Tuning rationale
├── PortfolioShowcaseDraft.md
└── ManualValidat*        — QA checklists
```

---

## Key Technical Decisions

| Decision | Rationale |
|----------|-----------|
| **Separate Flow/ from Battle/** | Scene navigation independent of combat logic; clean entry points for future menu systems |
| **ProgressState static class** | Simple cross-scene state without MonoBehaviour lifecycle issues; easy to replace with persistence later |
| **StageSelectController via serialized fields** | Inspector-driven card references; no hardcoded scene object lookups |
| **Data-driven stages & enemies** | Adding Stage 4+ = data additions only; Encounter difficulty tuned per stage in one place |
| **Separate Result Data/Evaluator/Presenter** | Prevents result logic from bloating BattleManager; each class has one responsibility |
| **Editor automation** | Scene generators + validators + auto-tester enable rapid iteration without manual setup; 221 auto checks catch regressions |

---

## Roadmap

- [x] Core battle loop (attack, skill, guard, enemy AI)
- [x] Stage encounters (normal → boss per stage)
- [x] Result system (ranks, rewards, tips)
- [x] Title → Stage Select → Battle → Result full scene flow
- [x] Stage lock/unlock via ProgressState
- [x] 3 stages with data-driven encounters
- [x] Continue button label ("Next Encounter")
- [ ] Save/Load persistence for ProgressState
- [ ] BattleManager UI polish / sprite integration
- [ ] Additional stages (Stage 4+)
- [ ] Shop / inventory system

---

## Tech Stack

- **Engine**: Unity 6000.4.6f1 (URP)
- **Language**: C#
- **UI**: TextMeshPro + uGUI
- **Testing**: Editor script-based auto-tests (221 checks)

---

## License

MIT — free to use as a learning reference or portfolio template.
