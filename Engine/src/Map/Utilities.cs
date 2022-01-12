using System;

namespace Civ2engine
{
    public class Utilities
    {
        /// <summary>
        /// Compute the square euclidean distance between to tiles
        /// </summary>
        /// <param name="startTile"></param>
        /// <param name="tile"></param>
        /// <returns></returns>
        internal static double DistanceTo(IMapItem startTile, IMapItem tile)
        {
            return Math.Pow(startTile.X - tile.X,2) + Math.Pow(startTile.Y - tile.Y, 2);
        }
    }
}