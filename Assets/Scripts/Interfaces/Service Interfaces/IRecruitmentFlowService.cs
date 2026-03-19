using UnityEngine;

public interface IRecruitmentFlowService : IDisposableService
{
    Vector2Int PendingSpawnPos {get ;}
}
