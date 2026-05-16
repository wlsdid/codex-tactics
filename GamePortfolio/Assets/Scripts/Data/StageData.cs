using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StageData
{
    public string stageName = "Stage 1-1";
    public string encounterName = "Slime Scout";
    public EnemyData enemy = new EnemyData();

    public string BuildDisplayName()
    {
        return $"{stageName}: {encounterName}";
    }

    public string BuildObjectiveText()
    {
        return $"Objective: Defeat {encounterName}";
    }

    public static StageData CreateStage1Normal()
    {
        return new StageData
        {
            stageName = "Stage 1-1",
            encounterName = "Slime Scout",
            enemy = new EnemyData(
                "Slime",
                80,
                ElementType.Fire,
                new EnemyPatternData
                {
                    normalAttackMessageVerb = "attacks",
                    normalAttackDamage = 15,
                    strongAttackName = "Heavy Slam",
                    strongAttackDamage = 30,
                    strongAttackEveryTurns = 3
                }
            )
        };
    }

    public static StageData CreateStage1Boss()
    {
        return new StageData
        {
            stageName = "Stage 1-2",
            encounterName = "Slime King",
            enemy = new EnemyData(
                "Slime King",
                140,
                ElementType.Fire,
                new EnemyPatternData
                {
                    normalAttackMessageVerb = "commands royal slime waves",
                    normalAttackDamage = 18,
                    strongAttackName = "Royal Slam",
                    strongAttackDamage = 36,
                    strongAttackEveryTurns = 3
                }
            )
        };
    }

    public static StageData CreateStage2Normal()
    {
        return new StageData
        {
            stageName = "Stage 2-1",
            encounterName = "Wolf Scout",
            enemy = new EnemyData(
                "Wolf Scout",
                100,
                ElementType.Nature,
                new EnemyPatternData
                {
                    normalAttackMessageVerb = "lunges",
                    normalAttackDamage = 18,
                    strongAttackName = "Pack Howl",
                    strongAttackDamage = 35,
                    strongAttackEveryTurns = 3
                }
            )
        };
    }

    public static StageData CreateStage2Boss()
    {
        return new StageData
        {
            stageName = "Stage 2-2",
            encounterName = "Alpha Wolf",
            enemy = new EnemyData(
                "Alpha Wolf",
                180,
                ElementType.Nature,
                new EnemyPatternData
                {
                    normalAttackMessageVerb = "leads the pack",
                    normalAttackDamage = 22,
                    strongAttackName = "Alpha Strike",
                    strongAttackDamage = 42,
                    strongAttackEveryTurns = 3
                }
            )
        };
    }

    public static List<StageData> GetEncountersForStage(int stageIndex)
    {
        var list = new List<StageData>();
        switch (stageIndex)
        {
            case 0:
                list.Add(CreateStage1Normal());
                list.Add(CreateStage1Boss());
                break;
            case 1:
                list.Add(CreateStage2Normal());
                list.Add(CreateStage2Boss());
                break;
            default:
                list.Add(CreateStage1Normal());
                list.Add(CreateStage1Boss());
                break;
        }
        return list;
    }
}
