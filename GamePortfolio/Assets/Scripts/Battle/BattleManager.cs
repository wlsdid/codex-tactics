using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleManager : MonoBehaviour
{
    [Header("Battle State")]
    [SerializeField] private BattleState currentState;

    [Header("Balance Config (optional)")]
    [SerializeField] private BattleBalanceConfig balanceConfig;

    [Header("Player Stats")]
    [SerializeField] private string playerName = "Hero";

    [Header("Stage Encounters")]
    [SerializeField] private List<StageData> stageEncounters = new List<StageData>();
    [NonSerialized] private int currentStageIndex;

    // These are overwritten by ApplyCurrentStageData() — keep as private backing fields
    private string enemyName = "Slime";
    private int enemyMaxHp = 80;
    private ElementType enemyWeakness = ElementType.Fire;
    private EnemyPatternData enemyPattern = new EnemyPatternData();

    // Skill names kept serialized for Inspector readability
    [SerializeField] private string basicSkillName = "Slash";
    [SerializeField] private string fireSkillName = "Fire Bolt";

    // Runtime backing fields with defaults (config-driven via Config* helpers)
    private int burnDamagePerTurn = 3;
    private int burnTurnDuration = 2;
    private int guardDamageReductionPercent = 50;
    private int sRankRewardGold = 150;
    private int aRankRewardGold = 120;
    private int bRankRewardGold = 100;
    private int defeatRewardGold = 0;

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
    [SerializeField] private TMP_Text runStatusText;
    [SerializeField] private TMP_Text stageText;
    [SerializeField] private TMP_Text stageObjectiveText;
    [SerializeField] private TMP_Text stageProgressText;
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
    [SerializeField] private Button continueButton;

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
    private int totalGoldEarned;
    private bool currentBattleRewardClaimed;
    private readonly HashSet<int> rewardedStageIndexes = new HashSet<int>();
    private readonly List<string> battleLogEntries = new List<string>();
    private int battleLogSequence;

    public string DebugPlayerHpText => playerHpText != null ? playerHpText.text : "";
    public string DebugPlayerApText => playerApText != null ? playerApText.text : "";
    public string DebugEnemyHpText => enemyHpText != null ? enemyHpText.text : "";
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
    public string DebugRunStatusText => runStatusText != null ? runStatusText.text : "";
    public string DebugStageText => stageText != null ? stageText.text : "";
    public string DebugStageObjectiveText => stageObjectiveText != null ? stageObjectiveText.text : "";
    public string DebugStageProgressText => stageProgressText != null ? stageProgressText.text : "";
    public bool DebugRetryButtonVisible => retryButton != null && retryButton.gameObject.activeSelf;
    public bool DebugRetryButtonInteractable => retryButton != null && retryButton.interactable;
    public bool DebugContinueButtonVisible => continueButton != null && continueButton.gameObject.activeSelf;
    public bool DebugContinueButtonInteractable => continueButton != null && continueButton.interactable;
    public bool DebugResultSummaryPanelVisible => resultSummaryPanel != null && resultSummaryPanel.activeSelf;
    public int DebugTotalDamageDealt => totalDamageDealt;
    public int DebugTotalDamageTaken => totalDamageTaken;
    public int DebugGuardUseCount => guardUseCount;
    public int DebugSkillsUsedCount => skillsUsedCount;
    public int DebugTotalGoldEarned => totalGoldEarned;

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
        EnsureStageEncounters();
        ApplyCurrentStageData();
        EnsureEnemyPattern();

        player = new CharacterData(playerName, ConfigPlayerMaxHp, 20, ElementType.None, ConfigPlayerMaxAp);
        enemy = new CharacterData(enemyName, enemyMaxHp, enemyPattern.normalAttackDamage, enemyWeakness);
        basicAttackSkill = new SkillData(basicSkillName, ConfigBasicSkillPower, ConfigBasicSkillApCost, ElementType.Physical, StatusEffectType.None);
        fireSkill = new SkillData(fireSkillName, ConfigFireSkillPower, ConfigFireSkillApCost, ElementType.Fire, StatusEffectType.Burn);
        basicAttackSkill.description = "Reliable no-cost physical attack.";
        fireSkill.description = "Costs AP, hits the enemy weakness, and applies Burn.";
        playerIsGuarding = false;
        enemyTurnCount = 0;
        totalDamageDealt = 0;
        totalDamageTaken = 0;
        guardUseCount = 0;
        skillsUsedCount = 0;
        currentBattleRewardClaimed = false;
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

        if (continueButton != null)
        {
            continueButton.onClick.RemoveListener(OnClickContinueButton);
            continueButton.onClick.AddListener(OnClickContinueButton);
        }

        SetRetryButtonVisible(false);
        SetContinueButtonVisible(false);
        SetResultSummaryVisible(false, "");

        UpdateUI("Battle Start!");
        StartPlayerTurn();
    }

    private void StartPlayerTurn()
    {
        currentState = BattleState.PlayerTurn;
        player.RecoverAp(ConfigPlayerApRecovery);
        UpdateActionButtons();
        UpdateUI($"Player Turn: recovered {ConfigPlayerApRecovery} AP. Choose an action.");
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

    public void OnClickContinueButton()
    {
        if (currentState != BattleState.Victory || !HasNextStage())
        {
            return;
        }

        StopAllCoroutines();
        currentStageIndex++;
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
            enemy.ApplyStatusEffect(StatusEffectType.Burn, ConfigBurnTurnDuration);
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
            enemy.TakeDamage(ConfigBurnDamagePerTurn);
            TrackDamageDealt(enemyHpBeforeBurn);
            enemy.ReduceStatusTurn();
            UpdateUI($"{enemy.characterName} takes {ConfigBurnDamagePerTurn} burn damage.");

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
            damage = Mathf.Max(1, damage * (100 - ConfigGuardReductionPercent) / 100);
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
            message += $" Extra effect: {skill.statusEffectType} for {ConfigBurnTurnDuration} turns.";
        }

        return message;
    }

    private void EndBattle(BattleState resultState)
    {
        currentState = resultState;
        SetActionButtonsInteractable(false);
        SetRetryButtonVisible(true);
        SetContinueButtonVisible(resultState == BattleState.Victory && HasNextStage());
        SetResultSummaryVisible(true, BuildResultSummaryText(resultState));

        if (resultState == BattleState.Victory)
        {
            UpdateUI(BuildVictoryGuideMessage());
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
            playerHpText.text = BuildResourceText($"{player.characterName} HP", player.currentHp, player.maxHp);
        }

        UpdateResourceSlider(playerHpSlider, player.currentHp, player.maxHp);

        if (playerApText != null)
        {
            playerApText.text = BuildResourceText("AP", player.currentAp, player.maxAp);
        }

        UpdateResourceSlider(playerApSlider, player.currentAp, player.maxAp);

        if (playerStatusText != null)
        {
            playerStatusText.text = BuildPlayerStatusText();
        }

        if (enemyHpText != null)
        {
            enemyHpText.text = BuildResourceText($"{enemy.characterName} HP", enemy.currentHp, enemy.maxHp);
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

        if (runStatusText != null)
        {
            runStatusText.text = BuildRunStatusText();
        }

        if (stageText != null)
        {
            stageText.text = BuildStageText();
        }

        if (stageObjectiveText != null)
        {
            stageObjectiveText.text = BuildStageObjectiveText();
        }

        if (stageProgressText != null)
        {
            stageProgressText.text = BuildStageProgressText();
        }

        if (messageText != null)
        {
            messageText.text = message;
        }

        UpdateSkillHelpText();

        AddBattleLogEntry(message);
    }

    private string BuildResourceText(string label, int currentValue, int maxValue)
    {
        int percentage = maxValue > 0 ? Mathf.RoundToInt((float)currentValue / maxValue * 100f) : 0;
        return $"{label}: {currentValue}/{maxValue} ({percentage}%)";
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

        while (battleLogEntries.Count > ConfigMaxBattleLogEntries)
        {
            battleLogEntries.RemoveAt(0);
        }

        RefreshBattleLogText();
    }

    private void RefreshBattleLogText()
    {
        if (battleLogText != null)
        {
            if (battleLogEntries.Count == 0)
            {
                battleLogText.text = "Recent Actions\nNo actions yet.";
                return;
            }

            battleLogText.text = "Recent Actions\n" + string.Join("\n", battleLogEntries);
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

    private void SetContinueButtonVisible(bool isVisible)
    {
        if (continueButton == null)
        {
            return;
        }

        continueButton.interactable = isVisible;
        continueButton.gameObject.SetActive(isVisible);
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
        BattleResultData resultData = BuildBattleResultData(resultState);
        if (resultState == BattleState.Victory)
        {
            ClaimBattleReward(resultData.rewardGold);
        }

        resultData.totalGold = totalGoldEarned;
        return BattleResultPresenter.BuildSummaryText(resultData);
    }

    private void ClaimBattleReward(int rewardGold)
    {
        if (currentBattleRewardClaimed || rewardedStageIndexes.Contains(currentStageIndex))
        {
            return;
        }

        totalGoldEarned += Mathf.Max(0, rewardGold);
        rewardedStageIndexes.Add(currentStageIndex);
        currentBattleRewardClaimed = true;
    }

    private BattleResultData BuildBattleResultData(BattleState resultState)
    {
        EnsureEnemyPattern();

        string rank = BattleResultEvaluator.BuildRank(resultState, enemyTurnCount, totalDamageTaken, balanceConfig);
        string lastEnemyPattern = BattleResultEvaluator.BuildLastEnemyPatternLabel(enemyTurnCount, enemyPattern);
        string paceLabel = BattleResultEvaluator.BuildPaceLabel(resultState, enemyTurnCount, balanceConfig);

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
            rewardGold = BattleResultEvaluator.BuildRewardGold(rank, ConfigSRankRewardGold, ConfigARankRewardGold, ConfigBRankRewardGold, ConfigDefeatRewardGold),
            totalGold = totalGoldEarned,
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
        string guardHelp = $"Guard: reduce next enemy attack by {ConfigGuardReductionPercent}%.";
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

    private void EnsureStageEncounters()
    {
        if (stageEncounters == null)
        {
            stageEncounters = new List<StageData>();
        }

        if (stageEncounters.Count == 0)
        {
            stageEncounters.Add(StageData.CreateStage1Normal());
            stageEncounters.Add(StageData.CreateStage1Boss());
        }

        currentStageIndex = Mathf.Clamp(currentStageIndex, 0, stageEncounters.Count - 1);
    }

    private void ApplyCurrentStageData()
    {
        StageData currentStage = GetCurrentStageData();
        if (currentStage == null || currentStage.enemy == null)
        {
            return;
        }

        enemyName = currentStage.enemy.enemyName;
        enemyMaxHp = currentStage.enemy.maxHp;
        enemyWeakness = currentStage.enemy.weakness;
        enemyPattern = currentStage.enemy.pattern;
    }

    private StageData GetCurrentStageData()
    {
        EnsureStageEncounters();
        return stageEncounters[currentStageIndex];
    }

    private bool HasNextStage()
    {
        EnsureStageEncounters();
        return currentStageIndex < stageEncounters.Count - 1;
    }

    private StageData GetNextStageData()
    {
        if (!HasNextStage())
        {
            return null;
        }

        return stageEncounters[currentStageIndex + 1];
    }

    private string BuildStageText()
    {
        StageData currentStage = GetCurrentStageData();
        return currentStage != null ? currentStage.BuildDisplayName() : "Stage: Unknown";
    }

    private string BuildRunStatusText()
    {
        if (currentState == BattleState.Victory)
        {
            return HasNextStage() ? "Run Status: Encounter Clear - Continue to Next" : "Run Status: Final Clear - Stage 1 Complete";
        }

        if (currentState == BattleState.Defeat)
        {
            return "Run Status: Retry Current Encounter";
        }

        return "Run Status: Stage 1 In Progress";
    }

    private string BuildStageObjectiveText()
    {
        StageData currentStage = GetCurrentStageData();
        if (currentStage == null)
        {
            return "Objective: Unknown";
        }

        if (currentState == BattleState.Victory)
        {
            if (HasNextStage())
            {
                StageData nextStage = GetNextStageData();
                string nextStageName = nextStage != null ? nextStage.BuildDisplayName() : "next encounter";
                return $"Objective Complete: {currentStage.BuildDisplayName()} | Continue to {nextStageName}";
            }

            return "Objective Complete: Stage 1 cleared | Final Clear";
        }

        if (currentState == BattleState.Defeat)
        {
            return $"Objective Failed: Retry {currentStage.BuildDisplayName()}";
        }

        return currentStage.BuildObjectiveText();
    }

    private string BuildStageProgressText()
    {
        EnsureStageEncounters();
        int currentEncounterNumber = Mathf.Clamp(currentStageIndex + 1, 1, stageEncounters.Count);
        string statusLabel = "Active";

        if (currentState == BattleState.Victory)
        {
            statusLabel = HasNextStage() ? "Encounter Clear" : "Stage Clear";
        }
        else if (currentState == BattleState.Defeat)
        {
            statusLabel = "Retry Needed";
        }

        return $"Progress: Encounter {currentEncounterNumber}/{stageEncounters.Count} | {statusLabel}";
    }

    private string BuildVictoryGuideMessage()
    {
        StageData currentStage = GetCurrentStageData();
        string currentStageName = currentStage != null ? currentStage.BuildDisplayName() : "Encounter";

        if (HasNextStage())
        {
            StageData nextStage = GetNextStageData();
            string nextStageName = nextStage != null ? nextStage.BuildDisplayName() : "the next encounter";
            return $"Victory! {currentStageName} cleared. Press Continue to enter {nextStageName}.";
        }

        return "Final Clear! Stage 1 completed. Review Total Gold, then Retry the boss if you want to practice.";
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
