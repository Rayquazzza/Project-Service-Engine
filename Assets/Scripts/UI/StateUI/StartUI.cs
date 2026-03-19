using UnityEngine;

public class StartUI : BaseStateUIPanel
{

    public void StartGame()
    {
        gameStateService?.ChangeGameState(E_GameState.STARTING);
        Canvas.ForceUpdateCanvases();
    }

    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}