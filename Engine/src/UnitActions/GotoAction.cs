
using Civ2engine.MapObjects;
using Civ2engine.Units;
using Model.Core.Units;

namespace Civ2engine.UnitActions;

internal class GotoAction(Unit unit, Tile tile, Game game) : TileAction(unit, tile, "Goto")
{
    public override void Execute()
    {
        var path = Path.CalculatePathBetween(game, Unit.CurrentLocation, Tile, Unit.Domain, Unit.MaxMovePoints, Unit.Owner, Unit.Alpine, Unit.IgnoreZonesOfControl, false);
        path?.Follow(game, Unit);
    }
}