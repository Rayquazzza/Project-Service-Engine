using System;
using UnityEngine;

public interface IPlayersService : IDisposableService
{

    public event Action<Player> OnPlayerEliminated;
    void RegisterPlayer(PlayerDataSO data, Camera camera);
    public Camera GetCurrentPlayerCamera();
    PlayerDataSO GetPlayerById(int id);
    int GetIdByPlayer(PlayerDataSO data);
    Player CurrentPlayer { get; }
    void NextTurn();
}