using System;
using System.Collections.Generic;
using UnityEngine;

public interface ISelectionService
{
    public event Action<Cell> OnCellRightClicked;
    public event Action<IOccupant, List<Vector2Int>> OnSelectionUpdated;
    public event Action<IOccupant, Vector2Int> OnMoveRequest;
    List<Vector2Int> GetCurrentRange();
    void OnCellLeftClicked(Cell clickedCell);
    IOccupant SelectedOccupant { get; }


}
