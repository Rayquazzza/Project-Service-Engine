using System;
using UnityEngine;

public class RecruitmentService : IRecruitmentService
{
    private const string LogTag = "<b><color=#AB6BDB>[Recruitment Service]</color></b>";

    private ITurnService turn;
    private IEconomyService economy;
    private IGridService grid;

    private UnitDataSO lastUnitBought;

    public event EventHandler<UnitRecruitedArgs> OnUnitRecruited;

    public RecruitmentService()
    {
        GameServiceLocator.Register<IRecruitmentService>(this);
    }

    public void Init()
    {
        turn = GameServiceLocator.Get<ITurnService>();
        economy = GameServiceLocator.Get<IEconomyService>();
        grid = GameServiceLocator.Get<IGridService>();

        turn.OnTurnChanged += HandleTurnChanged;
    }

    private void HandleTurnChanged(Player player)
    {
        lastUnitBought = null;
    }

    public void RecruitUnit(UnitDataSO unitData, Vector2Int spawnPos)
    {
        Cell targetCell = grid.GetCell(spawnPos);

        if (targetCell == null || targetCell.IsFull)
        {
            Debug.LogWarning($"{LogTag} Recrutement impossible : La cellule à {spawnPos} est pleine !");
            return;
        }

        Player currentPlayer = turn.CurrentPlayer;

        if (unitData == null) return;

        if (!economy.CanAfford(currentPlayer, unitData.Cost))
        {
            Debug.LogWarning($"{LogTag} Fonds insuffisants pour {currentPlayer}");
            return;
        }

        if (lastUnitBought != null && lastUnitBought != unitData)
        {
            Debug.Log($"{LogTag} Limitation : Un seul type d'unité par tour.");
            return;
        }

        economy?.Spend(currentPlayer, unitData.Cost);
        lastUnitBought = unitData;

        IOccupant newUnit = unitData.CreateUnitInstance(currentPlayer);
        grid.SpawnOccupant(spawnPos, newUnit);

        OnUnitRecruited?.Invoke(this, new UnitRecruitedArgs { Unit = newUnit, Position = spawnPos });

        Debug.Log($"{LogTag} {unitData.name} recruté en {spawnPos}");
    }

    public void Dispose()
    {
        if (turn != null) turn.OnTurnChanged -= HandleTurnChanged;
        GameServiceLocator.Unregister<IRecruitmentService>();
    }
}

public class UnitRecruitedArgs : EventArgs
{
    public IOccupant Unit { get; set; }
    public Vector2Int Position { get; set; }
}