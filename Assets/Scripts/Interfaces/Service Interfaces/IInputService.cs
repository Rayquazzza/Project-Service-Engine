using System;

public interface IInputService
{
    event Action<CellView> OnCellHoverChanged;
    event Action<CellView> OnCellLeftClicked;
    event Action<CellView> OnCellRightClicked;
}