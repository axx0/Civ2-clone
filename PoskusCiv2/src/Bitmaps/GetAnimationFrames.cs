using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using RTciv2.Forms;
using RTciv2.Units;
using ExtensionMethods;

namespace RTciv2.Bitmaps
{
    public class GetAnimationFrames
    {

        //Get animation frames for waiting unit
        public static List<Bitmap> UnitWaiting()
        {
            List<Bitmap> animationFrames = new List<Bitmap>();

            //Get coords of central tile & which squares are to be drawn
            int[] centralCoords = MapPanel.ActiveXY;   //either from active unit or moving pieces
            List<int[]> coordsOffsetsToBeDrawn = new List<int[]>
            {
                new int[] {-2, -2},
                new int[] {0, -2},
                new int[] {2, -2},
                new int[] {-1, -1},
                new int[] {1, -1},
                new int[] {-2, 0},
                new int[] {0, 0},
                new int[] {2, 0},
                new int[] {-1, 1},
                new int[] {1, 1},
                new int[] {0, 2}
            };

            //Get 2 frames (one with and other without the active unit/moving piece)
            for (int frame = 0; frame < 2; frame++)
            {
                Bitmap _bitmap = new Bitmap(64, 48);
                using (Graphics g = Graphics.FromImage(_bitmap))
                {
                    g.FillRectangle(Brushes.Black, new Rectangle(0, 0, 64, 48));    //fill bitmap with black (necessary for correct drawing if image is on upper map edge)

                    int zoom = 8;
                    foreach (int[] coordsOffsets in coordsOffsetsToBeDrawn)
                    {
                        //change coords of central offset
                        int x = centralCoords[0] + coordsOffsets[0];
                        int y = centralCoords[1] + coordsOffsets[1];

                        if (x >= 0 && y >= 0 && x < 2 * Data.MapXdim && y < Data.MapYdim)    //make sure you're not drawing tiles outside map bounds
                        {
                            //Tiles
                            int[] realCoords = Ext.Civ2xy(new int[] { x, y });  //real coords from civ2 coords
                            g.DrawImage(
                                Images.TerrainBitmap(realCoords[0], realCoords[1]),
                                32 * coordsOffsets[0],
                                16 * coordsOffsets[1] + 16);

                            //Units
                            List<IUnit> unitsHere = Game.Units.Where(u => u.X == x && u.Y == y).ToList();
                            if (unitsHere.Any())
                            {
                                IUnit unit;
                                //If this is not tile with active unit or viewing piece, draw last unit on stack
                                if (!(x == MapPanel.ActiveXY[0] && y == MapPanel.ActiveXY[1]))
                                {
                                    unit = unitsHere.Last();
                                    if (!unit.IsInCity)
                                    {
                                        g.DrawImage(
                                            Images.CreateUnitBitmap(unit, unitsHere.Count() > 1, zoom),
                                            32 * coordsOffsets[0],
                                            16 * coordsOffsets[1]);
                                    }
                                }
                                //This tile has active unit/viewing piece
                                else
                                {
                                    //Viewing pieces mode is enabled, so draw last unit on stack
                                    if (MapPanel.ViewPiecesMode)
                                    {
                                        unit = unitsHere.Last();
                                        if (!unit.IsInCity)
                                        {
                                            g.DrawImage(
                                                Images.CreateUnitBitmap(unit, unitsHere.Count() > 1, zoom),
                                                32 * coordsOffsets[0],
                                                16 * coordsOffsets[1]);
                                        }
                                    }
                                }
                            }

                            //Cities
                            City city = Game.Cities.Find(c => c.X == x && c.Y == y);
                            if (city != null)
                            {
                                g.DrawImage(
                                    Images.CreateCityBitmap(city, true, 8),
                                    32 * coordsOffsets[0],
                                    16 * coordsOffsets[1]);
                            }

                            //Draw active unit if it's not moving
                            if (unitsHere.Any())
                            {
                                //This tile has active unit/viewing piece
                                if (x == MapPanel.ActiveXY[0] && y == MapPanel.ActiveXY[1])
                                {
                                    if (!MapPanel.ViewPiecesMode)
                                    {
                                        if (frame == 0) //for first frame draw unit, for second not
                                        {
                                            g.DrawImage(
                                                Images.CreateUnitBitmap(Game.Instance.ActiveUnit, unitsHere.Count() > 1, zoom),
                                                32 * coordsOffsets[0],
                                                16 * coordsOffsets[1]);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    //City names
                    foreach (int[] coordsOffsets in coordsOffsetsToBeDrawn)
                    {
                        //change coords of central offset
                        int x = centralCoords[0] + coordsOffsets[0];
                        int y = centralCoords[1] + coordsOffsets[1];

                        if (x >= 0 && y >= 0 && x < 2 * Data.MapXdim && y < Data.MapYdim)    //make sure you're not drawing tiles outside map bounds
                        {
                            City city = Game.Cities.Find(c => c.X == x && c.Y == y);
                            if (city != null)
                            {
                                Bitmap cityNameBitmap = Images.CreateCityNameBitmap(city, 8);
                                g.DrawImage(
                                    cityNameBitmap,
                                    32 * coordsOffsets[0] + 32 - cityNameBitmap.Width / 2,
                                    16 * coordsOffsets[1] + 3 * 8 + 16);
                            }
                        }
                    }

                    ////Viewing piece (is drawn on top of everything)
                    //if (MapPanel.ViewingPiecesMode)
                    //{
                    //    if (frame == 0)
                    //    {
                    //        g.DrawImage(Images.ViewPiece, 0, 16);
                    //    }
                    //}
                }
                animationFrames.Add(_bitmap);
            }

            return animationFrames;
        }

        //Get animation frames for moving unit
        public static List<Bitmap> UnitMoving()
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

                    if (xCiv2 >= 0 && yCiv2 >= 0 && xCiv2 < 2 * Data.MapXdim && yCiv2 < Data.MapYdim)    //make sure you're not drawing tiles outside map bounds
                    {
                        //Tiles
                        int civId = MapPanel.CivIdWhoseMapIsDisplayed;
                        if ((civId < 8 && Game.TerrainTile[xReal, yReal].Visibility[civId]) || civId == 8)
                        {
                            g.DrawImage(
                                Images.TerrainBitmap(xReal, yReal),
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
                                        if (xRealNew >= 0 && xRealNew < Data.MapXdim && yRealNew >= 0 && yRealNew < Data.MapYdim)   //don't observe outside map limits
                                            if (!Game.TerrainTile[xRealNew, yRealNew].Visibility[civId])   //surrounding tile is not visible -> dither
                                                g.DrawImage(Images.DitherDots[tileX, tileY],
                                                            32 * coordsOffsets[0] + 64 + 32 * tileX,
                                                            16 * coordsOffsets[1] + 48 + 16 * tileY);
                                    }
                        }

                        //Units
                        List<IUnit> unitsHere = Game.Units.Where(u => u.X == xCiv2 && u.Y == yCiv2).ToList();
                        //If active unit is in this list-- > remove it
                        if (unitsHere.Contains(Game.Instance.ActiveUnit))
                        {
                            unitsHere.Remove(Game.Instance.ActiveUnit);
                        }
                        if (unitsHere.Any())
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
                                        Images.CreateUnitBitmap(unit, unitsHere.Count() > 1, zoom),
                                        32 * coordsOffsets[0] + 64,
                                        16 * coordsOffsets[1] + 32);
                                }
                            }
                            //This tile has active unit/viewing piece
                            else
                            {
                                //Viewing pieces mode is enabled, so draw last unit on stack
                                if (MapPanel.ViewPiecesMode)
                                {
                                    unit = unitsHere.Last();
                                    if (!unit.IsInCity)
                                    {
                                        g.DrawImage(
                                            Images.CreateUnitBitmap(unit, unitsHere.Count() > 1, zoom),
                                            32 * coordsOffsets[0] + 64,
                                            16 * coordsOffsets[1] + 32);
                                    }
                                }
                            }
                        }

                        //Cities
                        City city = Game.Cities.Find(c => c.X == xCiv2 && c.Y == yCiv2);
                        if (city != null)
                        {
                            g.DrawImage(
                                Images.CreateCityBitmap(city, true, 8),
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

                    if (x >= 0 && y >= 0 && x < 2 * Data.MapXdim && y < Data.MapYdim)    //make sure you're not drawing tiles outside map bounds
                    {
                        City city = Game.Cities.Find(c => c.X == x && c.Y == y);
                        if (city != null)
                        {
                            Bitmap cityNameBitmap = Images.CreateCityNameBitmap(city, 8);
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
                    //    g.DrawImage(Images.ViewPiece, 64, 48);
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
                        Images.CreateUnitBitmap(unit, false, zoom),
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
        //                g.DrawImage(Images.ViewPiece, 0, 0, 32 * MapPanel.ActiveXY[0], 16 * MapPanel.ActiveXY[0]);
        //        }
        //        animationFrames.Add(_mainBitmap);
        //    }
        //    return animationFrames;
        //}

    }
}
