using System;

public interface IGameStateService : IDisposableService
{
    public event Action<E_GameState> OnGameStateChanged;

    public void ChangeGameState(E_GameState newGameState);

    public E_GameState GetCurrentGameState();


}
