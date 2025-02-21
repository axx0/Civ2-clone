using Civ2engine.Units;
using Model.Core;

namespace Civ2engine.UnitActions;

public class BuildCityAction(Unit unit, IGame game, string? name = null) : FullTurnAction(unit)
{
    protected override void DoAction()
    {
        CityActions.BuildCity(Unit.CurrentLocation, Unit, game, name ?? CityActions.GetCityName(Unit.Owner, game));
    }
}