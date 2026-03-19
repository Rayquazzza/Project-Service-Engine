using TMPro;
using UnityEngine;
using DG.Tweening;

public class CombatantDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI vatText;
    [SerializeField] private TextMeshProUGUI lossText; // Nouveau champ pour les pertes

    private int currentVatValue;
    private float currentLossValue;

    public void SetupInitial(string name)
    {
        nameText.text = name;
        vatText.text = "VAT: 0";
        lossText.text = ""; // Caché au début
        lossText.gameObject.SetActive(false);
    }

    // Tween pour le score VAT
    public Tween AnimateVat(int targetVat, float duration)
    {
        return DOTween.To(() => currentVatValue, x => {
            currentVatValue = x;
            vatText.text = $"VAT: {currentVatValue}";
        }, targetVat, duration).SetEase(Ease.OutQuad);
    }

    // Tween pour le pourcentage de pertes
    public Tween AnimateLoss(float targetLoss, float duration)
    {
        lossText.gameObject.SetActive(true);
        return DOTween.To(() => currentLossValue, x => {
            currentLossValue = x;
            lossText.text = $"Pertes: {currentLossValue * 100:0}%";
        }, targetLoss, duration).SetEase(Ease.OutQuad);
    }
}