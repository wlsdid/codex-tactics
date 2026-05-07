using UnityEngine;

[System.Serializable]
public class CharacterData
{
    public string characterName;
    public int maxHp;
    public int currentHp;
    public int attackPower;
    public ElementType weaknessElement;

    public CharacterData(string name, int hp, int attack)
    {
        characterName = name;
        maxHp = hp;
        currentHp = hp;
        attackPower = attack;
        weaknessElement = ElementType.None;
    }

    public CharacterData(string name, int hp, int attack, ElementType weakness)
    {
        characterName = name;
        maxHp = hp;
        currentHp = hp;
        attackPower = attack;
        weaknessElement = weakness;
    }

    public bool IsDead()
    {
        return currentHp <= 0;
    }

    public void TakeDamage(int damage)
    {
        currentHp -= damage;

        if (currentHp < 0)
        {
            currentHp = 0;
        }
    }
}
