using UnityEngine;

public interface IOccupant
{

    Player OwnerId { get; }

    Vector2Int GridPosition { get; set; }

    void OnPlaced(Vector2Int position);
    void OnRemoved();
}
