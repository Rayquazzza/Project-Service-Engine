using UnityEngine;

public class TurnHandlerUI : MonoBehaviour
{
    public void NextTurnButton()
    {
        GameServiceLocator.Get<ITurnService>().NextTurn();
    }
}
