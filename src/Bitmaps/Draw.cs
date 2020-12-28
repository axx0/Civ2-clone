using ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using civ2.Enums;
using civ2.Forms;
using civ2.Units;
using civ2.Terrains;

namespace civ2.Bitmaps
{
    public static partial class Draw
    {
        private static Game Game => Game.Instance;
        private static Map Map => Map.Instance;

        //public static void CreateLoadGameGraphics()
        //{
        //    //Creates bitmaps from current folder
        //    //LoadGraphicsAssetsFromFiles();

        //    //Create graphic of each map tile
        //    for (int col = 0; col < Data.MapXdim; col++)
        //        for (int row = 0; row < Data.MapYdim; row++)
        //            Map.Tile[col, row].Graphic = TerrainBitmap(col, row);

        //    //Add cities+units graphics to each tile & create image of maps (what each civ sees + one entire visible)
        //    Game.CivsMap = new Bitmap[9];            
        //    for (int civ = 0; civ < 9; civ++) CreateWholeMapImage(civ);  //What each civ (index=0...7) sees, additionally (index=8) for entire revealed map
        //}

        //Create image of civ's world maps
        public static void CreateWholeMapImage(int civ)
        {
            //Game.CivsMap[civ] = new Bitmap(64 * Data.MapXdim + 32, 32 * Data.MapYdim + 16);

            //using (Graphics g = Graphics.FromImage(Game.CivsMap[civ]))
            //{
            //    for (int row = 0; row < Data.MapYdim; row++)
            //    {
            //        for (int col = 0; col < Data.MapXdim; col++)
            //        {
            //            //Draw only if the tile is visible for each civ (index=8...whole map visible)
            //            if ((civ < 8 && Map.Tile[col, row].Visibility[civ]) || civ == 8)
            //            {
            //                //Tiles
            //                g.DrawImage(
            //                    Map.Tile[col, row].Graphic,
            //                    64 * col + 32 * (row % 2),
            //                    16 * row);

            //                //Implement dithering in all 4 directions if necessary
            //                if (civ != 8)
            //                    for (int tileX = 0; tileX < 2; tileX++)
            //                        for (int tileY = 0; tileY < 2; tileY++)
            //                        {
            //                            int[] offset = new int[] { -1, 1};
            //                            int col_ = 2 * col + (row % 2); //real->civ2 coords
            //                            int colNew_ = col_ + offset[tileX];
            //                            int rowNew = row + offset[tileY];
            //                            int colNew = (colNew_ - rowNew % 2) / 2; //back to real coords
            //                            if (colNew >= 0 && colNew < Data.MapXdim && rowNew >= 0 && rowNew < Data.MapYdim)   //don't observe outside map limits
            //                                if (!Map.Tile[colNew, rowNew].Visibility[civ])   //surrounding tile is not visible -> dither
            //                                    g.DrawImage(DitherDots[tileX, tileY],
            //                                                64 * col + 32 * (row % 2) + 32 * tileX,
            //                                                16 * row + 16 * tileY);
            //                        }

            //                //Units
            //                int[] coords = Ext.XYciv2(new int[] { col, row });  //civ2 coords from real coords
            //                int col2 = coords[0];
            //                int row2 = coords[1];
            //                List<IUnit> unitsHere = Game.Units.Where(u => u.X == col2 && u.Y == row2).ToList();
            //                if (unitsHere.Any())
            //                {
            //                    IUnit unit = unitsHere.Last();
            //                    int zoom = 8;
            //                    if (!unit.IsInCity)
            //                    {
            //                        g.DrawImage(
            //                            CreateUnitBitmap(unit, unitsHere.Count() > 1, zoom),
            //                            32 * col2,
            //                            16 * row2 - 16);
            //                    }
            //                }

            //                //Cities
            //                City city = Game.Cities.Find(c => c.X == col2 && c.Y == row2);
            //                if (city != null)
            //                {
            //                    g.DrawImage(
            //                        CreateCityBitmap(city, true, 8),
            //                        32 * col2,
            //                        16 * row2 - 16);
            //                }
            //            }                            
            //        }
            //    }

            //    //City name text is drawn last
            //    foreach (City city in Game.Cities)
            //    {
            //        int[] ColRow = Ext.Civ2xy(new int[] { city.X, city.Y });  //real coords from civ2 coords
            //        if ((civ < 8 && Map.Tile[ColRow[0], ColRow[1]].Visibility[civ]) || civ == 8)
            //        {
            //            Bitmap cityNameBitmap = CreateCityNameBitmap(city, 8);
            //            g.DrawImage(
            //                cityNameBitmap,
            //                32 * city.X + 32 - cityNameBitmap.Width / 2,
            //                16 * city.Y + 3 * 8);
            //        }
            //    }
            //}
        }

