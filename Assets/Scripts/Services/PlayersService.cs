using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayersService : IPlayersService
{
    private const string LogTag = "<b><color=#B20600>[Player Service]</color></b>";
    private Dictionary<int, PlayerDataSO> playersById = new Dictionary<int, PlayerDataSO>();

    private List<Player> players = new List<Player>();
    private int currentIndex = 0;

    public Player CurrentPlayer => players[currentIndex];


    private IGridService gridService;

    private Dictionary<Player, int> unitCounts = new Dictionary<Player, int>();
    public event Action<Player> OnPlayerDefeated;
    public event Action<Player> OnPlayerEliminated;

    public PlayersService()
    {
        GameServiceLocator.Register<IPlayersService>(this);
    }

    public void Init()
    {
        gridService = GameServiceLocator.Get<IGridService>();

        gridService.OnOccupantSpawned += RegisterUnit;
        gridService.OnOccupantDestroyed += UnregisterUnit;
    }

    private void RegisterUnit(IOccupant unit)
    {
        if (!unitCounts.ContainsKey(unit.OwnerId)) unitCounts[unit.OwnerId] = 0;
        unitCounts[unit.OwnerId]++;
        Debug.Log($"[PlayerService] {unit.OwnerId} possède {unitCounts[unit.OwnerId]} unités.");
    }

    public Camera GetCurrentPlayerCamera()
    {
        return CurrentPlayer?.PlayerCamera;
    }

    private void UnregisterUnit(IOccupant unit)
    {
        if (unitCounts.ContainsKey(unit.OwnerId))
        {
            unitCounts[unit.OwnerId]--;
            if (unitCounts[unit.OwnerId] <= 0)
            {
                OnPlayerEliminated?.Invoke(unit.OwnerId);
            }
        }
    }

    public void RegisterPlayer(PlayerDataSO data, Camera playerCam)
    {
        players.Add(new Player(data, playerCam));
    }


    public int GetIdByPlayer(PlayerDataSO data)
    {
        foreach (var pair in playersById)
        {
            if (pair.Value == data) return pair.Key;
        }
        return 0; 
    }

    public PlayerDataSO GetPlayerById(int id)
    {
        return playersById.GetValueOrDefault(id);
    }

    public void NextTurn()
    {
        currentIndex = (currentIndex + 1) % players.Count;
    }

    public void Dispose()
    {
        if (gridService != null)
        {
            gridService.OnOccupantSpawned -= RegisterUnit;
            gridService.OnOccupantDestroyed -= UnregisterUnit;
        }

       GameServiceLocator.Unregister<IPlayersService>();
    }

    
}