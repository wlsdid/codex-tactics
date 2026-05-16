# Player Shield System: Defensive Protection Design (플레이어 쉴드 시스템: 방어적 보호 설계)

## Overview

The Shield system is a player-side protective mechanic granted by the Earth Wall skill. Unlike Burn (enemy DoT) and Stun (enemy skip-turn), Shield operates on the player side — it absorbs incoming damage before it reaches the player's HP.

## Core mechanic

```
Earth Wall hit → Shield applied to player (15 absorption)
     ↓
Enemy attacks → Shield absorbs damage → Remaining passes to player
     ↓
Shield consumed (one-use)
```

### Key properties

| Property | Value | Config field |
|----------|-------|-------------|
| Absorption amount | 15 | `earthSkillShieldAmount` |
| Usage | One-hit, fully consumed | — |
| Stacking | Overwrites previous shield | — |
| Interaction with Guard | Guard + Shield = double reduction | — |

## Implementation

```csharp
// In UsePlayerSkill (Earth Wall hit):
if (skill == earthSkill)
{
    playerShieldAmount = CfgEarthSkillShieldAmount;
}

// In ResolveEnemyAttack (before damage application):
bool shieldAbsorbed = playerShieldAmount > 0;
if (shieldAbsorbed)
{
    int absorbed = Mathf.Min(playerShieldAmount, damage);
    damage = Mathf.Max(1, damage - absorbed);
    playerShieldAmount = 0;
}
```

## Design rationale

### Why player-side (not enemy status effect)?
Status effects (Burn, Stun) are applied to the enemy. Shield is unique because it protects the player. This makes Earth Wall a **defensive** skill, distinct from the other elemental skills which are all **offensive**:

| Skill | Target | Primary Effect |
|-------|--------|---------------|
| Slash | Enemy | Damage only |
| Fire Bolt | Enemy | Damage + Burn DoT |
| Ice Lance | Enemy | Damage + Stun skip |
| Lightning Strike | Enemy | Pure burst damage |
| **Earth Wall** | **Both** | **Damage + player Shield** |

### Why one-use (not duration-based)?
A multi-turn shield would be too powerful for 2 AP. The game's combat is tuned for 3-4 turn encounters. A 2-turn shield would negate 2 enemy turns, trivializing the fight. One-use keeps Earth Wall as a tactical tool (block one strong attack) rather than an I-win button.

### Why 15 absorption?
- Stage 1 normal attack: 15 damage → Shield blocks 100%
- Stage 1 strong attack: 30 damage → Shield blocks 50% (15/30)
- Stage 4 normal attack: 22 damage → Shield blocks 68% (15/22)
- Stage 4 strong attack: 40 damage → Shield blocks 38% (15/40)

15 is enough to fully block the weakest attacks early-game, and provides meaningful (but not complete) protection later. It's strong enough to be worth 2 AP, but weak enough that the player still needs Guard for heavy hits.

### Interaction with Guard
Guard reduces damage by 50% first, then Shield absorbs from the reduced amount. Stacking both is powerful but costs 1 action (2 AP + Guard = 2 turns of investment), which is balanced by the turn economy.

## UI feedback

The system shows shield status in three ways:
1. **Player shield text**: "Shield: 15" (visible at all times while active)
2. **Battle message**: "Shield active! (15 damage absorbed next hit)."
3. **Impact text on hit**: "Impact: Shield absorbed 15 damage, reduced to 5"

This triple feedback ensures the player always knows their shield status.

## Portfolio relevance

1. **Player-side effect** — Unique among the skill set (only defensive skill)
2. **Configurable absorption** — Designer-tunable via BattleBalanceConfig
3. **Tactical depth** — Shield vs Guard decision adds meaningful choice
4. **Full element coverage** — Earth element completes Stage 3 weakness targeting
5. **Auto-battle integration** — AI uses Earth Wall defensively when HP is low
