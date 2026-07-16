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
    public string enemyName = "Goblin";
    public int maxHp = 80;
    public ElementType weakness = ElementType.Fire;
    public EnemyPatternData pattern = new EnemyPatternData();
    public EnemyVisualVariant visualVariant = EnemyVisualVariant.Goblin;
    // BattleVisualId is the exact extracted-sprite identity used by the 3v3 battlefield.
    public BattleVisualId visualId = BattleVisualId.Goblin;

    public EnemyData()
    {
    }

    public EnemyData(string name, int hp, ElementType weaknessElement, EnemyPatternData enemyPattern, EnemyVisualVariant visual = EnemyVisualVariant.Goblin, BattleVisualId battleVisualId = BattleVisualId.Goblin)
    {
        enemyName = name;
        maxHp = hp;
        weakness = weaknessElement;
        pattern = enemyPattern;
        visualVariant = visual;
        visualId = battleVisualId;
    }
}
