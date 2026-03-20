using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;

public class GridDisplay : MonoBehaviour, IGridDisplayService
{
    //GRID SIZE
    private int width;
    private int height;

    [Header("REFERENCES")]
    [Min(1f)]
    [SerializeField] private float cellSpacing = 1f;
    [SerializeField] private GameObject cellPrefab;
    [SerializeField] private GameObject cellsParent;
    [SerializeField] private GameObject unitsParent;
    [SerializeField] private UnitDataSO scoutDataSO;

    //DICTIONARIES
    private Dictionary<Vector2Int, CellView> cellViews = new Dictionary<Vector2Int, CellView>();
    private Dictionary<IOccupant, UnitView> unitViews = new Dictionary<IOccupant, UnitView>();

    // SERVICES VARIABLES
    private IGridService gridService;
    private ISelectionService selectionService;
    private IFogOfWarService fogService;
    private IPoolingService poolingService;
    private IPathfindingService pathfindingService;

    //EVENTS
    public event Action<Vector3, Vector2> OnVisualGridGenerated;

    //---------- START ----------

    private void Awake()
    {
        GameServiceLocator.Register<IGridDisplayService>(this);
    }

    private void Start()
    {       
        if (cellPrefab == null || cellsParent == null || unitsParent == null || scoutDataSO == null)
        {
            Debug.LogError("GridDisplay: One or more references are not assigned in the inspector. Please assign them to avoid null reference errors.");
            return;
        }

        GetServices(); //Get Services we need     
        SubscribeToEvents(); 
    }

    private void GetServices()
    {
        gridService = GameServiceLocator.Get<IGridService>();
        fogService = GameServiceLocator.Get<IFogOfWarService>();
        selectionService = GameServiceLocator.Get<ISelectionService>();
        poolingService = GameServiceLocator.Get<IPoolingService>();
        pathfindingService = GameServiceLocator.Get<IPathfindingService>();
    }

    private void SubscribeToEvents()
    {
        if (gridService != null) gridService.OnGridInitialized += HandleGridInitialized;
        if (gridService != null) gridService.OnOccupantMoved += UpdateVisualPosition;
        if (gridService != null) gridService.OnOccupantPlaced += HandleOccupantPlaced;
        if(gridService != null) gridService.OnCombatRequested += HandleCombatRequested;
        if (fogService != null) fogService.OnFogOfWarUpdated += HandleFogUpdated;
        if (selectionService != null) selectionService.OnSelectionUpdated += HandleSelectionUpdated;
    }

    //-----------------------------------------------------------------

    //---------- METHODS EVENTS HANDLER ----------

    private void HandleCombatRequested(List<IOccupant> attackers, Cell targetCell, Action onComplete)
    {
        var unit = attackers[0];
        CellView destView = GetCellView(targetCell.Coords);
        Vector3 worldPos = destView != null ? destView.GetAnchorForUnit(unit).position : GetWorldPositionFromCoords(targetCell.Coords);

        if (unitViews.TryGetValue(unit, out UnitView view)) StartCoroutine(AnimateThenResolve(view, worldPos, onComplete));
        else onComplete?.Invoke();
    }
    private IEnumerator AnimateThenResolve(UnitView view, Vector3 target, Action onComplete)
    {
        yield return view.FollowPath(new List<Vector3> { target });
        onComplete?.Invoke(); 
    }

    private void HandleFogUpdated(List<Vector2Int> visibleCells, List<IOccupant> visibleUnits)
    {
        foreach (var kvp in cellViews)
        {
            kvp.Value.SetVisibility(visibleCells.Contains(kvp.Key));
        }

        foreach (var kvp in unitViews)
        {
            kvp.Value.SetVisible(visibleUnits.Contains(kvp.Key));
        }
    }

    private void HandleOccupantPlaced(IOccupant occupant, Vector2Int coords)
    {
        if (GetUnitView(occupant) != null) return;

        if (occupant is BaseUnit unit)
        {
            SpawnUnitPrefab(occupant, unit.UnitData);
        }
    }

    private void HandleSelectionUpdated(IOccupant unit, List<Vector2Int> range)
    {
        foreach (var view in cellViews.Values)
        {
            //Debug.Log("Resetting cell view at " + view.GetData().Coords);
            view.MarkAsRange(false);
        }

        if (range != null)
        {
            foreach (var pos in range)
            {
                //Debug.Log("Marking cell view at " + pos + " as in range");
                if (cellViews.TryGetValue(pos, out var view))
                {
                    view.MarkAsRange(true);
                }
            }
        }
    }

    private void HandleGridInitialized(Vector2Int gridSize)
    {
        GenerateVisualGrid(gridSize.x, gridSize.y);
    }

    private void HandleStackChanged(Vector2Int coords)
    {
        Cell cell = gridService.GetCell(coords);
        CellView cv = GetCellView(coords);

        foreach (var occupant in cell.Occupants)
        {
            UnitView uv = GetUnitView(occupant);
            Vector3 targetAnchor = cv.GetAnchorForUnit(occupant).position;
            StartCoroutine(uv.FollowPath(new List<Vector3> { targetAnchor }));
        }
    }

    //-----------------------------------------------------------------

    //---------- GRID AND UNITS VISUAL ----------

