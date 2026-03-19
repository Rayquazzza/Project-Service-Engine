using UnityEngine;

public class UnitService : IUnitService
{

    public UnitService()
    {
        GameServiceLocator.Register<IUnitService>(this);
    }

    public void Dispose()
    {
      GameServiceLocator.Unregister<IUnitService>();
    }

    public void Init()
    {
      
    }

}
