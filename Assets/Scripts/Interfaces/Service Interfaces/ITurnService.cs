using System;
using UnityEngine;

public interface ITurnService : IDisposableService
{
    event Action<Player> OnTurnChanged;
    Player CurrentPlayer { get; }
    void NextTurn();
    void RegisterPlayer(Player player);
}
