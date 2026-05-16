# Element Labels in Enemy Intent: Visual Communication (적 인텐트 속성 표시: 시각적 커뮤니케이션)

## Overview

Element labels in the enemy intent display tell the player what element the enemy's attack belongs to. This small UI feature bridges the gap between game mechanics and player understanding — without it, the player has to remember each enemy's element from the stage select screen.

## How it looks

```
Without label:  Next Enemy: Normal Attack (15)
With label:     Next Enemy: [Fire] Normal Attack (15)
                Next Enemy: [Lightning] Thunder Dive (40)
```

## Implementation

```csharp
private string enemyElementLabel = "";
public void SetEnemyElementLabel(ElementType element)
{
    if (element == ElementType.None || element == ElementType.Physical)
        enemyElementLabel = "";
    else
        enemyElementLabel = $"[{element}] ";
}

private void SetEnemyIntentText(...)
{
    enemyIntentText.text = pattern.IsStrongAttackTurn(nextTurn)
        ? $"Next Enemy: {enemyElementLabel}{pattern.strongAttackName} ({pattern.strongAttackDamage})"
        : $"Next Enemy: {enemyElementLabel}Normal Attack ({pattern.normalAttackDamage})";
}
```

## Design rationale

### Why brackets [Fire]?
Square brackets create a visual anchor that separates the element label from the attack name. The player can scan for the bracket to quickly identify the element without reading the full text. This is a common UI convention (similar to equipment slots in RPGs).

### Why prefix the element (not color-coding)?
Color-coding element names (e.g., red text for Fire) would require extra UI texture work and might conflict with the HP bar color system. Text labels are zero-config, zero-texture, and work regardless of the player's color perception.

### Why show element on every attack (not just weaknesses)?
Knowing the enemy's element helps the player plan their next action. If a Fire Slime attacks with [Fire], the player knows hitting it with Ice Lance (Ice element) will deal extra damage. The element label is a constant reminder of the elemental cycle.

### What about Physical/None enemies?
Physical attacks (Slash) and None-element enemies show no prefix. This is intentional: Physical is the absence of element, and showing [Physical] would be confusing (what does a Physical weakness mean?). The empty prefix naturally separates elemental enemies from non-elemental ones.

## How players learn

1. Stage select shows element icons/descriptions
2. First battle: player sees `[Fire]` on enemy intent
3. Player tries Ice Lance → sees `Weakness x1.5` impact text
4. Player learns: `[Fire]` element = hit with Ice = extra damage

This three-step teach-and-reinforce loop makes the element system discoverable without a tutorial screen.

## Portfolio relevance

1. **Zero-config UI** — Text-based, no textures or color management
2. **Immediate recognition** — Bracketed prefix is scannable
3. **Teachable moment** — Pairing with Weakness impact text teaches the system
4. **Low code impact** — Single field, two methods, no new components
