using Civ2engine.Enums;
using Model.Core.Units;

namespace Civ2engine.Scripting.UnitActions;

public class FortifyAction(Unit baseUnit, Game game) : FullTurnAction(baseUnit, "Fortify", game)
{
    protected override void DoAction()
    {
        BaseUnit.Order = (int)OrderType.Fortify;
    }
}