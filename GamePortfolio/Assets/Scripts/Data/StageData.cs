using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StageData
{
    public string stageName = "Stage 1-1";
    public string encounterName = "Slime Scout";
    [TextArea(2, 4)]
    public string encounterDescription = "";
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
            encounterDescription = "A small slime scout patrols the area.\nA good opportunity to test your skills.",
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
            encounterDescription = "The Slime King emerges!\nThis towering blob commands respect.",
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
            encounterDescription = "A wolf scout prowls the moonlit clearing.\nIts pack may be nearby...",
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
            encounterDescription = "The Alpha Wolf leads the charge!\nIts howl echoes through the night.",
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

    public static StageData CreateStage3Normal()
    {
        return new StageData
        {
            stageName = "Stage 3-1",
            encounterName = "Golem Sentry",
            encounterDescription = "A stone golem blocks the path ahead.\nIts rocky hide shrugs off weak attacks.",
            enemy = new EnemyData(
                "Golem Sentry",
                120,
                ElementType.Earth,
                new EnemyPatternData
                {
                    normalAttackMessageVerb = "pounds",
                    normalAttackDamage = 20,
                    strongAttackName = "Bedrock Slam",
                    strongAttackDamage = 38,
                    strongAttackEveryTurns = 3
                }
            )
        };
    }

    public static StageData CreateStage3Boss()
    {
        return new StageData
        {
            stageName = "Stage 3-2",
            encounterName = "Ancient Golem",
            encounterDescription = "The Ancient Golem awakens from its slumber!\nThe ground trembles with each step.",
            enemy = new EnemyData(
                "Ancient Golem",
                220,
                ElementType.Earth,
                new EnemyPatternData
                {
                    normalAttackMessageVerb = "crumbles earth",
                    normalAttackDamage = 25,
                    strongAttackName = "Cataclysm",
                    strongAttackDamage = 48,
                    strongAttackEveryTurns = 3
                }
            )
        };
    }

    public static StageData CreateStage4Normal()
    {
        return new StageData
        {
            stageName = "Stage 4-1",
            encounterName = "Storm Hawk",
            encounterDescription = "A Storm Hawk circles overhead.\nLightning crackles in its feathers.",
            enemy = new EnemyData(
                "Storm Hawk",
                140,
                ElementType.Lightning,
                new EnemyPatternData
                {
                    normalAttackMessageVerb = "swoops",
                    normalAttackDamage = 22,
                    strongAttackName = "Thunder Dive",
                    strongAttackDamage = 40,
                    strongAttackEveryTurns = 3
                }
            )
        };
    }

    public static StageData CreateStage4Boss()
    {
        return new StageData
        {
            stageName = "Stage 4-2",
            encounterName = "Thunder Phoenix",
            encounterDescription = "The legendary Thunder Phoenix rises!\nThe sky darkens as it spreads its wings.",
            enemy = new EnemyData(
                "Thunder Phoenix",
                250,
                ElementType.Lightning,
                new EnemyPatternData
                {
                    normalAttackMessageVerb = "calls lightning",
                    normalAttackDamage = 28,
                    strongAttackName = "Skyfall",
                    strongAttackDamage = 55,
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
            case 2:
                list.Add(CreateStage3Normal());
                list.Add(CreateStage3Boss());
                break;
            case 3:
                list.Add(CreateStage4Normal());
                list.Add(CreateStage4Boss());
                break;
            default:
                list.Add(CreateStage1Normal());
                list.Add(CreateStage1Boss());
                break;
        }
        return list;
    }
}
