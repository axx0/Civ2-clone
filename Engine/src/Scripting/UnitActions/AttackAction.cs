using Civ2engine.MapObjects;
using Civ2engine.UnitActions;
using Model.Core.Units;

namespace Civ2engine.Scripting.UnitActions;

public class AttackAction(Unit baseUnit, Tile tile, Game game) : TileAction(baseUnit, tile, "Attack", game)
{
    public override void Execute()
    {
        MovementFunctions.AttackAtTile(BaseUnit, game, Tile);
    }
}