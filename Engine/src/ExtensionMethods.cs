using System;

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
        public static int[] Civ2Xy(this int[] coordXy)
        {
            return new int[] { (coordXy[0] - coordXy[1] % 2) / 2, coordXy[1] };
        }

        // Convert real XY coordinates to civ2 XY coordinates
        public static int[] XYciv2(this int[] coordXy)
        {
            return new int[] { 2 * coordXy[0] + (coordXy[1] % 2), coordXy[1] };
        }

        // Return position of "i" relative to zoom
        public static int ZoomScale(this int i, int zoom)
        {
            return (int)((8.0 + (float)zoom) / 8.0 * i);
        }
    }
}
