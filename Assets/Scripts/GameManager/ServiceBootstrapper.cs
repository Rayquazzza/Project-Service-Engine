using System.Collections.Generic;
using UnityEngine;

public class ServiceBootstrapper : MonoBehaviour
{
    private List<IDisposableService> disposableServices = new List<IDisposableService>();

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        Initialize();
    }

    private void Initialize()
    {
        var services = new object[]
        {
            new GameStateService(),
            new TurnService(),
            new GridService(),
            new PathfindingService(),
            new RecruitmentService(),
            new RecruitmentFlowService(),
            new EconomyService(),
            new FogOfWarService(),
            new UnitService(),
            new CombatService(),
            new PlayersService()
        };

        foreach (var service in services)
        {
            Register(service);
        }
    }

    private void Register<T>(T service) where T : class
    {
        if (service is IDisposableService disposable)
        {
            disposableServices.Add(disposable);
        }
    }


    private void Start()
    {
        foreach (var service in disposableServices)
        {
            service.Init();
        }
    }

    private void OnDestroy()
    {
        for (int i = disposableServices.Count - 1; i >= 0; i--)
        {
            if (disposableServices[i] != null) disposableServices[i].Dispose();
        }
        disposableServices.Clear();
    }
}