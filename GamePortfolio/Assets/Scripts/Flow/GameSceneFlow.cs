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

    public void LoadTitle()
    {
        SceneManager.LoadScene(TitleSceneName);
    }

    public void LoadStageSelect()
    {
        SceneManager.LoadScene(StageSelectSceneName);
    }

    public void LoadBattle()
    {
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
