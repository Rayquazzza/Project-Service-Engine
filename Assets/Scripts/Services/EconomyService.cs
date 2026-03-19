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
        ApplyRandomMultipliers();
    }

    private void ApplyRandomMultipliers()
    {
        var vitalZones = gridService.GetAllCells().Where(c => c.IsVitalZone).Select(c => c.Coords).ToList();

        foreach (var cell in gridService.GetAllCells())
        {
            float random = Random.Range(settings.CellMultiplierRange.min,settings.CellMultiplierRange.max);

            if (vitalZones.Count > 0)
            {
                int closestDist = vitalZones.Min(vitalZone => Mathf.Abs(cell.Coords.x - vitalZone.x) + Mathf.Abs(cell.Coords.y - vitalZone.y));

                if (closestDist <= settings.ProximityRadius)
                {
                    float penalty = 1f - (settings.ProximityRadius - closestDist) * settings.ProximityPenaltyPerTile;
                    random *= Mathf.Max(penalty, 0f); 
                }
            }

            cell.ResourceMultiplier = random;
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