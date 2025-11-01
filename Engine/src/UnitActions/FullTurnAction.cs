using Civ2engine.Units;
using Model.Core.Units;

namespace Civ2engine.UnitActions;

public abstract class FullTurnAction(Unit unit, string type) : UnitAction(unit, type)
{
    public override void Execute()
    {
        DoAction();
        Unit.MovePointsLost = Unit.MaxMovePoints;
    }

    protected abstract void DoAction();
}