using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum StageModifierType
{
    None = 0,
    TutorialField,
    PackPressure,
    Stoneguard,
    StormSurge,
    VoidDrain,
    RadiantTrial
}

[System.Serializable]
public class StageData
{
    public string stageName = "Stage 1-1";
    public string encounterName = "Slime Scout";
    [TextArea(2, 4)]
    public string encounterDescription = "";
    public EnemyData enemy = new EnemyData();
    public StageModifierType stageModifier = StageModifierType.None;
    [TextArea(2, 4)]
    public string stageModifierDescription = "";

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
            stageModifier = StageModifierType.TutorialField,
            stageModifierDescription = "A safe training ground. No special hazards.",
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
            stageModifier = StageModifierType.TutorialField,
            stageModifierDescription = "A safe training ground. No special hazards.",
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
            stageModifier = StageModifierType.PackPressure,
            stageModifierDescription = "Enemy strong attacks come more frequently!",
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
            stageModifier = StageModifierType.PackPressure,
            stageModifierDescription = "Enemy strong attacks come more frequently!",
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
            stageModifier = StageModifierType.Stoneguard,
            stageModifierDescription = "Enemy starts with reinforced break defense.",
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
            stageModifier = StageModifierType.Stoneguard,
            stageModifierDescription = "Enemy starts with reinforced break defense.",
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
            stageModifier = StageModifierType.StormSurge,
            stageModifierDescription = "Every 3 turns, residual lightning strikes.",
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
            stageModifier = StageModifierType.StormSurge,
            stageModifierDescription = "Every 3 turns, residual lightning strikes.",
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

    public static StageData CreateStage5Normal()
    {
        return new StageData
        {
            stageName = "Stage 5-1",
            encounterName = "Shadow Wraith",
            encounterDescription = "A shadowy wraith drifts through the darkness.\nThe chill of void emanates from its form.",
            stageModifier = StageModifierType.VoidDrain,
            stageModifierDescription = "Shadow energy drains AP over time.",
            enemy = new EnemyData(
                "Shadow Wraith",
                160,
                ElementType.Dark,
                new EnemyPatternData
                {
                    normalAttackMessageVerb = "lashes out with shadow",
                    normalAttackDamage = 25,
                    strongAttackName = "Void Grasp",
                    strongAttackDamage = 45,
                    strongAttackEveryTurns = 3
                }
            )
        };
    }

    public static StageData CreateStage5Boss()
    {
        return new StageData
        {
            stageName = "Stage 5-2",
            encounterName = "Shadow Lord",
            encounterDescription = "The Shadow Lord descends from the void!\nDarkness pulses with malevolent intent.",
            stageModifier = StageModifierType.VoidDrain,
            stageModifierDescription = "Shadow energy drains AP over time.",
            enemy = new EnemyData(
                "Shadow Lord",
                280,
                ElementType.Dark,
                new EnemyPatternData
                {
                    normalAttackMessageVerb = "commands shadow tendrils",
                    normalAttackDamage = 30,
                    strongAttackName = "Oblivion Strike",
                    strongAttackDamage = 55,
                    strongAttackEveryTurns = 3
                }
            )
        };
    }

    public static StageData CreateStage6Normal()
    {
        return new StageData
        {
            stageName = "Stage 6-1",
            encounterName = "Light Warden",
            encounterDescription = "A radiant warden stands guard.\nLight pulses with protective energy.",
            stageModifier = StageModifierType.RadiantTrial,
            stageModifierDescription = "The ultimate trial. Enemies are relentless.",
            enemy = new EnemyData(
                "Light Warden",
                180,
                ElementType.Light,
                new EnemyPatternData
                {
                    normalAttackMessageVerb = "strikes with holy light",
                    normalAttackDamage = 28,
                    strongAttackName = "Radiance Blast",
                    strongAttackDamage = 50,
                    strongAttackEveryTurns = 3
                }
            )
        };
    }

    public static StageData CreateStage6Boss()
    {
        return new StageData
        {
            stageName = "Stage 6-2",
            encounterName = "Holy Sentinel",
            encounterDescription = "The Holy Sentinel descends in a pillar of light!\nIts divine power is unmatched.",
            stageModifier = StageModifierType.RadiantTrial,
            stageModifierDescription = "The ultimate trial. Enemies are relentless.",
            enemy = new EnemyData(
                "Holy Sentinel",
                320,
                ElementType.Light,
                new EnemyPatternData
                {
                    normalAttackMessageVerb = "commands divine judgment",
                    normalAttackDamage = 32,
                    strongAttackName = "Heavenly Wrath",
                    strongAttackDamage = 60,
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
            case 4:
                list.Add(CreateStage5Normal());
                list.Add(CreateStage5Boss());
                break;
            case 5:
                list.Add(CreateStage6Normal());
                list.Add(CreateStage6Boss());
                break;
            default:
                list.Add(CreateStage1Normal());
                list.Add(CreateStage1Boss());
                break;
        }
        return list;
    }
}
