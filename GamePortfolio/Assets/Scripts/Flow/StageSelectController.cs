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
    [SerializeField] private Image stage1CardBg;
    [SerializeField] private Image stage2CardBg;
    [SerializeField] private TMP_Text stage2StatusText;

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
    private bool stage1Unlocked = true;
    private bool stage2Unlocked = false;

    // Stage metadata for descriptions
    private static readonly string[] StageNames = {
        "Slime Scout Route",
        "Wolf Ambush"
    };

    private static readonly string[] StageDescriptions = {
        "A basic encounter against slimes.\nLearn the combat basics: Attack, Guard, Fire Skill, and Break.\nDefeat the Slime Scout to advance.",
        "Wolf packs hunt in the moonlit clearing.\nRequires completing Slime Scout Route first.\nBeware of coordinated attacks."
    };

    private static readonly string[] StageStageNames = {
        "Stage 1-1: Slime Scout",
        "Stage 1-2: Wolf Ambush"
    };

    /// <summary>Selected stage index (0-based) for BattleScene to read.</summary>
    public static int SelectedStageIndex { get; private set; } = -1;

    private void Start()
    {
        // Wire buttons
        if (stage1CardButton != null)
            stage1CardButton.onClick.AddListener(() => OnStageCardClicked(0));
        if (stage2CardButton != null)
            stage2CardButton.onClick.AddListener(() => OnStageCardClicked(1));
        if (backButton != null)
            backButton.onClick.AddListener(OnBackClicked);
        if (startBattleButton != null)
            startBattleButton.onClick.AddListener(OnStartBattleClicked);

        // Initialize UI state
        UpdateCardVisuals();
    }

    private void OnStageCardClicked(int index)
    {
        if (index == 0 && !stage1Unlocked) return;
        if (index == 1 && !stage2Unlocked) return;

        selectedStageIndex = index;
        UpdateCardVisuals();
        UpdateDescription(index);
        UpdateStartBattleButton();
    }

    private void OnStartBattleClicked()
    {
        if (selectedStageIndex < 0) return;
        if (selectedStageIndex == 0 && !stage1Unlocked) return;
        if (selectedStageIndex == 1 && !stage2Unlocked) return;

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
        // Stage 1 — selectable
        if (stage1CardButton != null)
        {
            stage1CardButton.interactable = stage1Unlocked;
            SetCardState(stage1CardBg, stage1CardButton.interactable, stage1Unlocked);
        }

        // Stage 2 — locked
        if (stage2CardButton != null)
        {
            stage2CardButton.interactable = stage2Unlocked;
            SetCardState(stage2CardBg, stage2CardButton.interactable, stage2Unlocked);
        }
        if (stage2StatusText != null)
            stage2StatusText.text = stage2Unlocked ? "Available" : "Locked";
    }

    private void SetCardState(Image bg, bool canInteract, bool isUnlocked)
    {
        if (bg == null) return;

        if (!isUnlocked)
            bg.color = lockedColor;
        else if (canInteract && selectedStageIndex >= 0 && IsCardForStage(bg, selectedStageIndex))
            bg.color = selectedColor;
        else
            bg.color = normalColor;
    }

    private bool IsCardForStage(Image cardBg, int stageIndex)
    {
        // Match by name convention
        if (cardBg == null) return false;
        return stageIndex == 0 && cardBg.name.Contains("Stage Card 1") ||
               stageIndex == 1 && cardBg.name.Contains("Stage Card 2");
    }

    private void UpdateDescription(int index)
    {
        if (index < 0 || index >= StageNames.Length) return;
        if (stageNameText != null)
            stageNameText.text = StageStageNames[index];
        if (stageDescriptionText != null)
            stageDescriptionText.text = StageDescriptions[index];
    }

    private void UpdateStartBattleButton()
    {
        if (startBattleButton == null) return;
        bool canStart = selectedStageIndex >= 0 &&
                        (selectedStageIndex == 0 ? stage1Unlocked : stage2Unlocked);
        startBattleButton.interactable = canStart;
    }

    // ── Public debug accessors (for auto-test) ──

    public bool DebugStage1CardButtonExists => stage1CardButton != null;
    public bool DebugStage2CardButtonExists => stage2CardButton != null;
    public bool DebugStage1CardInteractable => stage1CardButton != null && stage1CardButton.interactable;
    public bool DebugStage2CardInteractable => stage2CardButton != null && stage2CardButton.interactable;
    public bool DebugStartBattleButtonExists => startBattleButton != null;
    public bool DebugStartBattleButtonInteractable => startBattleButton != null && startBattleButton.interactable;
    public bool DebugBackButtonExists => backButton != null;
    public bool DebugStageNameTextExists => stageNameText != null;
    public bool DebugStageDescriptionTextExists => stageDescriptionText != null;
    public string DebugStage2StatusText => stage2StatusText != null ? stage2StatusText.text : "";
    public int DebugSelectedStageIndex => selectedStageIndex;

    public void DebugSelectStage(int index)
    {
        if (index == 0) OnStageCardClicked(0);
        else if (index == 1) OnStageCardClicked(1);
    }
}
