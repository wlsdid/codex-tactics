# Reviewer Quick Check

Use this checklist when a reviewer has less than 60 seconds to judge the current demo.

## Fast review path

1. Open the project in Unity: `C:\Users\jywls\Desktop\game_portfolio\GamePortfolio`.
2. Start from `TitleScene` or inspect the README GIF first.
3. In battle, follow the visible in-game hint and the `Capture Rehearsal 1/5 -> 5/5` prompt:
   `Click Hero -> Fire -> Guard -> Result -> Retry`.
4. Confirm the rehearsal text advances after Hero select, Fire, Guard, Result, and Retry reset.
5. Confirm the demo shows a complete vertical slice: Title, Stage Select, Battle, Result, Retry/Continue.

## What to look for

- Contextual tactical command UI: commands appear after selecting the ally, not as always-on global buttons.
- Capture rehearsal readability: the top-center prompt tells the reviewer exactly which short-recording step is next.
- AP/HP/status readability: Fire consumes AP, Guard changes the defensive state, result metrics summarize choices.
- Feedback quality: projectile, hit spark, impact ring, damage popup, screen shake, idle bob/hit reaction.
- Data-driven structure: stages, enemies, skills, balance, and result evaluation are split into readable C# files.
- Verification evidence: Unity `Tools > Codex Tactics` validators and `Docs/Captures/` generated GIF/screenshots.

## Under-60-second acceptance

The current portfolio demo is successful if a reviewer can understand these three points without reading code:

1. The game has a playable RPG loop, not just isolated UI screens.
2. The player has meaningful choices: skill, guard, retry/continue.
3. The project is documented and automatically verified enough to be maintained by a beginner developer.
