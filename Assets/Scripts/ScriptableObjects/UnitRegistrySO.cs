using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitRegistry", menuName = "Configs/UnitRegistry")]
public class UnitRegistrySO : ScriptableObject
{
    public List<UnitDataSO> AllUnits;
}