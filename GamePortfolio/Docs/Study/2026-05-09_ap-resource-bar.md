# Study Note — AP Resource Bar UI

## Concept

A resource bar is a UI slider that maps a gameplay value to a visual fill amount.

For AP:

- `minValue` is `0`.
- `maxValue` is the character's `maxAp`.
- `value` is the character's `currentAp`.

## Applied in this project

`BattleManager.UpdateResourceSlider` now updates both HP sliders and the Player AP slider. This keeps the UI update rule simple:

```csharp
slider.minValue = 0f;
slider.maxValue = maxValue;
slider.value = Mathf.Clamp(currentValue, 0, maxValue);
```

The generated scene creates `Player AP Slider` below `Player AP Text`, then links it to `BattleManager.playerApSlider` with a serialized reference.

## Why it matters

Text is precise, but bars are faster to read during combat. Showing AP as both text and a bar makes the `Fire Skill` cost visible and helps players understand when they can or cannot use the skill.

## Beginner takeaway

When a gameplay value changes, update every UI element that represents that value in one shared refresh method. This prevents text and bars from drifting out of sync.
