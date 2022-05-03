using System;

namespace Civ2engine.MapObjects
{
    public static class Utilities
    {
        /// <summary>
        /// Compute the square euclidean distance between to tiles
        /// </summary>
        /// <param name="startTile"></param>
        /// <param name="tile"></param>
        /// <returns></returns>
        internal static double DistanceTo(IMapItem startTile, IMapItem tile, bool flatWorld)
        {
            return Math.Pow(startTile.X - tile.X, 2) + Math.Pow(startTile.Y - tile.Y, 2);
        }
    }
}