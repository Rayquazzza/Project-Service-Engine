using System.Collections.Generic;

public interface IConfigService : IDisposableService
{
    void SetRegistry(UnitRegistrySO registry);
    List<UnitDataSO> GetAllUnits();


}
