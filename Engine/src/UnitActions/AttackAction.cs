using Civ2engine.MapObjects;
using Civ2engine.Units;
using Model.Core;
using Model.Core.Units;

namespace Civ2engine.UnitActions;

public class AttackAction(Unit unit, Tile tile, IGame game) : TileAction(unit, tile, "Attack")
{
    public override void Execute()
    {
        MovementFunctions.AttackAtTile(unit, game, Tile);
    }
}