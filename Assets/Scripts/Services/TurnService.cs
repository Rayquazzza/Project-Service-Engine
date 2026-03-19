using System;
using System.Collections.Generic;
using UnityEngine;

public class TurnService : ITurnService
{
    private const string LogTag = "<b><color=orange>[Turn Service]</color></b>";
    public event Action<Player> OnTurnChanged;
    private List<Player> players = new List<Player>();
    private int currentIndex = 0;
    public Player CurrentPlayer => players.Count > 0 ? players[currentIndex] : null;

    public void RegisterPlayer(Player playerInstance)
    {
        players.Add(playerInstance);
        Debug.Log($"{LogTag} Player registered: {playerInstance.Data.name}");
    }

    public void NextTurn()
    {
        if (players.Count == 0)
        {
            Debug.LogWarning($"{LogTag} No players registered. Cannot proceed to next turn.");
            return;
        }
            
        currentIndex = (currentIndex + 1) % players.Count;
        Debug.Log($"{LogTag} Next turn: {CurrentPlayer.Data.name}");
        OnTurnChanged?.Invoke(CurrentPlayer);
    }

    public void Dispose()
    {
        GameServiceLocator.Unregister<ITurnService>();
    }

    public void Init()
    {
        
    }

    public void ResetPlayer()
    {
        players.Clear();
        currentIndex = 0;
    }

    public TurnService()
    {
        GameServiceLocator.Register<ITurnService>(this);
    }
}
