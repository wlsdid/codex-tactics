using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private ElementType enemyWeakness = ElementType.Fire;
    [SerializeField] private EnemyPatternData enemyPattern = new EnemyPatternData();

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

    [Header("Status Effect")]
    [SerializeField] private int burnDamagePerTurn = 3;
    [SerializeField] private int burnTurnDuration = 2;

    [Header("Guard")]
    [SerializeField] private int guardDamageReductionPercent = 50;

    [Header("Result Reward")]
    [SerializeField] private int sRankRewardGold = 150;
    [SerializeField] private int aRankRewardGold = 120;
    [SerializeField] private int bRankRewardGold = 100;
    [SerializeField] private int defeatRewardGold = 0;

    [Header("UI")]
    [SerializeField] private TMP_Text playerHpText;
    [SerializeField] private Slider playerHpSlider;
    [SerializeField] private TMP_Text playerApText;
    [SerializeField] private Slider playerApSlider;
    [SerializeField] private TMP_Text playerStatusText;
    [SerializeField] private TMP_Text enemyHpText;
    [SerializeField] private Slider enemyHpSlider;
    [SerializeField] private TMP_Text enemyStatusText;
    [SerializeField] private TMP_Text enemyIntentText;
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private TMP_Text skillHelpText;
    [SerializeField] private TMP_Text battleLogText;
    [SerializeField] private TMP_Text resultSummaryText;
    [SerializeField] private GameObject resultSummaryPanel;
    [SerializeField] private Button attackButton;
    [SerializeField] private Button fireSkillButton;
    [SerializeField] private Button endTurnButton;
    [SerializeField] private Button guardButton;
    [SerializeField] private Button retryButton;

    private CharacterData player;
    private CharacterData enemy;
    private SkillData basicAttackSkill;
    private SkillData fireSkill;
    private bool playerIsGuarding;
    private int enemyTurnCount;
    private int totalDamageDealt;
    private int totalDamageTaken;
    private int guardUseCount;
    private int skillsUsedCount;
    private readonly List<string> battleLogEntries = new List<string>();
    private int battleLogSequence;
    private const int MaxBattleLogEntries = 6;

    public string DebugPlayerHpText => playerHpText != null ? playerHpText.text : "";
    public float DebugPlayerHpBarValue => playerHpSlider != null ? playerHpSlider.value : -1f;
    public float DebugPlayerHpBarMaxValue => playerHpSlider != null ? playerHpSlider.maxValue : -1f;
    public float DebugPlayerApBarValue => playerApSlider != null ? playerApSlider.value : -1f;
    public float DebugPlayerApBarMaxValue => playerApSlider != null ? playerApSlider.maxValue : -1f;
    public float DebugEnemyHpBarValue => enemyHpSlider != null ? enemyHpSlider.value : -1f;
    public float DebugEnemyHpBarMaxValue => enemyHpSlider != null ? enemyHpSlider.maxValue : -1f;
    public string DebugMessageText => messageText != null ? messageText.text : "";
    public string DebugSkillHelpText => skillHelpText != null ? skillHelpText.text : "";
    public string DebugBattleLogText => battleLogText != null ? battleLogText.text : "";
    public string DebugResultSummaryText => resultSummaryText != null ? resultSummaryText.text : "";
    public string DebugPlayerStatusText => playerStatusText != null ? playerStatusText.text : "";
    public string DebugEnemyStatusText => enemyStatusText != null ? enemyStatusText.text : "";
    public string DebugEnemyIntentText => enemyIntentText != null ? enemyIntentText.text : "";
    public bool DebugRetryButtonVisible => retryButton != null && retryButton.gameObject.activeSelf;
    public bool DebugRetryButtonInteractable => retryButton != null && retryButton.interactable;
    public bool DebugResultSummaryPanelVisible => resultSummaryPanel != null && resultSummaryPanel.activeSelf;
    public int DebugTotalDamageDealt => totalDamageDealt;
    public int DebugTotalDamageTaken => totalDamageTaken;
    public int DebugGuardUseCount => guardUseCount;
    public int DebugSkillsUsedCount => skillsUsedCount;

    private void Start()
    {
        StartBattle();
    }

    public void DebugStartBattleForTest()
    {
        StartBattle();
    }

    public void DebugEndBattleForTest(BattleState resultState)
    {
        EndBattle(resultState);
    }

    private void StartBattle()
    {
        currentState = BattleState.Start;
        EnsureEnemyPattern();

        player = new CharacterData(playerName, playerMaxHp, playerAttack, ElementType.None, playerMaxAp);
        enemy = new CharacterData(enemyName, enemyMaxHp, enemyPattern.normalAttackDamage, enemyWeakness);
        basicAttackSkill = new SkillData(basicSkillName, basicSkillPower, basicSkillApCost, basicSkillElement, StatusEffectType.None);
        fireSkill = new SkillData(fireSkillName, fireSkillPower, fireSkillApCost, fireSkillElement, StatusEffectType.Burn);
        basicAttackSkill.description = "Reliable no-cost physical attack.";
        fireSkill.description = "Costs AP, hits the enemy weakness, and applies Burn.";
        playerIsGuarding = false;
        enemyTurnCount = 0;
        totalDamageDealt = 0;
        totalDamageTaken = 0;
        guardUseCount = 0;
        skillsUsedCount = 0;
        battleLogEntries.Clear();
        battleLogSequence = 0;
        RefreshBattleLogText();

        if (attackButton != null)
        {
            attackButton.onClick.RemoveListener(OnClickAttackButton);
            attackButton.onClick.AddListener(OnClickAttackButton);
        }

        if (fireSkillButton != null)
        {
            fireSkillButton.onClick.RemoveListener(OnClickFireSkillButton);
            fireSkillButton.onClick.AddListener(OnClickFireSkillButton);
        }

        if (endTurnButton != null)
        {
            endTurnButton.onClick.RemoveListener(OnClickEndTurnButton);
            endTurnButton.onClick.AddListener(OnClickEndTurnButton);
        }

        if (guardButton != null)
        {
            guardButton.onClick.RemoveListener(OnClickGuardButton);
            guardButton.onClick.AddListener(OnClickGuardButton);
        }

        if (retryButton != null)
        {
            retryButton.onClick.RemoveListener(OnClickRetryButton);
            retryButton.onClick.AddListener(OnClickRetryButton);
        }

        SetRetryButtonVisible(false);
        SetResultSummaryVisible(false, "");

        UpdateUI("Battle Start!");
        StartPlayerTurn();
    }

    private void StartPlayerTurn()
    {
        currentState = BattleState.PlayerTurn;
        player.RecoverAp(playerApRecoveryPerTurn);
        UpdateActionButtons();
        UpdateUI($"Player Turn: recovered {playerApRecoveryPerTurn} AP. Choose an action.");
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

    public void OnClickGuardButton()
    {
        GuardAndEndPlayerTurn();
    }

    public void OnClickRetryButton()
    {
        if (currentState != BattleState.Victory && currentState != BattleState.Defeat)
        {
            return;
        }

        StopAllCoroutines();
        StartBattle();
    }

    private void EndPlayerTurn()
    {
        if (currentState != BattleState.PlayerTurn)
        {
            return;
        }

        SetActionButtonsInteractable(false);
        UpdateUI($"{player.characterName} skipped the turn. You can recover more AP next turn.");

        if (Application.isPlaying)
        {
            StartCoroutine(EnemyTurnRoutine());
        }
    }

    private void GuardAndEndPlayerTurn()
    {
        if (currentState != BattleState.PlayerTurn)
        {
            return;
        }

        playerIsGuarding = true;
        guardUseCount++;
        SetActionButtonsInteractable(false);
        UpdateUI($"{player.characterName} guards. Next enemy attack damage is reduced.");

        if (Application.isPlaying)
        {
            StartCoroutine(EnemyTurnRoutine());
        }
    }

    private void UsePlayerSkill(SkillData skill)
    {
        if (currentState != BattleState.PlayerTurn)
        {
            return;
        }

        if (!player.SpendAp(skill.apCost))
        {
            UpdateUI($"Not enough AP. {skill.skillName} needs {skill.apCost} AP.");
            UpdateActionButtons();
            return;
        }

        SetActionButtonsInteractable(false);
        skillsUsedCount++;
        int damage = CalculateSkillDamage(enemy, skill);
        int enemyHpBeforeDamage = enemy.currentHp;
        enemy.TakeDamage(damage);
        TrackDamageDealt(enemyHpBeforeDamage);

        if (skill.statusEffectType == StatusEffectType.Burn)
        {
            enemy.ApplyStatusEffect(StatusEffectType.Burn, burnTurnDuration);
        }

        UpdateUI(BuildSkillMessage(skill, damage));

        if (enemy.IsDead())
        {
            EndBattle(BattleState.Victory);
            return;
        }

        if (Application.isPlaying)
        {
            StartCoroutine(EnemyTurnRoutine());
        }
    }

    private IEnumerator EnemyTurnRoutine()
    {
        currentState = BattleState.EnemyTurn;

        if (enemy.HasStatusEffect(StatusEffectType.Burn))
        {
            int enemyHpBeforeBurn = enemy.currentHp;
            enemy.TakeDamage(burnDamagePerTurn);
            TrackDamageDealt(enemyHpBeforeBurn);
            enemy.ReduceStatusTurn();
            UpdateUI($"{enemy.characterName} takes {burnDamagePerTurn} burn damage.");

            if (enemy.IsDead())
            {
                yield return new WaitForSeconds(1.0f);
                EndBattle(BattleState.Victory);
                yield break;
            }

            yield return new WaitForSeconds(1.0f);
        }

        yield return new WaitForSeconds(1.0f);

        ResolveEnemyAttack();

        if (player.IsDead())
        {
            EndBattle(BattleState.Defeat);
            yield break;
        }

        yield return new WaitForSeconds(1.0f);
        StartPlayerTurn();
    }

    public void DebugResolveEnemyAttackForTest()
    {
        ResolveEnemyAttack();
    }

    private void ResolveEnemyAttack()
    {
        EnsureEnemyPattern();
        enemyTurnCount++;
        int damage = enemyPattern.GetDamageForTurn(enemyTurnCount);
        bool isStrongAttackTurn = enemyPattern.IsStrongAttackTurn(enemyTurnCount);

        if (playerIsGuarding)
        {
            damage = Mathf.Max(1, damage * (100 - guardDamageReductionPercent) / 100);
            playerIsGuarding = false;
            int playerHpBeforeDamage = player.currentHp;
            player.TakeDamage(damage);
            TrackDamageTaken(playerHpBeforeDamage);

            if (isStrongAttackTurn)
            {
                UpdateUI($"{enemy.characterName} uses {enemyPattern.strongAttackName} on turn {enemyTurnCount}! {player.characterName} guards and takes {damage} damage.");
                return;
            }

            UpdateUI($"{enemy.characterName} {enemyPattern.normalAttackMessageVerb}! {player.characterName} guards and takes {damage} damage.");
            return;
        }

        int playerHpBeforeNormalDamage = player.currentHp;
        player.TakeDamage(damage);
        TrackDamageTaken(playerHpBeforeNormalDamage);

        if (isStrongAttackTurn)
        {
            UpdateUI($"{enemy.characterName} uses {enemyPattern.strongAttackName} on turn {enemyTurnCount}! {player.characterName} takes {damage} damage.");
            return;
        }

        UpdateUI($"{enemy.characterName} {enemyPattern.normalAttackMessageVerb}! {player.characterName} takes {damage} damage.");
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

    private void TrackDamageDealt(int enemyHpBeforeDamage)
    {
        totalDamageDealt += Mathf.Max(0, enemyHpBeforeDamage - enemy.currentHp);
    }

    private void TrackDamageTaken(int playerHpBeforeDamage)
    {
        totalDamageTaken += Mathf.Max(0, playerHpBeforeDamage - player.currentHp);
    }

    private string BuildSkillMessage(SkillData skill, int damage)
    {
        string message = $"{player.characterName} uses {skill.skillName}! {enemy.characterName} takes {damage} damage. ({skill.elementType})";

        if (skill.HasStatusEffect())
        {
            message += $" Extra effect: {skill.statusEffectType} for {burnTurnDuration} turns.";
        }

        return message;
    }

    private void EndBattle(BattleState resultState)
    {
        currentState = resultState;
        SetActionButtonsInteractable(false);
        SetRetryButtonVisible(true);
        SetResultSummaryVisible(true, BuildResultSummaryText(resultState));

        if (resultState == BattleState.Victory)
        {
            UpdateUI("Victory! Enemy defeated.");
        }
        else
        {
            UpdateUI("Defeat... Try again.");
        }
    }

    private void UpdateUI(string message)
    {
        if (playerHpText != null)
        {
            playerHpText.text = $"{player.characterName} HP: {player.currentHp}/{player.maxHp}";
        }

        UpdateResourceSlider(playerHpSlider, player.currentHp, player.maxHp);

        if (playerApText != null)
        {
            playerApText.text = $"AP: {player.currentAp}/{player.maxAp}";
        }

        UpdateResourceSlider(playerApSlider, player.currentAp, player.maxAp);

        if (playerStatusText != null)
        {
            playerStatusText.text = BuildPlayerStatusText();
        }

        if (enemyHpText != null)
        {
            enemyHpText.text = $"{enemy.characterName} HP: {enemy.currentHp}/{enemy.maxHp}";
        }

        UpdateResourceSlider(enemyHpSlider, enemy.currentHp, enemy.maxHp);

        if (enemyStatusText != null)
        {
            enemyStatusText.text = BuildEnemyStatusText();
        }

        if (enemyIntentText != null)
        {
            enemyIntentText.text = BuildEnemyIntentText();
        }

        if (messageText != null)
        {
            messageText.text = message;
        }

        UpdateSkillHelpText();

        AddBattleLogEntry(message);
    }

    private void UpdateResourceSlider(Slider slider, int currentValue, int maxValue)
    {
        if (slider == null)
        {
            return;
        }

        slider.minValue = 0f;
        slider.maxValue = maxValue;
        slider.value = Mathf.Clamp(currentValue, 0, maxValue);
    }

    private void AddBattleLogEntry(string message)
    {
        if (string.IsNullOrWhiteSpace(message) || message == "Battle Start!")
        {
            return;
        }

        battleLogSequence++;
        battleLogEntries.Add($"{battleLogSequence}. {message}");

        while (battleLogEntries.Count > MaxBattleLogEntries)
        {
            battleLogEntries.RemoveAt(0);
        }

        RefreshBattleLogText();
    }

    private void RefreshBattleLogText()
    {
        if (battleLogText != null)
        {
            battleLogText.text = string.Join("\n", battleLogEntries);
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

    private void SetGuardButtonInteractable(bool isInteractable)
    {
        if (guardButton != null)
        {
            guardButton.interactable = isInteractable;
        }
    }

    private void SetActionButtonsInteractable(bool isInteractable)
    {
        SetAttackButtonInteractable(isInteractable);
        SetFireSkillButtonInteractable(isInteractable);
        SetEndTurnButtonInteractable(isInteractable);
        SetGuardButtonInteractable(isInteractable);
    }

    private void SetRetryButtonVisible(bool isVisible)
    {
        if (retryButton == null)
        {
            return;
        }

        retryButton.interactable = isVisible;
        retryButton.gameObject.SetActive(isVisible);
    }

    private void SetResultSummaryVisible(bool isVisible, string summary)
    {
        if (resultSummaryText != null)
        {
            resultSummaryText.text = summary;
            resultSummaryText.gameObject.SetActive(isVisible);
        }

        if (resultSummaryPanel != null)
        {
            resultSummaryPanel.SetActive(isVisible);
        }
    }

    private string BuildResultSummaryText(BattleState resultState)
    {
        return BattleResultPresenter.BuildSummaryText(BuildBattleResultData(resultState));
    }

    private BattleResultData BuildBattleResultData(BattleState resultState)
    {
        EnsureEnemyPattern();

        string rank = BattleResultEvaluator.BuildRank(resultState, enemyTurnCount, totalDamageTaken);
        string lastEnemyPattern = BattleResultEvaluator.BuildLastEnemyPatternLabel(enemyTurnCount, enemyPattern);
        string paceLabel = BattleResultEvaluator.BuildPaceLabel(resultState, enemyTurnCount);

        return new BattleResultData
        {
            resultLabel = resultState == BattleState.Victory ? "Victory" : "Defeat",
            enemyTurns = enemyTurnCount,
            playerName = player.characterName,
            playerCurrentHp = player.currentHp,
            playerMaxHp = player.maxHp,
            playerCurrentAp = player.currentAp,
            playerMaxAp = player.maxAp,
            enemyName = enemy.characterName,
            enemyCurrentHp = enemy.currentHp,
            enemyMaxHp = enemy.maxHp,
            damageDealt = totalDamageDealt,
            damageTaken = totalDamageTaken,
            guardUses = guardUseCount,
            skillsUsed = skillsUsedCount,
            paceLabel = paceLabel,
            survivalLabel = BattleResultEvaluator.BuildSurvivalLabel(player.currentHp, player.maxHp),
            rank = rank,
            rewardGold = BattleResultEvaluator.BuildRewardGold(rank, sRankRewardGold, aRankRewardGold, bRankRewardGold, defeatRewardGold),
            resultTip = BattleResultEvaluator.BuildResultTip(rank, lastEnemyPattern, enemyPattern.strongAttackName),
            lastEnemyPattern = lastEnemyPattern
        };
    }


    private void UpdateActionButtons()
    {
        SetAttackButtonInteractable(player.HasEnoughAp(basicAttackSkill.apCost));
        SetFireSkillButtonInteractable(player.HasEnoughAp(fireSkill.apCost));
        SetEndTurnButtonInteractable(currentState == BattleState.PlayerTurn);
        SetGuardButtonInteractable(currentState == BattleState.PlayerTurn);
        UpdateSkillHelpText();
    }

    private void UpdateSkillHelpText()
    {
        if (skillHelpText == null || basicAttackSkill == null || fireSkill == null)
        {
            return;
        }

        EnsureEnemyPattern();
        string attackHelp = BuildSkillHelpLine(basicAttackSkill);
        string fireHelp = BuildSkillHelpLine(fireSkill);
        string guardHelp = $"Guard: reduce next enemy attack by {guardDamageReductionPercent}%.";
        string turnHint = enemyPattern.BuildPatternHelpText();
        skillHelpText.text = attackHelp + "\n" + fireHelp + "\n" + guardHelp + "\n" + turnHint;
    }

    private string BuildSkillHelpLine(SkillData skill)
    {
        string line = $"{skill.skillName}: {skill.power} power, {skill.apCost} AP, {skill.elementType}.";

        if (skill.HasStatusEffect())
        {
            line += $" Applies {skill.statusEffectType}.";
        }

        if (!string.IsNullOrWhiteSpace(skill.description))
        {
            line += $" {skill.description}";
        }

        return line;
    }

    private void EnsureEnemyPattern()
    {
        if (enemyPattern == null)
        {
            enemyPattern = new EnemyPatternData();
        }
    }

    private string BuildPlayerStatusText()
    {
        if (currentState == BattleState.Victory || currentState == BattleState.Defeat)
        {
            return "Status: Battle ended";
        }

        if (playerIsGuarding)
        {
            return "Status: Guarding";
        }

        return "Status: Ready";
    }

    private string BuildEnemyStatusText()
    {
        if (enemy.currentStatusEffect == StatusEffectType.None)
        {
            return "Status: None";
        }

        return $"Status: {enemy.currentStatusEffect} ({enemy.statusTurnsRemaining} turns)";
    }

    private string BuildEnemyIntentText()
    {
        if (currentState == BattleState.Victory || currentState == BattleState.Defeat)
        {
            return "Next Enemy: Battle ended";
        }

        EnsureEnemyPattern();
        int nextEnemyTurn = enemyTurnCount + 1;

        if (enemyPattern.IsStrongAttackTurn(nextEnemyTurn))
        {
            return $"Next Enemy: {enemyPattern.strongAttackName} ({enemyPattern.strongAttackDamage})";
        }

        return $"Next Enemy: Normal Attack ({enemyPattern.normalAttackDamage})";
    }
}
