using Civ2engine.MapObjects;
using Civ2engine.UnitActions;
using Model.Core.Units;

namespace Civ2engine.Scripting.UnitActions;

public class MoveAction(Unit baseUnit, Tile possibleMove, Game game) : TileAction(baseUnit, possibleMove, "Move", game)
{
    public override void Execute()
    {
        MovementFunctions.UnitMoved(game, BaseUnit, Tile, BaseUnit.CurrentLocation);
    }
}