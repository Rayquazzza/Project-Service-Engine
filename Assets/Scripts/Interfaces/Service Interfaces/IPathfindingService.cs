using System.Collections.Generic;
using UnityEngine;

public interface IPathfindingService : IDisposableService
{
    List<Vector2Int> FindPath(Vector2Int startPos, Vector2Int targetPos);
}
