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
                    strongAttackEveryTurns = 1
                }
            )
        };
    }
}
