using System;
using System.Linq;
using Civ2engine.MapObjects;
using Model.Core.Units;

namespace Civ2engine.UnitActions;

public class UnloadAction(Unit unit, Tile possibleMove, Game game) : TileAction(unit, possibleMove, "Unload")
{
    public override void Execute()
    {
        var unitToMove = Unit.CarriedUnits.First();
        MovementFunctions.UnitMoved(game, unitToMove, Tile, Unit.CurrentLocation);
    }
}