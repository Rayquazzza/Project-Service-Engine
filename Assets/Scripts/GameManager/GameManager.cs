using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    [Header("Setup")]
    [SerializeField] private PlayerController[] playerControllers = new PlayerController[2];
    [SerializeField] private GridDataSO gridData;
    [SerializeField] private EconomySettingsSO settings;

    //Services
    private IGameStateService gameStateService;
    private IGridService gridService;
    private ITurnService turnService;
    private IEconomyService economyService;
    private IGridDisplayService gridDisplayService;

    private void Start()
    {
        GetServices();

        gameStateService.OnGameStateChanged += HandleGameStateChanged;
        gridService.OnVitalZoneCaptured += HandleVitalZoneCaptured;
        gameStateService.ChangeGameState(E_GameState.MAIN_MENU);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            gameStateService?.ChangeGameState(E_GameState.GAME_OVER);
        }
    }
    private void GetServices()
    {
        gameStateService = GameServiceLocator.Get<IGameStateService>();
        gridService = GameServiceLocator.Get<IGridService>();
        turnService = GameServiceLocator.Get<ITurnService>();
        economyService = GameServiceLocator.Get<IEconomyService>();
        gridDisplayService = GameServiceLocator.Get<IGridDisplayService>();
    }

    private void HandleGameStateChanged(E_GameState state)
    {
        if (state == E_GameState.STARTING)
        {
            gridService?.InitializeGrid(gridData.GridWidth, gridData.GridHeight);
            if (settings) economyService.Setup(settings);

            PlayersSetup();

            gameStateService?.ChangeGameState(E_GameState.IN_GAME);

            turnService?.NextTurn();
        }
    }


    private void PlayersSetup()
    {
        List<Player> tempPlayers = new List<Player>();

        foreach (var pc in playerControllers)
        {
            Player player = new Player(pc.Data, pc.Cam);

            turnService.RegisterPlayer(player);

            pc.LinkToPlayer(player);
            tempPlayers.Add(player);
        }

        if (tempPlayers.Count >= 2)
        {
            gridService.SetupVitalZones(tempPlayers[0], tempPlayers[1]);
            gridDisplayService.SpawnInitialUnit(tempPlayers[0]);
            gridDisplayService.SpawnInitialUnit(tempPlayers[1]);
        }
    }

    private void HandleVitalZoneCaptured(Player loser)
    {
        Debug.Log($"Game Over - {loser.Data.playerName} a perdu sa zone vitale !");
        gameStateService?.ChangeGameState(E_GameState.GAME_OVER);
    }

    private void OnDestroy()
    {
        if (gameStateService != null) gameStateService.OnGameStateChanged -= HandleGameStateChanged;
        if (gridService != null) gridService.OnVitalZoneCaptured -= HandleVitalZoneCaptured;
    }
}
