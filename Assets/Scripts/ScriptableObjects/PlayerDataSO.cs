using UnityEngine;

[CreateAssetMenu(fileName = "PlayerDataSO", menuName = "ScriptableObjects/PlayerDataSO", order = 2)]
public class PlayerDataSO : ScriptableObject
{
    public string playerName;
    [Min(0)]public int Money;

    public Color PlayerColor;
}
