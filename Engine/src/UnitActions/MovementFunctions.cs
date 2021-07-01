using System;
using System.Collections.Generic;
using System.Linq;
using Civ2engine.Enums;
using Civ2engine.Events;
using Civ2engine.Terrains;
using Civ2engine.Units;

namespace Civ2engine.UnitActions.Move
{
    public static class MovementFunctions
    {
        public static void TryMoveNorth()
        {
            if (Game.Instance.CurrentMap.ActiveXY[1] > 1)
            {
                MoveC2(0, -2);
            }
        }

        public static void TryMoveNorthEast()
        {
            if (Game.Instance.CurrentMap.ActiveXY[1] == 0)
            {
                return;
            }
            if (Game.Instance.CurrentMap.ActiveXY[0] < Game.Instance.CurrentMap.XDim*2-2)
            {
                MoveC2(1, -1);
            }else if (!Game.Instance.Options.FlatEarth)
            {
                MoveC2(-Game.Instance.CurrentMap.XDim*2 +2 , -1);
            }
        }

        public static void TryMoveEast()
        {
            if (Game.Instance.CurrentMap.ActiveXY[0] < Game.Instance.CurrentMap.XDim *2 -2)
            {
                MoveC2(2, 0);
            }else if (!Game.Instance.Options.FlatEarth)
            {
                MoveC2(-Game.Instance.CurrentMap.XDim*2 +2 , 0);
            }
        }
        
        public static void TryMoveSouthEast()
        {
            if (Game.Instance.CurrentMap.ActiveXY[1] >= Game.Instance.CurrentMap.YDim - 1)
            {
                return;
            }

            if (Game.Instance.CurrentMap.ActiveXY[0] < Game.Instance.CurrentMap.XDim * 2 - 1)
            {
                MoveC2(1, 1);
            }
            else if (!Game.Instance.Options.FlatEarth)
            {
                MoveC2(-Game.Instance.CurrentMap.XDim * 2 + 2, 1);
            }
        }
        
        public static void TryMoveSouth()
        {
            if (Game.Instance.CurrentMap.ActiveXY[1] < Game.Instance.CurrentMap.YDim-2)
            {
                MoveC2(0, 2);
            }
        }

        public static void TryMoveSouthWest()
        {
            if (Game.Instance.CurrentMap.ActiveXY[1] >= Game.Instance.CurrentMap.YDim -1)
            {
                return;
            }
            
            if (Game.Instance.CurrentMap.ActiveXY[0] > 0)
            {
                MoveC2(-1, 1);
            }
            else if(!Game.Instance.Options.FlatEarth)
            {
                MoveC2(Game.Instance.CurrentMap.XDim*2-2, 1);
            }
        }

        public static void TryMoveWest()
        {
            if (Game.Instance.CurrentMap.ActiveXY[0] > 0)
            {
                MoveC2(-2, 0);
            }
            else if(!Game.Instance.Options.FlatEarth)
            {
                MoveC2(Game.Instance.CurrentMap.XDim*2-2, 0);
            }
        }

        public static void TryMoveNorthWest()
        {
            if (Game.Instance.CurrentMap.ActiveXY[1] == 0)
            {
                return;
            }
            if (Game.Instance.CurrentMap.ActiveXY[0] > 0)
            {
                MoveC2(-1, -1);
            }
            else if(!Game.Instance.Options.FlatEarth)
            {
                MoveC2(Game.Instance.CurrentMap.XDim*2-1, -1);
            }
        }

