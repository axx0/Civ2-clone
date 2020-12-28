using civ2.Enums;
using System.Collections.Generic;
using System.Drawing;

namespace civ2.Bitmaps
{
    public static class SpriteDict
    {
        public static Dictionary<UnitType, Rectangle> Units;
        public static Dictionary<UnitType, Point> UnitsShieldLoc;

        public static void Set()
        {
            // Set unit sprite locations
            Units = new Dictionary<UnitType, Rectangle>();
            UnitsShieldLoc = new Dictionary<UnitType, Point>();
            int count = 0;
            int x_loc, y_loc;
            for (int row = 0; row < 7; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    // Set x-y starting location of unit
                    Units.Add((UnitType)count, new Rectangle((64 * col) + 1 + col, (48 * row) + 1 + row, 64, 48));

                    // Determine where the unit shield is located (x-y), pixel must be blue color
                    // x-direction
                    x_loc = 0;
                    for (int ix = 0; ix < 64; ix++)
                        if (Images.UnitSpritemap.GetPixel((65 * col) + ix, 49 * row) == Color.FromArgb(0, 0, 255)) x_loc = ix;
                    // y-direction
                    y_loc = 0;
                    for (int iy = 0; iy < 48; iy++)
                        if (Images.UnitSpritemap.GetPixel(65 * col, (49 * row) + iy) == Color.FromArgb(0, 0, 255)) y_loc = iy;
                    UnitsShieldLoc.Add((UnitType)count, new Point(x_loc, y_loc));

                    count++;
                }
            }
        }
    }
}
