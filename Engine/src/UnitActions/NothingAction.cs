using Civ2engine.Units;
using Model.Core.Units;

namespace Civ2engine.UnitActions;

public class NothingAction(Unit unit) : FullTurnAction(unit)
{
    protected override void DoAction()
    {
        
    }
}