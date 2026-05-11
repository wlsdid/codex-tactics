# Study Note - Result Summary Panel

## Concept

Important UI information should have enough contrast against the background. Text placed directly over another text-heavy area can be difficult to read.

A simple panel behind the result summary creates visual grouping:

- result text belongs together,
- battle log stays separate,
- Victory/Defeat state feels clearer.

## Applied in this project

The scene builder creates a semi-transparent `Image` before creating `Result Summary Text`, so the panel is behind the text in the Canvas hierarchy.

```csharp
Image resultSummaryPanel = CreatePanel(
    canvas.transform,
    "Result Summary Panel",
    new Vector2(0, -145),
    new Vector2(940, 130),
    new Color(0.06f, 0.07f, 0.10f, 0.86f)
);
```

`BattleManager` toggles the panel with the summary text:

```csharp
resultSummaryPanel.SetActive(isVisible);
```

## Why this is beginner-readable

This is not a new gameplay system. It is a small UI composition improvement:

1. create a panel object,
2. give it a size and color,
3. place it behind text,
4. show/hide it with the result summary.

## Portfolio explanation

This shows UI polish and readability thinking. Even simple prototypes benefit from clear information hierarchy, especially when the screen has HP bars, action buttons, logs, and result text at the same time.
