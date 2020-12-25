using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using civ2.Units;

namespace civ2.Bitmaps
{
    public class GetAnimationFrames : BaseInstance
    {
        // Get animation frames for waiting unit
        public static List<Bitmap> UnitWaiting(bool viewPieceMode)
        {
            List<Bitmap> animationFrames = new List<Bitmap>();

            // Get coords of central tile & which squares are to be drawn
            int[] centralCoords = Game.ActiveXY;   // Either from active unit or moving pieces
            List<int[]> coordsOffsetsToBeDrawn = new List<int[]>
            {
                new int[] {0, -2},
                new int[] {-1, -1},
                new int[] {1, -1},
                new int[] {0, 0},
                new int[] {-1, 1},
                new int[] {1, 1},
                new int[] {0, 2}
            };

            // Get 2 frames (one with and other without the active unit/moving piece)
            for (int frame = 0; frame < 2; frame++)
            {
                Bitmap _bitmap = new Bitmap(2 * Game.Xpx, 3 * Game.Ypx);
                using (Graphics g = Graphics.FromImage(_bitmap))
                {
                    // Fill bitmap with black (necessary for correct drawing if image is on upper map edge)
                    g.FillRectangle(Brushes.Black, new Rectangle(0, 0, 2 * Game.Xpx, 3 * Game.Ypx));

                    foreach (int[] coordsOffsets in coordsOffsetsToBeDrawn)
                    {
                        // Change coords of central offset
                        int x = centralCoords[0] + coordsOffsets[0];
                        int y = centralCoords[1] + coordsOffsets[1];
                        int[] coordsOffsetsPx = new int[] { coordsOffsets[0] * Game.Xpx, coordsOffsets[1] * Game.Ypx };

                        if (x < 0 || y < 0 || x >= 2 * Map.Xdim || y >= Map.Ydim) break;    // Make sure you're not drawing tiles outside map bounds

                        // Tiles
                        g.DrawImage(ModifyImage.ResizeImage(Map.TileC2(x, y).Graphic, Game.Zoom), coordsOffsetsPx[0], coordsOffsetsPx[1] + Game.Ypx);

                        // Units
                        List<IUnit> unitsHere = Game.GetUnits.Where(u => u.X == x && u.Y == y).ToList();
                        if (unitsHere.Count > 0)
                        {
                            IUnit unit;
                            // If this is not tile with active unit or viewing piece, draw last unit on stack
                            if (!(x == Game.ActiveXY[0] && y == Game.ActiveXY[1]))
                            {
                                unit = unitsHere.Last();
                                if (!unit.IsInCity)
                                    g.DrawImage(unit.Graphic(unitsHere.Count > 1, Game.Zoom), coordsOffsetsPx[0], coordsOffsetsPx[1]);
                            }
                            // This tile has active unit/viewing piece
                            else
                            {
                                // Viewing piece mode is enabled, so draw last unit on stack
                                if (viewPieceMode)
                                {
                                    unit = unitsHere.Last();
                                    if (!unit.IsInCity)
                                        g.DrawImage(unit.Graphic(unitsHere.Count > 1, Game.Zoom), coordsOffsetsPx[0], coordsOffsetsPx[1]);
                                }
                            }
                        }

                        // City
                        City city = Game.GetCities.Find(c => c.X == x && c.Y == y);
                        if (city != null)
                            g.DrawImage(city.Graphic(true, Game.Zoom), coordsOffsetsPx[0], coordsOffsetsPx[1]);

                        // Draw active unit if it's not moving
                        if (unitsHere.Count > 0)
                        {
                            // This tile has active unit/viewing piece
                            if (x == Game.ActiveXY[0] && y == Game.ActiveXY[1])
                            {
                                if (!viewPieceMode)
                                {
                                    if (frame == 0) // For first frame draw unit, for second not
                                        g.DrawImage(Game.ActiveUnit.Graphic(unitsHere.Count > 1, Game.Zoom), coordsOffsetsPx[0], coordsOffsetsPx[1]);
                                }
                            }
                        }
                    }

                    // City names
                    foreach (int[] coordsOffsets in coordsOffsetsToBeDrawn)
                    {
                        // Change coords of central offset
                        int x = centralCoords[0] + coordsOffsets[0];
                        int y = centralCoords[1] + coordsOffsets[1];

                        if (x >= 0 && y >= 0 && x < 2 * Map.Xdim && y < Map.Ydim) break;   // Make sure you're not drawing tiles outside map bounds

                        City city = Game.GetCities.Find(c => c.X == x && c.Y == y);
                        if (city != null)
                        {
                            Bitmap cityNameBitmap = Draw.CityName(city, Game.Zoom);
                            g.DrawImage(cityNameBitmap,
                                Game.Xpx * (coordsOffsets[0] + 1) - cityNameBitmap.Width / 2,
                                Game.Ypx * coordsOffsets[1] + 5 * 2 / Game.Ypx + Game.Ypx);
                        }
                    }

                    ////Viewing piece (is drawn on top of everything)
                    //if (MapPanel.ViewingPiecesMode)
                    //{
                    //    if (frame == 0)
                    //    {
                    //        g.DrawImage(Draw.ViewPiece, 0, 16);
                    //    }
                    //}
                }
                animationFrames.Add(_bitmap);
            }

            return animationFrames;
        }

