using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages scene transitions for the Title → Stage Select → Battle flow.
/// Attach to a persistent GameObject or to each scene's controller.
/// </summary>
public class GameSceneFlow : MonoBehaviour
{
    public const string TitleSceneName = "TitleScene";
    public const string StageSelectSceneName = "StageSelectScene";
    public const string BattleSceneName = "BattleScene";

    private void Awake()
    {
        SaveManager.Load();
    }

    public void LoadTitle()
    {
        if (ScreenFade.Instance != null)
            ScreenFade.Instance.TransitionToScene(TitleSceneName);
        else
            SceneManager.LoadScene(TitleSceneName);
    }

    public void LoadStageSelect()
    {
        if (ScreenFade.Instance != null)
            ScreenFade.Instance.TransitionToScene(StageSelectSceneName);
        else
            SceneManager.LoadScene(StageSelectSceneName);
    }

    public void LoadBattle()
    {
        if (ScreenFade.Instance != null)
            ScreenFade.Instance.TransitionToScene(BattleSceneName);
        else
            SceneManager.LoadScene(BattleSceneName);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
