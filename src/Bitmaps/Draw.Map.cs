using System;
using System.Drawing;
using ExtensionMethods;
using civ2.Units;
using System.Collections.Generic;
using System.Linq;

namespace civ2.Bitmaps
{
    public static partial class Draw
    {
        // civId
        // = 0...7 for 8 civs (including barbarians with civId=7)
        // = 8 for revealed map
        public static Bitmap DrawMap(int civ, bool flatEarth)   // Draw for normal zoom level
        {
            // Define a bitmap for drawing
            var mapPic = new Bitmap(32 * (2 * Map.Xdim + 1), 16 * (2 * Map.Ydim + 1));

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
                        if ((civ < 8 && Map.IsTileVisibleC2(col, row, civ)) || civ == 8)
                        {
                            // Tiles
                            g.DrawImage(Map.TileC2(col, row).Graphic, 32 * col, 16 * row);

                            // Implement dithering in all 4 directions where non-visible tiles are
                            if (civ != 8)
                            {
                                int[] offset = new int[] { -1, 1 };
                                for (int tileX = 0; tileX < 2; tileX++)
                                {
                                    for (int tileY = 0; tileY < 2; tileY++)
                                    {
                                        int colNew = col + offset[tileX];
                                        int rowNew = row + offset[tileY];
                                        if (colNew >= 0 && colNew < 2 * Map.Xdim && rowNew >= 0 && rowNew < Map.Ydim)   // Don't observe outside map limits
                                        {
                                            if (!Map.IsTileVisibleC2(colNew, rowNew, civ))   // Surrounding tile is not visible -> dither
                                                g.DrawImage(Images.DitherDots[tileX, tileY], 32 * (col + tileX), 16 * (row + tileY));
                                        }
                                    }
                                }
                            }

                            // Units
                            List<IUnit> unitsHere = Game.GetUnits.Where(u => u.X == col && u.Y == row).ToList();
                            if (unitsHere.Count > 0)
                            {
                                IUnit unit = unitsHere.Last();
                                if (!unit.IsInCity)
                                {
                                    Draw.Unit(g, unit, unitsHere.Count > 1, zoom, new Point(32 * col, 16 * row - 16));
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
                    if ((civ < 8 && Map.IsTileVisibleC2(city.X, city.Y, civ)) || civ == 8)
                    {
                        //Bitmap cityNameBitmap = Draw.CityName(city, zoom);
                        //g.DrawImage(cityNameBitmap, 32 * city.X + 32 - (cityNameBitmap.Width / 2), 16 * city.Y + 3 * 8);
                    }
                }
            }

            return mapPic;
        }

        public static Bitmap MapPart(int civ, int startX, int startY, int width, int height, int zoom, bool flatEarth, bool drawGrid)
        {
            // Define a bitmap for drawing
            var mapPic = new Bitmap(Game.Xpx * (2 * width + 1), Game.Ypx * (height + 1));

            // Draw map
            List<Tuple<City, Point>> cityList = new List<Tuple<City, Point>>(); // Make a list of cities on visible map + their locations so you can draw city names
            using (var g = Graphics.FromImage(mapPic))
            {
                // Draw
                for (int _row = 0; _row < height; _row++)
                {
                    for (int _col = 0; _col < width; _col++)
                    {
                        // Determine column index in civ2-style coords
                        int col = 2 * _col + (_row % 2);
                        int row = _row;

                        // Draw only if the tile is visible for each civ (index=8...whole map visible)
                        if ((civ < 8 && Map.IsTileVisibleC2(startX + col, startY + row, civ)) || civ == 8)
                        {
                            // Tiles
                            g.DrawImage(Map.TileC2(startX + col, startY + row).GraphicZoom(zoom), 4 * (8 + zoom) * col, 2 * (8 + zoom) * row);

                            // Implement dithering in all 4 directions where non-visible tiles are
                            if (civ != 8)
                            {
                                int[] offset = new int[] { -1, 1 };
                                for (int tileX = 0; tileX < 2; tileX++)
                                {
                                    for (int tileY = 0; tileY < 2; tileY++)
                                    {
                                        int colNew = col + offset[tileX];
                                        int rowNew = row + offset[tileY];
                                        if (colNew >= 0 && colNew < 2 * Map.Xdim && rowNew >= 0 && rowNew < Map.Ydim)   // Don't observe outside map limits
                                        {
                                            if (!Map.IsTileVisibleC2(colNew, rowNew, civ))   // Surrounding tile is not visible -> dither
                                                g.DrawImage(Images.DitherDots[tileX, tileY], 4 * (8 + zoom) * (col + tileX), 2 * (8 + zoom) * (row + tileY));
                                        }
                                    }
                                }
                            }

                            // Units
                            List<IUnit> unitsHere = Game.GetUnits.Where(u => u.X == startX + col && u.Y == startY + row).ToList();
                            if (unitsHere.Count > 0)
                            {
                                IUnit unit = unitsHere.Last();
                                if (!unit.IsInCity)
                                {
                                    Draw.Unit(g, unit, unitsHere.Count > 1, zoom, new Point(4 * (8 + zoom) * col, 2 * (8 + zoom) * (row - 1)));
                                }
                            }

                            // Cities
                            City city = Game.GetCities.Find(c => c.X == startX + col && c.Y == startY + row);
                            if (city != null)
                            {
                                g.DrawImage(Draw.City(city, true, zoom), 4 * (8 + zoom) * col, 2 * (8 + zoom) * (row - 1));
                                // Add city drawn on map & its position to list for drawing city names
                                cityList.Add(new Tuple<City, Point>(city, new Point(4 * (8 + zoom) * col + 4 * (8 + zoom), 2 * (8 + zoom) * (row + 2) + 2)));
                            }
                        }
                    }
                }

                // Grid
                if (drawGrid)
                {
                    for (int _row = 0; _row < height; _row++)
                    {
                        for (int _col = 0; _col < width; _col++)
                        {
                            // Determine column index in civ2-style coords
                            int col = 2 * _col + (_row % 2);
                            int row = _row;

                            // Draw only if the tile is visible for each civ (index=8...whole map visible)
                            if ((civ < 8 && Map.IsTileVisibleC2(startX + col, startY + row, civ)) || civ == 8)
                            {
                                g.DrawImage(ModifyImage.ResizeImage(Images.GridLines, zoom), 4 * (8 + zoom) * col, 2 * (8 + zoom) * row);
                            }
                        }
                    }
                }

                // City name text is drawn last
                foreach (Tuple<City, Point> city in cityList)
                {
                    Draw.CityName(g, city.Item1, zoom, city.Item2);
                }
            }

            return mapPic;
        }
    }
}
