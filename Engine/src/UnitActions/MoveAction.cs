using Civ2engine.MapObjects;
using Civ2engine.Units;
using Model.Core.Units;

namespace Civ2engine.UnitActions;

public class MoveAction(Unit unit, Tile possibleMove, Game game) : TileAction(unit, possibleMove, "Move")
{
    public override void Execute()
    {
        MovementFunctions.UnitMoved(game, Unit, Tile, Unit.CurrentLocation);
    }
}