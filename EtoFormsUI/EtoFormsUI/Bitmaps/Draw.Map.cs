using System.Collections.Generic;
using System.Linq;
using Eto.Drawing;
using Civ2engine;
using Civ2engine.Enums;
using Civ2engine.MapObjects;
using Civ2engine.Units;
using EtoFormsUIExtensionMethods;

namespace EtoFormsUI
{
    public static partial class Draw
    {
        /// <summary>
        /// Make a bitmap of a part of map.
        /// </summary>
        /// <param name="civ">Map view of a of tribe (tribe's id).</param>
        /// <param name="startX">Starting X-coord shown on map.</param>
        /// <param name="startY">Starting Y-coord shown on map.</param>
        /// <param name="width">Width of map shown.</param>
        /// <param name="height">Height of map shown.</param>
        /// <param name="flatEarth">Flat earth map?</param>
        /// <param name="mapRevealed">Is map revealed?</param>
        /// <param name="showActiveUnit"></param>
        /// <param name="highlightList"></param>
        /// <param name="pathDebug"></param>
        /// <returns></returns>
        public static Bitmap MapPart(int civ, int startX, int startY, int width, int height, bool flatEarth,
            bool mapRevealed, bool showActiveUnit, IList<Tile> highlightList, Dictionary<Tile, Route> pathDebug)
        {
            // Define a bitmap for drawing
            var mapPic = new Bitmap(Map.Xpx * (2 * width + 1), Map.Ypx * (height + 1), PixelFormat.Format32bppRgba);

            // Draw map
            var cityList = new List<City>(); // Make a list of cities on visible map + their locations so you can draw city names
            using var g = new Graphics(mapPic);

            var activeUnit = Game.ActiveUnit;
            
            // Draw
            for (int _row = -2; _row < height + 2; _row++)
            {
                for (int _col = -2; _col < width; _col++)
                {
                    // Determine column index in civ2-style coords
                    int col = 2 * _col + (_row % 2);
                    int row = _row;

                    // Don't draw beyond borders
                    var xC2 = startX + col;
                    var yC2 = startY + row;
                    if (xC2 < 0 || yC2 < 0 || xC2 >= Map.XDimMax || yC2 >= Map.YDim) continue;

                    var tile = Map.TileC2(xC2, yC2);
                    // Draw only if the tile is visible for each civ
                    if (!mapRevealed && !tile.Visibility[civ]) continue;

                    // Tiles
                    Draw.Tile(g, xC2, yC2, Map.Zoom, new Point(Map.Xpx * col, Map.Ypx * row));

                    // Implement dithering in all 4 directions where non-visible tiles are
                    if (!mapRevealed)
                    {
                        int[] offset = new int[] { -1, 1 };
                        for (int tileX = 0; tileX < 2; tileX++)
                        {
                            for (int tileY = 0; tileY < 2; tileY++)
                            {
                                int colNew = xC2 + offset[tileX];
                                int rowNew = yC2 + offset[tileY];
                                if (colNew >= 0 && colNew < Map.XDimMax && rowNew >= 0 && rowNew < Map.YDim)   // Don't observe outside map limits
                                {
                                    if (!Map.IsTileVisibleC2(colNew, rowNew, civ))   // Surrounding tile is not visible -> dither
                                        g.DrawImage( MapImages.Terrains[Map.MapIndex].DitherMask[tileX*2 + tileY], Map.Xpx * (col + tileX), Map.Ypx * (row + tileY));
                                }
                            }
                        }
                    }

                    // Units
                    var unitsHere = tile.UnitsHere;
                    if (unitsHere.Count > 0 && (showActiveUnit || unitsHere.All(u=>u != activeUnit)) && Map.IsCurrentlyVisible(tile))
                    {
                        Unit unit = unitsHere.Last();
                        if (unitsHere.Any(u => u.Domain == UnitGAS.Sea)) unit = unitsHere.Last(u => u.Domain == UnitGAS.Sea);   // Show naval unit
                        if (!unit.IsInCity)
                        {
                            Draw.Unit(g, unit, unitsHere.Count > 1, Map.Zoom, new Point(Map.Xpx * col, Map.Ypx * (row - 1)));
                        }
                    }

                    // Cities
                    var city = tile.CityHere;
                    if (city != null)
                    {
                        Draw.City(g, city, true, Map.Zoom, new Point(Map.Xpx * col, Map.Ypx * (row - 1)));
                        // Add city drawn on map & its position to list for drawing city names
                        cityList.Add(city);
                    }

                    if (highlightList != null && highlightList.Contains(tile))
                    {
                        Draw.ViewPiece(g, Map.Zoom, new Point(Map.Xpx * col, Map.Ypx * (row - 0)));
                    }

                    if (pathDebug != null && pathDebug.ContainsKey(tile))
                    {
                        g.DrawText(new Font("Times New Roman", 10.ZoomScale(Map.Zoom), FontStyle.Bold),Colors.Black, new Point(Map.Xpx * col + Map.Xpx , Map.Ypx * row),
                            $"{pathDebug[tile].Steps}:{pathDebug[tile].Cost}");
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

            return mapPic;
        }
    }
}
