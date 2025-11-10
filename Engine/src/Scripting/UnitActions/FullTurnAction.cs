using Model.Core.Units;

namespace Civ2engine.Scripting.UnitActions;

public abstract class FullTurnAction(Unit baseUnit, string type, Game game) : UnitAction(baseUnit, type, game)
{
    public override void Execute()
    {
        DoAction();
        BaseUnit.MovePointsLost = BaseUnit.MaxMovePoints;
    }

    protected abstract void DoAction();
}