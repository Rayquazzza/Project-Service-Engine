using UnityEngine;

[CreateAssetMenu(fileName = "EconomySettings", menuName = "Settings/EconomySettings")]
public class EconomySettingsSO : ScriptableObject
{
    [Header("Base Rules")]
    public int BaseTurnIncome = 10;
    [Header("Cell Modifiers")]
    public int VitalZoneMultiplier = 5;
    public int VitalZoneCellMultiplier = 5; 
    public int ProximityMultiplier = 2;   
    public FloatRange CellMultiplierRange;
}