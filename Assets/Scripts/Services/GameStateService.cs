using System;
using UnityEngine;
public class GameStateService : IGameStateService
{
    private const string LogTag = "<b><color=#D956C0>[GameState Service]</color></b>";
    private E_GameState currentGameState;


    public event Action<E_GameState> OnGameStateChanged;


    public GameStateService()
    {
        GameServiceLocator.Register<IGameStateService>(this);
    }


    public void ChangeGameState(E_GameState newGameState)
    {
        currentGameState = newGameState;
        Debug.Log($"{LogTag} Game state changed to {currentGameState}");
        OnGameStateChanged?.Invoke(currentGameState);
    }

    public E_GameState GetCurrentGameState()
    {
        //Debug.Log($"{LogTag} Current game state is {currentGameState}");
        return currentGameState;
    }

    public void Dispose()
    {
        GameServiceLocator.Unregister<IGameStateService>();
    }

    public void Init()
    {
        
    }
}
