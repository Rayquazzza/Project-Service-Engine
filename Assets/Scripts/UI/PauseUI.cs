using UnityEngine;

public class PauseUI : BaseStateUIPanel
{
    public void OnResumeClick()
    {
        if (gameStateService != null)
        {
            gameStateService.ChangeGameState(E_GameState.IN_GAME);
        }
    }

    public void OnQuitClick()
    {
        Application.Quit();
    }
}