        public static void MoveC2(int deltaX, int deltaY)
        {
            var game = Game.Instance;
            var unit = game.ActiveUnit;
            if (unit == null)
            {
                throw new NotSupportedException("No unit selected");
            }

            var destX = unit.X + deltaX;
            var destY = unit.Y + deltaY;

            var unitsOnTarget = game.AllUnits.Where(u => u.X == destX && u.Y == destY).ToList();
            if (unitsOnTarget.Count > 0 && unitsOnTarget[0].Owner != unit.Owner)
            {
                if (unit.AttackBase == 0)
                {
                    game.TriggerUnitEvent(UnitEventType.MovementBlocked, unit, BlockedReason.ZeroAttackStrength);
                    return;
                }         
                
                
                var tileTo = game.CurrentMap.TileC2(destX, destY);
                var city = game.GetCities.FirstOrDefault(c => c.X == tileTo.X && c.Y == tileTo.Y);
                if (city != null)
                {
                    //Anything can attack or defend a city
                    Attack(game, unit, tileTo, unitsOnTarget, city);
                    return;
                }

                if (tileTo.Type == TerrainType.Ocean)
                {
                    if (unit.Domain == UnitGAS.Ground)
                    {
                        // Ground units cannot attack into the sea
                        return;
                    }
                }
                else
                {
                    if (unit.SubmarineAdvantagesDisadvantages)
                    {
                        //Submarines can't attack land
                        return;
                    }
                }

                if (!unit.CanAttackAirUnits && unitsOnTarget.Any(u => u.Domain == UnitGAS.Air))
                {
                    game.TriggerUnitEvent(UnitEventType.MovementBlocked, unit, BlockedReason.CannotAttackAirUnits);
                    return;
                }
                
                Attack(game, unit, tileTo, unitsOnTarget, null);
            }
            else
            {
                Moveto(game, unit, destX, destY);
            }

            if (unit.Domain == UnitGAS.Air && unit.MovePoints == 0)
            {
                //TODO: Air unit out of fuel check
            }
        }

        private static void Attack(Game game, Unit attacker, Tile tile, List<Unit> unitsOnTargetSquare, City cityOnTargetSquare)
        {           

            // Primary defender is the unit with largest defense factor
            var defender = unitsOnTargetSquare[0];
            var defenseFactor = defender.DefenseFactor(attacker, cityOnTargetSquare);
            for (var i = 1; i < unitsOnTargetSquare.Count; i++)
            {
                var altDefenseFactor = unitsOnTargetSquare[i].DefenseFactor(attacker, cityOnTargetSquare);
                if (altDefenseFactor > defenseFactor)
                {
                    defender = unitsOnTargetSquare[i];
                    defenseFactor = altDefenseFactor;
                }
            }

            // Calculate odds of attacker winning combat (a round of battle)
            double a = attacker.AttackFactor(defender);

            //Calculate the firepower of the attacker and defender
            var fpA = attacker.FirepowerBase;
            var fpD = defender.FirepowerBase;

            if (attacker.Domain == UnitGAS.Sea && defender.Domain == UnitGAS.Ground)
            {
                // When a sea unit attacks a land unit, both units have their firepower reduced to 1
                fpA = 1;
                fpD = 1;
            }else if (attacker.Domain != UnitGAS.Sea && defender.Domain == UnitGAS.Sea &&
                      tile.Type != TerrainType.Ocean)
            {
                // Cought in port (A sea unit’s firepower is reduced to 1 when it is caught in port (or on a land square) by a land or air unit; The attacking air or land unit’s firepower is doubled)
                fpA *= 2;
                fpD = 1;
            }else if (attacker.Domain == UnitGAS.Air && defender.Domain == UnitGAS.Air && defender.FuelRange == 0)
            {
                // Helicopters attacked by fighters have firepower reduced to 1
                fpD = 1;
            }
            
            var probAttackerWins = defenseFactor >= a ? (a * 8 - 1) / (2 * defenseFactor * 8) : 1 - (defenseFactor * 8 + 1) / (2 * a * 8);

            // Battle -> Loop through combat rounds until a unit loses its HP
            var random = new Random();
            var combatRoundsAttackerWins = new List<bool>();  // Register combat outcomes
            var attackerHitpoints = new List<int>();  // Register attacker hitpoints in each round
            var defenderHitpoints = new List<int>();  // Register defender hitpoints in each round
            do
            {
                var rand = random.Next(0, 1000) / 1000.0;
                var attackerWinsRound = probAttackerWins > rand;
                attackerHitpoints.Add(attacker.HitPoints);
                defenderHitpoints.Add(defender.HitPoints);
                if (attackerWinsRound)
                {
                    defender.HitPointsLost += fpA;
                    combatRoundsAttackerWins.Add(true);
                }
                else 
                { 
                    attacker.HitPointsLost += fpD;
                    combatRoundsAttackerWins.Add(false);
                }
            } while (attacker.HitPoints > 0 && defender.HitPoints > 0);

            var attackerWinsBattle = defender.HitPoints <= 0;

            if (attackerWinsBattle)
            {
                attacker.MovePointsLost += game.Rules.Cosmic.MovementMultiplier;
                // Defender loses - kill all units on the tile (except if on city & if in fortress/airbase)
                if (cityOnTargetSquare != null || tile.Fortress || tile.Airbase)
                {
                    defender.Dead = true;
                    //_casualties.Add(defender);
                    //_units.Remove(defender);
                }
                else
                {
                    foreach (var unit in unitsOnTargetSquare)
                    {
                        unit.Dead = true;
                        //_casualties.Add(unit);
                        //_units.Remove(unit);
                    }
                }
            }
            else
            {
                attacker.Dead = true;
                //_casualties.Add(attacker);
                //_units.Remove(attacker);
            }

            game.TriggerUnitEvent(new UnitEventArgs(UnitEventType.Attack, attacker, defender, combatRoundsAttackerWins, attackerHitpoints, defenderHitpoints));
        }

