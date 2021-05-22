using System.Collections.Generic;
using System.Linq;
using Eto.Drawing;
using Civ2engine;
using Civ2engine.Units;
using Civ2engine.Events;
using Civ2engine.Enums;

namespace EtoFormsUI
{
    public class GetAnimationFrames : BaseInstance
    {
        // Get animation frames for waiting unit/view piece
        public static List<Bitmap> Waiting(int[] activeXY)
        {
            var animationFrames = new List<Bitmap>();

            // Get coords of central tile & which squares are to be drawn
            var coordsOffsetsToBeDrawn = new List<int[]>
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
            int[] coordsOffsetsPx;
            int x, y;
            for (int frame = 0; frame < 2; frame++)
            {
                var _bitmap = new Bitmap(2 * Map.Xpx, 3 * Map.Ypx, PixelFormat.Format32bppRgba);
                using (var g = new Graphics(_bitmap))
                {
                    // Fill bitmap with black (necessary for correct drawing if image is on upper map edge)
                    g.FillRectangle(Brushes.Black, new Rectangle(0, 0, 2 * Map.Xpx, 3 * Map.Ypx));

                    foreach (int[] coordsOffsets in coordsOffsetsToBeDrawn)
                    {
                        // Change coords of central offset
                        x = activeXY[0] + coordsOffsets[0];
                        y = activeXY[1] + coordsOffsets[1];
                        coordsOffsetsPx = new int[] { coordsOffsets[0] * Map.Xpx, coordsOffsets[1] * Map.Ypx };

                        if (x < 0 || y < 0 || x >= 2 * Map.Xdim || y >= Map.Ydim) continue;    // Make sure you're not drawing tiles outside map bounds

                        // Tiles
                        Draw.Tile(g, x, y, Map.Zoom, new Point(coordsOffsetsPx[0], coordsOffsetsPx[1] + Map.Ypx));

                        // Units
                        List<IUnit> unitsHere = Game.UnitsHere(x, y);
                        if (unitsHere.Count > 0 && Game.CityHere(x, y) == null)
                        {
                            IUnit unit;
                            // If this is not tile with active unit or viewing piece, draw last unit on stack
                            if (!(x == activeXY[0] && y == activeXY[1]) || Map.ViewPieceMode)
                            {
                                unit = unitsHere.Last();
                                Draw.Unit(g, unit, unit.IsInStack, Map.Zoom, new Point(coordsOffsetsPx[0], coordsOffsetsPx[1]));
                            }
                        }

                        // City
                        City city = Game.CityHere(x, y);
                        if (city != null) Draw.City(g, city, true, Map.Zoom, new Point(coordsOffsetsPx[0], coordsOffsetsPx[1]));

                        // Draw active unit if it's not moving
                        if (unitsHere.Count > 0 && x == activeXY[0] && y == activeXY[1] && !Map.ViewPieceMode)
                        {
                            // Draw unit only for 1st frame
                            if (frame == 0) Draw.Unit(g, Game.GetActiveUnit, Game.GetActiveUnit.IsInStack, Map.Zoom, new Point(coordsOffsetsPx[0], coordsOffsetsPx[1]));
                        }
                    }

                    // Gridlines
                    if (Game.Options.Grid)
                    {
                        foreach (int[] coordsOffsets in coordsOffsetsToBeDrawn)
                        {
                            coordsOffsetsPx = new int[] { coordsOffsets[0] * Map.Xpx, coordsOffsets[1] * Map.Ypx + Map.Ypx };
                            Draw.Grid(g, Map.Zoom, new Point(coordsOffsetsPx[0], coordsOffsetsPx[1]));
                        }
                    }

                    // City names
                    coordsOffsetsToBeDrawn.Add(new int[] { -2, -2 });
                    coordsOffsetsToBeDrawn.Add(new int[] { 2, -2 });
                    coordsOffsetsToBeDrawn.Add(new int[] { -1, -3 });
                    coordsOffsetsToBeDrawn.Add(new int[] { 1, -3 });
                    foreach (int[] coordsOffsets in coordsOffsetsToBeDrawn)
                    {
                        // Change coords of central offset
                        x = activeXY[0] + coordsOffsets[0];
                        y = activeXY[1] + coordsOffsets[1];

                        if (x < 0 || y < 0 || x >= 2 * Map.Xdim || y >= Map.Ydim) continue;   // Make sure you're not drawing tiles outside map bounds
                        
                        City city = Game.CityHere(x, y);
                        if (city != null) Draw.CityName(g, city, Map.Zoom, new int[] { coordsOffsets[0], coordsOffsets[1] + 1 });
                    }

                    // View piece (is drawn on top of everything)
                    if (Map.ViewPieceMode)
                    {
                        if (frame == 1) Draw.ViewPiece(g, Map.Zoom, new Point(0, Map.Ypx));
                    }
                }
                animationFrames.Add(_bitmap);
            }

            return animationFrames;
        }

