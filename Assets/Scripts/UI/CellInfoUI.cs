using UnityEngine;
using TMPro; // N'oublie pas TextMeshPro

public class CellInfoUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject panel; 
    [SerializeField] private TextMeshProUGUI coordsText;
    [SerializeField] private TextMeshProUGUI ownerText;
    [SerializeField] private TextMeshProUGUI occupantsText;

    private IInputService gridInput;


    private void Start()
    {
        gridInput = GameServiceLocator.Get<IInputService>();

        gridInput.OnCellHoverChanged += HandleHoverChanged;

        panel.SetActive(false);
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
        if (data.Coords != null) coordsText.text = $"Coords: {data.Coords.x}, {data.Coords.y}";

        bool hasOwner = data.IsVitalZone && data.ZoneOwner != null;
        ownerText.gameObject.SetActive(hasOwner);
        if (hasOwner) ownerText.text = $"Owner: {data.ZoneOwner.Data.name}";

        bool hasOccupants = data.Occupants != null && data.Occupants.Count > 0;
        occupantsText.gameObject.SetActive(hasOccupants);
        if (hasOccupants) occupantsText.text = $"Units: {data.Occupants.Count}";
    }

    private void OnDestroy()
    {
        if (gridInput != null) gridInput.OnCellHoverChanged -= HandleHoverChanged;
    }
}