        //Redraw invisible to visible tiles in the map
        public static void RedrawMap(int[] centralCoords)
        {
            //Bitmap mapPart = new Bitmap(3 * 64, 3 * 32 + 16);   //part which will be pasted on top of existing world map

            ////Get coords of central tile & which squares are to be drawn
            //List<int[]> coordsOffsetsToBeDrawn = new List<int[]>
            //{
            //    new int[] {-2, -4},
            //    new int[] {0, -4},
            //    new int[] {2, -4},
            //    new int[] {-3, -3},
            //    new int[] {-1, -3},
            //    new int[] {1, -3},
            //    new int[] {3, -3},
            //    new int[] {-2, -2},
            //    new int[] {0, -2},
            //    new int[] {2, -2},
            //    new int[] {-3, -1},
            //    new int[] {-1, -1},
            //    new int[] {1, -1},
            //    new int[] {3, -1},
            //    new int[] {-2, 0},
            //    new int[] {0, 0},
            //    new int[] {2, 0},
            //    new int[] {-3, 1},
            //    new int[] {-1, 1},
            //    new int[] {1, 1},
            //    new int[] {3, 1},
            //    new int[] {-2, 2},
            //    new int[] {0, 2},
            //    new int[] {2, 2},
            //    new int[] {-3, 3},
            //    new int[] {-1, 3},
            //    new int[] {1, 3},
            //    new int[] {3, 3},
            //    new int[] {-2, 4},
            //    new int[] {0, 4},
            //    new int[] {2, 4}
            //};

            //int zoom = 8;

            //using (Graphics g = Graphics.FromImage(mapPart))
            //{
            //    g.FillRectangle(Brushes.Black, new Rectangle(0, 0, 3 * 64, 3 * 32 + 16));    //fill bitmap with black (necessary for correct drawing if image is on upper map edge)

            //    foreach (int[] coordsOffsets in coordsOffsetsToBeDrawn)
            //    {
            //        //change coords of central offset
            //        int xCiv2 = centralCoords[0] + coordsOffsets[0];
            //        int yCiv2 = centralCoords[1] + coordsOffsets[1];
            //        int xReal = (xCiv2 - yCiv2 % 2) / 2;
            //        int yReal = yCiv2;

            //        if (xCiv2 >= 0 && yCiv2 >= 0 && xCiv2 < 2 * Data.MapXdim && yCiv2 < Data.MapYdim)    //make sure you're not drawing tiles outside map bounds
            //        {
            //            //Tiles
            //            int civId = Game.Instance.ActiveCiv.Id;
            //            if ((civId < 8 && Map.Tile[xReal, yReal].Visibility[civId]) || civId == 8)
            //            {
            //                g.DrawImage(
            //                    Images.TerrainBitmap(xReal, yReal),
            //                    32 * coordsOffsets[0] + 64,
            //                    16 * coordsOffsets[1] + 48);

            //                //Implement dithering in all 4 directions if necessary
            //                if (civId != 8)
            //                    for (int tileX = 0; tileX < 2; tileX++)
            //                        for (int tileY = 0; tileY < 2; tileY++)
            //                        {
            //                            int[] offset = new int[] { -1, 1 };
            //                            int xCiv2New = xCiv2 + offset[tileX];
            //                            int yCiv2New = yCiv2 + offset[tileY];
            //                            int xRealNew = (xCiv2New - yCiv2New % 2) / 2; //back to real coords
            //                            int yRealNew = yCiv2New;
            //                            if (xRealNew >= 0 && xRealNew < Data.MapXdim && yRealNew >= 0 && yRealNew < Data.MapYdim)   //don't observe outside map limits
            //                                if (!Map.Tile[xRealNew, yRealNew].Visibility[civId])   //surrounding tile is not visible -> dither
            //                                    g.DrawImage(Images.DitherDots[tileX, tileY],
            //                                                32 * coordsOffsets[0] + 64 + 32 * tileX,
            //                                                16 * coordsOffsets[1] + 48 + 16 * tileY);
            //                        }
            //            }

            //            //Units
            //            List<IUnit> unitsHere = Game.Units.Where(u => u.X == xCiv2 && u.Y == yCiv2).ToList();
            //            //If active unit is in this list-- > remove it
            //            if (unitsHere.Contains(Game.Instance.ActiveUnit))
            //            {
            //                unitsHere.Remove(Game.Instance.ActiveUnit);
            //            }
            //            if (unitsHere.Any())
            //            {
            //                IUnit unit;
            //                //If this is not tile with active unit or viewing piece, draw last unit on stack
            //                if (!unitsHere.Contains(Game.Instance.ActiveUnit))
            //                {
            //                    unit = unitsHere.Last();
            //                    if (!unit.IsInCity)
            //                    {
            //                        g.DrawImage(
            //                            Images.CreateUnitBitmap(unit, unitsHere.Count() > 1, zoom),
            //                            32 * coordsOffsets[0] + 64,
            //                            16 * coordsOffsets[1] + 32);
            //                    }
            //                }
            //                //This tile has active unit/viewing piece
            //                else
            //                {
            //                    //Viewing pieces mode is enabled, so draw last unit on stack
            //                    if (MapPanel.ViewPiecesMode)
            //                    {
            //                        unit = unitsHere.Last();
            //                        if (!unit.IsInCity)
            //                        {
            //                            g.DrawImage(
            //                                Images.CreateUnitBitmap(unit, unitsHere.Count() > 1, zoom),
            //                                32 * coordsOffsets[0] + 64,
            //                                16 * coordsOffsets[1] + 32);
            //                        }
            //                    }
            //                }
            //            }

            //            //Cities
            //            City city = Game.Cities.Find(c => c.X == xCiv2 && c.Y == yCiv2);
            //            if (city != null)
            //            {
            //                g.DrawImage(
            //                    Images.CreateCityBitmap(city, true, 8),
            //                    32 * coordsOffsets[0] + 64,
            //                    16 * coordsOffsets[1] + 32);
            //            }
            //        }
            //    }

            //    //City names
            //    //Add additional coords for drawing city names
            //    coordsOffsetsToBeDrawn.Add(new int[] { -3, -5 });
            //    coordsOffsetsToBeDrawn.Add(new int[] { -1, -5 });
            //    coordsOffsetsToBeDrawn.Add(new int[] { 1, -5 });
            //    coordsOffsetsToBeDrawn.Add(new int[] { 3, -5 });
            //    coordsOffsetsToBeDrawn.Add(new int[] { -4, -2 });
            //    coordsOffsetsToBeDrawn.Add(new int[] { 4, -2 });
            //    coordsOffsetsToBeDrawn.Add(new int[] { -4, 0 });
            //    coordsOffsetsToBeDrawn.Add(new int[] { 4, 0 });
            //    coordsOffsetsToBeDrawn.Add(new int[] { -4, 2 });
            //    coordsOffsetsToBeDrawn.Add(new int[] { 4, 2 });
            //    foreach (int[] coordsOffsets in coordsOffsetsToBeDrawn)
            //    {
            //        //change coords of central offset
            //        int x = centralCoords[0] + coordsOffsets[0];
            //        int y = centralCoords[1] + coordsOffsets[1];

            //        if (x >= 0 && y >= 0 && x < 2 * Data.MapXdim && y < Data.MapYdim)    //make sure you're not drawing tiles outside map bounds
            //        {
            //            City city = Game.Cities.Find(c => c.X == x && c.Y == y);
            //            if (city != null)
            //            {
            //                Bitmap cityNameBitmap = Images.CreateCityNameBitmap(city, 8);
            //                g.DrawImage(
            //                    cityNameBitmap,
            //                    32 * coordsOffsets[0] + 64 + 32 - cityNameBitmap.Width / 2,
            //                    16 * coordsOffsets[1] + 3 * 8 + 48);
            //            }
            //        }
            //    }
            //}
        }

        public static List<Bitmap> CreateMapAnimation(AnimationType animation)
        {
            List<Bitmap> _bitmapList = new List<Bitmap>();

            //switch (animation)
            //{
            //    case AnimationType.UnitWaiting:
            //        {
            //            for (int i = 0; i < 2; i++)
            //            {
            //                Bitmap _bitmap = new Bitmap(64, 32);
            //                using (Graphics g = Graphics.FromImage(_bitmap))
            //                {
            //                    if (MapPanel.ViewPiecesMode)
            //                    {

            //                    }
            //                    else    //unit is active
            //                    {
            //                        int x = Game.Instance.ActiveUnit.X;
            //                        int y = Game.Instance.ActiveUnit.Y;
            //                        g.DrawImage(Game.CivsMap[Game.Instance.ActiveCiv.Id], new Rectangle(32 * x, 16 * y, 64, 48));
            //                        if (i == 1) g.DrawImage(Game.Instance.ActiveUnit.GraphicMapPanel, 0, 16);
            //                    }
            //                }
            //                _bitmapList.Add(_bitmap);
            //            }
            //            break;
            //        }
            //}

            return _bitmapList;
        }
    }
}
