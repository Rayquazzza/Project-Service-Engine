using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraGridFit : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float distanceOffset = -2f;

    private Camera cam;

    void Awake() => cam = GetComponent<Camera>();

    private IGridDisplayService gridDisplayService;

    void Start()
    {
        gridDisplayService = GameServiceLocator.Get<IGridDisplayService>();
        gridDisplayService.OnVisualGridGenerated += FitToGrid;
    }

    [ContextMenu("Fit To Grid")]
    public void FitToGrid(Vector3 center, Vector2 gridSize)
    {
        float halfFovV = cam.fieldOfView * 0.5f * Mathf.Deg2Rad;
        float halfFovH = Mathf.Atan(Mathf.Tan(halfFovV) * cam.aspect);

        Vector3[] corners = new Vector3[]
        {
        center + new Vector3( gridSize.x * 0.5f, 0,  gridSize.y * 0.5f),
        center + new Vector3(-gridSize.x * 0.5f, 0,  gridSize.y * 0.5f),
        center + new Vector3( gridSize.x * 0.5f, 0, -gridSize.y * 0.5f),
        center + new Vector3(-gridSize.x * 0.5f, 0, -gridSize.y * 0.5f),
        };

        float maxDist = 0f;
        foreach (var corner in corners)
        {
            Vector3 local = transform.InverseTransformPoint(corner);
            float distV = Mathf.Abs(local.y) / Mathf.Tan(halfFovV);
            float distH = Mathf.Abs(local.x) / Mathf.Tan(halfFovH);
            maxDist = Mathf.Max(maxDist, distV + local.z, distH + local.z);
        }

        transform.position = center - transform.forward * (maxDist + distanceOffset);
    }

    void OnDestroy()
    {
        if (gridDisplayService != null)
            gridDisplayService.OnVisualGridGenerated -= FitToGrid;
    }
}