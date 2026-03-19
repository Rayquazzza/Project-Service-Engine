using System;
using UnityEngine;

public interface IRecruitmentService : IDisposableService
{
    public event EventHandler<UnitRecruitedArgs> OnUnitRecruited;
    void RecruitUnit(UnitDataSO unitData, Vector2Int spawnPos);
}
