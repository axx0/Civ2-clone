using System.Collections.Generic;

namespace Civ2engine.MapObjects
{
    public static class TileExtensions
    {
        public static IEnumerable<Tile> CityRadius(this Tile tile, bool nullForInvalid = false)
        {
            return tile.Map.CityRadius(tile, nullForInvalid);
        }

        public static IEnumerable<Tile> Neighbours(this Tile tile)
        {
            return tile.Map.Neighbours(tile);
        }
    }
}