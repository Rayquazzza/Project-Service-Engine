using System.Collections.Generic;
using UnityEngine;

public struct PathResult
{
    public List<Vector2Int> Path;
    public Vector2Int MoveDestination; 
    public bool AttackTriggered;      
}