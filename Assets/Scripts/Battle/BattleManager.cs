using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleManager : MonoBehaviour
{
    [Header("Battle State")]
    [SerializeField] private BattleState currentState;

    [Header("Characters")]
    [SerializeField] private string playerName = "Hero";
    [SerializeField] private int playerMaxHp = 100;
    [SerializeField] private int playerAttack = 20;
    [SerializeField] private int playerMaxAp = 3;
    [SerializeField] private int playerApRecoveryPerTurn = 1;

    [SerializeField] private string enemyName = "Slime";
    [SerializeField] private int enemyMaxHp = 80;
    [SerializeField] private int enemyAttack = 15;
    [SerializeField] private ElementType enemyWeakness = ElementType.Fire;

    [Header("Player Skill")]
    [SerializeField] private string basicSkillName = "Slash";
    [SerializeField] private int basicSkillPower = 20;
    [SerializeField] private int basicSkillApCost = 0;
    [SerializeField] private ElementType basicSkillElement = ElementType.Physical;

    [Header("UI")]
    [SerializeField] private TMP_Text playerHpText;
    [SerializeField] private TMP_Text playerApText;
    [SerializeField] private TMP_Text enemyHpText;
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private Button attackButton;

    private CharacterData player;
    private CharacterData enemy;
    private SkillData basicAttackSkill;

    private void Start()
    {
        StartBattle();
    }

    private void StartBattle()
    {
        currentState = BattleState.Start;

        player = new CharacterData(playerName, playerMaxHp, playerAttack, ElementType.None, playerMaxAp);
        enemy = new CharacterData(enemyName, enemyMaxHp, enemyAttack, enemyWeakness);
        basicAttackSkill = new SkillData(basicSkillName, basicSkillPower, basicSkillApCost, basicSkillElement, StatusEffectType.None);

        if (attackButton != null)
        {
            attackButton.onClick.AddListener(OnClickAttackButton);
        }

        UpdateUI("전투 시작!");
        StartPlayerTurn();
    }

    private void StartPlayerTurn()
    {
        currentState = BattleState.PlayerTurn;
        player.RecoverAp(playerApRecoveryPerTurn);
        SetAttackButtonInteractable(player.HasEnoughAp(basicAttackSkill.apCost));
        UpdateUI($"플레이어 턴: AP {playerApRecoveryPerTurn} 회복. 행동을 선택하세요.");
    }

    public void OnClickAttackButton()
    {
        if (currentState != BattleState.PlayerTurn)
        {
            return;
        }

        if (!player.SpendAp(basicAttackSkill.apCost))
        {
            UpdateUI($"AP가 부족합니다. 필요 AP: {basicAttackSkill.apCost}");
            return;
        }

        SetAttackButtonInteractable(false);
        int damage = CalculateSkillDamage(enemy, basicAttackSkill);
        enemy.TakeDamage(damage);
        UpdateUI($"{player.characterName}의 {basicAttackSkill.skillName}! {enemy.characterName}에게 {damage} 피해. ({basicAttackSkill.elementType})");

        if (enemy.IsDead())
        {
            EndBattle(BattleState.Victory);
            return;
        }

        StartCoroutine(EnemyTurnRoutine());
    }

    private IEnumerator EnemyTurnRoutine()
    {
        currentState = BattleState.EnemyTurn;
        yield return new WaitForSeconds(1.0f);

        player.TakeDamage(enemy.attackPower);
        UpdateUI($"{enemy.characterName}의 공격! {player.characterName}에게 {enemy.attackPower} 피해.");

        if (player.IsDead())
        {
            EndBattle(BattleState.Defeat);
            yield break;
        }

        yield return new WaitForSeconds(1.0f);
        StartPlayerTurn();
    }

    private int CalculateSkillDamage(CharacterData target, SkillData skill)
    {
        int damage = skill.power;

        if (skill.elementType != ElementType.None && skill.elementType == target.weaknessElement)
        {
            damage += 10;
        }

        return damage;
    }

    private void EndBattle(BattleState resultState)
    {
        currentState = resultState;
        SetAttackButtonInteractable(false);

        if (resultState == BattleState.Victory)
        {
            UpdateUI("승리! 적을 처치했습니다.");
        }
        else
        {
            UpdateUI("패배... 다시 도전하세요.");
        }
    }

    private void UpdateUI(string message)
    {
        if (playerHpText != null)
        {
            playerHpText.text = $"{player.characterName} HP: {player.currentHp}/{player.maxHp}";
        }

        if (playerApText != null)
        {
            playerApText.text = $"AP: {player.currentAp}/{player.maxAp}";
        }

        if (enemyHpText != null)
        {
            enemyHpText.text = $"{enemy.characterName} HP: {enemy.currentHp}/{enemy.maxHp}";
        }

        if (messageText != null)
        {
            messageText.text = message;
        }
    }

    private void SetAttackButtonInteractable(bool isInteractable)
    {
        if (attackButton != null)
        {
            attackButton.interactable = isInteractable;
        }
    }
}
