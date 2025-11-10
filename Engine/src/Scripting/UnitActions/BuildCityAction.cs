using Civ2engine.UnitActions;
using Model.Core.Units;

namespace Civ2engine.Scripting.UnitActions;

public class BuildCityAction(Unit baseUnit, Game game, string? name = null) : FullTurnAction(baseUnit, "BuildCity", game)
{
    protected override void DoAction()
    {
        CityActions.BuildCity(BaseUnit, game, name ?? CityActions.GetCityName(BaseUnit.Owner, game));
    }
}