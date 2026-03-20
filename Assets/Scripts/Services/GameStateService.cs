using System;
using UnityEngine;

public class GameStateService : IGameStateService
{
    private const string LogTag = "<b><color=#D956C0>[GameState Service]</color></b>";
    private E_GameState currentGameState;
    private bool isChangingState = false;
    public event Action<E_GameState> OnGameStateChanged;

    public GameStateService()
    {
        GameServiceLocator.Register<IGameStateService>(this);
    }

    public void ChangeGameState(E_GameState newGameState)
    {
        if (isChangingState)
        {
            Debug.LogWarning($"{LogTag} State change to {newGameState} ignored — already changing state.");
            return;
        }

        isChangingState = true;
        currentGameState = newGameState;
        Debug.Log($"{LogTag} Game state changed to {currentGameState}");
        OnGameStateChanged?.Invoke(currentGameState);
        isChangingState = false;
    }

    public E_GameState GetCurrentGameState() => currentGameState;

    public void Dispose()
    {
        GameServiceLocator.Unregister<IGameStateService>();
    }

    public void Init() { }
}