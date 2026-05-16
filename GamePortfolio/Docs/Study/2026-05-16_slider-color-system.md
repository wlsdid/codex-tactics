# Slider Color System: Dynamic Resource Visualization (슬라이더 색상 시스템: 동적 자원 시각화)

## Overview

The slider color system maps resource ratios to distinct colors, giving players at-a-glance awareness of their HP and AP status without reading numbers. It transforms a generic grey bar into an intuitive heatmap of danger and opportunity.

## Color thresholds

### HP bars (player + enemy)

| Ratio | Color | Hex | Meaning |
|-------|-------|-----|---------|
| >60% | Green | #37B861 | Safe |
| 30-60% | Yellow | #D9B82E | Warning |
| <30% | Red | #D1383D | Critical |

### AP bars

| Ratio | Color | Hex | Meaning |
|-------|-------|-----|---------|
| >66% | Blue | #428FFF | Enough for any skill |
| 33-66% | Cyan | #42DCBF | Moderate |
| <33% | Orange | #EB8F2E | Low — basic attack only |

## Implementation

```csharp
private void SetSliderColorByRatio(Slider slider, int current, int max,
    Color highColor, Color midColor, Color lowColor)
{
    float ratio = max > 0 ? (float)current / max : 0f;
    Color targetColor;
    if (ratio > 0.6f) targetColor = highColor;
    else if (ratio > 0.3f) targetColor = midColor;
    else targetColor = lowColor;
    
    var fillRect = slider.fillRect;
    if (fillRect != null)
    {
        var img = fillRect.GetComponent<Image>();
        if (img != null) img.color = targetColor;
    }
}
```

## Design rationale

### Green → Yellow → Red (HP)
This is the universal health color language. Green means "keep doing what you're doing." Yellow means "consider guarding or using a tactical skill." Red means "one more hit and you're done." Every player understands this instinctively.

### Blue → Cyan → Orange (AP)
AP isn't life-threatening, so we use a different palette. Blue signals "you can use any skill." Cyan means "pick carefully." Orange means "you can only basic attack." The cooler palette contrasts with HP's warm alarm colors, preventing visual confusion.

### Threshold choices

- **60% HP** — Most enemies' strong attacks deal 30-48 damage. At 60% HP (60/100), a single strong attack won't kill but will push into yellow. This gives the player one turn of warning.
- **30% HP** — Below 30 HP, even a normal attack (15-28 damage) could be lethal. Red triggers urgent decision-making.
- **66% AP** — Lightning Strike costs 3 AP out of 3 max. 66% = 2 AP, enough for Fire Bolt or Ice Lance. Below 66% (1 AP), only Ice Lance or basic attack is possible.
- **33% AP** — Below 33% (0 AP, since AP values are integers). Only basic attack.

## Why this matters for portfolio

1. **Communicates without reading** — Players see the color before the number
2. **Universal language** — Red = danger, green = safe, no localisation needed
3. **Separate palettes** — Warm (HP) vs cool (AP) prevents confusion
4. **Configurable** — Colors and thresholds can be tuned per game
5. **Reusable** — `SetSliderColorByRatio()` works for any resource slider
