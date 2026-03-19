using NUnit.Framework;
using System;
using UnityEngine;
using System.Collections.Generic;

public interface IFogOfWarService : IDisposableService
{
    public event Action<List<Vector2Int>,List<IOccupant>> OnFogOfWarUpdated;
    void UpdateVisibility(Player currentPlayer);
}
