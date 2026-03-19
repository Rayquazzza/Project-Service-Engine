using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridService : IGridService
{
    // LOG TAG
    private const string LogTag = "<b><color=cyan>[Grid Service]</color></b>";

    // DICTIONARIES
    private readonly Dictionary<Vector2Int, Cell> cells = new Dictionary<Vector2Int, Cell>();

    // EVENTS
    public event Action<Vector2Int> OnGridInitialized;
    public event Action<List<IOccupant>, Vector2Int, Vector2Int, List<Vector2Int>, Action> OnOccupantMoved;
    public event Action<IOccupant, Vector2Int> OnOccupantPlaced;
    public event Action<IOccupant> OnOccupantRemoved;
    public event Action<IOccupant> OnOccupantSpawned;
    public event Action<IOccupant> OnOccupantDestroyed; 
    public event Action OnCombatFound;
    public event Action<List<IOccupant>, Cell, Action> OnCombatRequested;
    public event Action<Player> OnVitalZoneCaptured;

    // GRID PROPERTIES
    public int Width { get; private set; }
    public int Depth { get; private set; }

    // SERVICES VARIABLES
    private ISelectionService selectionService;
    private ICombatService combatService;
    private ITurnService turnService;
    private IPathfindingService pathfindingService;


    private bool hasMovedThisTurn;

    // ---------- SERVICES/CONSTRUCTOR METHODS ----------

    public GridService()
    {
        GameServiceLocator.Register<IGridService>(this);
    }

    public void Init()
    {
        combatService = GameServiceLocator.Get<ICombatService>();
        selectionService = GameServiceLocator.Get<ISelectionService>();
        turnService = GameServiceLocator.Get<ITurnService>();

        pathfindingService = GameServiceLocator.Get<IPathfindingService>();

        if (selectionService != null) selectionService.OnMoveRequest += HandleMoveRequest;
        if (turnService != null) turnService.OnTurnChanged += HandleTurnChanged;
    }

   

    public void Dispose()
    {
        if (selectionService != null) selectionService.OnMoveRequest -= HandleMoveRequest;
        GameServiceLocator.Unregister<IGridService>();
    }

    public void InitializeGrid(int width, int depth)
    {
        Width = width;
        Depth = depth;
        cells.Clear();

        for (int x = 0; x < width; x++)
            for (int z = 0; z < depth; z++)
                cells.Add(new Vector2Int(x, z), new Cell(x, z));

        OnGridInitialized?.Invoke(new Vector2Int(width, depth));
        Debug.Log($"{LogTag} Grid initialized: {width}x{depth}");
    }

    // ---------- EVENT HANDLER METHODS ----------

    private void HandleMoveRequest(IOccupant leader, Vector2Int target)
    {
        if (hasMovedThisTurn) return;

        Vector2Int startPos = leader.GridPosition;
        Cell sourceCell = GetCell(startPos);
        if (sourceCell == null || sourceCell.IsEmpty) return;

        var travelers = sourceCell.Occupants.Count <= 1 ? new List<IOccupant> { leader } : sourceCell.Occupants.OrderBy(u => (u as BaseUnit)?.UnitData.AttackPower ?? 0).Skip(1).ToList();

        List<Vector2Int> fullPath = pathfindingService?.FindPath(startPos, target);
        if (fullPath == null || fullPath.Count == 0) return;

        PathResult result = GetPathUntilEnemy(fullPath, leader);

        foreach (var unit in travelers) RemoveOccupant(startPos, unit);

        hasMovedThisTurn = true;

        OnOccupantMoved?.Invoke(travelers, startPos, result.MoveDestination, result.Path, () =>
        {
            if (result.AttackTriggered)
            {
                HandleCombat(travelers, GetCell(result.Path.Last()));
            }
            else
            {
                PlaceOccupants(result.MoveDestination, travelers);
            }
            Debug.Log($"{LogTag} Mouvement terminé à {result.MoveDestination}");
        });
    }

    private void HandleTurnChanged(Player player)
    {
        hasMovedThisTurn = false;
    }

    // ---------- GRID LOGIC METHODS ----------

    public void SpawnOccupant(Vector2Int coords, IOccupant unit)
    {
        if (PlaceOccupants(coords, new List<IOccupant> { unit })) OnOccupantSpawned?.Invoke(unit);
    }

    public bool PlaceOccupants(Vector2Int coords, List<IOccupant> movingUnits)
    {
        Cell cell = GetCell(coords);
        if (cell == null || movingUnits == null || movingUnits.Count == 0) return false;

        if (!cell.IsEmpty && cell.Occupants[0].OwnerId != movingUnits[0].OwnerId)
        {

            return HandleCombat(movingUnits, cell);
        }
           
        foreach (var unit in movingUnits) AddUnitToCell(cell, unit);
        cell.SetZoneOwner(movingUnits[0].OwnerId);

        return true;
    }

    private bool HandleCombat(List<IOccupant> attackers, Cell cell)
    {

        void ResolveCombatCallback()
        {
            var defenders = new List<IOccupant>(cell.Occupants);
            var combatService = GameServiceLocator.Get<ICombatService>();
            var winnerLeader = combatService.ResolveCombat(attackers, defenders);
            bool attackersWon = winnerLeader.OwnerId == attackers[0].OwnerId;

            foreach (var unit in attackers.Concat(defenders).ToList())
                if (unit is IDamageable d && d.IsDead)
                    RemoveDefinitiveOccupant(unit.GridPosition, unit);

            if (attackersWon)
            {
                var survivors = attackers.Where(a => !(a as IDamageable).IsDead).ToList();
                foreach (var atk in survivors) AddUnitToCell(cell, atk);
            }
        }

        OnCombatRequested?.Invoke(attackers, cell, ResolveCombatCallback);

        return true;
    }




    private void AddUnitToCell(Cell cell, IOccupant unit)
    {
        if (cell.Occupants.Contains(unit)) return;
        cell.AddOccupant(unit);

        var owner = unit.OwnerId;

        if (cell.IsVitalZone && cell.ZoneOwner != null && cell.ZoneOwner != owner)
        {
            OnVitalZoneCaptured?.Invoke(cell.ZoneOwner); // le joueur qui PERD
        }

        cell.SetZoneOwner(owner);
        unit.OnPlaced(cell.Coords);
        OnOccupantPlaced?.Invoke(unit, cell.Coords);
    }

    public void RemoveOccupant(Vector2Int coords, IOccupant occupant)
    {
        Cell cell = GetCell(coords);
        if (cell != null && occupant != null)
        {
            occupant.OnRemoved();
            cell.RemoveOccupant(occupant);
            Debug.Log($"{LogTag} Occupant removed from {coords}");
        }
    }

    private void RemoveDefinitiveOccupant(Vector2Int coords, IOccupant occupant)
    {
        RemoveOccupant(coords, occupant);
        OnOccupantRemoved?.Invoke(occupant);
    }

    // ---------- NAVIGATION & SEARCH METHODS ----------

    public Cell GetCell(Vector2Int coords) => cells.TryGetValue(coords, out Cell cell) ? cell : null;

    public bool IsWithinBounds(Vector2Int coords) => coords.x >= 0 && coords.x < Width && coords.y >= 0 && coords.y < Depth;

    public List<Cell> GetNeighbors(Vector2Int center)
    {
        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.right, Vector2Int.left };
        return directions.Select(dir => center + dir).Where(IsWithinBounds).Select(GetCell).ToList();
    }


    public List<Vector2Int> GetAvailableMoves(IOccupant unit)
    {
        var reachableTiles = new List<Vector2Int>();
        if (!(unit is BaseUnit baseUnit)) return reachableTiles;

        var nodesToVisit = new Queue<Vector2Int>();
        var distances = new Dictionary<Vector2Int, int> { { unit.GridPosition, 0 } };
        nodesToVisit.Enqueue(unit.GridPosition);

        while (nodesToVisit.Count > 0)
        {
            Vector2Int current = nodesToVisit.Dequeue();
            int currentDist = distances[current];

            if (currentDist >= baseUnit.MoveRange) continue;

            foreach (var neighbor in GetNeighbors(current))
            {
                if (distances.ContainsKey(neighbor.Coords)) continue;

                bool isEnemy = !neighbor.IsEmpty && neighbor.Occupants[0].OwnerId != unit.OwnerId;

                if (neighbor.IsEmpty)
                {
                    distances[neighbor.Coords] = currentDist + 1;
                    reachableTiles.Add(neighbor.Coords);
                    nodesToVisit.Enqueue(neighbor.Coords);
                }

                else if (isEnemy)
                {
                    distances[neighbor.Coords] = currentDist + 1;
                    reachableTiles.Add(neighbor.Coords);
                }
            }
        }
        return reachableTiles;
    }

    private PathResult GetPathUntilEnemy(List<Vector2Int> path, IOccupant mover)
    {
        for (int i = 0; i < path.Count; i++)
        {
            Cell cell = GetCell(path[i]);
            if (cell != null && !cell.IsEmpty && cell.Occupants[0].OwnerId != mover.OwnerId)
            {
                return new PathResult
                {
                    Path = path.Take(i + 1).ToList(),

                    MoveDestination = i > 0 ? path[i - 1] : mover.GridPosition,
                    AttackTriggered = true
                };
            }
        }

        // Pas d'ennemi, chemin normal
        return new PathResult
        {
            Path = path,
            MoveDestination = path.Last(),
            AttackTriggered = false
        };
    }

    // ---------- VITAL ZONES & UTILS ----------

    public void SetupVitalZones(Player p1, Player p2)
    {
        int minDistance = Mathf.Max(Width, Depth) / 2;
        Vector2Int posP1 = FindValidZonePosition();
        Vector2Int posP2 = FindValidZonePosition(posP1, minDistance);

        ApplyVitalZone(posP1, p1);
        ApplyVitalZone(posP2, p2);
    }

    private Vector2Int FindValidZonePosition(Vector2Int? otherZone = null, int minDistance = 0)
    {
        var candidates = cells.Keys.Where(c => c.x < Width * 0.3f || c.x > Width * 0.7f || c.y < Depth * 0.3f || c.y > Depth * 0.7f).Where(c => !otherZone.HasValue || Vector2Int.Distance(c, otherZone.Value) >= minDistance).ToList();

        if (candidates.Count == 0)
        {
            Debug.LogWarning($"{LogTag} Aucune position valide trouvée pour la zone vitale !");
            return Vector2Int.zero;
        }

        return candidates[UnityEngine.Random.Range(0, candidates.Count)];
    }

    private void ApplyVitalZone(Vector2Int coords, Player owner)
    {
        Cell cell = GetCell(coords);
        if (cell == null) return;
        cell.IsVitalZone = true;
        cell.SetZoneOwner(owner);
        Debug.Log($"{LogTag} Zone Vitale de {owner} placée en {coords}");
    }

    public IEnumerable<Cell> GetAllCells() => cells.Values;

    public bool HasPlayerMovedThisTurn() => hasMovedThisTurn;

}