using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtensionMethods
{
    public static class Ext
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

        //Convert civ2-style XY coordinates to real XY coordinates
        public static int[] Civ2xy(this int[] coordXY)
        {
            int[] newcoordXY = { (coordXY[0] - coordXY[1] % 2) / 2, coordXY[1] };
            return newcoordXY;
        }

        //Convert real XY coordinates to civ2 XY coordinates
        public static int[] XYciv2(int[] coordXY)
        {
            int[] newcoordXY = { 2 * coordXY[0] + (coordXY[1] % 2), coordXY[1] };
            return newcoordXY;
        }
    }
}
