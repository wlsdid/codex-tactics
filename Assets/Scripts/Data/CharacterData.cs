using UnityEngine;

[System.Serializable]
public class CharacterData
{
    public string characterName;
    public int maxHp;
    public int currentHp;
    public int attackPower;
    public ElementType weaknessElement;

    [Header("AP")]
    public int maxAp;
    public int currentAp;

    [Header("Status Effect")]
    public StatusEffectType currentStatusEffect;
    public int statusTurnsRemaining;

    public CharacterData(string name, int hp, int attack)
    {
        characterName = name;
        maxHp = hp;
        currentHp = hp;
        attackPower = attack;
        weaknessElement = ElementType.None;
        maxAp = 0;
        currentAp = 0;
        currentStatusEffect = StatusEffectType.None;
        statusTurnsRemaining = 0;
    }

    public CharacterData(string name, int hp, int attack, ElementType weakness)
    {
        characterName = name;
        maxHp = hp;
        currentHp = hp;
        attackPower = attack;
        weaknessElement = weakness;
        maxAp = 0;
        currentAp = 0;
        currentStatusEffect = StatusEffectType.None;
        statusTurnsRemaining = 0;
    }

    public CharacterData(string name, int hp, int attack, ElementType weakness, int ap)
    {
        characterName = name;
        maxHp = hp;
        currentHp = hp;
        attackPower = attack;
        weaknessElement = weakness;
        maxAp = ap;
        currentAp = ap;
        currentStatusEffect = StatusEffectType.None;
        statusTurnsRemaining = 0;
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

    public bool HasEnoughAp(int apCost)
    {
        return currentAp >= apCost;
    }

    public bool SpendAp(int apCost)
    {
        if (!HasEnoughAp(apCost))
        {
            return false;
        }

        currentAp -= apCost;
        return true;
    }

    public void RecoverAp(int amount)
    {
        currentAp += amount;

        if (currentAp > maxAp)
        {
            currentAp = maxAp;
        }
    }

    public bool HasStatusEffect(StatusEffectType statusEffect)
    {
        return currentStatusEffect == statusEffect && statusTurnsRemaining > 0;
    }

    public void ApplyStatusEffect(StatusEffectType statusEffect, int turns)
    {
        currentStatusEffect = statusEffect;
        statusTurnsRemaining = turns;
    }

    public void ReduceStatusTurn()
    {
        if (statusTurnsRemaining <= 0)
        {
            currentStatusEffect = StatusEffectType.None;
            statusTurnsRemaining = 0;
            return;
        }

        statusTurnsRemaining -= 1;

        if (statusTurnsRemaining <= 0)
        {
            currentStatusEffect = StatusEffectType.None;
            statusTurnsRemaining = 0;
        }
    }
}
