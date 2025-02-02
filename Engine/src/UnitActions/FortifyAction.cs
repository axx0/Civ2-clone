using Civ2engine.Enums;
using Civ2engine.Units;

namespace Civ2engine.UnitActions;

public class FortifyAction(Unit unit) : FullTurnAction(unit)
{
    protected override void DoAction()
    {
        Unit.Order = (int)OrderType.Fortify;
    }
}