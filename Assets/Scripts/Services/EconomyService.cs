using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EconomyService : IEconomyService
{
    private EconomySettingsSO settings;
    private ITurnService turnService;
    private IGridService gridService;

    public EconomyService()
    {
        GameServiceLocator.Register<IEconomyService>(this);
    }

    public void Init()
    {
        turnService = GameServiceLocator.Get<ITurnService>();
        gridService = GameServiceLocator.Get<IGridService>();
        if (turnService != null) turnService.OnTurnChanged += GenerateResources;
    }

    public void Dispose()
    {
        turnService = GameServiceLocator.Get<ITurnService>();
        if (turnService != null) turnService.OnTurnChanged -= GenerateResources;
        GameServiceLocator.Unregister<IEconomyService>();
    }

    public void Setup(EconomySettingsSO settings)
    {
        this.settings = settings;
        foreach (var cell in gridService.GetAllCells())
        {
            int random = Random.Range((int)settings.CellMultiplierRange.min, (int)settings.CellMultiplierRange.max + 1);
            cell.ResourceMultiplier = random;
        }
    }

    public void ApplyVitalZoneProximity()
    {
        var vitalZones = gridService.GetAllCells().Where(c => c.IsVitalZone).Select(c => c.Coords).ToList();

        if (vitalZones.Count == 0) return;

        foreach (var cell in gridService.GetAllCells())
        {
            int closestDist = vitalZones.Min(vz => Mathf.Abs(cell.Coords.x - vz.x) + Mathf.Abs(cell.Coords.y - vz.y));

            if (closestDist <= 3)
                cell.ResourceMultiplier = 2;
        }
    }
    public bool CanAfford(Player player, int amount)
    {
        return player.CurrentMoney >= amount;
    }

    public void Spend(Player player, int amount)
    {
        if (CanAfford(player, amount))
            player.CurrentMoney -= amount;
        if(player.CurrentMoney < 0) player.CurrentMoney = 0;
    }

    public void AddMoney(Player player, int amount)
    {
        player.CurrentMoney += amount;
    }

    public void GenerateResources(Player player)
    {
        var gridService = GameServiceLocator.Get<IGridService>();
        float total = 0;

        foreach (var cell in gridService.GetAllCells())
        {
            if (cell.IsEmpty) continue;
            if (cell.Occupants[0].OwnerId != player) continue;

            if (cell.IsVitalZone)
                total += settings.BaseTurnIncome * settings.VitalZoneMultiplier;
            else
                total += settings.BaseTurnIncome * cell.ResourceMultiplier;
        }

        AddMoney(player, Mathf.RoundToInt(total));
    }
}