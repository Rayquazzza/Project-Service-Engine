using System.Collections.Generic;
using UnityEngine;

public class CellView : MonoBehaviour
{
    private Cell data;

    [Header("References")]
    [SerializeField] private List<Transform> unitAnchors;
    [SerializeField] private MeshRenderer meshRenderer;

    [Space(10)]
    [Header("Cell Colors")]
    [SerializeField] private Color defaultColor = Color.white;
    [SerializeField] private Color highlightColor = Color.blue;
    [SerializeField] private Color rangeColor = Color.yellow;

    private bool isHovered = false;
    private bool isInSelectionRange = false;
    private bool isVisibleToCurrentPlayer = true;

    public void Initialize(Cell cellData)
    {
        data = cellData;
        gameObject.name = $"Cell_{data.Coords.x}_{data.Coords.y}";

        data.OnCellDataChanged += UpdateVisuals;

        UpdateVisuals();
    }

    private E_CellVisualState GetCurrentState()
    {
        if (!isVisibleToCurrentPlayer) return E_CellVisualState.Hidden;
        if (data.ZoneOwner != null) return E_CellVisualState.Owned;
        if (isHovered) return E_CellVisualState.Hovered;
        if (isInSelectionRange) return E_CellVisualState.InRange;
        return E_CellVisualState.Default;
    }

    private void UpdateVisuals()
    {
        if (meshRenderer == null) return;
        meshRenderer.material.color = GetCurrentState() switch
        {
            E_CellVisualState.Hovered => highlightColor,
            E_CellVisualState.Hidden => defaultColor,
            E_CellVisualState.Owned => data.ZoneOwner.Data.PlayerColor,
            E_CellVisualState.InRange => rangeColor,
            _ => defaultColor
        };
    }

    public void Highlight(bool active)
    {
        isHovered = active;
        UpdateVisuals();
    }

    public void MarkAsRange(bool isInRange)
    {
        isInSelectionRange = isInRange;
        UpdateVisuals();
    }

    public void SetVisibility(bool isVisible)
    {
        isVisibleToCurrentPlayer = isVisible;
        UpdateVisuals();
    }

    public void ResetColor()
    {
        isHovered = false;
        isInSelectionRange = false;
        UpdateVisuals();
    }

    public Transform GetAnchorForUnit(IOccupant unit)
    {
        if (data == null || unitAnchors == null || unitAnchors.Count == 0) return transform;
        int index = data.Occupants.IndexOf(unit);
        return GetAnchorAtIndex(index == -1 ? 0 : index);
    }

    public Transform GetAnchorAtIndex(int index)
    {
        if (unitAnchors == null || unitAnchors.Count == 0) return transform;
        int safeIndex = Mathf.Clamp(index, 0, unitAnchors.Count - 1);
        return unitAnchors[safeIndex];
    }

    public Cell GetData()
    {
        return data;
    }

    private void OnDestroy()
    {
        if (data != null) data.OnCellDataChanged -= UpdateVisuals;
    }
}