using UnityEngine;
using TMPro;
using System;
using Unity.VisualScripting; // N'oublie pas TextMeshPro

public class CellInfoUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject panel; 
    [SerializeField] private TextMeshProUGUI coordsText;
    [SerializeField] private TextMeshProUGUI ownerText;
    [SerializeField] private TextMeshProUGUI occupantsText;
    [SerializeField] private TextMeshProUGUI vitalZoneText;
    [SerializeField] private TextMeshProUGUI resourceMultiplierText;

    private IInputService gridInput;
    private ITurnService turnService;

    private Player currentPlayer;


    private void Start()
    {
        if(panel == null || coordsText == null || ownerText == null || occupantsText == null || vitalZoneText == null || resourceMultiplierText == null)
        {
            Debug.LogError("CellInfoUI: One or more UI elements are not assigned in the inspector.");
            return;
        }
        gridInput = GameServiceLocator.Get<IInputService>();
        turnService = GameServiceLocator.Get<ITurnService>();
        turnService.OnTurnChanged += HandleTurnChanged;

        gridInput.OnCellHoverChanged += HandleHoverChanged;

        panel.SetActive(false);
    }

    private void HandleTurnChanged(Player player)
    {
        currentPlayer = player;
        Debug.Log($"CellInfoUI: Current player changed to {currentPlayer.Data.playerName}");
    }

    private void HandleHoverChanged(CellView cellView)
    {
        if (cellView == null)
        {
            panel.SetActive(false);
            return;
        }

        panel.SetActive(true);
        UpdateUI(cellView.GetData());
    }

    private void UpdateUI(Cell data)
    {
        coordsText.text = $"Coords: {data.Coords.x}, {data.Coords.y}";
        resourceMultiplierText.text = $"Resource Multiplier: {data.ResourceMultiplier}x";

        bool isOwnedByCurrentPlayer = data.ZoneOwner == currentPlayer;

        bool hasOwner = data.ZoneOwner != null && data.ZoneOwner == currentPlayer;
        ownerText.gameObject.SetActive(hasOwner);
        if (hasOwner) ownerText.text = $"Owner: {data.ZoneOwner.Data.playerName}";

        bool hasOccupants = isOwnedByCurrentPlayer && data.Occupants != null && data.Occupants.Count > 0;
        occupantsText.gameObject.SetActive(hasOccupants);
        if (hasOccupants) occupantsText.text = $"Units: {data.Occupants.Count}";

        bool showVitalZone = isOwnedByCurrentPlayer && data.IsVitalZone;
        vitalZoneText.gameObject.SetActive(showVitalZone);
        if (showVitalZone) vitalZoneText.text = "Vital Zone";
    }

    private void OnDestroy()
    {     
        if (gridInput != null) gridInput.OnCellHoverChanged -= HandleHoverChanged;
        if (turnService != null) turnService.OnTurnChanged -= HandleTurnChanged;
    }
}