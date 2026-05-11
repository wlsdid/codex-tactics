# Study Note - Manual Unity Validation Checklist

Automated checks and manual checks answer different questions.

## Automated checks

The editor menu test is good for repeatable logic checks:

- expected HP/AP values
- result summary text
- counter resets
- button visibility
- internal debug values

It is fast and consistent, but it does not prove the full Editor experience looks good to a player.

## Manual checklist

A manual checklist is useful for visual and interaction checks:

- UI is readable on screen
- buttons can be clicked in the intended order
- status text is understandable
- result summary panel is visually separated from the battle log
- screenshots/GIFs can be captured for README

## Portfolio lesson

For a game portfolio, validation evidence should be understandable to non-programmers too. A checkbox record can show:

> I did not only write code. I tested the scene as a playable loop and recorded what passed.

This supports a better development-process story than simply saying "it works".
