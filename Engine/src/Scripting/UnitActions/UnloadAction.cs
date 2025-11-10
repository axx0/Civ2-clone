using System.Linq;
using Civ2engine.MapObjects;
using Civ2engine.UnitActions;
using Model.Core.Units;

namespace Civ2engine.Scripting.UnitActions;

public class UnloadAction(Unit baseUnit, Tile possibleMove, Game game) : TileAction(baseUnit, possibleMove, "Unload", game)
{
    public override void Execute()
    {
        var unitToMove = BaseUnit.CarriedUnits.First();
        MovementFunctions.UnitMoved(game, unitToMove, Tile, BaseUnit.CurrentLocation);
    }
}