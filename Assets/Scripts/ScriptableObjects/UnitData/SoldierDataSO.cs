using UnityEngine;

[CreateAssetMenu(fileName = "SoldierDataSO", menuName = "ScriptableObjects/UnitDataSO/SoldierDataSO", order = 2)]
public class SoldierDataSO : UnitDataSO
{
    public override BaseUnit CreateUnitInstance(Player player)
    {
        return new Soldier(this, player);
    }
}
