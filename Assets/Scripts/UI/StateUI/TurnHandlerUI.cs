using UnityEngine;

public class TurnHandlerUI : BaseStateUIPanel
{
    public void NextTurnButton()
    {
        if(gameStateService.GetCurrentGameState() == E_GameState.IN_GAME)GameServiceLocator.Get<ITurnService>().NextTurn();
    }
}
