# Codex Tactics Portfolio Showcase Draft

This page is a draft for the later portfolio write-up. It summarizes what the prototype currently demonstrates and what evidence still needs to be captured in Unity Play Mode.

## Project overview

`Codex Tactics` is a Unity 2D turn-based RPG battle prototype. The current scope is intentionally small: one hero, one slime enemy, a complete battle loop, visible player choices, an enemy pattern preview, and a result screen.

## Current playable loop

```text
Start Battle
-> Player Turn
-> choose Attack / Fire Skill / Guard / End Turn
-> Enemy Turn
-> repeat until Victory or Defeat
-> Result Summary
-> Retry
```

## Portfolio systems demonstrated

### 1. Battle state flow

The battle uses a clear state flow instead of mixing every action together. This makes the project easier to explain and extend.

Relevant files:

- `Assets/Scripts/Battle/BattleManager.cs`
- `Assets/Scripts/Battle/BattleState.cs`
- `Docs/BattleStateMachine.md`

### 2. Data-driven battle values

Character, skill, status, and enemy pattern values are separated into small data classes/enums.

Relevant files:

- `Assets/Scripts/Data/CharacterData.cs`
- `Assets/Scripts/Data/SkillData.cs`
- `Assets/Scripts/Data/EnemyPatternData.cs`
- `Assets/Scripts/Data/StatusEffectType.cs`
- `Assets/Scripts/Data/ElementType.cs`

### 3. Tactical player choices

The prototype has several different decisions:

- `Attack`: no-cost reliable damage.
- `Fire Skill`: AP cost, higher damage, weakness bonus, Burn effect.
- `Guard`: sacrifices tempo to reduce incoming damage.
- `End Turn`: skips action to continue the turn cycle.

### 4. Enemy intent and pattern readability

The enemy previews its next action:

- `Normal Attack (15)`
- `Heavy Slam (30)` every 3rd enemy turn

This makes the battle more tactical because the player can choose Guard before a predictable danger spike.

### 5. Result summary and evaluation

The result screen reports not only Victory/Defeat, but also how the player reached that result:

- damage dealt
- damage taken
- Guard uses
- Skills used
- Pace
- Rank
- Reward
- Tip
- Last enemy pattern

Relevant files:

- `Assets/Scripts/Battle/BattleResultData.cs`
- `Assets/Scripts/Battle/BattleResultEvaluator.cs`
- `Assets/Scripts/Battle/BattleResultPresenter.cs`

### 6. Editor validation workflow

The project includes Unity Editor menu automation so the test scene and battle logic can be checked repeatedly.

Unity menu path:

```text
Tools > Codex Tactics > Create Battle Test Scene
Tools > Codex Tactics > Validate Battle Test Scene
Tools > Codex Tactics > Run Battle Logic Auto Test
```

Relevant files:

- `Assets/Editor/BattleSceneAutoBuilder.cs`
- `Assets/Editor/BattleAutoTestRunner.cs`

## Current verification evidence

Latest automated verification has passed:

- Unity batch compile: PASS
- Battle scene validation: PASS
- Battle logic auto test: PASS

Manual Play Mode evidence still needs to be captured by opening Unity and recording screenshots/GIFs.

## Screenshot/GIF targets

Save future media under:

```text
Docs/Captures/
```

Recommended captures:

1. Start state: HP/AP bars, enemy intent, action buttons.
2. Fire Skill: AP decrease, Burn status, damage feedback.
3. Guard: player status changes to `Guarding`, reduced damage is shown.
4. Heavy Slam preview: enemy intent shows the strong attack.
5. Result summary: Pace, Rank, Reward, Tip, and Retry button visible.

## Short portfolio explanation draft

> I built a small Unity turn-based RPG battle prototype focused on a complete vertical slice. The player can attack, spend AP on a Fire skill, Guard against predictable enemy patterns, and retry after Victory or Defeat. I separated battle flow, result data, result evaluation, and result text formatting into different scripts so the code stays readable as the result screen grows. I also added Unity Editor validation tools to regenerate the test scene and automatically verify the battle logic.

## Next polish before final portfolio use

1. Capture screenshots/GIFs from Unity Play Mode.
2. Add those captures to the README.
3. Replace placeholder UI art with simple pixel-art-style panels/buttons.
4. Add one additional enemy or boss pattern after the current battle loop is visually documented.
