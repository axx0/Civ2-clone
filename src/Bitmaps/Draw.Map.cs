using System.Drawing;
using ExtensionMethods;
using civ2.Units;
using System.Collections.Generic;
using System.Linq;

namespace civ2.Bitmaps
{
    public partial class Draw
    {
        // civId
        // = 0...7 for 8 civs (including barbarians with civId=7)
        // = 8 for revealed map
        public static Bitmap DrawMap(int civ)
        {
            // Define a bitmap for drawing
            Bitmap mapPic = new Bitmap(64 * Map.Xdim + 32, 32 * Map.Ydim + 16);

            // Draw map
            int zoom = 8;   // Default zoom
            using (Graphics g = Graphics.FromImage(mapPic))
            {
                for (int row = 0; row < Map.Ydim; row++)
                {
                    for (int col = 0; col < Map.Xdim; col++)
                    {
                        // Draw only if the tile is visible for each civ (index=8...whole map visible)
                        if ((civ < 8 && Map.Visibility[col, row][civ]) || civ == 8)
                        {
                            // Tiles
                            g.DrawImage(
                                Map.Tile[col, row].Graphic,
                                64 * col + 32 * (row % 2),
                                16 * row);

                            // Implement dithering in all 4 directions if necessary
                            if (civ != 8)
                                for (int tileX = 0; tileX < 2; tileX++)
                                    for (int tileY = 0; tileY < 2; tileY++)
                                    {
                                        int[] offset = new int[] { -1, 1 };
                                        int col_ = 2 * col + (row % 2);     // Real->civ2 coords
                                        int colNew_ = col_ + offset[tileX];
                                        int rowNew = row + offset[tileY];
                                        int colNew = (colNew_ - rowNew % 2) / 2;    // Back to real coords
                                        if (colNew >= 0 && colNew < Map.Xdim && rowNew >= 0 && rowNew < Map.Ydim)   // Don't observe outside map limits
                                            if (!Map.Visibility[colNew, rowNew][civ])   // Surrounding tile is not visible -> dither
                                                g.DrawImage(Images.DitherDots[tileX, tileY],
                                                            64 * col + 32 * (row % 2) + 32 * tileX,
                                                            16 * row + 16 * tileY);
                                    }


                            // Units
                            int[] coords = Ext.XYciv2(new int[] { col, row });  // civ2 coords from real coords
                            int col2 = coords[0];
                            int row2 = coords[1];
                            List<IUnit> unitsHere = Game.GetUnits.Where(u => u.X == col2 && u.Y == row2).ToList();
                            if (unitsHere.Any())
                            {
                                IUnit unit = unitsHere.Last();
                                if (!unit.IsInCity)
                                {
                                    g.DrawImage(
                                        Draw.Unit(unit, unitsHere.Count() > 1, zoom),
                                        32 * col2,
                                        16 * row2 - 16);
                                }
                            }

                            // Cities
                            City city = Game.GetCities.Find(c => c.X == col2 && c.Y == row2);
                            if (city != null)
                            {
                                g.DrawImage(
                                    Draw.City(city, true, zoom),
                                    32 * col2,
                                    16 * row2 - 16);
                            }
                        }
                    }
                }

                // City name text is drawn last
                foreach (City city in Game.GetCities)
                {
                    int[] ColRow = Ext.Civ2xy(new int[] { city.X, city.Y });  // Real coords from civ2 coords
                    if ((civ < 8 && Map.Visibility[ColRow[0], ColRow[1]][civ]) || civ == 8)
                    {
                        Bitmap cityNameBitmap = Draw.CityName(city, zoom);
                        g.DrawImage(
                            cityNameBitmap,
                            32 * city.X + 32 - cityNameBitmap.Width / 2,
                            16 * city.Y + 3 * 8);
                    }
                }
            }

            return mapPic;
        }
    }
}
