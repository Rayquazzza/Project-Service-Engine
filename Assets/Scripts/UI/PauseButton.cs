using UnityEngine;
using UnityEngine.UI;

public class PauseButton : MonoBehaviour
{

    [SerializeField] Button pauseButton;
    private IGameStateService gameStateService;

    private void Start()
    {
        gameStateService = GameServiceLocator.Get<IGameStateService>();
        pauseButton.onClick.AddListener(OnPauseClick);
    }
    public void OnPauseClick()
    {
        if (gameStateService != null)
        {
            gameStateService.ChangeGameState(E_GameState.PAUSED);
        }
    }
}
