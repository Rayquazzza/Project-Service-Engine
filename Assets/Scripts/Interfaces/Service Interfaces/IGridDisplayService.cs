
using System;
using UnityEngine;

public interface IGridDisplayService
{


    public event Action<Vector3, Vector2> OnVisualGridGenerated;
    void SpawnInitialUnit(Player player);

}
