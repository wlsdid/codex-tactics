using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls the stage select screen: card selection, locked state, description updates,
/// and Start Battle / Back navigation.
/// </summary>
public class StageSelectController : MonoBehaviour
{
    [Header("Stage Cards")]
    [SerializeField] private Button stage1CardButton;
    [SerializeField] private Button stage2CardButton;
    [SerializeField] private Button stage3CardButton;
    [SerializeField] private Button stage4CardButton;
    [SerializeField] private Button stage5CardButton;
    [SerializeField] private Button stage6CardButton;
    [SerializeField] private Image stage1CardBg;
    [SerializeField] private Image stage2CardBg;
    [SerializeField] private Image stage3CardBg;
    [SerializeField] private Image stage4CardBg;
    [SerializeField] private Image stage5CardBg;
    [SerializeField] private Image stage6CardBg;
    [SerializeField] private TMP_Text stage1StatusText;
    [SerializeField] private TMP_Text stage2StatusText;
    [SerializeField] private TMP_Text stage3StatusText;
    [SerializeField] private TMP_Text stage4StatusText;
    [SerializeField] private TMP_Text stage5StatusText;
    [SerializeField] private TMP_Text stage6StatusText;

    [Header("Description Panel")]
    [SerializeField] private TMP_Text stageNameText;
    [SerializeField] private TMP_Text stageDescriptionText;

    [Header("Buttons")]
    [SerializeField] private Button startBattleButton;
    [SerializeField] private Button backButton;

    [Header("Colors")]
    [SerializeField] private Color selectedColor = new Color(0.15f, 0.20f, 0.35f, 0.95f);
    [SerializeField] private Color normalColor = new Color(0.07f, 0.07f, 0.12f, 0.92f);
    [SerializeField] private Color lockedColor = new Color(0.04f, 0.04f, 0.07f, 0.92f);

    private int selectedStageIndex = -1;

    // Stage metadata for descriptions
    private static readonly string[] StageNames = {
        "Slime Scout Route",
        "Wolf Ambush",
        "Golem Depths",
        "Storm Peaks",
        "Shadow Realm",
        "Sanctuary of Radiance"
    };

    private static readonly ElementType[] StageElements = {
        ElementType.Fire,      // Stage 1: Slime/Fire
        ElementType.Nature,    // Stage 2: Wolf/Nature
        ElementType.Earth,     // Stage 3: Golem/Earth
        ElementType.Lightning, // Stage 4: Storm Hawk/Lightning
        ElementType.Dark,      // Stage 5: Shadow Wraith/Dark
        ElementType.Light      // Stage 6: Light Warden/Light
    };

    private static readonly string[] StageDescriptions = {
        "A basic encounter against slimes.\nLearn the combat basics: Attack, Guard, Fire Skill, and Break.\nDefeat the Slime Scout to advance.",
        "Wolf packs hunt in the moonlit clearing.\nRequires completing Slime Scout Route first.\nBeware of coordinated attacks.",
        "Ancient golems guard the underground depths.\nTougher enemies with stronger defenses.\nRequires completing Wolf Ambush first.",
        "Lightning birds rule the high peaks.\nFast, powerful enemies with devastating aerial attacks.\nRequires completing Golem Depths first.",
        "Darkness consumes the Shadow Realm.\nOnly the strongest can face the void.\nRequires completing Storm Peaks first.",
        "The Sanctuary of Radiance awaits.\nHoly light tests the worthy.\nRequires completing Shadow Realm first."
    };

    private static readonly string[] StageStageNames = {
        "Stage 1-1: Slime Scout",
        "Stage 1-2: Wolf Ambush",
        "Stage 2-1: Golem Depths",
        "Stage 2-2: Storm Peaks",
        "Stage 3-1: Shadow Realm",
        "Stage 3-2: Sanctuary of Radiance"
    };

    private static readonly string[] StageDifficulty = {
        "★",      // Stage 1
        "★",      // Stage 2
        "★★",     // Stage 3
        "★★",     // Stage 4
        "★★★",    // Stage 5
        "★★★"     // Stage 6
    };

    private static readonly string[] StageRewardGold = {
        "Rank Reward: 100-150G",     // Stage 1
        "Rank Reward: 100-150G",     // Stage 2
        "Rank Reward: 100-150G",     // Stage 3
        "Rank Reward: 100-150G",     // Stage 4
        "Rank Reward: 100-150G",     // Stage 5
        "Rank Reward: 100-150G"      // Stage 6
    };

    private static readonly string[] StageRewardXP = {
        "80 XP",     // Stage 1: 50 + (0+1)*30
        "110 XP",    // Stage 2: 50 + (1+1)*30
        "140 XP",    // Stage 3: 50 + (2+1)*30
        "170 XP",    // Stage 4: 50 + (3+1)*30
        "200 XP",    // Stage 5: 50 + (4+1)*30
        "230 XP"     // Stage 6: 50 + (5+1)*30
    };

    /// <summary>Selected stage index (0-based) for BattleScene to read.</summary>
    public static int SelectedStageIndex { get; private set; } = -1;

