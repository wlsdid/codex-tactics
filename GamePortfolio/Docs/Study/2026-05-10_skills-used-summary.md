# Study Note - Skills Used Summary

A result screen is easier to explain when it separates **outcome metrics** from **choice metrics**.

## Outcome metrics

These describe what happened to HP:

- `Damage dealt`
- `Damage taken`
- remaining HP/AP
- Victory/Defeat

## Choice metrics

These describe what the player chose:

- `Guard uses`
- `Skills used`

`Skills used` is not the same as damage dealt. A player could use a low-damage skill many times, or win with fewer stronger actions. Tracking the count gives a small clue about play style.

## Implementation idea

The counter should increase only after a skill action is accepted:

```csharp
if (!player.SpendAp(skill.apCost))
{
    return;
}

skillsUsedCount++;
```

That keeps failed inputs, such as not having enough AP, from polluting the combat report.

## Portfolio explanation

This is a simple example of telemetry-style combat reporting:

> The result summary records not only victory/defeat, but also how the player reached that result: total damage, defensive Guard choices, and successful offensive skill actions.

Later, this could support tuning questions such as:

- Are players using Fire Skill too often?
- Does Guard reduce damage enough to feel useful?
- Should rank rules consider skill count or remaining AP?
