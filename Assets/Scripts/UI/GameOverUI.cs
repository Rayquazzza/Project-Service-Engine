using UnityEngine;

public class GameOverUI : BaseStateUIPanel
{
    protected override void Start()
    {
        base.Start();
    }

    public void Restart()
    {
        gameStateService.ChangeGameState(E_GameState.STARTING);
    }

    public void ExitToMainMenu()
    {
        gameStateService.ChangeGameState(E_GameState.MAIN_MENU);
    }
}
