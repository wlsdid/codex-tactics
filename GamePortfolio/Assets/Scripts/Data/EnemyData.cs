using UnityEngine;

[System.Serializable]
public enum EnemyVisualVariant
{
    Goblin = 0,
    Skeleton,
    Orc,
    Lich,
    Golem,
    DarkKnight
}

[System.Serializable]
public class EnemyData
{
    public string enemyName = "Slime";
    public int maxHp = 80;
    public ElementType weakness = ElementType.Fire;
    public EnemyPatternData pattern = new EnemyPatternData();
    public EnemyVisualVariant visualVariant = EnemyVisualVariant.Goblin;

    public EnemyData()
    {
    }

    public EnemyData(string name, int hp, ElementType weaknessElement, EnemyPatternData enemyPattern, EnemyVisualVariant visual = EnemyVisualVariant.Goblin)
    {
        enemyName = name;
        maxHp = hp;
        weakness = weaknessElement;
        pattern = enemyPattern;
        visualVariant = visual;
    }
}
