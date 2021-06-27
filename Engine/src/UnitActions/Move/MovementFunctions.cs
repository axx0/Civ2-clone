using System;
using System.Linq;
using Civ2engine.Enums;
using Civ2engine.Terrains;
using Civ2engine.Units;

namespace Civ2engine.UnitActions.Move
{
    internal static class MovementFunctions
    {
        public static void MoveC2(int deltaX, int deltaY, bool isDiagonal)
        {
            var game = Game.Instance;
            var unit = game.ActiveUnit;
            if (unit == null)
            {
                throw new NotSupportedException("No unit selected");
            }

            var destX = unit.X + deltaX;
            var destY = unit.Y + deltaY;

            if (game.AllUnits.Any(u => u.X == destX && u.Y == destY && u.Owner != unit.Owner))
            {
                
            }
            else
            {
                Moveto(game, unit, destX, destY, isDiagonal);
            }

            if (unit.Domain == UnitGAS.Air && unit.MovePoints == 0)
            {
                //TODO: Air unit out of fuel check
            }
        }

        private static void Moveto(Game game, Unit unit, int destX, int destY, bool idDiagonal)
        {  
            var tileFrom = game.CurrentMap.TileC2(unit.X, unit.Y);
            var tileTo = game.CurrentMap.TileC2(destX, destY);
            if (!unit.IgnoreZonesOfControl && !IsFriendlyTile(game,tileTo, unit) && IsNextToEnemy(game,unit,tileFrom) && IsNextToEnemy(game, unit, tileTo))
            {
                game.TriggerUnitEvent(UnitEventType.StatusUpdate, unit);
                return;
            }

            if (UnitMoved(game, unit, destX, destY, idDiagonal, tileTo, tileFrom))
            {
                var neighbours = game.CurrentMap.Neighbours(tileTo).Where(n => !n.Visibility[unit.Owner.Id]).ToList();
                if (neighbours.Count > 0)
                {
                    neighbours.ForEach(n => n.Visibility[unit.Owner.Id] = true);
                    game.TriggerMapEvent(MapEventType.UpdateMap, neighbours);
                }
            }
        }

