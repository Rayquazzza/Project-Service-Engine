using UnityEngine;

public class TurnHandlerUI : BaseStateUIPanel
{
    public void NextTurnButton()
    {
        GameServiceLocator.Get<ITurnService>().NextTurn();
    }
}
