using UnityEngine;

[CreateAssetMenu(fileName = "ScoutDataSO", menuName = "ScriptableObjects/UnitDataSO/ScoutDataSO", order = 1)]
public class ScoutDataSO : UnitDataSO
{
    public override BaseUnit CreateUnitInstance(Player player)
    {
        return new Scout(this, player);
    }
}
