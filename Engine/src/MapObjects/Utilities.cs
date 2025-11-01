using System;

namespace Civ2engine.MapObjects
{
    public static class Utilities
    {
        /// <summary>
        /// Compute the square Euclidean distance between to tiles
        /// </summary>
        /// <param name="startTile"></param>
        /// <param name="tile"></param>
        /// <returns></returns>
        internal static double DistanceTo(Tile startTile, Tile tile)
        {
            if(startTile == tile) return 0;
            if (startTile.Map.Flat)
            {
                return Math.Pow(startTile.X - tile.X, 2) + Math.Pow(startTile.Y - tile.Y, 2);
            }
            var diffX = Math.Abs(startTile.X - tile.X);
            var diffY = startTile.Y - tile.Y;
            return Math.Sqrt(Math.Pow(Math.Min(diffX, startTile.Map.XDim - diffX), 2) + Math.Pow(diffY, 2));
        }
    }
}