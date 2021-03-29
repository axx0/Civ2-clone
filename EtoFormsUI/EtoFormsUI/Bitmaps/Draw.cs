using EtoFormsUIExtensionMethods;
using Eto.Drawing;
using Civ2engine;

namespace EtoFormsUI
{
    public static partial class Draw
    {
        private static Game Game => Game.Instance;
        private static Map Map => Map.Instance;

        public static void ViewPiece(Graphics g, int zoom, Point dest)
        {
            using var piecePic = Images.ViewPiece.Resize(zoom);
            g.DrawImage(piecePic, dest);
        }

        public static void Grid(Graphics g, int zoom, Point dest)
        {
            using var gridPic = Images.GridLines.Resize(zoom);
            g.DrawImage(gridPic, dest);
        }

        public static void Dither(Graphics g, int tileX, int tileY, int zoom, Point dest)
        {
            using var ditherPic = Images.DitherDots[tileX, tileY].Resize(zoom);
            g.DrawImage(ditherPic, dest);
        }

        public static void BattleAnim(Graphics g, int frame, int zoom, Point dest)
        {
            using var battlePic = Images.BattleAnim[frame].Resize(zoom);
            g.DrawImage(battlePic, dest);
        }

        public static void Checkbox(Graphics g, bool check, Point dest)
        {
            g.AntiAlias = false;
            using var _brush2 = new SolidBrush(Colors.White);
            using var _brush3 = new SolidBrush(Color.FromArgb(128, 128, 128));
            using var _penB = new Pen(Colors.Black);
            using var _penW = new Pen(Colors.White);
            using var _penG = new Pen(Color.FromArgb(192, 192, 192));
            g.FillRectangle(_brush2, new Rectangle(dest.X + 3, dest.Y + 2, 15, 17));
            g.FillRectangle(_brush2, new Rectangle(dest.X + 2, dest.Y + 3, 17, 15));
            g.FillRectangle(_brush3, new Rectangle(dest.X + 4, dest.Y + 3, 13, 15));
            g.FillRectangle(_brush3, new Rectangle(dest.X + 3, dest.Y + 4, 15, 13));
            g.DrawLine(_penB, dest.X + 4, dest.Y + 3, dest.X + 16, dest.Y + 3);
            g.DrawLine(_penB, dest.X + 3, dest.Y + 4, dest.X + 3, dest.Y + 16);
            g.DrawLine(_penB, dest.X + 3, dest.Y + 4, dest.X + 4, dest.Y + 4);
            g.DrawLine(_penB, dest.X + 4, dest.Y + 19, dest.X + 18, dest.Y + 19);
            g.DrawLine(_penB, dest.X + 18, dest.Y + 18, dest.X + 19, dest.Y + 18);
            g.DrawLine(_penB, dest.X + 19, dest.Y + 4, dest.X + 19, dest.Y + 17);

            if (check)
            {
                g.DrawLine(_penB, dest.X + 21, dest.Y + 3, dest.X + 25, dest.Y + 3);
                g.DrawLine(_penB, dest.X + 20, dest.Y + 4, dest.X + 23, dest.Y + 4);
                g.DrawLine(_penB, dest.X + 19, dest.Y + 5, dest.X + 21, dest.Y + 5);
                g.DrawLine(_penB, dest.X + 18, dest.Y + 6, dest.X + 20, dest.Y + 6);
                g.DrawLine(_penB, dest.X + 17, dest.Y + 7, dest.X + 19, dest.Y + 7);
                g.DrawLine(_penB, dest.X + 16, dest.Y + 8, dest.X + 18, dest.Y + 8);
                g.DrawLine(_penB, dest.X + 15, dest.Y + 9, dest.X + 17, dest.Y + 9);
                g.DrawLine(_penB, dest.X + 5, dest.Y + 10, dest.X + 6, dest.Y + 10);
                g.DrawLine(_penB, dest.X + 14, dest.Y + 10, dest.X + 16, dest.Y + 10);
                g.DrawLine(_penB, dest.X + 6, dest.Y + 11, dest.X + 7, dest.Y + 11);
                g.DrawLine(_penB, dest.X + 14, dest.Y + 11, dest.X + 16, dest.Y + 11);
                g.DrawLine(_penB, dest.X + 7, dest.Y + 12, dest.X + 8, dest.Y + 12);
                g.DrawLine(_penB, dest.X + 13, dest.Y + 12, dest.X + 15, dest.Y + 12);
                g.DrawLine(_penB, dest.X + 8, dest.Y + 13, dest.X + 14, dest.Y + 13);
                g.DrawLine(_penB, dest.X + 12, dest.Y + 13, dest.X + 15, dest.Y + 13);
                g.DrawLine(_penB, dest.X + 12, dest.Y + 14, dest.X + 14, dest.Y + 14);
                g.DrawLine(_penB, dest.X + 9, dest.Y + 15, dest.X + 12, dest.Y + 15);
                g.DrawLine(_penB, dest.X + 10, dest.Y + 16, dest.X + 12, dest.Y + 16);
                g.DrawLine(_penB, dest.X + 11, dest.Y + 16, dest.X + 11, dest.Y + 17);
                g.DrawLine(_penW, dest.X + 20, dest.Y + 1, dest.X + 22, dest.Y + 1);
                g.DrawLine(_penW, dest.X + 19, dest.Y + 2, dest.X + 20, dest.Y + 2);
                g.DrawLine(_penG, dest.X + 20, dest.Y + 2, dest.X + 22, dest.Y + 2);
                g.DrawLine(_penW, dest.X + 18, dest.Y + 3, dest.X + 19, dest.Y + 3);
                g.DrawLine(_penG, dest.X + 19, dest.Y + 3, dest.X + 20, dest.Y + 3);
                g.DrawLine(_penW, dest.X + 17, dest.Y + 4, dest.X + 18, dest.Y + 4);
                g.DrawLine(_penG, dest.X + 18, dest.Y + 4, dest.X + 19, dest.Y + 4);
                g.DrawLine(_penW, dest.X + 16, dest.Y + 5, dest.X + 17, dest.Y + 5);
                g.DrawLine(_penG, dest.X + 17, dest.Y + 5, dest.X + 18, dest.Y + 5);
                g.DrawLine(_penW, dest.X + 15, dest.Y + 6, dest.X + 16, dest.Y + 6);
                g.DrawLine(_penG, dest.X + 16, dest.Y + 6, dest.X + 17, dest.Y + 6);
                g.DrawLine(_penW, dest.X + 14, dest.Y + 7, dest.X + 15, dest.Y + 7);
                g.DrawLine(_penG, dest.X + 15, dest.Y + 7, dest.X + 16, dest.Y + 7);
                g.DrawLine(_penW, dest.X + 4, dest.Y + 8, dest.X + 5, dest.Y + 8);
                g.DrawLine(_penG, dest.X + 5, dest.Y + 8, dest.X + 5, dest.Y + 9);
                g.DrawLine(_penW, dest.X + 13, dest.Y + 8, dest.X + 14, dest.Y + 8);
                g.DrawLine(_penG, dest.X + 14, dest.Y + 8, dest.X + 15, dest.Y + 8);
                g.DrawLine(_penG, dest.X + 6, dest.Y + 9, dest.X + 6, dest.Y + 10);
                g.DrawLine(_penG, dest.X + 13, dest.Y + 9, dest.X + 14, dest.Y + 9);
                g.DrawLine(_penG, dest.X + 7, dest.Y + 10, dest.X + 7, dest.Y + 11);
                g.DrawLine(_penW, dest.X + 12, dest.Y + 10, dest.X + 13, dest.Y + 10);
                g.DrawLine(_penG, dest.X + 13, dest.Y + 10, dest.X + 13, dest.Y + 11);
                g.DrawLine(_penG, dest.X + 8, dest.Y + 11, dest.X + 8, dest.Y + 12);
                g.DrawLine(_penW, dest.X + 11, dest.Y + 11, dest.X + 12, dest.Y + 11);
                g.DrawLine(_penG, dest.X + 12, dest.Y + 11, dest.X + 13, dest.Y + 11);
                g.DrawLine(_penG, dest.X + 9, dest.Y + 12, dest.X + 9, dest.Y + 13);
                g.DrawLine(_penG, dest.X + 11, dest.Y + 12, dest.X + 12, dest.Y + 12);
                g.DrawLine(_penG, dest.X + 9, dest.Y + 13, dest.X + 11, dest.Y + 13);
                g.DrawLine(_penG, dest.X + 9, dest.Y + 14, dest.X + 11, dest.Y + 14);
                g.DrawLine(_penG, dest.X + 10, dest.Y + 14, dest.X + 10, dest.Y + 15);
            }
        }

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
        //public static void CreateWholeMapImage(int civ)
        //{
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
        //}

        //Redraw invisible to visible tiles in the map
        //public static void RedrawMap(int[] centralCoords)
        //{
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
        //}

        //public static List<Bitmap> CreateMapAnimation(AnimationType animation)
        //{
        //    List<Bitmap> _bitmapList = new List<Bitmap>();

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

        //    return _bitmapList;
        //}
    }
}