        private static void Moveto(Game game, Unit unit, int destX, int destY)
        {  
            var tileFrom = game.CurrentMap.TileC2(unit.X, unit.Y);
            var tileTo = game.CurrentMap.TileC2(destX, destY);
            if (!unit.IgnoreZonesOfControl && !IsFriendlyTile(game,tileTo, unit) && IsNextToEnemy(game,unit,tileFrom) && IsNextToEnemy(game, unit, tileTo))
            {
                game.TriggerUnitEvent(UnitEventType.MovementBlocked, unit, BlockedReason.ZOC);
                return;
            }

            if (UnitMoved(game, unit, tileTo, tileFrom))
            {
                var neighbours = game.CurrentMap.Neighbours(tileTo).Where(n => !n.Visibility[unit.Owner.Id]).ToList();
                if (neighbours.Count > 0)
                {
                    neighbours.ForEach(n => n.Visibility[unit.Owner.Id] = true);
                    game.TriggerMapEvent(MapEventType.UpdateMap, neighbours);
                }
            }
        }

        internal static bool UnitMoved(Game game, Unit unit, Tile tileTo, Tile tileFrom)
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
                            u.Owner == unit.Owner && u.X == tileTo.X && u.Y == tileTo.Y && u.ShipHold > u.CarriedUnits.Count);
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

                    if (cosmicRules.RiverMovement < moveCost && tileFrom.River && tileTo.River && Math.Abs(tileTo.X - tileFrom.X) == 1 && Math.Abs(tileTo.Y - tileFrom.Y) == 1) //For rivers only for diagonal movement
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
                                UnitMoved(game, u, tileTo, tileFrom);
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
                            u.X == tileTo.X && u.Y == tileTo.Y && u.CanCarryAirUnits && u.CarriedUnits.Count < 20);
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
                    unitMoved = true;
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
                unit.X = tileTo.X;
                unit.Y = tileTo.Y;
                if (unit.CarriedUnits.Count > 0)
                {
                    unit.CarriedUnits.ForEach(u =>
                    {
                        u.PrevXY = unit.PrevXY;
                        u.X = unit.X;
                        u.Y = unit.Y;
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

        public static IEnumerable<Tile> GetPossibleMoves(Game game, Tile tile, Unit unit)
        {
            var neighbours = unit.Domain == UnitGAS.Ground
                ? game.CurrentMap.Neighbours(tile).Where(n => n.Type != TerrainType.Ocean)
                : game.CurrentMap.Neighbours(tile);
            if (unit.IgnoreZonesOfControl || !IsNextToEnemy(game, unit, tile))
            {
                return neighbours;
            }
            return neighbours
                .Where(n => game.AllUnits.Any(c => c.X == n.X && c.Y == n.Y) || !IsNextToEnemy(game, unit, n));
        }
    }
}