    private void GenerateVisualGrid(int width, int height)
    {
        foreach (Transform child in cellsParent.transform)
            poolingService.ReturnToPool(cellPrefab, child.gameObject);

        cellViews.Clear();

        this.width = width;
        this.height = height;

        
        
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                Vector3 position = GameUtils.GetWorldPosition(width, height, cellSpacing, x, z);
                position.y = -0.1f;
                GameObject cellVisual = poolingService.GetFromPool(cellPrefab, position, Quaternion.identity);

                cellVisual?.transform.SetParent(cellsParent.transform);

                Vector2Int coords = new Vector2Int(x, z);

                CellView view = cellVisual.GetComponentInChildren<CellView>();
                cellViews.Add(coords, view);

                Cell cell = gridService?.GetCell(new Vector2Int(x, z));
                view?.Initialize(cell);
            }
        }

        float worldWidth = (width - 1) * cellSpacing + cellSpacing; 
        float worldDepth = (height - 1) * cellSpacing + cellSpacing;
        Vector3 center = Vector3.zero;

        OnVisualGridGenerated?.Invoke(center, new Vector2(worldWidth, worldDepth));

    }

    private void UpdateVisualPosition(List<IOccupant> units, Vector2Int from, Vector2Int to, List<Vector2Int> gridPath, Action onComplete)
    {
        if (gridPath == null || gridPath.Count == 0) { onComplete?.Invoke(); return; }

        StartCoroutine(AnimateGroupMovement(units, to, gridPath, onComplete));
    }

    private IEnumerator AnimateGroupMovement(List<IOccupant> units, Vector2Int to, List<Vector2Int> gridPath, Action onComplete)
    {
        List<Coroutine> activeMovements = new List<Coroutine>();
        CellView destinationView = GetCellView(to);

        for (int i = 0; i < units.Count; i++)
        {
            IOccupant unit = units[i];

            if (unitViews.TryGetValue(unit, out UnitView view))
            {
                List<Vector3> unitWorldPath = new List<Vector3>();

                for (int j = 0; j < gridPath.Count - 1; j++)
                {
                    unitWorldPath.Add(GetWorldPositionFromCoords(gridPath[j]));
                }

                Vector3 finalPos;
                if (destinationView != null)
                {
                    finalPos = destinationView.GetAnchorAtIndex(i).position;
                }
                else
                {
                    finalPos = GetWorldPositionFromCoords(to);
                }

                unitWorldPath.Add(finalPos);
                activeMovements.Add(StartCoroutine(view.FollowPath(unitWorldPath)));
            }
        }

        foreach (var routine in activeMovements) yield return routine;

        onComplete?.Invoke();
    }
    public void SpawnInitialUnit(Player player)
    {
        foreach (var cell in gridService.GetAllCells())
        {
            if (cell.IsVitalZone && cell.ZoneOwner == player)
            {
                IOccupant scout = new Scout(scoutDataSO, player);
                gridService.SpawnOccupant(cell.Coords, scout);
                break;
            }
        }
    }

    private void MoveUnitVisual(IOccupant occupant, List<Vector3> path)
    {
        if (unitViews.TryGetValue(occupant, out UnitView view))
        {
            StartCoroutine(view.FollowPath(path));
        }
    }    

    private void SpawnUnitPrefab(IOccupant occupant, UnitDataSO data)
    {
        CellView cellView = GetCellView(occupant.GridPosition);

        Vector3 worldPos = cellView != null ? cellView.GetAnchorForUnit(occupant).position : GetWorldPositionFromCoords(occupant.GridPosition);

        GameObject unitGO = poolingService?.GetFromPool(data.Prefab, worldPos, Quaternion.identity);
        unitGO.transform.SetParent(unitsParent.transform);

        UnitView view = unitGO.GetComponent<UnitView>();
        RegisterUnitView(occupant, view);
    }

    //-----------------------------------------------------------------

    //---------- UTILS METHODS ----------

    private CellView GetCellView(Vector2Int coords)
    {
        return cellViews.GetValueOrDefault(coords);
    }

    private Vector3 GetWorldPositionFromCoords(Vector2Int coords)
    {
        return GameUtils.GetWorldPosition(width, height, cellSpacing, coords.x, coords.y);
    }

    private UnitView GetUnitView(IOccupant occupant)
    {
        if (occupant == null) return null;

        if (unitViews.TryGetValue(occupant, out UnitView view))
        {
            return view;
        }

        return null;
    }

    private void RegisterUnitView(IOccupant occupant, UnitView view)
    {
        unitViews[occupant] = view;
    }

    //-----------------------------------------------------------------


    //---------- ON DESTROY ----------

    private void OnDestroy()
    {
        UnSubscribeToEvents();

        GameServiceLocator.Unregister<IGridDisplayService>();
    }

    private void UnSubscribeToEvents()
    {
        if (gridService != null) gridService.OnGridInitialized -= HandleGridInitialized;
        if (gridService != null) gridService.OnOccupantMoved -= UpdateVisualPosition;
        if (gridService != null) gridService.OnOccupantPlaced -= HandleOccupantPlaced;
        if (gridService != null) gridService.OnCombatRequested -= HandleCombatRequested;
        if (fogService != null) fogService.OnFogOfWarUpdated -= HandleFogUpdated;
        if (selectionService != null) selectionService.OnSelectionUpdated -= HandleSelectionUpdated;     
    }

   

    //-----------------------------------------------------------------
}