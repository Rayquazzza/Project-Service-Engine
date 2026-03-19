using UnityEngine;

[CreateAssetMenu(fileName = "TankDataSO", menuName = "ScriptableObjects/UnitDataSO/TankDataSO", order = 3)]
public class TankDataSO : UnitDataSO
{
    public override BaseUnit CreateUnitInstance(Player player)
    {
        return new Tank(this, player);
    }
}
