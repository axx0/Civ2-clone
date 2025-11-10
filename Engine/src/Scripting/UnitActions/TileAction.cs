using Civ2engine.MapObjects;
using Model.Core.Units;

namespace Civ2engine.Scripting.UnitActions;

public abstract class TileAction(Unit baseUnit, Tile tile, string type, Game game) : UnitAction(baseUnit, type, game, tile)
{
    public Tile Tile { get; } = tile;
}