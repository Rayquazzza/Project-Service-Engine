using UnityEngine;

public interface IEconomyService : IDisposableService
{
    void Setup(EconomySettingsSO data);
    bool CanAfford(Player player, int amount);
    void Spend(Player player, int amount);
    void AddMoney(Player player, int amount);
    void GenerateResources(Player player);
    void ApplyVitalZoneProximity();

}
