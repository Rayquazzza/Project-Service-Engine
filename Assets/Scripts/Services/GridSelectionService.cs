using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GridInput))]
public class GridSelectionService : MonoBehaviour, ISelectionService
{
    private const string LogTag = "<b><color=#D956C0>[Selection Service]</color></b>";
    private List<Vector2Int> currentRange = new List<Vector2Int>();

    public event Action<IOccupant, List<Vector2Int>> OnSelectionUpdated;
    public event Action<IOccupant, Vector2Int> OnMoveRequest;
    public event Action<Cell> OnCellRightClicked;

    public IOccupant SelectedOccupant { get; private set; }

    private IGridService gridService;
    private ITurnService turnService;
    private GridInput gridInput;

    private void Awake()
    {
        GameServiceLocator.Register<ISelectionService>(this);
    }

    private void Start()
    {
        gridService = GameServiceLocator.Get<IGridService>();
        turnService = GameServiceLocator.Get<ITurnService>();

        gridInput = GetComponent<GridInput>();

        gridInput.OnCellLeftClicked += HandleLeftClick;
        gridInput.OnCellRightClicked += HandleRightClick;

       if (turnService != null) turnService.OnTurnChanged += HandleTurnChanged;
    }

    private void HandleTurnChanged(Player player)
    {
        ClearSelection();   
        OnSelectionUpdated?.Invoke(null, null);
    }

    private void HandleRightClick(CellView view)
    {
        if (!view.GetData().IsEmpty) OnCellRightClicked?.Invoke(view.GetData());
    }

    private void HandleLeftClick(CellView view)
    {
        Debug.Log($"{LogTag} Cell Left Clicked at {view.GetData().Coords}");
        OnCellLeftClicked(view.GetData());
    }

    public List<Vector2Int> GetCurrentRange()
    {
        return currentRange;
    }

    public void OnCellLeftClicked(Cell clickedCell)
    {
        IOccupant occupantInCase = !clickedCell.IsEmpty ? clickedCell.Occupants[0] : null;

        bool isEnemy = occupantInCase != null && occupantInCase.OwnerId != turnService.CurrentPlayer;
        bool hasAlreadyMoved = gridService.HasPlayerMovedThisTurn(); // ← extrait ici
        bool isPlayerUnit = occupantInCase != null && occupantInCase.OwnerId == turnService.CurrentPlayer && !hasAlreadyMoved;
        bool inRange = currentRange.Contains(clickedCell.Coords);

        if (SelectedOccupant != null && inRange && !hasAlreadyMoved) // ← ajout du check
        {
            if (clickedCell.IsEmpty || isEnemy)
            {
                Debug.Log($"{LogTag} Move/Combat Request at {clickedCell.Coords}");
                OnMoveRequest?.Invoke(SelectedOccupant, clickedCell.Coords);
                ClearSelection();
                OnSelectionUpdated?.Invoke(null, null);
                return;
            }
        }

        if (isPlayerUnit)
        {
            SelectedOccupant = occupantInCase;
            currentRange = gridService?.GetAvailableMoves(SelectedOccupant);
            OnSelectionUpdated?.Invoke(SelectedOccupant, currentRange);
        }
        else
        {
            ClearSelection();
            OnSelectionUpdated?.Invoke(null, null);
        }
    }

    private void ClearSelection()
    {
        SelectedOccupant = null;
        currentRange.Clear();
    }

    private void OnDestroy()
    {
        GameServiceLocator.Unregister<ISelectionService>();

        if (gridInput != null)
        {
            gridInput.OnCellLeftClicked -= HandleLeftClick;
            gridInput.OnCellRightClicked -= HandleRightClick;
        }
    }
}