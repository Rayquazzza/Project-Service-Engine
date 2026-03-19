using System;
using System.Collections.Generic;
using UnityEngine;

public class AudioObserver : MonoBehaviour
{

    [Header("AUDIO EVENTS")]
    [SerializeField] private GameEventSO unitSpawnedEvent;
    [SerializeField] private GameEventSO unitMovedEvent;
    [SerializeField] private GameEventSO combatResolvedEvent;


    //SERVICES
    private ICombatService combatService;
    private IGridService gridService;

    private void Awake()
    {
       
    }

    private void Start()
    {
        if (unitSpawnedEvent == null || unitMovedEvent == null || combatResolvedEvent == null)
        {
            Debug.LogError("AudioObserver: One or more GameEventSO references are not assigned in the inspector. Please assign them to avoid null reference errors.");
            return;
        }
        combatService = GameServiceLocator.Get<ICombatService>();
        gridService = GameServiceLocator.Get<IGridService>();

        combatService.OnCombatResolved += HandleCombatResolved;
        gridService.OnOccupantMoved += HandleUnitMoved;
        gridService.OnOccupantSpawned += HandleUnitSpawned;
    }

    private void HandleUnitSpawned(IOccupant occupant)
    {
       unitSpawnedEvent?.Raise();
    }

    private void HandleUnitMoved(List<IOccupant> occupant, Vector2Int int1, Vector2Int int2, List<Vector2Int> list, Action onComplete)
    {
       unitMovedEvent?.Raise();
    }

    private void HandleCombatResolved(object sender, OnCombatResolvedArgs e)
    {
        combatResolvedEvent?.Raise();
    }
}
