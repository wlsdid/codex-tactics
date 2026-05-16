# Auto-Battle AI: Decision Tree Architecture (자동 전투 AI: 의사결정 트리 구조)

## Overview

The auto-battle system implements a priority-based decision tree that lets the AI play the game autonomously. It's designed for demo mode, balance testing, and accessibility — not as a replacement for player skill, but as a QA and showcase tool.

## Decision tree design

The AI evaluates actions in strict priority order, returning immediately when a valid action is found:

```
1. Guard if strong attack incoming (survival)
2. Use weakness skill if Break gauge non-empty (tactical)
3. Lightning Strike — highest raw damage (burst)
4. Ice Lance — cheap, stun (control)
5. Fire Bolt — moderate cost, burn (sustain)
6. Basic attack — free, reliable (fallback)
```

### Priority rationale

**1. Guard > all** — Auto-battle exists for demo/QA purposes. A dead player ruins the demo. Guarding against strong attacks (every 3rd turn) ensures the AI survives long enough to demonstrate the full skill set. This mirrors how a human player would play cautiously in a first playthrough.

**2. Weakness > raw damage** — The Break system rewards hitting weaknesses: depleting the Break gauge triggers a Broken state that makes the next hit deal 1.5x more damage. Prioritizing weakness skills maximizes long-term DPS even when raw damage is lower.

**3. Lightning Strike > other skills** — Among skills that don't match the enemy's weakness, Lightning Strike deals the most damage (40 power). It's expensive (3 AP) but a good DPAP (damage per action point) ratio.

**4. Ice Lance > Fire Bolt** — Ice Lance costs only 1 AP and applies Stun, which skips an enemy turn. Stun effectively prevents the enemy from attacking, saving the player more HP than the 5 damage difference between Ice Lance (25) and Fire Bolt (30).

**5. Fire Bolt > basic** — Fire Bolt costs 2 AP and applies Burn (3 damage/turn for 2 turns, total +6), making it strictly better than basic attack when AP permits.

**6. Basic attack fallback** — Always available, costs 0 AP. The AI never has to end its turn without acting.

## Code structure

```csharp
private void ExecuteAutoAction()
{
    if (currentState != BattleState.PlayerTurn || player == null || enemy == null) return;
    if (enemy.IsDead()) { EndBattle(BattleState.Victory); return; }

    // Priority 1: Guard against incoming strong attack
    if (enemyPattern.IsStrongAttackTurn(enemyTurnCount + 1) && !playerIsGuarding)
    { GuardAndEndPlayerTurn(); return; }

    // Priority 2: Weakness skill if break gauge remains
    SkillData weaknessSkill = GetWeaknessSkill();
    if (weaknessSkill != null && !enemy.isBroken && player.HasEnoughAp(weaknessSkill.apCost))
    { UsePlayerSkill(weaknessSkill); return; }

    // Priority 3-6: skill cascade
    if (player.HasEnoughAp(lightningSkill.apCost)) { UsePlayerSkill(lightningSkill); return; }
    if (player.HasEnoughAp(iceSkill.apCost)) { UsePlayerSkill(iceSkill); return; }
    if (player.HasEnoughAp(fireSkill.apCost)) { UsePlayerSkill(fireSkill); return; }
    OnClickAttackButton(); // fallback
}
```

## Key design decisions

### Why not ML/branching AI?
The decision tree is deterministic and predictable. For a portfolio demo, this is a feature: reviewers can see the AI make sensible choices and understand *why*. A neural network approach would be overengineered and harder to debug.

### Why guard on strong attack prediction?
The system reads `enemyTurnCount + 1` to predict the enemy's next action. Since strong attacks happen on a fixed interval (every 3 turns), the AI can guard preemptively. This is identical to how the human player reads the enemy intent display.

### Why no target selection?
All encounters are 1v1 — there's only one enemy. Target selection would be needed for multi-enemy battles (future work).

## Extensibility

Adding a new skill to the AI requires only adding one line to `ExecuteAutoAction()` in the correct priority position. The `GetWeaknessSkill()` helper automatically checks all skills for element matching.

## Portfolio relevance

This system demonstrates:
1. **Priority-based AI** — not random, not ML, just clean conditional logic
2. **Game-aware decisions** — reads enemy intent, Break status, AP levels
3. **Clean integration** — auto-battle uses existing `UsePlayerSkill()`, no separate combat pipeline
4. **Toggle-able** — player can switch between manual and auto at any time
5. **Testable** — the decision tree is fully deterministic, making it easy to write regression tests
