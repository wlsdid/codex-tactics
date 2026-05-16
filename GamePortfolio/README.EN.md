# GamePortfolio — 2D Turn-Based RPG Prototype

> **A learn-by-building portfolio project**: A fully playable 2D turn-based battle system built entirely in Unity over 37 incremental batches. Features 6 unique stages, 5 player skills, 2 items, status effects, auto-battle AI, difficulty modes, save/load, and full visual/audio feedback.

---

## Quick Links
- Korean README: [`README.md`](README.md)
- Devlog: [`Docs/Devlog/`](Docs/Devlog/)
- Study Docs: [`Docs/Study/`](Docs/Study/)

---

## Game Loop

```
Title Screen → Stage Select → Battle (6 stages × 2 encounters = 12 battles) → Result/Bonuses → Next Encounter → Stage Select
```

## Features (37 Batches)

### Battle System
- **6 Stages** — Fire, Nature, Earth, Lightning, Dark, Light (2 encounters each, 12 total)
- **5 Player Skills** — Slash (Phys/0AP), Ice Lance (Ice+Stun/1AP), Earth Wall (Earth+Shield/2AP), Fire Bolt (Fire+Burn/2AP), Lightning Strike (Lightning/3AP)
- **Skill Unlock** — Skills unlock progressively as you clear stages
- **Element System** — 1.5x weakness multiplier, 7 elements (Fire/Ice/Lightning/Nature/Dark/Light/Earth)
- **Status Effects** — Burn (DoT), Stun (skip turn), Shield (absorb damage), Break (1.5x bonus damage), Enrage (enemy berserks at low HP)
- **Difficulty Modes** — Normal / Hard (1.5x HP, 1.3x damage, 2x Break gauge)

### Items
- **Potion** — Restore 30 HP (3 units)
- **Ether** — Restore 2 AP (2 units)
- Auto-battle: Potion at HP<30%, Ether at AP<1

### AI & Automation
- **Auto-Battle AI** — Priority tree: Guard → Items → Weakness → Lightning → Ice → Earth → Fire → Basic Attack
- **Enemy Enrage** — Below 30% HP: 1.5x damage multiplier + ENRAGED! indicator

### UI & Visual Effects
- **Dynamic HP/AP Bars** — Green→Yellow→Red (HP), Blue→Cyan→Orange (AP)
- **2x Speed Toggle**
- **Status Overlays** — Burn (red pulse), Stun (blue pulse), Broken (white flash)
- **Screen Shake** — On strong enemy attacks
- **Hit Flash** — Enemy white flash / Player red flash on damage
- **Skill Projectiles** — Colored by element (Fire=orange, Ice=blue, Lightning=yellow, Earth=green)
- **Screen Fade** — Black fade in/out between encounters
- **Pause Menu** — Resume / Quit to Stage Select

### Save & Progression
- **JSON Save** — Persistent stage completion
- **Level/XP** — XP on stage clear, +20 Max HP per level
- **Stage Bonuses** — Untouchable(+50g), Swift Victory(+30g), Jack of All Trades(+20g), Iron Wall(+15g), Pure Combat(+10g)

### Audio
- **BGM** — Battle/Victory music hooks
- **SFX** — Attack/Skill/Guard/Hit/Victory/Defeat sounds
- **AudioManager** — Singleton, DontDestroyOnLoad

### Editor Tools
- `Tools > Codex Tactics > Create Battle Test Scene` — Auto-generates full battle scene
- `Tools > Codex Tactics > Validate Battle Test Scene` — 220+ validation checks
- `Tools > Codex Tactics > Create Stage Select Scene` — Auto-generates stage select scene

## System Requirements

- **Unity:** 6000.4.6f1 (or compatible)
- **Platform:** Windows/Mac
- **Storage:** ~100MB

## Quick Start

1. Add/open project folder in Unity Hub
2. `Tools > Codex Tactics > Create Battle Test Scene`
3. `Tools > Codex Tactics > Validate Battle Test Scene` (should PASS)
4. `Tools > Codex Tactics > Create Stage Select Scene`
5. Add scenes to **Build Settings**: TitleScene, StageSelectScene, BattleScene
6. Press Play (start from TitleScene)

## Testing

- **Edit Mode Tests:** `Window > General > Test Runner` > `EditMode` > `Run All`
- **Scene Validate:** `Tools > Codex Tactics > Validate Battle Test Scene`
- Battle logic auto-test + BattleScene validation PASS
- Testable headlessly via `-batchmode -nographics`

## Documentation Structure

```
Docs/
├── Devlog/        — Daily development logs (51 files)
└── Study/         — System design deep-dives (45 files)
```

## Tech Stack

- **Unity 6000.4.6f1** (URP, 2D Orthographic)
- **C#** — Pure C#, zero external libraries
- **TextMeshPro** — UI text rendering
- **Batchmode** — CI/CD compatible (WSL tested)

## Commit History

| Batch | Feature |
|-------|---------|
| 1-11 | Core battle loop, elements, Break/Stun, stage select |
| 12-15 | Element multiplier, Ice Lance, Stage 4, Lightning Strike |
| 16-21 | Portfolio docs, auto-battle, speed toggle, HP colors, Earth Wall |
| 22-25 | Stage 5-6, screen shake, save system, audio system |
| 26-30 | Status overlays, items, stage bonuses, difficulty modes |
| 31-37 | Enrage AI, stage select builder, pause menu, projectiles, fade, skill unlock |
