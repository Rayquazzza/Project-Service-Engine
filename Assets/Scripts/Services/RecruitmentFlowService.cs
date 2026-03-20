using System;
using UnityEngine;

public class RecruitmentFlowService : IRecruitmentFlowService
{
    private ISelectionService selectionService;
    public Vector2Int PendingSpawnPos { get; private set; }

    public RecruitmentFlowService()
    {
        GameServiceLocator.Register<IRecruitmentFlowService>(this);
    }
  
    public void Init()
    {
        selectionService = GameServiceLocator.Get<ISelectionService>();
        if (selectionService != null)
            selectionService.OnCellRightClicked += HandleRightClickRequest;
    }

    private void HandleRightClickRequest(Cell clickedCell)
    {
        if (clickedCell.Occupants.Count < 9 || clickedCell.IsVitalZone)
        {
            PendingSpawnPos = clickedCell.Coords;
            GameServiceLocator.Get<IGameStateService>().ChangeGameState(E_GameState.RECRUITMENT);
        }
    }

    public void Dispose()
    {
        if (selectionService != null)
            selectionService.OnCellRightClicked -= HandleRightClickRequest;

        GameServiceLocator.Unregister<IRecruitmentFlowService>();
    }
}
