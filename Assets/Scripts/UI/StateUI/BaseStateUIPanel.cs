using UnityEngine;

public abstract class BaseStateUIPanel : MonoBehaviour
{

    private const string LogTag = "<b><color=#E36B46>[BaseUI Panel]</color></b>";
    [SerializeField] protected GameObject root;
    [SerializeField] protected E_GameState activeState;

    protected IGameStateService gameStateService;
    protected virtual void Start()
    {
        gameStateService = GameServiceLocator.Get<IGameStateService>();
        if (gameStateService != null)
            gameStateService.OnGameStateChanged += HandleStateChanged;
    }

    protected virtual void OnDestroy()
    {
        if (gameStateService != null)
            gameStateService.OnGameStateChanged -= HandleStateChanged;
    }

    private void HandleStateChanged(E_GameState newState)
    {
        if (root != null)
        {
            root.SetActive(newState == activeState);
            Debug.Log($"{LogTag} State changed to {newState}, panel {GetType()} {(newState == activeState ? "activated" : "deactivated")}");
        }
            

        OnStateChanged(newState);
    }

    protected virtual void OnStateChanged(E_GameState newState) { }
}