        private static bool UnitMoved(Game game, Unit unit, int destX, int destY, bool isDiagonal, Tile tileTo, Tile tileFrom)
        {
            var isCity = tileTo.IsCityPresent;
            var unitMoved = isCity;
            var cosmicRules = game.Rules.Cosmic;
            var moveCost = cosmicRules.MovementMultiplier;
            switch (unit.Domain)
            {
                case UnitGAS.Ground:
                {
                    if (tileTo.Type == TerrainType.Ocean)
                    {
                        //Check if we can board a ship there
                        var availableShip = game.AllUnits.FirstOrDefault(u =>
                            u.Owner == unit.Owner && u.X == destX && u.Y == destY && u.ShipHold > u.CarriedUnits.Count);
                        if (availableShip != null)
                        {
                            availableShip.CarriedUnits.Add(unit);
                            moveCost = unit.MovePoints;
                            unit.InShip = availableShip;
                            unitMoved = true;
                            unit.Order = OrderType.Sleep;
                        }

                        break;
                    }


                    if (isCity || tileTo.Railroad)
                    {
                        if (tileFrom.Railroad)
                        {
                            moveCost = cosmicRules.RailroadMovement;
                        }
                        else if (tileFrom.Road)
                        {
                            moveCost = cosmicRules.RoadMovement;
                        }
                    }
                    else if (tileTo.Road && (tileFrom.Road || tileFrom.IsCityPresent || tileFrom.Railroad))
                    {
                        moveCost = cosmicRules.RoadMovement;
                    }
                    else
                    {
                        moveCost *= tileTo.MoveCost;
                    }

                    // If alpine movement could be less use that
                    if (cosmicRules.AlpineMovement < moveCost && unit.Alpine)
                    {
                        moveCost = cosmicRules.AlpineMovement;
                    }

                    if (isDiagonal && cosmicRules.RiverMovement < moveCost && tileFrom.River &&
                        tileTo.River) //For rivers only for diagonal movement
                    {
                        moveCost = cosmicRules.RiverMovement;
                    }

                    unitMoved = true;
                    break;
                }
                case UnitGAS.Sea:
                {
                    if (tileTo.Type != TerrainType.Ocean)
                    {
                        if (!isCity && unit.CarriedUnits.Count > 0)
                        {
                            //Make landfall
                            unit.CarriedUnits.ForEach(u =>
                            {
                                u.Order = OrderType.NoOrders;
                                UnitMoved(game, u, destX, destY,isDiagonal, tileTo, tileFrom);
                                u.InShip = null;
                            });
                            unit.CarriedUnits.Clear();
                            return true;
                        }

                        break;
                    }

                    if (unit.ShipHold > 0 && unit.CarriedUnits.Count < unit.ShipHold)
                    {
                        if (tileFrom.Terrain.Type == TerrainType.Ocean)
                        {
                            foreach (var unaccountedUnit in game.AllUnits
                                .Where(u => u.X == unit.X && u.Y == unit.Y && u.InShip == null &&
                                            u.Domain == UnitGAS.Ground)
                                .Take(unit.ShipHold - unit.CarriedUnits.Count))
                            {
                                unaccountedUnit.InShip = unit;
                                unaccountedUnit.Order = OrderType.Sleep;
                                unit.CarriedUnits.Add(unaccountedUnit);
                            }
                        }
                        else if (tileFrom.IsCityPresent)
                        {
                            foreach (var unaccountedUnit in game.AllUnits
                                .Where(u => u.X == unit.X && u.Y == unit.Y && u.InShip == null &&
                                            u.Domain == UnitGAS.Ground && u.Order == OrderType.Sleep)
                                .Take(unit.ShipHold - unit.CarriedUnits.Count))
                            {
                                unaccountedUnit.InShip = unit;
                                unit.CarriedUnits.Add(unaccountedUnit);
                            }
                        }
                    }

                    unitMoved = true;
                    break;
                }
                case UnitGAS.Air:
                {
                    if (unit.InShip != null)
                    {
                        unit.InShip.CarriedUnits.Remove(unit);
                        unit.InShip = null;
                    }

                    if (tileTo.Airbase || tileTo.IsCityPresent)
                    {
                        moveCost = unit.MovePoints;
                    }
                    else
                    {
                        var carrier = game.AllUnits.FirstOrDefault(u =>
                            u.X == destX && u.Y == destY && u.CanCarryAirUnits && u.CarriedUnits.Count < 20);
                        if (carrier != null)
                        {
                            moveCost = unit.MovePoints;
                            carrier.CarriedUnits.Add(unit);
                            unit.InShip = carrier;
                        }
                    }

                    unitMoved = true;
                    break;
                }
                case UnitGAS.Special:

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            // If unit moved, update its X-Y coords
            if (unitMoved)
            {
                unit.MovePointsLost += moveCost;
                // Set previous coords
                unit.PrevXY = new[] {unit.X, unit.Y};

                // Set new coords
                unit.X = destX;
                unit.Y = destY;
                if (unit.CarriedUnits.Count > 0)
                {
                    unit.CarriedUnits.ForEach(u =>
                    {
                        u.PrevXY = unit.PrevXY;
                        u.X = destX;
                        u.Y = destY;
                    });
                    if (isCity) //If we're docking activate units carried
                    {
                        unit.CarriedUnits.ForEach(u =>
                        {
                            u.Order = OrderType.NoOrders;
                            u.InShip = null;
                        });
                        unit.CarriedUnits.Clear();
                    }
                }

                game.TriggerUnitEvent(UnitEventType.MoveCommand, unit);
            }

            return unitMoved;
        }

        private static bool IsFriendlyTile(Game game, Tile tileTo, Unit unit)
        {
            return game.GetCities.Any(c => c.X == tileTo.X && c.Y == tileTo.Y && c.Owner == unit.Owner) ||
                   game.AllUnits.Any(c => c.X == tileTo.X && c.Y == tileTo.Y && c.Owner == unit.Owner);
        }

        private static bool IsNextToEnemy(Game game, Unit unit, Tile tileFrom)
        {
            return game.CurrentMap.Neighbours(tileFrom).Any(t =>
                game.AllUnits.Any(u => u.X == t.X && u.Y == t.Y && u.Owner != unit.Owner && u.InShip == null && u.Domain == unit.Domain));
        }
    }
}