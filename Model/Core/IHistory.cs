using Civ2engine;

namespace Model.Core;

public interface IHistory
{
    int TotalCitiesBuilt(int civId);
    void CityBuilt(City city);
    void AdvanceDiscovered(int advanceIndex, int targetCiv);
}