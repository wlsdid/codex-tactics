# Study Note - 2026-05-11 StageData and EnemyData

## Concept learned

A battle can become easier to expand when enemy and stage values are stored in small data classes instead of being hardcoded directly in the battle flow.

## Where it was applied

Files:

- `Assets/Scripts/Data/EnemyData.cs`
- `Assets/Scripts/Data/StageData.cs`
- `Assets/Scripts/Battle/BattleManager.cs`

`EnemyData` stores one enemy preset:

```csharp
public string enemyName;
public int maxHp;
public ElementType weakness;
public EnemyPatternData pattern;
```

`StageData` stores one encounter:

```csharp
public string stageName;
public string encounterName;
public EnemyData enemy;
```

`BattleManager` keeps a list of `StageData` presets and loads the current one when a battle starts.

## Why this helps

Before this change, the prototype mostly behaved like one isolated battle. With `StageData`, the game can now move from a normal encounter to a boss encounter without duplicating battle logic.

This keeps the code beginner-readable because each preset is still just a plain C# class, not a large database system.

## Important behavior choice

`Continue` changes the current stage index and starts the next encounter.

`Retry` does not reset the whole stage list. It restarts the current encounter, which is better for a player who loses or wants to retry the boss.

## Next learning step

After screenshots/GIFs are captured, this structure can grow into:

1. More `StageData` presets.
2. A small stage-select or title scene.
3. Party/member data.
4. ScriptableObjects later, only if plain classes become hard to manage.
