using Civ2engine;
using Model.Core.Cities;

namespace Model.Core;

public interface IHistory
{
    void CityBuilt(City city);
    void AdvanceDiscovered(int advanceIndex, Civilization targetCiv);
}