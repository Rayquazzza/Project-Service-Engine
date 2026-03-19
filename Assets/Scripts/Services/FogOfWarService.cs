using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class FogOfWarService : IFogOfWarService
{
    private const string LogTag = "<b><color=#ADE247>[FogOfWar Service]</color></b>";
    private ITurnService turnService;
    private IGridService gridService;

    public FogOfWarService()
    {
        GameServiceLocator.Register<IFogOfWarService>(this);
    }

    public event Action<List<Vector2Int>, List<IOccupant>> OnFogOfWarUpdated;

    public void Dispose()
    {
        GameServiceLocator.Unregister<IFogOfWarService>();
    }

    public void Init()
    {
        turnService = GameServiceLocator.Get<ITurnService>();
        gridService = GameServiceLocator.Get<IGridService>();

        if (turnService != null)
        {
            turnService.OnTurnChanged += UpdateVisibility;
        }
    }

    public void UpdateVisibility(Player currentPlayer)
    {
        gridService = GameServiceLocator.Get<IGridService>();

        List<Vector2Int> visibleCells = new List<Vector2Int>();
        List<IOccupant> visibleUnits = new List<IOccupant>();

        foreach (var cell in gridService.GetAllCells())
        {
            bool isPlayerZone = cell.ZoneOwner == currentPlayer;
            bool hasPlayerUnit = !cell.IsEmpty && cell.Occupants[0].OwnerId == currentPlayer;
            bool hasEnemyUnit = !cell.IsEmpty && cell.Occupants[0].OwnerId != currentPlayer;

            bool isCellVisible = !hasEnemyUnit && (!cell.IsVitalZone || isPlayerZone);

            if (isCellVisible)
            {
                visibleCells.Add(cell.Coords);
            }

            if (!cell.IsEmpty)
            {
                List<IOccupant> units = cell.Occupants;

                foreach (var unit in units)
                {
                    bool isUnitVisible = (unit.OwnerId == currentPlayer);

                    if (isUnitVisible)
                    {
                        visibleUnits.Add(unit);
                    }
                }

            }
        }

        Debug.Log($"{LogTag} Fog Of War Updating");


        OnFogOfWarUpdated?.Invoke(visibleCells, visibleUnits);
    }

}
