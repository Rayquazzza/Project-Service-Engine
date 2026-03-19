using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;
using System;

public class CombatUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private CanvasGroup panelCanvasGroup;
    [SerializeField] private CombatantDisplay attackerDisplay;
    [SerializeField] private CombatantDisplay defenderDisplay;
    [SerializeField] private TextMeshProUGUI winnerText;
    [SerializeField] private Button closeButton;

    [Header("Settings")]
    [SerializeField] private float countDuration = 1.0f;
    [SerializeField] private float knockDistance = 50f; // Distance du choc

    private ICombatService combatService;
    private Sequence combatSequence;

    private void Start()
    {
        combatService = GameServiceLocator.Get<ICombatService>();
        combatService.OnCombatResolved += HandleCombatResolved;

        if (closeButton != null)
            closeButton.onClick.AddListener(Hide);

        winnerText.gameObject.SetActive(false);
    }

    public void Hide()
    {
        combatSequence?.Kill();

        DOTween.Sequence()
            .Append(panelCanvasGroup.DOFade(0f, 0.25f).SetEase(Ease.InCubic))
            .Join(panel.transform.DOScale(0.9f, 0.25f))
            .OnComplete(() => panel.SetActive(false))
            .SetUpdate(true)
            .Play();
    }

    private void HandleCombatResolved(object sender, OnCombatResolvedArgs e)
    {
        string winnerName = e.AttackerLossRatio < e.DefenderLossRatio ? e.Attacker.OwnerId.Data.playerName : e.Defender.OwnerId.Data.playerName;

        Show(e.Attacker.OwnerId.Data.playerName, Mathf.RoundToInt(e.AttackerVAT), e.AttackerLossRatio, e.Defender.OwnerId.Data.playerName, Mathf.RoundToInt(e.DefenderVAT), e.DefenderLossRatio, winnerName);
    }

    public void Show(string atkName, int atkPower, float atkRatio, string defName, int defPower, float defRatio, string winnerName)
    {
        combatSequence?.Kill();
        panel.SetActive(true);
        winnerText.gameObject.SetActive(false);
        closeButton.gameObject.SetActive(false);
        panelCanvasGroup.alpha = 0;

        // Reset positions et échelles
        attackerDisplay.SetupInitial(atkName);
        defenderDisplay.SetupInitial(defName);
        attackerDisplay.transform.localScale = Vector3.zero;
        defenderDisplay.transform.localScale = Vector3.zero;

        combatSequence = DOTween.Sequence();

        // 1. Apparition des panneaux
        combatSequence.Append(panelCanvasGroup.DOFade(1f, 0.3f));
        combatSequence.Join(attackerDisplay.transform.DOScale(1f, 0.4f).SetEase(Ease.OutBack));
        combatSequence.Join(defenderDisplay.transform.DOScale(1f, 0.4f).SetEase(Ease.OutBack));

        // 2. Incrémentation des VAT en simultané
        combatSequence.Append(attackerDisplay.AnimateVat(atkPower, countDuration));
        combatSequence.Join(defenderDisplay.AnimateVat(defPower, countDuration));

        // 3. L'ENTRECHOC (Collision)
        // On fait bouger l'un vers l'autre puis rebond
        combatSequence.AppendInterval(0.2f);
        combatSequence.Append(attackerDisplay.transform.DOBlendableLocalMoveBy(new Vector3(knockDistance, 0, 0), 0.1f).SetEase(Ease.InExpo));
        combatSequence.Join(defenderDisplay.transform.DOBlendableLocalMoveBy(new Vector3(-knockDistance, 0, 0), 0.1f).SetEase(Ease.InExpo));

        // Impact (petit shake de caméra ou de panel ici si tu veux)
        combatSequence.Append(panel.transform.DOPunchPosition(Vector3.up * 10f, 0.2f));

        // Retour à la place initiale
        combatSequence.Append(attackerDisplay.transform.DOBlendableLocalMoveBy(new Vector3(-knockDistance, 0, 0), 0.2f).SetEase(Ease.OutBounce));
        combatSequence.Join(defenderDisplay.transform.DOBlendableLocalMoveBy(new Vector3(knockDistance, 0, 0), 0.2f).SetEase(Ease.OutBounce));

        // 4. Affichage des pertes
        combatSequence.Append(attackerDisplay.AnimateLoss(atkRatio, countDuration));
        combatSequence.Join(defenderDisplay.AnimateLoss(defRatio, countDuration));

        // 5. Affichage du Vainqueur
        combatSequence.AppendInterval(0.3f);
        combatSequence.AppendCallback(() => {
            winnerText.text = $"VICTOIRE : {winnerName} !";
            winnerText.gameObject.SetActive(true);
        });
        combatSequence.Append(winnerText.transform.DOScale(1.2f, 0.5f).From(0f).SetEase(Ease.OutElastic));

        combatSequence.AppendInterval(0.3f);
        combatSequence.AppendCallback(() => closeButton.gameObject.SetActive(true));
        combatSequence.Append(closeButton.transform.DOScale(1f, 0.3f).From(0f).SetEase(Ease.OutBack));

        combatSequence.SetUpdate(true).Play();
    }
}