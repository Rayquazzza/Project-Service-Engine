using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitData", menuName = "ScriptableObjects/UnitDataSO", order = 3)]
public abstract class UnitDataSO : ScriptableObject
{
    public string UnitName;
    public int Cost;
    public int MaxHealth;
    public int MoveRange;
    public int AttackPower;
    public GameObject Prefab;

    public abstract BaseUnit CreateUnitInstance(Player player);
   
}
