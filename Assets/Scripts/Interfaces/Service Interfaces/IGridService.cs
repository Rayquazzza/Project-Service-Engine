using System;
using System.Collections.Generic;
using UnityEngine;

public interface IGridService : IDisposableService
{
    // EVENTS
    public event Action<Vector2Int> OnGridInitialized;
    public event Action<List<IOccupant>, Vector2Int, Vector2Int, List<Vector2Int>, Action> OnOccupantMoved;
    public event Action<IOccupant, Vector2Int> OnOccupantPlaced;
    public event Action<IOccupant> OnOccupantRemoved;
    public event Action<IOccupant> OnOccupantSpawned;
    public event Action<IOccupant> OnOccupantDestroyed;
    public event Action<List<IOccupant>, Cell, Action> OnCombatRequested;
    public event Action<Player> OnVitalZoneCaptured;

    void InitializeGrid(int width, int depth);

    // Cell management
    Cell GetCell(Vector2Int coords);
    IEnumerable<Cell> GetAllCells();


    bool IsWithinBounds(Vector2Int coords);

    // Unit/Occupant management

    void SpawnOccupant(Vector2Int coords,IOccupant occupant);
    bool PlaceOccupants(Vector2Int coords, List<IOccupant> occupants);
    void RemoveOccupant(Vector2Int coords, IOccupant occupant);
    List<Cell> GetNeighbors(Vector2Int coords);
    List<Vector2Int> GetAvailableMoves(IOccupant unit);
    void SetupVitalZones(Player p1, Player p2); 
    public bool HasPlayerMovedThisTurn();
}