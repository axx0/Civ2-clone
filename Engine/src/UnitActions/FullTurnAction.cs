using Civ2engine.Units;

namespace Civ2engine.UnitActions;

public abstract class FullTurnAction(Unit unit) : UnitAction(unit)
{
    public override void Execute()
    {
        DoAction();
        Unit.MovePointsLost = Unit.MaxMovePoints;
    }

    protected abstract void DoAction();
}