using Civ2engine.Units;
using Model.Core;
using Model.Core.Units;

namespace Civ2engine.UnitActions;

public class BuildCityAction(Unit unit, IGame game, string? name = null) : FullTurnAction(unit)
{
    protected override void DoAction()
    {
        CityActions.BuildCity(Unit.CurrentLocation, Unit, game, name ?? CityActions.GetCityName(Unit.Owner, game));
    }
}