using System;
using System.Collections.Generic;
using UnityEngine;

public interface ICombatService : IDisposableService
{

    public event EventHandler<OnCombatResolvedArgs> OnCombatResolved;  
    IOccupant ResolveCombat(List<IOccupant> attackers, List<IOccupant> defenders);
}
