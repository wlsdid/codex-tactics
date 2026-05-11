using UnityEngine;

[System.Serializable]
public class SkillData
{
    [Header("Basic Info")]
    public string skillName;
    public string description;

    [Header("Battle Values")]
    public int power;
    public int apCost;
    public ElementType elementType;
    public StatusEffectType statusEffectType;

    public SkillData(string name, int skillPower, int cost, ElementType element, StatusEffectType statusEffect)
    {
        skillName = name;
        description = "";
        power = skillPower;
        apCost = cost;
        elementType = element;
        statusEffectType = statusEffect;
    }

    public bool HasStatusEffect()
    {
        return statusEffectType != StatusEffectType.None;
    }
}
