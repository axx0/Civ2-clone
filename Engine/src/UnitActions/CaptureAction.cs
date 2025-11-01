using System;
using Civ2engine.Units;
using Model.Core;
using Model.Core.Units;

namespace Civ2engine.UnitActions;

public class CaptureAction(Unit unit, City city, IGame game) : TileAction(unit, city.Location, "Capture")
{
    public override void Execute()
    {
        MovementFunctions.UnitMoved(game, Unit, Tile, Unit.CurrentLocation);
    }
}