using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitHandlerUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI unitNameText;
    [SerializeField] private TextMeshProUGUI unitStatText;
    [SerializeField] private TextMeshProUGUI unitCostText;
    [SerializeField] private Button recruitmentButton;

    private UnitDataSO data;

    private Action<UnitDataSO> onBuyRequested;

    public void Setup(UnitDataSO data, Action<UnitDataSO> onBuyRequested)
    {
        this.data = data;
        this.onBuyRequested = onBuyRequested;

        unitNameText.text = data.UnitName;
        unitCostText.text = $"{data.Cost} Or";
        unitStatText.text = $"ATK: {data.AttackPower} | MOV: {data.MoveRange}";

        recruitmentButton.onClick.RemoveAllListeners();
        recruitmentButton.onClick.AddListener(OnRecruitClicked);
    }

    private void OnRecruitClicked()
    {
        onBuyRequested.Invoke(data);
    }
}