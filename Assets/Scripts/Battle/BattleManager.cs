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

    [Header("Fire Skill")]
    [SerializeField] private string fireSkillName = "Fire Bolt";
    [SerializeField] private int fireSkillPower = 30;
    [SerializeField] private int fireSkillApCost = 2;
    [SerializeField] private ElementType fireSkillElement = ElementType.Fire;

    [Header("UI")]
    [SerializeField] private TMP_Text playerHpText;
    [SerializeField] private TMP_Text playerApText;
    [SerializeField] private TMP_Text enemyHpText;
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private Button attackButton;
    [SerializeField] private Button fireSkillButton;
    [SerializeField] private Button endTurnButton;

    private CharacterData player;
    private CharacterData enemy;
    private SkillData basicAttackSkill;
    private SkillData fireSkill;

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
        fireSkill = new SkillData(fireSkillName, fireSkillPower, fireSkillApCost, fireSkillElement, StatusEffectType.Burn);

        if (attackButton != null)
        {
            attackButton.onClick.AddListener(OnClickAttackButton);
        }

        if (fireSkillButton != null)
        {
            fireSkillButton.onClick.AddListener(OnClickFireSkillButton);
        }

        if (endTurnButton != null)
        {
            endTurnButton.onClick.AddListener(OnClickEndTurnButton);
        }

        UpdateUI("전투 시작!");
        StartPlayerTurn();
    }

    private void StartPlayerTurn()
    {
        currentState = BattleState.PlayerTurn;
        player.RecoverAp(playerApRecoveryPerTurn);
        UpdateActionButtons();
        UpdateUI($"플레이어 턴: AP {playerApRecoveryPerTurn} 회복. 행동을 선택하세요.");
    }

    public void OnClickAttackButton()
    {
        UsePlayerSkill(basicAttackSkill);
    }

    public void OnClickFireSkillButton()
    {
        UsePlayerSkill(fireSkill);
    }

    public void OnClickEndTurnButton()
    {
        EndPlayerTurn();
    }

    private void EndPlayerTurn()
    {
        if (currentState != BattleState.PlayerTurn)
        {
            return;
        }

        SetActionButtonsInteractable(false);
        UpdateUI($"{player.characterName}가 행동하지 않고 턴을 넘겼습니다. 다음 턴에 AP를 더 모을 수 있습니다.");
        StartCoroutine(EnemyTurnRoutine());
    }

    private void UsePlayerSkill(SkillData skill)
    {
        if (currentState != BattleState.PlayerTurn)
        {
            return;
        }

        if (!player.SpendAp(skill.apCost))
        {
            UpdateUI($"AP가 부족합니다. {skill.skillName} 필요 AP: {skill.apCost}");
            UpdateActionButtons();
            return;
        }

        SetActionButtonsInteractable(false);
        int damage = CalculateSkillDamage(enemy, skill);
        enemy.TakeDamage(damage);
        UpdateUI(BuildSkillMessage(skill, damage));

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

    private string BuildSkillMessage(SkillData skill, int damage)
    {
        string message = $"{player.characterName}의 {skill.skillName}! {enemy.characterName}에게 {damage} 피해. ({skill.elementType})";

        if (skill.HasStatusEffect())
        {
            message += $" 추가 효과 후보: {skill.statusEffectType}";
        }

        return message;
    }

    private void EndBattle(BattleState resultState)
    {
        currentState = resultState;
        SetActionButtonsInteractable(false);

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

    private void SetFireSkillButtonInteractable(bool isInteractable)
    {
        if (fireSkillButton != null)
        {
            fireSkillButton.interactable = isInteractable;
        }
    }

    private void SetEndTurnButtonInteractable(bool isInteractable)
    {
        if (endTurnButton != null)
        {
            endTurnButton.interactable = isInteractable;
        }
    }

    private void SetActionButtonsInteractable(bool isInteractable)
    {
        SetAttackButtonInteractable(isInteractable);
        SetFireSkillButtonInteractable(isInteractable);
        SetEndTurnButtonInteractable(isInteractable);
    }

    private void UpdateActionButtons()
    {
        SetAttackButtonInteractable(player.HasEnoughAp(basicAttackSkill.apCost));
        SetFireSkillButtonInteractable(player.HasEnoughAp(fireSkill.apCost));
        SetEndTurnButtonInteractable(currentState == BattleState.PlayerTurn);
    }
}
