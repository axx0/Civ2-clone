using System;
using Civ2engine.Units;

namespace Civ2engine.UnitActions;

public class CaptureAction(Unit unit, City city) : TileAction(unit, city.Location)
{
    public override void Execute()
    {
        
    }
}