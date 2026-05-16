# Element Weakness Multiplier System (속성 약점 배율 시스템)

## Overview

The element weakness system transforms damage calculation from a flat bonus (+10) into a configurable multiplier. This makes elemental matchups meaningful, visible, and designer-friendly.

## Core mechanic

Each skill has an `ElementType` (Fire, Ice, Lightning, Nature, Dark, Light, Earth, Physical).
Each enemy has a `weaknessElement`.

When `skill.elementType == enemy.weaknessElement`:
- Damage = skill.power × **weaknessDamageMultiplier** (default 1.5x)
- Break gauge is reduced by 1
- Impact text shows `Weakness x1.5`

When elements don't match:
- Damage = skill.power × **neutralDamageMultiplier** (default 1.0x)

Physical skills (Slash) bypass the system entirely — always 1× with no elemental label.

## Config-driven design (BattleBalanceConfig)

All magic numbers live in a ScriptableObject that designers can tune in the Unity Inspector:

| Field | Default | Range | Purpose |
|-------|---------|-------|---------|
| `weaknessDamageMultiplier` | 1.5 | 1.0–3.0 | How much extra damage on weakness hits |
| `neutralDamageMultiplier` | 1.0 | 0.1–1.0 | Baseline for non-matching elements (can be lowered for "resisted" hits) |

Setting `neutralDamageMultiplier = 0.75` would make off-element hits deal 75% damage, adding resistance as a second dimension.

## UI visibility

The impact text communicates element effectiveness clearly:

```
Impact: Fire Bolt dealt 45 damage | Weakness x1.5 | Break 1/2 | Burn applied
Impact: Slash dealt 20 damage | Physical
```

The battle log message also shows the effectiveness label:
```
Hero uses Fire Bolt! Slime takes 45 damage. (Fire | Weakness)
```

## Portfolio relevance

This system demonstrates:

1. **Designer-friendly tuning** — ScriptableObject config instead of hardcoded values
2. **Clean separation** — damage calculation is isolated in `CalculateSkillDamage()`, element lookup in `GetElementEffectiveness()`
3. **Scalable architecture** — easy to add resistance (0.5x), immunity (0x), absorption (heal from matching element), or complex elemental cycles (Fire > Nature > Earth > Fire)
4. **Player communication** — UI explicitly shows why damage is higher/lower, teaching the player the element system organically
5. **Tested** — all damage expectations verified through automated regression tests (221 checks)

## Future potential

- **Elemental cycle** — define full rock-paper-scissors relationships (Fire > Nature > Earth > Fire)
- **Resistance/immunity** — some enemies resist certain elements
- **Dual elements** — skills or enemies with two elements
- **Elemental synergy** — chaining same-element attacks for bonus effects
