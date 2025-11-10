using Model.Core.Units;

namespace Civ2engine.Scripting.UnitActions;

public class NothingAction(Unit baseUnit, Game game) : FullTurnAction(baseUnit, "Nothing", game)
{
    protected override void DoAction()
    {
        
    }
}