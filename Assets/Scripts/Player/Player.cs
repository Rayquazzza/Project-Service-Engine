using System;
using UnityEngine;

public class Player
{
    public PlayerDataSO Data { get; private set; }
    public Camera PlayerCamera { get; private set; }
    
    private int currentMoney;
    public int CurrentMoney 
    { 
        get => currentMoney;
        set
        {
            currentMoney = value;
            OnMoneyChanged?.Invoke(currentMoney);
        }
    }

    public event Action<int> OnMoneyChanged;

    public Player(PlayerDataSO data, Camera camera)
    {
        Data = data;
        PlayerCamera = camera;
        currentMoney = data.Money;
    }
}