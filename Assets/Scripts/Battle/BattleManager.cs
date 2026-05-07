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

    [SerializeField] private string enemyName = "Slime";
    [SerializeField] private int enemyMaxHp = 80;
    [SerializeField] private int enemyAttack = 15;

    [Header("UI")]
    [SerializeField] private TMP_Text playerHpText;
    [SerializeField] private TMP_Text enemyHpText;
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private Button attackButton;

    private CharacterData player;
    private CharacterData enemy;

    private void Start()
    {
        StartBattle();
    }

    private void StartBattle()
    {
        currentState = BattleState.Start;

        player = new CharacterData(playerName, playerMaxHp, playerAttack);
        enemy = new CharacterData(enemyName, enemyMaxHp, enemyAttack);

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
        SetAttackButtonInteractable(true);
        UpdateUI("플레이어 턴: 공격을 선택하세요.");
    }

    public void OnClickAttackButton()
    {
        if (currentState != BattleState.PlayerTurn)
        {
            return;
        }

        SetAttackButtonInteractable(false);
        enemy.TakeDamage(player.attackPower);
        UpdateUI($"{player.characterName}의 공격! {enemy.characterName}에게 {player.attackPower} 피해.");

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
