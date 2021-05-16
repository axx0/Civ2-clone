using System;
using Civ2engine.Enums;
using Civ2engine.Units;

namespace ExtensionMethods
{
    public static class ExtensionMethods
    {
        public static T[] RemoveAt<T>(this T[] source, int index)
        {
            T[] dest = new T[source.Length - 1];
            if (index > 0)
                Array.Copy(source, 0, dest, 0, index);

            if (index < source.Length - 1)
                Array.Copy(source, index + 1, dest, index, source.Length - index - 1);

            return dest;
        }

        // Convert civ2-style XY coordinates to real XY coordinates
        public static int[] Civ2xy(this int[] coordXY)
        {
            return new int[] { (coordXY[0] - coordXY[1] % 2) / 2, coordXY[1] };
        }

        // Convert real XY coordinates to civ2 XY coordinates
        public static int[] XYciv2(this int[] coordXY)
        {
            return new int[] { 2 * coordXY[0] + (coordXY[1] % 2), coordXY[1] };
        }

        // Return position of "i" relative to zoom
        public static int ZoomScale(this int i, int zoom)
        {
            return (int)((8.0 + (float)zoom) / 8.0 * i);
        }

        // Return new unit coords based on movement direction
        public static int[] NewUnitCoords(this IUnit unit, OrderType movementDir)
        {
            int[] deltaXY = new int[] { 0, 0 };
            switch (movementDir)
            {
                case OrderType.MoveSW: deltaXY = new int[] { -1, 1 }; break;
                case OrderType.MoveS: deltaXY = new int[] { 0, 2 }; break;
                case OrderType.MoveSE: deltaXY = new int[] { 1, 1 }; break;
                case OrderType.MoveE: deltaXY = new int[] { 2, 0 }; break;
                case OrderType.MoveNE: deltaXY = new int[] { 1, -1 }; break;
                case OrderType.MoveN: deltaXY = new int[] { 0, -2 }; break;
                case OrderType.MoveNW: deltaXY = new int[] { -1, -1 }; break;
                case OrderType.MoveW: deltaXY = new int[] { -2, 0 }; break;
            }
            return new int[] { unit.X + deltaXY[0], unit.Y + deltaXY[1] };
        }
    }
}
