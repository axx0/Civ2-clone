using Civ2engine.Units;
using Model.Core.Units;

namespace Civ2engine.UnitActions;

public abstract class UnitAction(Unit unit)
{
    internal readonly Unit Unit = unit;

    public int Priority { get; set; }
    public abstract void Execute();
}