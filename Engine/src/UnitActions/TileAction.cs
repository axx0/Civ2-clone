using Civ2engine.MapObjects;
using Civ2engine.Units;

namespace Civ2engine.UnitActions;

public abstract class TileAction(Unit unit, Tile tile) : UnitAction(unit)
{
    public Tile Tile { get; } = tile;
}