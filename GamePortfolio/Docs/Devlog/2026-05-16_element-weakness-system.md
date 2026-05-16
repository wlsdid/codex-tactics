# Devlog — 2026-05-16: Element Weakness Multiplier System

## What I did

Replaced the flat +10 weakness damage bonus with a configurable multiplier system.

### Before
```csharp
int dmg = skill.power;
if (weakness) dmg += 10;
```

### After
```csharp
float multiplier = weakness ? weaknessDamageMultiplier : neutralDamageMultiplier;
return Mathf.RoundToInt(skill.power * multiplier);
```

The multiplier is configured in `BattleBalanceConfig` (ScriptableObject):
- **weaknessDamageMultiplier** = 1.5x (default)
- **neutralDamageMultiplier** = 1.0x (default)

### Impact text improved
- **Weakness x1.5** — clearly shows when a skill hits the enemy's weakness
- **Neutral x1.0** — shown for non-matching elemental attacks
- **Physical** — shown for non-elemental attacks (Slash)

### Test expectations updated
- Fire Bolt vs Fire Slime (weakness): 30 power × 1.5 = **45 damage** (was 40)
- Impact text, HP display, and damage counter tests updated
- Result: **221 test checks, all PASS**

## Why this matters for portfolio
The old flat +10 bonus worked mechanically but didn't communicate *why* the damage was higher. The multiplier system is:
- **Config-driven** — designers can tune via ScriptableObject Inspector
- **Visible** — impact text shows the exact multiplier
- **Scalable** — easy to add resistance (0.5x), immunity (0x), or absorption (negative)
- **Readable** — `Weakness x1.5` is immediately understood

## Files changed
- `Assets/Scripts/Data/BattleBalanceConfig.cs` — new fields
- `Assets/Scripts/Battle/BattleManager.cs` — CalculateSkillDamage, BuildImpactText, new helper
- `Assets/Editor/BattleAutoTestRunner.cs` — updated expectations
