using System;
using System.Collections.Generic;
using UnityEngine;

public class Cell
{
    public const int MAX_OCCUPANTS = 9;
    public Vector2Int Coords { get; private set; }
    public List<IOccupant> Occupants { get; private set; } = new List<IOccupant>();

    public IOccupant PrimaryOccupant => Occupants.Count > 0 ? Occupants[0] : null;
    public bool IsEmpty => Occupants.Count > 0 ? false : true;

    public bool IsFull => Occupants.Count >= MAX_OCCUPANTS;

    public event Action OnCellDataChanged;
    private bool isVitalZone;
    public bool IsVitalZone
    {
        get => isVitalZone;
        set
        {
            isVitalZone = value;
            OnCellDataChanged?.Invoke(); 
        }
    }

    public Player ZoneOwner { get; private set; }

    public float ResourceMultiplier { get; set; } = 1f;

    public int gCost;
    public int hCost;
    public Cell parent;
    public int fCost => gCost + hCost;

    public Cell(int x, int z)
    {
        Coords = new Vector2Int(x, z);
    }

    public void AddOccupant(IOccupant occupant)
    {
        if (occupant == null) return; 

        if (!Occupants.Contains(occupant))
        {
            Occupants.Add(occupant);
        }
    }

    public void RemoveOccupant(IOccupant occupant)
    {
        if (occupant == null) return;

        if (Occupants.Contains(occupant))
        {
            Occupants.Remove(occupant);

            if (Occupants.Count == 0 && !IsVitalZone)
            {
                ZoneOwner = null;
            }

            OnCellDataChanged?.Invoke();
        }
    }

    public void SetZoneOwner(Player player)
    {
        ZoneOwner = player;
        OnCellDataChanged?.Invoke();
    }
}