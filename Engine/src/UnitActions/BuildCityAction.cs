using Civ2engine.Units;
using Model.Core;
using Model.Core.Units;

namespace Civ2engine.UnitActions;

public class BuildCityAction(Unit unit, IGame game, string? name = null) : FullTurnAction(unit, "BuildCity")
{
    protected override void DoAction()
    {
        CityActions.BuildCity(Unit, game, name ?? CityActions.GetCityName(Unit.Owner, game));
    }
}