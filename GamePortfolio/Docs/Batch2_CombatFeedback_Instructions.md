# Batch 2 Instruction — Combat Feedback Enhancement

## Goal

Make the battle scene feel responsive. Attacks, heals, enemy intents, and turn outcomes should be visible and understandable at a glance, without needing animation systems or external assets.

## Principle

Small, safe, text-based feedback. No imported assets. No animation systems. No complex state machines.

---

## Task A — Floating Damage/Heal Indicator

**What to implement:**

Add a lightweight `DamagePopup` system that spawns temporary floating text when damage or healing occurs.

**Where:**

- New file: `Assets/Scripts/Battle/DamagePopup.cs` (MonoBehaviour)
- Hook into existing `BattleManager.cs` where `OnPlayerAttack()` / `OnEnemyAttack()` / heal logic resolves

**Implementation details:**

1. `DamagePopup` prefab (generated via `BattleSceneAutoBuilder` or just created as a simple TMP Text in code):
   - Use a TMP_Text with a small font size (~24)
   - Orange/red for damage, green for healing
   - Spawn at the target's position (player or enemy card area)
   - Animate upward ~50px over 1.5 seconds, then fade and destroy
   - Use `transform.Translate(Vector3.up * speed * Time.deltaTime)` in Update() — no tween library needed
   - Destroy the GameObject after 1.5 seconds

2. Add a public method to `BattleUI.cs`:
   ```csharp
   public void ShowDamagePopup(int amount, Vector3 worldPosition, bool isHeal = false)
   ```
   This spawns the DamagePopup at the given position.

3. In `BattleManager.cs`, call it after each damage/heal resolves:
   - After `CalculateDamage()` in `OnPlayerAttack()` → call `battleUI.ShowDamagePopup(damage, enemyPosition)`
   - After heal in any method → call `battleUI.ShowDamagePopup(healAmount, playerPosition, true)`

**Do NOT:**
- Use any tweening/DoTween/LeanTween libraries
- Add collision or physics
- Create complex pooling systems yet — simple Instantiate/Destroy is fine for now

---

## Task B — Stronger Enemy Intent Display

**What to implement:**

Make the enemy's next action obvious and readable with stronger visual wording.

**Where:**

Modify `BattleUI.cs` — the existing enemy intent text area.

**Implementation details:**

1. Replace or enhance the plain enemy intent text with a colored, styled display:
   - **Normal Attack** → white text
   - **Strong Attack** → red/orange text with "!!" prefix
   - **Burning / Status** → orange text with "🔥" prefix (or just text like [BURN])
   - **Guard interaction** → blue text showing "Guard ineffective" vs "Guard reduced damage to X"

2. Add a small colored background panel behind the intent text (semi-transparent dark panel).

3. Update `BattleManager` where `enemyIntent` is set to include these styled strings.

**Minimum text examples:**

| Enemy action | Display text |
|---|---|
| NormalAttack | `Normal Attack` |
| StrongAttack | `!! Strong Attack` |
| Burning | `🔥 Burning` |
| Player Guard active | `Guarding — damage reduced` |
| Guard vs StrongAttack | `Guard partially ineffective` |

---

## Task C — Better Turn Result Summary

**What to implement:**

Replace or enhance the current result text with a cleaner multi-line summary showing what just happened.

**Where:**

Modify the result display in `BattleUI.cs`.

**Implementation details:**

The result summary after each turn should show, in order:
1. What the player did (Attack / Guard / Fire / Heal)
2. Damage dealt to enemy (or "Missed!")
3. What the enemy did
4. Damage dealt to player (or "Missed!")
5. Any status events (Burning dealt X damage)

Format example:
```
▸ Player Attacks! Dealt 24 damage.
▸ Enemy Strong Attacks!! Dealt 18 damage.
```

Use the existing `RoundResult` data from `BattleManager` — it should already have `playerAction`, `enemyAction`, `playerDamageDealt`, `enemyDamageDealt`, etc.

**Edge cases to handle:**
- `playerDamageDealt == 0 && playerAction == "Guard"` → show "Player Guards. Damage reduced."
- `playerDamageDealt == 0 && playerAction != "Guard"` → show "Player attacks! Missed!"
- Burning DoT tick → show as an extra line: "Burning deals 8 damage to enemy."

---

## Danger Zones / Pitfalls

- DO NOT edit `.unity` scene files by hand. Use `BattleSceneAutoBuilder` to auto-generate the scene.
- DO NOT delete or rename any existing fields in `BattleUI.cs` — only add new methods.
- DO NOT touch `ProjectSettings` or `UniversalRP.asset`.
- Check `git status` and `git diff` before any edits.
- After changes, run:
  - `Tools > Codex Tactics > Validate Battle Test Scene`
  - `Tools > Codex Tactics > Run Battle Logic Auto Test`
  - Check `git diff --check` for whitespace issues

## Verification Criteria

1. When player attacks, a floating damage number appears near enemy card ✅
2. When player heals, a green floating number appears near player card ✅
3. Enemy intent shows colored, styled text with appropriate prefix ✅
4. Turn result summary shows both player and enemy actions clearly ✅
5. All existing validation tests still PASS ✅
6. No new Unity settings or scene YAML files modified ✅