        // Get animation frames for moving unit
        public static List<Bitmap> UnitMoving(IUnit activeUnit)
        {
            List<Bitmap> animationFrames = new List<Bitmap>();

            // Get coords of central tile & which squares are to be drawn
            int[] activeUnitPrevXY = activeUnit.PrevXY;
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

            // First draw main bitmap with everything except the moving unit
            int[] coordsOffsetsPx;
            var _mainBitmap = new Bitmap(6 * Map.Xpx, 7 * Map.Ypx, PixelFormat.Format32bppRgba);
            using (var g = new Graphics(_mainBitmap))
            {
                g.FillRectangle(Brushes.Black, new Rectangle(0, 0, 6 * Map.Xpx, 7 * Map.Ypx));    // Fill bitmap with black (necessary for correct drawing if image is on upper map edge)

                foreach (int[] coordsOffsets in coordsOffsetsToBeDrawn)
                {
                    // Change coords of central offset
                    int x = activeUnitPrevXY[0] + coordsOffsets[0];
                    int y = activeUnitPrevXY[1] + coordsOffsets[1];
                    coordsOffsetsPx = new int[] { (coordsOffsets[0] + 2) * Map.Xpx, (coordsOffsets[1] + 3) * Map.Ypx };

                    if (x < 0 || y < 0 || x >= 2 * Map.Xdim || y >= Map.Ydim) continue;   // Make sure you're not drawing tiles outside map bounds

                    // Tiles
                    int civId = Map.WhichCivsMapShown;
                    if (Map.IsTileVisibleC2(x, y, civId) || Map.MapRevealed)
                    {
                        Draw.Tile(g, x, y, Map.Zoom, new Point(coordsOffsetsPx[0], coordsOffsetsPx[1]));

                        // Implement dithering in all 4 directions if necessary
                        if (!Map.MapRevealed)
                        {
                            for (int tileX = 0; tileX < 2; tileX++)
                            {
                                for (int tileY = 0; tileY < 2; tileY++)
                                {
                                    int[] offset = new int[] { -1, 1 };
                                    int xNew = x + offset[tileX];
                                    int yNew = y + offset[tileY];
                                    if (xNew >= 0 && xNew < 2 * Map.Xdim && yNew >= 0 && yNew < Map.Ydim)   // Don't observe outside map limits
                                        if (!Map.IsTileVisibleC2(xNew, yNew, civId))   // Surrounding tile is not visible -> dither
                                            Draw.Dither(g, tileX, tileY, Map.Zoom, new Point(coordsOffsetsPx[0] + Map.Xpx * tileX, coordsOffsetsPx[1] + Map.Ypx * tileY));
                                }
                            }
                        }
                    }

                    // Units
                    List<IUnit> unitsHere = Game.UnitsHere(x, y);

                    // If unit is ship, remove units that are in it
                    var unitsonship = Game.UnitsOnShip(activeUnit);
                    unitsHere.RemoveAll(u => unitsonship.Contains(u));

                    // If active unit is in this list-- > remove it
                    if (unitsHere.Contains(activeUnit)) unitsHere.Remove(activeUnit);
                    
                    if (unitsHere.Count > 0)
                    {
                        IUnit unit;
                        // If this is not tile with active unit or viewing piece, draw last unit on stack
                        if (!unitsHere.Contains(activeUnit))
                        {
                            unit = unitsHere.Last();
                            if (!unit.IsInCity)
                            {
                                Draw.Unit(g, unit, unitsHere.Count > 1, Map.Zoom, new Point(coordsOffsetsPx[0], coordsOffsetsPx[1] - Map.Ypx));
                            }
                        }
                    }

                    // Cities
                    City city = Game.CityHere(x, y);
                    if (city != null)
                    {
                        Draw.City(g, city, true, Map.Zoom, new Point(coordsOffsetsPx[0], coordsOffsetsPx[1] - Map.Ypx));
                    }
                }

                // City names
                // Add additional coords for drawing city names
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
                    // Change coords of central offset
                    int x = activeUnitPrevXY[0] + coordsOffsets[0];
                    int y = activeUnitPrevXY[1] + coordsOffsets[1];

                    if (x < 0 || y < 0 || x >= 2 * Map.Xdim || y >= Map.Ydim) continue;   // Make sure you're not drawing tiles outside map bounds
                    
                    City city = Game.CityHere(x, y);
                    if (city != null) Draw.CityName(g, city, Map.Zoom, new int[] { coordsOffsets[0] + 2, coordsOffsets[1] + 3 });
                }
            }

