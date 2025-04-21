using Civ2engine.MapObjects;
using Civ2engine.Units;
using Model.Core.Units;

namespace Civ2engine.UnitActions;

public class MoveAction(Unit unit, Tile possibleMove) : TileAction(unit, possibleMove)
{
    public override void Execute()
    {
        
    }
}