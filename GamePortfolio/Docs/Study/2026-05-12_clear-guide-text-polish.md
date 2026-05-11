# Study Note - 2026-05-12 Clear Guide Text Polish

## Topic

Small UI text changes can make a stage-based battle prototype easier to understand than adding another mechanic too early.

## What changed

The first clear state now says which encounter was cleared and where Continue will go next. The final clear state now reminds the player to review Total Gold and optionally retry the boss for practice.

## Lesson learned

A vertical slice needs clear state communication:

1. What did I just finish?
2. What button should I press next?
3. What will happen after I press it?
4. When the stage is fully complete, what should I look at?

The old text answered some of these questions, but used generic wording like `next encounter`. Naming `Stage 1-2: Slime King` makes the flow more concrete and easier to capture in screenshots.

## Beginner-readable implementation notes

- Keep guide text in helper methods such as `BuildVictoryGuideMessage()` instead of spreading string logic through `EndBattle()`.
- Reuse existing `StageData.BuildDisplayName()` so UI text stays consistent with the stage label.
- Update editor auto-tests when visible text changes, because portfolio-facing UI text is part of the expected behavior.

## Next learning target

After screenshots are captured, compare whether the current top UI is too text-heavy. If it is readable, the next small code feature could be one additional beginner-readable player skill.
