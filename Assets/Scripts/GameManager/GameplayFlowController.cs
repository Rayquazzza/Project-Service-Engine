using UnityEngine;

public class GameplayFlowController : MonoBehaviour
{
    private Vector2Int _pendingSpawnPos;

    private void Start()
    {
        var selection = GameServiceLocator.Get<ISelectionService>();
        if (selection != null)
        {
            selection.OnCellRightClicked += HandleRightClickRequest;
        }
    }

    private void HandleRightClickRequest(Cell clickedCell)
    {
        if (clickedCell.Occupants.Count < 9)
        {
            _pendingSpawnPos = clickedCell.Coords;

            GameServiceLocator.Get<IGameStateService>().ChangeGameState(E_GameState.RECRUITMENT);
        }
    }

    public Vector2Int GetCurrentSpawnPos() => _pendingSpawnPos;
}