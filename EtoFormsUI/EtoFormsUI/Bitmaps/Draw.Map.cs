using System.Collections.Generic;
using System.Linq;
using Eto.Drawing;
using Civ2engine;
using Civ2engine.Units;

namespace EtoFormsUI
{
    public static partial class Draw
    {
        public static Bitmap MapPart(int civ, int startX, int startY, int width, int height, bool flatEarth, bool mapRevealed)
        {
            // Define a bitmap for drawing
            var mapPic = new Bitmap(Map.Xpx * (2 * width + 1), Map.Ypx * (height + 1), PixelFormat.Format32bppRgba);

            // Draw map
            var cityList = new List<City>(); // Make a list of cities on visible map + their locations so you can draw city names
            using (var g = new Graphics(mapPic))
            {
                // Draw
                for (int _row = -2; _row < height + 2; _row++)
                {
                    for (int _col = -2; _col < width; _col++)
                    {
                        // Determine column index in civ2-style coords
                        int col = 2 * _col + (_row % 2);
                        int row = _row;

                        // Don't draw beyond borders
                        if (startX + col < 0 || startY + row < 0 || startX + col >= 2 * Map.Xdim || startY + row >= Map.Ydim) continue;

                        // Draw only if the tile is visible for each civ
                        if (!Map.IsTileVisibleC2(startX + col, startY + row, civ) && !mapRevealed) continue;

                        // Tiles
                        Draw.Tile(g, startX + col, startY + row, Map.Zoom, new Point(Map.Xpx * col, Map.Ypx * row));

                        // Implement dithering in all 4 directions where non-visible tiles are
                        if (!mapRevealed)
                        {
                            int[] offset = new int[] { -1, 1 };
                            for (int tileX = 0; tileX < 2; tileX++)
                            {
                                for (int tileY = 0; tileY < 2; tileY++)
                                {
                                    int colNew = startX + col + offset[tileX];
                                    int rowNew = startY + row + offset[tileY];
                                    if (colNew >= 0 && colNew < 2 * Map.Xdim && rowNew >= 0 && rowNew < Map.Ydim)   // Don't observe outside map limits
                                    {
                                        if (!Map.IsTileVisibleC2(colNew, rowNew, civ))   // Surrounding tile is not visible -> dither
                                            g.DrawImage(Images.DitherDots[tileX, tileY], Map.Xpx * (col + tileX), Map.Ypx * (row + tileY));
                                    }
                                }
                            }
                        }

                        // Units
                        List<IUnit> unitsHere = Game.UnitsHere(startX + col, startY + row);
                        if (unitsHere.Count > 0)
                        {
                            IUnit unit = unitsHere.Last();
                            //if (unitsHere.Contains(Game.GetActiveUnit) && Main.ViewPieceMode)
                            //{

                            //}
                            if (!unit.IsInCity)
                            {
                                Draw.Unit(g, unit, unitsHere.Count > 1, Map.Zoom, new Point(Map.Xpx * col, Map.Ypx * (row - 1)));
                            }
                        }

                        // Cities
                        City city = Game.CityHere(startX + col, startY + row);
                        if (city != null)
                        {
                            Draw.City(g, city, true, Map.Zoom, new Point(Map.Xpx * col, Map.Ypx * (row - 1)));
                            // Add city drawn on map & its position to list for drawing city names
                            cityList.Add(city);
                        }
                    }
                }

                // Grid
                if (Game.Options.Grid)
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
                                Draw.Grid(g, Map.Zoom, new Point(Map.Xpx * col, Map.Ypx * row));
                                //g.DrawImage(ModifyImage.Resize(Images.GridLines, zoom), 4 * (8 + zoom) * col, 2 * (8 + zoom) * row);
                            }
                        }
                    }
                }

                // City name text is drawn last
                foreach (City city in cityList)
                {
                    int[] destSq = new int[] { city.X - startX, city.Y - startY };
                    Draw.CityName(g, city, Map.Zoom, destSq);
                }
            }

            return mapPic;
        }
    }
}
