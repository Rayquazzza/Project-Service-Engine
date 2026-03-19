using System;
using System.Collections.Generic;
using UnityEngine;

public class CombatService : ICombatService
{
    private const string LogTag = "<b><color=red>[Grid Service]</color></b>";

    public event EventHandler<OnCombatResolvedArgs> OnCombatResolved;

    public CombatService()
    {
        GameServiceLocator.Register<ICombatService>(this);
    }

    public void Dispose()
    {
        GameServiceLocator.Unregister<ICombatService>();
    }

    public void Init()
    { 
        
    }

    public IOccupant ResolveCombat(List<IOccupant> attackers, List<IOccupant> defenders)
    {
        float totalAtkPower = CalculateTotalPower(attackers);
        float totalDefPower = CalculateTotalPower(defenders);

        bool attackersWin = totalAtkPower >= totalDefPower;

        float attackerRatio = attackersWin ? (totalDefPower / totalAtkPower) : 1.0f;
        float defenderRatio = attackersWin ? 1.0f : (totalAtkPower / totalDefPower);

        ApplyDamageToGroup(attackers, attackerRatio);
        ApplyDamageToGroup(defenders, defenderRatio);

        var args = new OnCombatResolvedArgs
        (
            attackers[0],
            defenders[0],
            totalAtkPower,
            totalDefPower,
            attackerRatio,
            defenderRatio
        );

        OnCombatResolved?.Invoke(this, args);

        return attackersWin ? attackers[0] : defenders[0];
    }

    private float CalculateTotalPower(List<IOccupant> group)
    {
        float total = 0;
        foreach (var unit in group)
            total += (unit as BaseUnit)?.UnitData.AttackPower ?? 0;
        return total;
    }

    private void ApplyDamageToGroup(List<IOccupant> group, float lossRatio)
    {
        foreach (var occupant in group)
        {
            if (occupant is IDamageable damageable)
            {
                float damage = damageable.Health * lossRatio;
                damageable.TakeDamage(damage);
            }
        }
    }
}