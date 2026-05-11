using UnityEngine;

[System.Serializable]
public class EnemyData
{
    public string enemyName = "Slime";
    public int maxHp = 80;
    public ElementType weakness = ElementType.Fire;
    public EnemyPatternData pattern = new EnemyPatternData();

    public EnemyData()
    {
    }

    public EnemyData(string name, int hp, ElementType weaknessElement, EnemyPatternData enemyPattern)
    {
        enemyName = name;
        maxHp = hp;
        weakness = weaknessElement;
        pattern = enemyPattern;
    }
}
