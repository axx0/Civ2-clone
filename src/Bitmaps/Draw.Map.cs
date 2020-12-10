using System;
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
        public static Bitmap DrawMap(int civ, bool flatEarth)
        {
            // Define a bitmap for drawing
            Bitmap mapPic = new Bitmap(64 * Map.Xdim + 32, 32 * Map.Ydim + 16);

            // Draw map
            int zoom = 0;   // Default zoom
            using (Graphics g = Graphics.FromImage(mapPic))
            {
                // Define starting and ending coords for drawing
                int colStart, colEnd;
                if (flatEarth)
                {
                    colStart = 0;
                    colEnd = Map.Xdim;
                }
                else    // Round world --> go few tiles beyond E and W borders in order to fill up blank space at map edge
                {
                    colStart = -2;
                    colEnd = Map.Xdim + 2;
                }

                // Draw
                for (int _row = 0; _row < Map.Ydim; _row++)
                {
                    for (int _col = colStart; _col < colEnd; _col++)
                    {
                        // Determine column index in civ2-style coords
                        int col = 2 * _col + (_row % 2);
                        int row = _row;

                        // Draw only if the tile is visible for each civ (index=8...whole map visible)
                        if ((civ < 8 && Map.VisibilityC2(col, row, civ)) || civ == 8)
                        {
                            // Tiles
                            g.DrawImage(Map.TileC2(col, row).Graphic, 32 * col, 16 * row);

                            // Implement dithering in all 4 directions where non-visible tiles are
                            if (civ != 8)
                            {
                                int[] offset = new int[] { -1, 1 };
                                for (int tileX = 0; tileX < 2; tileX++)
                                    for (int tileY = 0; tileY < 2; tileY++)
                                    {
                                        int colNew = col + offset[tileX];
                                        int rowNew = row + offset[tileY];
                                        if (colNew >= 0 && colNew < 2 * Map.Xdim && rowNew >= 0 && rowNew < Map.Ydim)   // Don't observe outside map limits
                                            if (!Map.VisibilityC2(colNew, rowNew, civ))   // Surrounding tile is not visible -> dither
                                                g.DrawImage(Images.DitherDots[tileX, tileY], 32 * (col + tileX), 16 * (row + tileY));
                                    }
                            }

                            // Units
                            List<IUnit> unitsHere = Game.GetUnits.Where(u => u.X == col && u.Y == row).ToList();
                            if (unitsHere.Any())
                            {
                                IUnit unit = unitsHere.Last();
                                if (!unit.IsInCity)
                                {
                                    g.DrawImage(Draw.Unit(unit, unitsHere.Count() > 1, zoom), 32 * col, 16 * row - 16);
                                }
                            }

                            // Cities
                            City city = Game.GetCities.Find(c => c.X == col && c.Y == row);
                            if (city != null)
                            {
                                g.DrawImage(Draw.City(city, true, zoom), 32 * col, 16 * row - 16);
                            }
                        }
                    }
                }

                // City name text is drawn last
                foreach (City city in Game.GetCities)
                {
                    //int[] ColRow = Ext.Civ2xy(new int[] { city.X, city.Y });  // Real coords from civ2 coords
                    if ((civ < 8 && Map.VisibilityC2(city.X, city.Y, civ)) || civ == 8)
                    {
                        Bitmap cityNameBitmap = Draw.CityName(city, zoom);
                        g.DrawImage(cityNameBitmap, 32 * city.X + 32 - cityNameBitmap.Width / 2, 16 * city.Y + 3 * 8);
                    }
                }
            }

            return mapPic;
        }
    }
}