            // Now draw the moving unit on top of main bitmap
            int noFramesForOneMove = 8;
            for (int frame = 0; frame < noFramesForOneMove; frame++)
            {
                // Make a clone of the main bitmap in order to draw frames with unit on it
                var _bitmapWithMovingUnit = new Bitmap(_mainBitmap);
                using (var g = new Graphics(_bitmapWithMovingUnit))
                {
                    // Draw active unit on top of everything (except if it's city, then don't draw the unit in last frame)
                    if (!(frame == noFramesForOneMove - 1 && Game.CityHere(activeUnit.X, activeUnit.Y) != null))
                    {
                        int[] unitDrawOffset = new int[] { activeUnit.X - activeUnit.PrevXY[0], activeUnit.Y - activeUnit.PrevXY[1] };
                        unitDrawOffset[0] *= Map.Xpx / noFramesForOneMove * (frame + 1);
                        unitDrawOffset[1] *= Map.Ypx / noFramesForOneMove * (frame + 1);
                        Draw.Unit(g, activeUnit, false, Map.Zoom, new Point(2 * Map.Xpx + unitDrawOffset[0], 2 * Map.Ypx + unitDrawOffset[1]));
                    }
                }
                animationFrames.Add(_bitmapWithMovingUnit);
            }

            return animationFrames;
        }

        // Get attack animation frames
        public static List<Bitmap> UnitAttack(UnitEventArgs e)
        {
            List<Bitmap> animationFrames = new List<Bitmap>();

            // Which squares are to be drawn
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

            // First draw main bitmap with everything except the moving unit
            int[] coordsOffsetsPx;
            var _mainBitmap = new Bitmap(6 * Map.Xpx, 7 * Map.Ypx, PixelFormat.Format32bppRgba);
            using (var g = new Graphics(_mainBitmap))
            {
                g.FillRectangle(Brushes.Black, new Rectangle(0, 0, 6 * Map.Xpx, 7 * Map.Ypx));    // Fill bitmap with black (necessary for correct drawing if image is on upper map edge)

                foreach (int[] coordsOffsets in coordsOffsetsToBeDrawn)
                {
                    // Change coords of central offset
                    int x = e.Attacker.X + coordsOffsets[0];
                    int y = e.Attacker.Y + coordsOffsets[1];
                    coordsOffsetsPx = new int[] { (coordsOffsets[0] + 2) * Map.Xpx, (coordsOffsets[1] + 3) * Map.Ypx };

                    if (x < 0 || y < 0 || x >= 2 * Map.Xdim || y >= Map.Ydim) continue;    // Make sure you're not drawing tiles outside map bounds

                    // Tiles
                    int civId = Map.WhichCivsMapShown;
                    if (Map.IsTileVisibleC2(x, y, civId) || Map.MapRevealed)
                    {
                        Draw.Tile(g, x, y, Map.Zoom, new Point(coordsOffsetsPx[0], coordsOffsetsPx[1]));

                        // Implement dithering in all 4 directions if necessary
                        if (!Map.MapRevealed)
                        {
                            for (int tileX = 0; tileX < 2; tileX++)
                            {
                                for (int tileY = 0; tileY < 2; tileY++)
                                {
                                    int[] offset = new int[] { -1, 1 };
                                    int xNew = x + offset[tileX];
                                    int yNew = y + offset[tileY];
                                    if (xNew >= 0 && xNew < 2 * Map.Xdim && yNew >= 0 && yNew < Map.Ydim)   // Don't observe outside map limits
                                        if (!Map.IsTileVisibleC2(xNew, yNew, civId))   // Surrounding tile is not visible -> dither
                                            Draw.Dither(g, tileX, tileY, Map.Zoom, new Point(coordsOffsetsPx[0] + Map.Xpx * tileX, coordsOffsetsPx[1] + Map.Ypx * tileY));
                                }
                            }
                        }
                    }

                    // Units
                    // If tile is with attacking & defending unit, draw these two first
                    // TODO: this won't draw correctly if unit is in city
                    if (x == e.Attacker.X && y == e.Attacker.Y)
                    {
                        Draw.Unit(g, e.Attacker, e.Attacker.IsInStack, Map.Zoom, new Point(coordsOffsetsPx[0], coordsOffsetsPx[1] - Map.Ypx));
                    }
                    else if (x == e.Defender.X && y == e.Defender.Y)
                    {
                        Draw.Unit(g, e.Defender, e.Defender.IsInStack, Map.Zoom, new Point(coordsOffsetsPx[0], coordsOffsetsPx[1] - Map.Ypx));
                    }
                    else    // Other units
                    {
                        var units = Game.UnitsHere(x, y);
                        if (units.Count > 0) 
                        {
                            var unit = units.Last();
                            if (!unit.IsInCity)
                            {
                                Draw.Unit(g, unit, unit.IsInStack, Map.Zoom, new Point(coordsOffsetsPx[0], coordsOffsetsPx[1] - Map.Ypx));
                            }
                        } 
                    }

                    // Cities
                    City city = Game.CityHere(x, y);
                    if (city != null) Draw.City(g, city, true, Map.Zoom, new Point(coordsOffsetsPx[0], coordsOffsetsPx[1] - Map.Ypx));
                }

                // City names
                // Add additional coords for drawing city names
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
                    // Change coords of central offset
                    int x = e.Attacker.X + coordsOffsets[0];
                    int y = e.Attacker.Y + coordsOffsets[1];

                    if(x < 0 || y < 0 || x >= 2 * Map.Xdim || y >= Map.Ydim) continue;    // Make sure you're not drawing tiles outside map bounds
                    
                    City city = Game.CityHere(x, y);
                    if (city != null) Draw.CityName(g, city, Map.Zoom, new int[] { coordsOffsets[0] + 2, coordsOffsets[1] + 3 });
                }
            }

