using System.Collections.Generic;
using UnityEngine;

public class PathfindingService : IPathfindingService
{
    private IGridService gridService;

    public PathfindingService()
    {        
        GameServiceLocator.Register<IPathfindingService>(this);
        gridService = GameServiceLocator.Get<IGridService>();
    }

    public List<Vector2Int> FindPath(Vector2Int startPos, Vector2Int targetPos)
    {
        Cell startNode = gridService.GetCell(startPos);
        Cell targetNode = gridService.GetCell(targetPos);

        if (startNode == null || targetNode == null) return null;

        List<Cell> openSet = new List<Cell>();
        HashSet<Cell> closedSet = new HashSet<Cell>();
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Cell currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost || (openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost))
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                return RetracePath(startNode, targetNode);
            }

            foreach (Cell neighbour in gridService.GetNeighbors(currentNode.Coords))
            {
                if (!neighbour.IsEmpty && neighbour != targetNode || closedSet.Contains(neighbour))
                    continue;

                int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newMovementCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = currentNode;

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }
            }
        }
        return null;
    }

    private List<Vector2Int> RetracePath(Cell startNode, Cell endNode)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        Cell currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode.Coords);
            currentNode = currentNode.parent;
        }
        path.Reverse();
        return path;
    }

    private int GetDistance(Cell nodeA, Cell nodeB)
    {
        int dstX = Mathf.Abs(nodeA.Coords.x - nodeB.Coords.x);
        int dstY = Mathf.Abs(nodeA.Coords.y - nodeB.Coords.y);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }

    public void Dispose()
    {
        GameServiceLocator.Unregister<IPathfindingService>();
    }

    public void Init()
    {
        
    }
}