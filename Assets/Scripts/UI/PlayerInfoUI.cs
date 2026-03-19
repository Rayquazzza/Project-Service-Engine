using UnityEngine;
using TMPro;
using System;

public class PlayerInfoUI : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private TextMeshProUGUI MoneyText;
    private Player player;
    public void Setup(Player player)
    {
        this.player = player;
        player.OnMoneyChanged += UpdateMoneyText;
        if( playerNameText) playerNameText.text = $"Player : {player.Data.playerName}";
    }

    private void UpdateMoneyText(int money)
    {
       if( MoneyText) MoneyText.text = $"Money: {money}";
    }

    private void OnDestroy()
    {
        player.OnMoneyChanged -= UpdateMoneyText;
    }
}
