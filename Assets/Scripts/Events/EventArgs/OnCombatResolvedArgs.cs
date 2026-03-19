using System;
using UnityEngine;

public class OnCombatResolvedArgs : EventArgs
{
    public IOccupant Attacker { get; set; }
    public IOccupant Defender { get; set; }
    public float AttackerVAT { get; set; }
    public float DefenderVAT { get; set; }
    public float AttackerLossRatio { get; set; }
    public float DefenderLossRatio { get; set; }

    public OnCombatResolvedArgs(IOccupant attacker, IOccupant defender, float attackerVAT, float defenderVAT, float attackerLossRatio, float defenderLossRatio)
    {
        Attacker = attacker;
        Defender = defender;
        AttackerVAT = attackerVAT;
        DefenderVAT = defenderVAT;
        AttackerLossRatio = attackerLossRatio;
        DefenderLossRatio = defenderLossRatio;
    }
}
