using System;
using UnityEngine;

public class RecruitmentUI : BaseStateUIPanel
{
    [SerializeField] private UnitRegistrySO recruitmentSO;
    [SerializeField] private GameObject unitBox;
    [SerializeField] private GameObject unitHandlerPrefab;

    private Vector2Int pendingSpawnPos;

    public void ClearRecruitment()
    {
        GameServiceLocator.Get<IGameStateService>().ChangeGameState(E_GameState.IN_GAME);
    }

    protected override void OnStateChanged(E_GameState newState)
    {
        if (newState == activeState)
        {
            PopulateRecruitment();
        }
    }

    private void PopulateRecruitment()
    {

        var recruitmentService = GameServiceLocator.Get<IRecruitmentService>();
        var recruitmentFlowService = GameServiceLocator.Get<IRecruitmentFlowService>();

        foreach (Transform child in unitBox.transform) Destroy(child.gameObject);

        Vector2Int spawnPos = recruitmentFlowService.PendingSpawnPos;

        foreach (var unitData in recruitmentSO.AllUnits)
        {
            GameObject handlerGO = Instantiate(unitHandlerPrefab, unitBox.transform);
            UnitHandlerUI handlerScript = handlerGO.GetComponent<UnitHandlerUI>();

            handlerScript.Setup(unitData, (data) => { recruitmentService.RecruitUnit(data, spawnPos); });
        }
    }
}