    private void Start()
    {
        // Wire stage card buttons
        WireStageCard(0, stage1CardButton);
        WireStageCard(1, stage2CardButton);
        WireStageCard(2, stage3CardButton);
        WireStageCard(3, stage4CardButton);
        WireStageCard(4, stage5CardButton);
        WireStageCard(5, stage6CardButton);
        // Stage 3-6 cards are optional (need scene objects) — WireStageCard handles null
        if (backButton != null)
            backButton.onClick.AddListener(OnBackClicked);
        if (startBattleButton != null)
            startBattleButton.onClick.AddListener(OnStartBattleClicked);

        // Initialize UI state
        UpdateCardVisuals();
    }

    private void WireStageCard(int index, Button cardButton)
    {
        if (cardButton != null)
        {
            int captured = index;
            cardButton.onClick.AddListener(() => OnStageCardClicked(captured));
        }
    }

    private void OnStageCardClicked(int index)
    {
        if (!ProgressState.IsStageUnlocked(index)) return;

        selectedStageIndex = index;
        UpdateCardVisuals();
        UpdateDescription(index);
        UpdateStartBattleButton();
    }

    private void OnStartBattleClicked()
    {
        if (selectedStageIndex < 0) return;
        if (!ProgressState.IsStageUnlocked(selectedStageIndex)) return;

        // Store selected stage for BattleScene
        SelectedStageIndex = selectedStageIndex;
        UnityEngine.SceneManagement.SceneManager.LoadScene(GameSceneFlow.BattleSceneName);
    }