        //Get animation frames for moving unit
        public static List<Bitmap> UnitMoving(bool viewPieceMode)
        {
            List<Bitmap> animationFrames = new List<Bitmap>();

            //Get coords of central tile & which squares are to be drawn
            int[] centralCoords = Game.Instance.ActiveUnit.LastXY;   //either from active unit or moving pieces
            List<int[]> coordsOffsetsToBeDrawn = new List<int[]>
            {
                new int[] {-2, -4},
                new int[] {0, -4},
                new int[] {2, -4},
                new int[] {-3, -3},
                new int[] {-1, -3},
                new int[] {1, -3},
                new int[] {3, -3},
                new int[] {-2, -2},
                new int[] {0, -2},
                new int[] {2, -2},
                new int[] {-3, -1},
                new int[] {-1, -1},
                new int[] {1, -1},
                new int[] {3, -1},
                new int[] {-2, 0},
                new int[] {0, 0},
                new int[] {2, 0},
                new int[] {-3, 1},
                new int[] {-1, 1},
                new int[] {1, 1},
                new int[] {3, 1},
                new int[] {-2, 2},
                new int[] {0, 2},
                new int[] {2, 2},
                new int[] {-3, 3},
                new int[] {-1, 3},
                new int[] {1, 3},
                new int[] {3, 3},
                new int[] {-2, 4},
                new int[] {0, 4},
                new int[] {2, 4}
            };

            int zoom = 8;

            //First draw main bitmap with everything except the moving unit
            Bitmap _mainBitmap = new Bitmap(3 * 64, 3 * 32 + 16);
            using (Graphics g = Graphics.FromImage(_mainBitmap))
            {
                g.FillRectangle(Brushes.Black, new Rectangle(0, 0, 3 * 64, 3 * 32 + 16));    //fill bitmap with black (necessary for correct drawing if image is on upper map edge)

                foreach (int[] coordsOffsets in coordsOffsetsToBeDrawn)
                {
                    //change coords of central offset
                    int xCiv2 = centralCoords[0] + coordsOffsets[0];
                    int yCiv2 = centralCoords[1] + coordsOffsets[1];
                    int xReal = (xCiv2 - yCiv2 % 2) / 2;
                    int yReal = yCiv2;

                    if (xCiv2 >= 0 && yCiv2 >= 0 && xCiv2 < 2 * Map.Xdim && yCiv2 < Map.Ydim)    //make sure you're not drawing tiles outside map bounds
                    {
                        //Tiles
                        int civId = Game.WhichCivsMapShown;
                        if ((civId < 8 && Map.Visibility[xReal, yReal][civId]) || civId == 8)
                        {
                            g.DrawImage(
                                Map.Tile[xReal, yReal].Graphic,
                                32 * coordsOffsets[0] + 64,
                                16 * coordsOffsets[1] + 48);

                            //Implement dithering in all 4 directions if necessary
                            if (civId != 8)
                                for (int tileX = 0; tileX < 2; tileX++)
                                    for (int tileY = 0; tileY < 2; tileY++)
                                    {
                                        int[] offset = new int[] { -1, 1 };
                                        int xCiv2New = xCiv2 + offset[tileX];
                                        int yCiv2New = yCiv2 + offset[tileY];
                                        int xRealNew = (xCiv2New - yCiv2New % 2) / 2; //back to real coords
                                        int yRealNew = yCiv2New;
                                        if (xRealNew >= 0 && xRealNew < Map.Xdim && yRealNew >= 0 && yRealNew < Map.Ydim)   //don't observe outside map limits
                                            if (!Map.Visibility[xRealNew, yRealNew][civId])   //surrounding tile is not visible -> dither
                                                g.DrawImage(Images.DitherDots[tileX, tileY],
                                                            32 * coordsOffsets[0] + 64 + 32 * tileX,
                                                            16 * coordsOffsets[1] + 48 + 16 * tileY);
                                    }
                        }

                        //Units
                        List<IUnit> unitsHere = Game.GetUnits.Where(u => u.X == xCiv2 && u.Y == yCiv2).ToList();
                        //If active unit is in this list-- > remove it
                        if (unitsHere.Contains(Game.Instance.ActiveUnit))
                        {
                            unitsHere.Remove(Game.Instance.ActiveUnit);
                        }
                        if (unitsHere.Count > 0)
                        {
                            IUnit unit;
                            //If this is not tile with active unit or viewing piece, draw last unit on stack
                            //if (!(x == MapPanel.ActiveXY[0] && y == MapPanel.ActiveXY[1]))
                            if (!unitsHere.Contains(Game.Instance.ActiveUnit))
                            {
                                unit = unitsHere.Last();
                                if (!unit.IsInCity)
                                {
                                    g.DrawImage(
                                        Draw.Unit(unit, unitsHere.Count > 1, zoom),
                                        32 * coordsOffsets[0] + 64,
                                        16 * coordsOffsets[1] + 32);
                                }
                            }
                            //This tile has active unit/viewing piece
                            else
                            {
                                //Viewing pieces mode is enabled, so draw last unit on stack
                                if (viewPieceMode)
                                {
                                    unit = unitsHere.Last();
                                    if (!unit.IsInCity)
                                    {
                                        g.DrawImage(
                                            Draw.Unit(unit, unitsHere.Count > 1, zoom),
                                            32 * coordsOffsets[0] + 64,
                                            16 * coordsOffsets[1] + 32);
                                    }
                                }
                            }
                        }

                        //Cities
                        City city = Game.GetCities.Find(c => c.X == xCiv2 && c.Y == yCiv2);
                        if (city != null)
                        {
                            g.DrawImage(
                                Draw.City(city, true, 8),
                                32 * coordsOffsets[0] + 64,
                                16 * coordsOffsets[1] + 32);
                        }
                    }
                }

                //City names
                //Add additional coords for drawing city names
                coordsOffsetsToBeDrawn.Add(new int[] { -3, -5 });
                coordsOffsetsToBeDrawn.Add(new int[] { -1, -5 });
                coordsOffsetsToBeDrawn.Add(new int[] { 1, -5 });
                coordsOffsetsToBeDrawn.Add(new int[] { 3, -5 });
                coordsOffsetsToBeDrawn.Add(new int[] { -4, -2 });
                coordsOffsetsToBeDrawn.Add(new int[] { 4, -2 });
                coordsOffsetsToBeDrawn.Add(new int[] { -4, 0 });
                coordsOffsetsToBeDrawn.Add(new int[] { 4, 0 });
                coordsOffsetsToBeDrawn.Add(new int[] { -4, 2 });
                coordsOffsetsToBeDrawn.Add(new int[] { 4, 2 });
                foreach (int[] coordsOffsets in coordsOffsetsToBeDrawn)
                {
                    //change coords of central offset
                    int x = centralCoords[0] + coordsOffsets[0];
                    int y = centralCoords[1] + coordsOffsets[1];

                    if (x >= 0 && y >= 0 && x < 2 * Map.Xdim && y < Map.Ydim)    //make sure you're not drawing tiles outside map bounds
                    {
                        City city = Game.GetCities.Find(c => c.X == x && c.Y == y);
                        if (city != null)
                        {
                            Bitmap cityNameBitmap = Draw.CityName(city, 8);
                            g.DrawImage(
                                cityNameBitmap,
                                32 * coordsOffsets[0] + 64 + 32 - cityNameBitmap.Width / 2,
                                16 * coordsOffsets[1] + 3 * 8 + 48);
                        }
                    }
                }
            }

            //Now draw the moving unit on top of main bitmap
            int noFramesForOneMove = 8;
            for (int frame = 0; frame < noFramesForOneMove; frame++)
            {
                //Make a clone of the main bitmap in order to draw frames with unit on it
                Bitmap _bitmapWithMovingUnit = new Bitmap(_mainBitmap);
                using (Graphics g = Graphics.FromImage(_bitmapWithMovingUnit))
                {
                    ////Viewing piece (is drawn on top of everything)
                    //if (MapPanel.ViewingPiecesMode)
                    //{
                    //    g.DrawImage(Draw.ViewPiece, 64, 48);
                    //}
                    ////Draw active unit on top of everything
                    //else
                    //{
                    int[] activeUnitDrawOffset = new int[] { 0, 0 };
                    switch (Game.Instance.ActiveUnit.LastMove)
                    {
                        case 0: activeUnitDrawOffset = new int[] { 1, -1 }; break;
                        case 1: activeUnitDrawOffset = new int[] { 2, 0 }; break;
                        case 2: activeUnitDrawOffset = new int[] { 1, 1 }; break;
                        case 3: activeUnitDrawOffset = new int[] { 0, 2 }; break;
                        case 4: activeUnitDrawOffset = new int[] { -1, 1 }; break;
                        case 5: activeUnitDrawOffset = new int[] { -2, 0 }; break;
                        case 6: activeUnitDrawOffset = new int[] { -1, -1 }; break;
                        case 7: activeUnitDrawOffset = new int[] { 0, -2 }; break;
                    }
                    activeUnitDrawOffset[0] *= 32 / noFramesForOneMove * (frame + 1);
                    activeUnitDrawOffset[1] *= 16 / noFramesForOneMove * (frame + 1);

                    IUnit unit = Game.Instance.ActiveUnit;
                    g.DrawImage(
                        Draw.Unit(unit, false, zoom),
                        64 + activeUnitDrawOffset[0],
                        32 + activeUnitDrawOffset[1]);
                    //}
                }
                animationFrames.Add(_bitmapWithMovingUnit);
            }

            return animationFrames;
        }

        //Get animation frames for view piece
        //public static List<Bitmap> ViewPiece()
        //{
        //    List<Bitmap> animationFrames = new List<Bitmap>();
        //    for (int frame = 0; frame < 2; frame++)
        //    {
        //        Bitmap _mainBitmap = new Bitmap(64, 32);
        //        using (Graphics g = Graphics.FromImage(_mainBitmap))
        //        {
        //            g.DrawImage(Game.WholeMap, 0, 0, 32 * MapPanel.ActiveXY[0], 16 * MapPanel.ActiveXY[0]);
        //            if (frame == 0)
        //                g.DrawImage(Draw.ViewPiece, 0, 0, 32 * MapPanel.ActiveXY[0], 16 * MapPanel.ActiveXY[0]);
        //        }
        //        animationFrames.Add(_mainBitmap);
        //    }
        //    return animationFrames;
        //}

    }
}
