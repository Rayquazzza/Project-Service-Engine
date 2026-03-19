using UnityEngine;

[CreateAssetMenu(fileName = "EconomySettings", menuName = "Settings/EconomySettings")]
public class EconomySettingsSO : ScriptableObject
{
    [Header("Base Rules")]
    public int BaseTurnIncome = 10;
    [Header("Cell Modifiers")]
    public int VitalZoneMultiplier = 5;
    public FloatRange CellMultiplierRange; // min/max pour les cases normales
    [Header("Vital Zone Proximity")]
    [Tooltip("Les cases dans ce rayon autour d'une vital zone ont leur multiplicateur réduit")]
    public int ProximityRadius = 3;
    [Tooltip("Réduction appliquée par case de proximité (ex: 0.15 = -15% par case)")]
    public float ProximityPenaltyPerTile = 0.15f;
}