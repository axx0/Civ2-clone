using Civ2engine;

namespace Model.Core;

public interface IHistory
{
    int TotalCitiesBuilt(Civilization civId);
    void CityBuilt(City city);
    void AdvanceDiscovered(int advanceIndex, Civilization targetCiv);
}