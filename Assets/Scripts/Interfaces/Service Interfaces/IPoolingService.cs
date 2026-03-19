using UnityEngine;

public interface IPoolingService : IDisposableService
{
    GameObject GetFromPool(GameObject prefab, Vector3 position, Quaternion rotation);

    void ReturnToPool(GameObject prefab, GameObject obj);
}