            // Now draw the battle animation on top of attacking & defending unit
            // Number of battle rounds / 5 determines number of explosions (must be at least one). Each explosion has 8 frames.
            Point point;
            for (int explosion = 0; explosion < e.CombatRoundsAttackerWins.Count / 5; explosion++)
            {
                for (int frame = 0; frame < 8; frame++)
                {
                    // Make a clone of the main bitmap in order to draw frames with unit on it
                    var _bitmapWithExplosions = new Bitmap(_mainBitmap);
                    using (var g = new Graphics(_bitmapWithExplosions))
                    {
                        // Draw chaning HP of both units
                        foreach (int[] coordsOffsets in coordsOffsetsToBeDrawn)
                        {
                            int x = e.Attacker.X + coordsOffsets[0];
                            int y = e.Attacker.Y + coordsOffsets[1];

                            if (x == e.Attacker.X && y == e.Attacker.Y)
                            {
                                Draw.UnitShield(g, e.Attacker.Type, e.Attacker.Owner.Id, e.Attacker.Order, e.Attacker.IsInStack, e.AttackerHitpoints[explosion * 5], e.Attacker.HitpointsBase, Map.Zoom, new Point(2 * Map.Xpx, 2 * Map.Ypx));
                                Draw.UnitSprite(g, e.Attacker.Type, false, false, Map.Zoom, new Point(2 * Map.Xpx, 2 * Map.Ypx));
                            }
                            else if (x == e.Defender.X && y == e.Defender.Y)
                            {
                                Draw.UnitShield(g, e.Defender.Type, e.Defender.Owner.Id, e.Defender.Order, e.Defender.IsInStack, e.DefenderHitpoints[explosion * 5], e.Defender.HitpointsBase, Map.Zoom, new Point((2 + coordsOffsets[0]) * Map.Xpx, (2 + coordsOffsets[1]) * Map.Ypx));
                                Draw.UnitSprite(g, e.Defender.Type, e.Defender.Order == OrderType.Sleep, e.Defender.Order == OrderType.Fortified, Map.Zoom, new Point((2 + coordsOffsets[0]) * Map.Xpx, (2 + coordsOffsets[1]) * Map.Ypx));
                            }
                        }

                        // Draw explosion on defender
                        if (e.CombatRoundsAttackerWins[explosion]) point = new Point((int)(Map.Xpx * (2.5 + e.Defender.X - e.Attacker.X)), Map.Ypx * (3 + (e.Defender.Y - e.Attacker.Y)));
                        // Draw explosion on attacker
                        else point = new Point((int)(Map.Xpx * 2.5), Map.Ypx * 3);

                        Draw.BattleAnim(g, frame, Map.Zoom, point);
                    }
                    animationFrames.Add(_bitmapWithExplosions);
                }
            }
            return animationFrames;
        }
    }
}
