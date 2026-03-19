using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GridInput : MonoBehaviour, IInputService   
{
    public event Action<CellView> OnCellHoverChanged;
    public event Action<CellView> OnCellLeftClicked;
    public event Action<CellView> OnCellRightClicked;

    private CellView lastHovered;
    private Camera targetCam;

    private IGameStateService gameStateService;
    private ITurnService turnService;

    private void Awake()
    {
        GameServiceLocator.Register<IInputService>(this);
    }

    private void Start()
    {
        gameStateService = GameServiceLocator.Get<IGameStateService>();
        turnService = GameServiceLocator.Get<ITurnService>();
        turnService.OnTurnChanged += HandleTurnChanged;
    }

    private void Update()
    {
        if(gameStateService == null || gameStateService.GetCurrentGameState() != E_GameState.IN_GAME) return;

        if (targetCam == null)
        {
            Debug.LogWarning("Target camera not set for GridInput.");
            return;
        }

        if (!IsMouseInViewport(targetCam))
        {
            ClearHover();
            return;
        }

        HandleMouseDetection();
    }

    private void HandleTurnChanged(Player player)
    {
        targetCam = player.PlayerCamera;
    }


    private void HandleMouseDetection()
    {
        Ray ray = targetCam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            CellView current = hit.collider.GetComponentInParent<CellView>();

            if (current != lastHovered)
            {
                lastHovered?.Highlight(false);
                lastHovered = current;

                if (current != null)
                {
                    lastHovered.Highlight(true);
                    OnCellHoverChanged?.Invoke(lastHovered);
                }
            }

            if (current != null)
            {
                if (Input.GetMouseButtonDown(0)) OnCellLeftClicked?.Invoke(current);
                if (Input.GetMouseButtonDown(1)) OnCellRightClicked?.Invoke(current);
            }
        }
        else { ClearHover(); }
    }

    private void ClearHover()
    {
        if (lastHovered != null)
        {
            lastHovered.Highlight(false);
            lastHovered = null;
            OnCellHoverChanged?.Invoke(null);
        }
    }

    private bool IsMouseInViewport(Camera cam)
    {
        Vector3 v = cam.ScreenToViewportPoint(Input.mousePosition);
        return v.x >= 0 && v.x <= 1 && v.y >= 0 && v.y <= 1;
    }

    private void OnDestroy()
    {
        GameServiceLocator.Unregister<IInputService>();
    }

}