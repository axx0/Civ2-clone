using Civ2engine;

namespace Model.Core;

public interface IHistory
{
    void CityBuilt(City city);
    void AdvanceDiscovered(int advanceIndex, Civilization targetCiv);
}