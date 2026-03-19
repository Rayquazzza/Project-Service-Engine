using System;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "GridDataSO", menuName = "ScriptableObjects/GridDataSO", order = 1)]
public class GridDataSO : ScriptableObject
{
    public GameObject CellPrefab;
    [Min(5)] public int GridWidth;
    [Min(5)] public int GridHeight;
    [Min(1f)] public float CellSpacing;   

}
