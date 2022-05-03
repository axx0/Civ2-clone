using System.Collections.Generic;
using System.Linq;
using Civ2engine;
using Civ2engine.Enums;
using Civ2engine.Events;
using Civ2engine.MapObjects;
using Civ2engine.Units;
using Eto.Drawing;

namespace EtoFormsUI
{
    public class GetAnimationFrames
    {
        // Get animation frames for moving unit
        public static List<Bitmap> UnitMoving(Unit activeUnit)
        {
            List<Bitmap> animationFrames = new List<Bitmap>();

            var Map = activeUnit.CurrentLocation.Map;

            // Get coords of central tile & which squares are to be drawn
            int[] activeUnitPrevXY = activeUnit.PrevXY;
            List<int[]> coordsOffsetsToBeDrawn = new List<int[]>
            {
                new[] {-2, -4},
                new[] {0, -4},
                new[] {2, -4},
                new[] {-3, -3},
                new[] {-1, -3},
                new[] {1, -3},
                new[] {3, -3},
                new[] {-2, -2},
                new[] {0, -2},
                new[] {2, -2},
                new[] {-3, -1},
                new[] {-1, -1},
                new[] {1, -1},
                new[] {3, -1},
                new[] {-2, 0},
                new[] {0, 0},
                new[] {2, 0},
                new[] {-3, 1},
                new[] {-1, 1},
                new[] {1, 1},
                new[] {3, 1},
                new[] {-2, 2},
                new[] {0, 2},
                new[] {2, 2},
                new[] {-3, 3},
                new[] {-1, 3},
                new[] {1, 3},
                new[] {3, 3},
                new[] {-2, 4},
                new[] {0, 4},
                new[] {2, 4}
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
                    coordsOffsetsPx = new[] { (coordsOffsets[0] + 2) * Map.Xpx, (coordsOffsets[1] + 3) * Map.Ypx };

                    if (x < 0 || y < 0 || x >= Map.XDimMax || y >= Map.YDim) continue;   // Make sure you're not drawing tiles outside map bounds

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
                                    int[] offset = { -1, 1 };
                                    int xNew = x + offset[tileX];
                                    int yNew = y + offset[tileY];
                                    if (xNew >= 0 && xNew < Map.XDimMax && yNew >= 0 && yNew < Map.YDim)   // Don't observe outside map limits
                                        if (!Map.IsTileVisibleC2(xNew, yNew, civId))   // Surrounding tile is not visible -> dither
                                            Draw.Dither(g, tileX, tileY, Map.Zoom, new Point(coordsOffsetsPx[0] + Map.Xpx * tileX, coordsOffsetsPx[1] + Map.Ypx * tileY));
                                }
                            }
                        }
                    }

                    // Units
                    var tile = Map.TileC2(x, y);
                    if (tile.CityHere == null)
                    {
                        // If unit is ship, remove units that are in it & remove active unit
                        //var unitsHere = tile.UnitsHere.Where(u => u.InShip == null && u != activeUnit).ToList();


                        if (tile.UnitsHere.Count > 0 && tile.UnitsHere.Any(u=> u!= activeUnit))
                        {
                            var unit = tile.GetTopUnit(u=>u != activeUnit);

                            Draw.Unit(g, unit, unit.IsInStack, Map.Zoom,
                                new Point(coordsOffsetsPx[0], coordsOffsetsPx[1] - Map.Ypx));

                        }
                    }
                    else
                    {
                        // Cities
                        Draw.City(g, tile.CityHere, true, Map.Zoom, new Point(coordsOffsetsPx[0], coordsOffsetsPx[1] - Map.Ypx));
                    }
                }

                // City names
                // Add additional coords for drawing city names
                coordsOffsetsToBeDrawn.Add(new[] { -3, -5 });
                coordsOffsetsToBeDrawn.Add(new[] { -1, -5 });
                coordsOffsetsToBeDrawn.Add(new[] { 1, -5 });
                coordsOffsetsToBeDrawn.Add(new[] { 3, -5 });
                coordsOffsetsToBeDrawn.Add(new[] { -4, -2 });
                coordsOffsetsToBeDrawn.Add(new[] { 4, -2 });
                coordsOffsetsToBeDrawn.Add(new[] { -4, 0 });
                coordsOffsetsToBeDrawn.Add(new[] { 4, 0 });
                coordsOffsetsToBeDrawn.Add(new[] { -4, 2 });
                coordsOffsetsToBeDrawn.Add(new[] { 4, 2 });
                foreach (int[] coordsOffsets in coordsOffsetsToBeDrawn)
                {
                    // Change coords of central offset
                    int x = activeUnitPrevXY[0] + coordsOffsets[0];
                    int y = activeUnitPrevXY[1] + coordsOffsets[1];

                    if (x < 0 || y < 0 || x >= Map.XDimMax || y >= Map.YDim) continue;   // Make sure you're not drawing tiles outside map bounds

                    var tile = Map.TileC2(x, y);
                    if (tile.CityHere != null) Draw.CityName(g, tile.CityHere, Map.Zoom, new[] { coordsOffsets[0] + 2, coordsOffsets[1] + 3 });
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
                    if (!(frame == noFramesForOneMove - 1 && activeUnit.CurrentLocation.CityHere != null))
                    {
                        int[] unitDrawOffset = { activeUnit.X - activeUnit.PrevXY[0], activeUnit.Y - activeUnit.PrevXY[1] };
                        unitDrawOffset[0] *= Map.Xpx / noFramesForOneMove * (frame + 1);
                        unitDrawOffset[1] *= Map.Ypx / noFramesForOneMove * (frame + 1);
                        Draw.Unit(g, activeUnit, activeUnit.CarriedUnits.Count > 0, Map.Zoom, new Point(2 * Map.Xpx + unitDrawOffset[0], 2 * Map.Ypx + unitDrawOffset[1]));
                    }
                }
                animationFrames.Add(_bitmapWithMovingUnit);
            }

            return animationFrames;
        }

        // Get attack animation frames
        public static List<Bitmap> UnitAttack(CombatEventArgs e)
        {
            var animationFrames = new List<Bitmap>();

            var map = e.Defender.Location.Map;
            // Which squares are to be drawn
            var coordsOffsetsToBeDrawn = new List<int[]>
            {
                new[] {-2, -4},
                new[] {0, -4},
                new[] {2, -4},
                new[] {-3, -3},
                new[] {-1, -3},
                new[] {1, -3},
                new[] {3, -3},
                new[] {-2, -2},
                new[] {0, -2},
                new[] {2, -2},
                new[] {-3, -1},
                new[] {-1, -1},
                new[] {1, -1},
                new[] {3, -1},
                new[] {-2, 0},
                new[] {0, 0},
                new[] {2, 0},
                new[] {-3, 1},
                new[] {-1, 1},
                new[] {1, 1},
                new[] {3, 1},
                new[] {-2, 2},
                new[] {0, 2},
                new[] {2, 2},
                new[] {-3, 3},
                new[] {-1, 3},
                new[] {1, 3},
                new[] {3, 3},
                new[] {-2, 4},
                new[] {0, 4},
                new[] {2, 4}
            };
            
            
            var defenderX = e.Defender.Location.X;
            var defenderY = e.Defender.Location.Y;
            
            var attackerX = e.Attacker.Location.X;
            var attackerY = e.Attacker.Location.Y;
            
            // First draw main bitmap with everything except the moving unit
            int[] coordsOffsetsPx;
            var mainBitmap = new Bitmap(6 * map.Xpx, 7 * map.Ypx, PixelFormat.Format32bppRgba);

            using (var g = new Graphics(mainBitmap))
            {
                g.FillRectangle(Brushes.Black, new Rectangle(0, 0, 6 * map.Xpx, 7 * map.Ypx));    // Fill bitmap with black (necessary for correct drawing if image is on upper map edge)

                foreach (int[] coordsOffsets in coordsOffsetsToBeDrawn)
                {
                    // Change coords of central offset
                    int x = attackerX + coordsOffsets[0];
                    int y = attackerY + coordsOffsets[1];
                    coordsOffsetsPx = new[] { (coordsOffsets[0] + 2) * map.Xpx, (coordsOffsets[1] + 3) * map.Ypx };

                    if (x < 0 || y < 0 || x >= map.XDimMax || y >= map.YDim) continue;    // Make sure you're not drawing tiles outside map bounds
                    
                    // Tiles
                    var tile = map.TileC2(x, y);
                    int civId = map.WhichCivsMapShown;
                    if (map.MapRevealed || tile.Visibility[civId])
                    {
                        Draw.Tile(g, x, y, map.Zoom, new Point(coordsOffsetsPx[0], coordsOffsetsPx[1]));

                        // Implement dithering in all 4 directions if necessary
                        if (!map.MapRevealed)
                        {
                            for (int tileX = 0; tileX < 2; tileX++)
                            {
                                for (int tileY = 0; tileY < 2; tileY++)
                                {
                                    int[] offset = { -1, 1 };
                                    var xNew = x + offset[tileX];
                                    var yNew = y + offset[tileY];
                                    if (xNew >= 0 && xNew < map.XDimMax && yNew >= 0 && yNew < map.YDim)   // Don't observe outside map limits
                                        if (!map.IsTileVisibleC2(xNew, yNew, civId))   // Surrounding tile is not visible -> dither
                                            Draw.Dither(g, tileX, tileY, map.Zoom, new Point(coordsOffsetsPx[0] + map.Xpx * tileX, coordsOffsetsPx[1] + map.Ypx * tileY));
                                }
                            }
                        }
                    }

                    // Units
                    // If tile is with attacking & defending unit, draw these two first
                    // TODO: this won't draw correctly if unit is in city
                    if (x == attackerX && y == attackerY)
                    {
                        Draw.Unit(g, e.Attacker, e.Attacker.IsInStack, map.Zoom, new Point(coordsOffsetsPx[0], coordsOffsetsPx[1] - map.Ypx));
                    }
                    else if (x == defenderX && y == defenderY)
                    {
                        Draw.Unit(g, e.Defender, e.Defender.IsInStack, map.Zoom, new Point(coordsOffsetsPx[0], coordsOffsetsPx[1] - map.Ypx));
                    }
                    else if (tile.CityHere == null && tile.UnitsHere.Count > 0) // Other units
                    {
                        var unit = tile.GetTopUnit();
                        Draw.Unit(g, unit, unit.IsInStack, map.Zoom,
                            new Point(coordsOffsetsPx[0], coordsOffsetsPx[1] - map.Ypx));
                    }

                    // Cities
                    if (tile.CityHere != null) Draw.City(g, tile.CityHere, true, map.Zoom, new Point(coordsOffsetsPx[0], coordsOffsetsPx[1] - map.Ypx));
                }

                // City names
                // Add additional coords for drawing city names
                coordsOffsetsToBeDrawn.Add(new[] { -3, -5 });
                coordsOffsetsToBeDrawn.Add(new[] { -1, -5 });
                coordsOffsetsToBeDrawn.Add(new[] { 1, -5 });
                coordsOffsetsToBeDrawn.Add(new[] { 3, -5 });
                coordsOffsetsToBeDrawn.Add(new[] { -4, -2 });
                coordsOffsetsToBeDrawn.Add(new[] { 4, -2 });
                coordsOffsetsToBeDrawn.Add(new[] { -4, 0 });
                coordsOffsetsToBeDrawn.Add(new[] { 4, 0 });
                coordsOffsetsToBeDrawn.Add(new[] { -4, 2 });
                coordsOffsetsToBeDrawn.Add(new[] { 4, 2 });
                foreach (int[] coordsOffsets in coordsOffsetsToBeDrawn)
                {
                    // Change coords of central offset
                    int x = attackerX + coordsOffsets[0];
                    int y = attackerY + coordsOffsets[1];

                    if(x < 0 || y < 0 || x >= map.XDimMax || y >= map.YDim) continue;    // Make sure you're not drawing tiles outside map bounds
                    
                    var tile = map.TileC2(x,y);
                    if (tile.CityHere != null) Draw.CityName(g, tile.CityHere, map.Zoom, new[] { coordsOffsets[0] + 2, coordsOffsets[1] + 3 });
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
                    var bitmapWithExplosions = new Bitmap(mainBitmap);
                    using (var g = new Graphics(bitmapWithExplosions))
                    {
                        // Draw changing HP of both units
                        foreach (var coordsOffsets in coordsOffsetsToBeDrawn)
                        {
                            var x = attackerX + coordsOffsets[0];
                            var y = attackerY + coordsOffsets[1];

                            if (x == attackerX && y == attackerY)
                            {
                                Draw.UnitShield(g, e.Attacker.Type, e.Attacker.Owner.Id, e.Attacker.Order, e.Attacker.IsInStack,
                                    e.Attacker.Hitpoints[explosion * 5], e.Attacker.HitpointsBase, map.Zoom,
                                    new Point(2 * map.Xpx, 2 * map.Ypx));
                                Draw.UnitSprite(g, e.Attacker.Type, false, false, map.Zoom,
                                    new Point(2 * map.Xpx, 2 * map.Ypx));
                            }
                            else if (x == defenderX && y == defenderY)
                            {
                                Draw.UnitShield(g, e.Defender.Type, e.Defender.Owner.Id, e.Defender.Order,
                                    e.Defender.IsInStack, e.Defender.Hitpoints[explosion * 5], e.Defender.HitpointsBase,
                                    map.Zoom,
                                    new Point((2 + coordsOffsets[0]) * map.Xpx, (2 + coordsOffsets[1]) * map.Ypx));
                                Draw.UnitSprite(g, e.Defender.Type, e.Defender.Order == OrderType.Sleep,
                                    e.Defender.Order == OrderType.Fortified, map.Zoom,
                                    new Point((2 + coordsOffsets[0]) * map.Xpx, (2 + coordsOffsets[1]) * map.Ypx));
                            }
                        }

                        // Draw explosion on defender
                        point = e.CombatRoundsAttackerWins[explosion]
                            ? new Point((int) (map.Xpx * (2.5 + defenderX - attackerX)),
                                map.Ypx * (3 + (defenderY - attackerY)))
                            : new Point((int) (map.Xpx * 2.5), map.Ypx * 3);

                        Draw.BattleAnim(g, frame, map.Zoom, point);
                    }

                    animationFrames.Add(bitmapWithExplosions);
                }
            }
            return animationFrames;
        }
    }
}
