using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private PlayerDataSO data;


    [SerializeField] private GameObject blackOverlay;
    [SerializeField] private PlayerInfoUI playerInfo;
    public Camera Cam { get; private set; }
    public PlayerDataSO Data => data;

    private Player linkedPlayer;

    private ITurnService turnService;

    private int originalMask;

    private void Awake()
    {
        Cam = GetComponentInChildren<Camera>();
        if (Cam != null) originalMask = Cam.cullingMask;
    }

    private void Start()
    {
        turnService = GameServiceLocator.Get<ITurnService>();
    }

    public void LinkToPlayer(Player playerInstance)
    {
        if (turnService == null) turnService = GameServiceLocator.Get<ITurnService>();

        linkedPlayer = playerInstance;
        turnService.OnTurnChanged += CheckMyTurn;

        UpdateCameraState(turnService.CurrentPlayer == linkedPlayer);
        if (playerInfo != null) playerInfo.Setup(linkedPlayer);
    }

    private void CheckMyTurn(Player currentPlayer)
    {
        bool isMyTurn = (currentPlayer == linkedPlayer);
        UpdateCameraState(isMyTurn);
    }

    private void UpdateCameraState(bool isActive)
    {
        if (Cam != null)
        {
            if (isActive)
            {
                Cam.cullingMask = originalMask;
                if (blackOverlay != null) blackOverlay.SetActive(false);
            }
            else
            {
                //Cam.cullingMask = 1 << LayerMask.NameToLayer("UI");

                if (blackOverlay != null) blackOverlay.SetActive(true);
            }

            Cam.enabled = true;
        }
    }

    private void OnDestroy()
    {
        if (turnService != null) turnService.OnTurnChanged -= CheckMyTurn;
    }
}