    private void OnBackClicked()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(GameSceneFlow.TitleSceneName);
    }

    private void UpdateCardVisuals()
    {
        UpdateSingleCardVisual(0, stage1CardButton, stage1CardBg, stage1StatusText);
        UpdateSingleCardVisual(1, stage2CardButton, stage2CardBg, stage2StatusText);
        UpdateSingleCardVisual(2, stage3CardButton, stage3CardBg, stage3StatusText);
        UpdateSingleCardVisual(3, stage4CardButton, stage4CardBg, stage4StatusText);
        UpdateSingleCardVisual(4, stage5CardButton, stage5CardBg, stage5StatusText);
        UpdateSingleCardVisual(5, stage6CardButton, stage6CardBg, stage6StatusText);
    }

    private void UpdateSingleCardVisual(int stageIndex, Button cardButton, Image cardBg, TMP_Text statusText)
    {
        if (cardButton == null) return;
        bool unlocked = ProgressState.IsStageUnlocked(stageIndex);
        bool completed = ProgressState.IsStageCompleted(stageIndex);
        cardButton.interactable = unlocked;
        SetCardState(cardBg, cardButton.interactable, unlocked, stageIndex, completed);
        if (statusText != null)
        {
            statusText.text = GetStageStatusText(stageIndex, unlocked, completed);
            if (unlocked)
                statusText.color = PlaceholderSpriteGenerator.GetElementTextColor(StageElements[stageIndex]);
            else
                statusText.color = new Color(0.5f, 0.5f, 0.5f);
        }
    }

    private void SetCardState(Image bg, bool canInteract, bool isUnlocked, int stageIndex, bool isCompleted)
    {
        if (bg == null) return;

        if (!isUnlocked)
        {
            bg.color = lockedColor;
        }
        else if (canInteract && selectedStageIndex >= 0 && IsCardForStage(bg, selectedStageIndex))
        {
            // Selected: bright element color
            var elem = stageIndex >= 0 && stageIndex < StageElements.Length
                ? PlaceholderSpriteGenerator.GetElementColor(StageElements[stageIndex])
                : normalColor;
            bg.color = new Color(elem.r, elem.g, elem.b, 0.95f);
        }
        else if (isCompleted)
        {
            // Completed: gold-tinted to differentiate from available
            bg.color = new Color(0.35f, 0.28f, 0.12f, 0.85f);
        }
        else
        {
            // Unlocked but not selected: muted element color
            if (stageIndex >= 0 && stageIndex < StageElements.Length)
            {
                var elemColor = PlaceholderSpriteGenerator.GetElementColor(StageElements[stageIndex]);
                bg.color = new Color(elemColor.r * 0.3f, elemColor.g * 0.3f, elemColor.b * 0.3f, 0.8f);
            }
            else
            {
                bg.color = normalColor;
            }
        }
    }

    private bool IsCardForStage(Image cardBg, int stageIndex)
    {
        // Match by name convention
        if (cardBg == null) return false;
        if (stageIndex == 0 && cardBg.name.Contains("Stage Card 1")) return true;
        if (stageIndex == 1 && cardBg.name.Contains("Stage Card 2")) return true;
        if (stageIndex == 2 && cardBg.name.Contains("Stage Card 3")) return true;
        if (stageIndex == 3 && cardBg.name.Contains("Stage Card 4")) return true;
        if (stageIndex == 4 && cardBg.name.Contains("Stage Card 5")) return true;
        if (stageIndex == 5 && cardBg.name.Contains("Stage Card 6")) return true;
        return false;
    }

    private void UpdateDescription(int index)
    {
        if (index < 0 || index >= StageNames.Length) return;
        if (stageNameText != null)
            stageNameText.text = StageStageNames[index];
        if (stageDescriptionText != null)
        {
            bool unlocked = ProgressState.IsStageUnlocked(index);
            bool completed = ProgressState.IsStageCompleted(index);
            string elementIcon = GetElementIcon(StageElements[index]);
            string difficulty = index >= 0 && index < StageDifficulty.Length ? StageDifficulty[index] : "★";
            string rewardGold = index >= 0 && index < StageRewardGold.Length ? StageRewardGold[index] : "";
            string rewardXP = index >= 0 && index < StageRewardXP.Length ? StageRewardXP[index] : "";

            string encounters = $"Encounters: {PlaceholderSpriteGenerator.StageEnemyNames[index]} → {PlaceholderSpriteGenerator.StageBossNames[index]}";
            string element = $"Element: {elementIcon} {StageElements[index]} | Difficulty: {difficulty}";
            string reward = $"Reward: {rewardGold} + Bonuses / {rewardXP}";

            string statusText = GetStageStatusText(index, unlocked, completed);
            string unlockCondition;
            if (completed)
                unlockCondition = $"Status: {statusText}";
            else if (unlocked)
                unlockCondition = $"Status: {statusText} — Click Start Battle";
            else if (index > 0)
                unlockCondition = $"Status: {statusText} — Clear \"{StageNames[index - 1]}\" to unlock";
            else
                unlockCondition = $"Status: {statusText}";

            string description = StageDescriptions[index];
            stageDescriptionText.text = $"{description}\n\n{encounters}\n{element}\n{reward}\n{unlockCondition}";
        }
    }

    private void UpdateStartBattleButton()
    {
        if (startBattleButton == null) return;
        bool canStart = selectedStageIndex >= 0 &&
                        ProgressState.IsStageUnlocked(selectedStageIndex);
        startBattleButton.interactable = canStart;
    }

    /// <summary>Get the first unlocked but not completed stage index (recommended next).</summary>
    private static int GetNextRecommendedStageIndex()
    {
        for (int i = 0; i < ProgressState.TotalStages; i++)
        {
            if (ProgressState.IsStageUnlocked(i) && !ProgressState.IsStageCompleted(i))
                return i;
        }
        return -1;
    }

    /// <summary>Get status text for a stage card based on progress.</summary>
    private static string GetStageStatusText(int index, bool unlocked, bool completed)
    {
        if (completed) return "✅ Cleared";
        if (!unlocked) return "🔒 Locked";
        int nextIndex = GetNextRecommendedStageIndex();
        if (index == nextIndex) return "⭐ Next";
        return "▶ Available";
    }

    /// <summary>Get an emoji icon representing the given element type.</summary>
    private static string GetElementIcon(ElementType element)
    {
        return element switch
        {
            ElementType.Fire => "🔥",
            ElementType.Ice => "❄",
            ElementType.Lightning => "⚡",
            ElementType.Nature => "🌿",
            ElementType.Earth => "🌍",
            ElementType.Dark => "🌑",
            ElementType.Light => "✨",
            _ => "❓"
        };
    }

    // ── Public debug accessors (for auto-test) ──

    public bool DebugStage1CardButtonExists => stage1CardButton != null;
    public bool DebugStage2CardButtonExists => stage2CardButton != null;
    public bool DebugStage3CardButtonExists => stage3CardButton != null;
    public bool DebugStage4CardButtonExists => stage4CardButton != null;
    public bool DebugStage5CardButtonExists => stage5CardButton != null;
    public bool DebugStage6CardButtonExists => stage6CardButton != null;
    public bool DebugStage1CardInteractable => stage1CardButton != null && stage1CardButton.interactable;
    public bool DebugStage2CardInteractable => stage2CardButton != null && stage2CardButton.interactable;
    public bool DebugStartBattleButtonExists => startBattleButton != null;
    public bool DebugStartBattleButtonInteractable => startBattleButton != null && startBattleButton.interactable;
    public bool DebugBackButtonExists => backButton != null;
    public bool DebugStageNameTextExists => stageNameText != null;
    public bool DebugStageDescriptionTextExists => stageDescriptionText != null;
    public string DebugStage1StatusText => stage1StatusText != null ? stage1StatusText.text : "";
    public string DebugStage2StatusText => stage2StatusText != null ? stage2StatusText.text : "";
    public string DebugStage3StatusText => stage3StatusText != null ? stage3StatusText.text : "";
    public string DebugStage4StatusText => stage4StatusText != null ? stage4StatusText.text : "";
    public string DebugStage5StatusText => stage5StatusText != null ? stage5StatusText.text : "";
    public string DebugStage6StatusText => stage6StatusText != null ? stage6StatusText.text : "";
    public int DebugSelectedStageIndex => selectedStageIndex;

    public void DebugSelectStage(int index)
    {
        if (index >= 0 && index < 6) OnStageCardClicked(index);
    }
}
