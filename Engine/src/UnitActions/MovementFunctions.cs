using Civ2engine.Enums;
using Civ2engine.Events;
using Civ2engine.MapObjects;
using Civ2engine.Terrains;
using Civ2engine.Units;
using Model.Core;
using Model.Core.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using Civ2engine.Advances;
using Model.Constants;
using Model.Core.GameRules;
using Model.Core.Mapping;

namespace Civ2engine.UnitActions
{
    public static class MovementFunctions
    {
        public static void TryMoveNorth(IGame instance)
        {
            var activeUnit = instance.ActivePlayer.ActiveUnit;
            if (ActiveUnitCannotMove(activeUnit))
            {
                //TODO: Handle error
                return;
            }
            
            if (activeUnit!.Y - 2 >= 0)
            {
                MoveC2(instance, activeUnit, 0, -2);
            }
            else
            {
                instance.Players[activeUnit.Owner.Id].MoveBlocked(activeUnit, BlockedReason.EdgeOfMap);
                return;
            }

            CheckForUnitTurnEnded(instance, activeUnit);
        }

        public static void TryMoveNorthEast(IGame instance)
        {
            var activeUnit = instance.ActivePlayer.ActiveUnit;
            if (ActiveUnitCannotMove(activeUnit))
            {
                //TODO: Handle error
                return;
            }

            var map = activeUnit!.CurrentLocation!.Map;
            if (activeUnit.Y - 1 >= 0 && (!map.Flat || activeUnit.X + 1 < map.XDimMax))
            {
                if (!map.Flat && activeUnit.X == map.XDimMax - 1)
                {
                    MoveC2(instance, activeUnit, -map.XDimMax + 2, -1);
                }
                else
                {
                    MoveC2(instance, activeUnit, 1, -1);
                }
            }
            else
            {
                instance.Players[activeUnit.Owner.Id].MoveBlocked(activeUnit, BlockedReason.EdgeOfMap);
                return;
            }

            CheckForUnitTurnEnded(instance, activeUnit);
        }

        public static bool ActiveUnitCannotMove(Unit? activeUnit)
        {
            return activeUnit == null || activeUnit.Dead || activeUnit.CurrentLocation == null || activeUnit.TurnEnded;
        }

        private static void CheckForUnitTurnEnded(IGame game, Unit activeUnit)
        {
            if (activeUnit.MovePoints <= 0 || activeUnit.Dead)
            {
                game.ChooseNextUnit();
            }
        }

        public static void TryMoveEast(IGame instance)
        {
            var activeUnit = instance.ActivePlayer.ActiveUnit;
            if (ActiveUnitCannotMove(activeUnit))
            {
                //TODO: Handle error
                return;
            }

            var map = activeUnit!.CurrentLocation!.Map;
            if (activeUnit.X + 2 < map.XDimMax)
            {
                MoveC2(instance, activeUnit, 2, 0);
            }
            else if (!map.Flat)
            {
                MoveC2(instance, activeUnit, -map.XDimMax + 2, 0);
            }
            else
            {
                instance.Players[activeUnit.Owner.Id].MoveBlocked(activeUnit, BlockedReason.EdgeOfMap);
                return;
            }
            
            CheckForUnitTurnEnded(instance,activeUnit);
        }
        
        public static void TryMoveSouthEast(IGame instance)
        {
            var activeUnit = instance.ActivePlayer.ActiveUnit;
            if (ActiveUnitCannotMove(activeUnit))
            {
                //TODO: Handle error
                return;
            }

            var map = activeUnit!.CurrentLocation!.Map;
            if (activeUnit.Y + 1 < map.YDim && (!map.Flat || activeUnit.X + 1 < map.XDimMax))
            {
                if (!map.Flat && activeUnit.X == map.XDimMax - 1)
                {
                    MoveC2(instance, activeUnit, -map.XDimMax + 2, 1);
                }
                else
                {
                    MoveC2(instance, activeUnit, 1, 1);
                }
            }
            else
            {
                instance.Players[activeUnit.Owner.Id].MoveBlocked(activeUnit, BlockedReason.EdgeOfMap);
                return;
            }

            CheckForUnitTurnEnded(instance,activeUnit);
        }
        
        public static void TryMoveSouth(IGame instance)
        {
            var activeUnit = instance.ActivePlayer.ActiveUnit;
            if (ActiveUnitCannotMove(activeUnit))
            {
                //TODO: Handle error
                return;
            }

            var map = activeUnit!.CurrentLocation!.Map;
            if (activeUnit.Y + 2 < map.YDim)
            {
                MoveC2(instance, activeUnit, 0, 2);
            }
            else
            {
                instance.Players[activeUnit.Owner.Id].MoveBlocked(activeUnit, BlockedReason.EdgeOfMap);
                return;
            }

            CheckForUnitTurnEnded(instance,activeUnit);
        }

        public static void TryMoveSouthWest(IGame instance)
        {
            var activeUnit = instance.ActivePlayer.ActiveUnit;
            if (ActiveUnitCannotMove(activeUnit))
            {
                //TODO: Handle error
                return;
            }
            
            var map = activeUnit!.CurrentLocation!.Map;
            if (activeUnit.Y + 1 < map.YDim && (!map.Flat || activeUnit.X > 0))
            {
                if (!map.Flat && activeUnit.X == 0)
                {
                    MoveC2(instance, activeUnit, map.XDimMax - 2, 1);
                }
                else
                {
                    MoveC2(instance, activeUnit, -1, 1);
                }
            }
            else
            {
                instance.Players[activeUnit.Owner.Id].MoveBlocked(activeUnit, BlockedReason.EdgeOfMap);
                return;
            }

            CheckForUnitTurnEnded(instance, activeUnit);
        }

        public static void TryMoveWest(IGame instance)
        {
            var activeUnit = instance.ActivePlayer.ActiveUnit;
            if (ActiveUnitCannotMove(activeUnit))
            {
                //TODO: Handle error
                return;
            }

            var map = activeUnit!.CurrentLocation!.Map;
            if (activeUnit.X - 2 >= 0)
            {
                MoveC2(instance, activeUnit, -2, 0);
            }
            else if(!map.Flat)
            {
                MoveC2(instance, activeUnit, map.XDimMax-2, 0);
            }
            else
            {
                instance.Players[activeUnit.Owner.Id].MoveBlocked(activeUnit, BlockedReason.EdgeOfMap);
                return;
            }

            CheckForUnitTurnEnded(instance, activeUnit);
        }

        public static void TryMoveNorthWest(IGame instance)
        {
            var activeUnit = instance.ActivePlayer.ActiveUnit;
            if (ActiveUnitCannotMove(activeUnit))
            {
                //TODO: Handle error
                return;
            }

            var map = activeUnit!.CurrentLocation!.Map;
            if (activeUnit.Y - 1 >= 0 && (!map.Flat || (map.Flat && activeUnit.X > 0)))
            {
                if (!map.Flat && activeUnit.CurrentLocation.X == 0)
                {
                    MoveC2(instance, activeUnit, map.XDimMax - 1, -1);
                }
                else
                {
                    MoveC2(instance, activeUnit, -1, -1);
                }
            }
            else
            {
                instance.Players[activeUnit.Owner.Id].MoveBlocked(activeUnit, BlockedReason.EdgeOfMap);
                return;
            }

            CheckForUnitTurnEnded(instance, activeUnit);
        }

        public static void MoveC2(IGame game, Unit unit, int deltaX, int deltaY)
        {
            if (unit == null)
            {
                throw new NotSupportedException("No unit selected");
            }

            var destX = unit.X + deltaX;
            var destY = unit.Y + deltaY;

            var tileTo = unit.CurrentLocation.Map.TileC2(destX, destY);
            if (tileTo.UnitsHere.Count > 0 && tileTo.UnitsHere[0].Owner != unit.Owner)
            {
                if (!AttackAtTile(unit, game, tileTo)) return;
            }
            else
            {
                Moveto(game, unit, destX, destY);
            }

            if (unit.Domain == UnitGas.Air && unit.MovePoints == 0)
            {
                //TODO: Air unit out of fuel check
            }
        }

        internal static bool AttackAtTile(Unit unit, IGame game, Tile tileTo)
        {
            if (unit.AttackBase == 0)
            {
                game.Players[unit.Owner.Id].MoveBlocked(unit, BlockedReason.ZeroAttackStrength);
                return false;
            }


            if (tileTo.CityHere != null)
            {
                // Empty enemy cities are captured by moving into them.  The barbarian AI can
                // target cities directly, so do not enter the combat resolver unless there is
                // actually an enemy defender on the city tile.
                if (!tileTo.UnitsHere.Any(u => !u.Dead && u.Owner != unit.Owner))
                {
                    Moveto(game, unit, tileTo.X, tileTo.Y);
                    return true;
                }

                // Anything can attack or defend a city when a defender is present.
                Attack(game, unit, tileTo);
                return true;
            }

            if (tileTo.Type == TerrainType.Ocean)
            {
                if (unit.Domain == UnitGas.Ground)
                {
                    // Ground units cannot attack into the sea
                    return false;
                }
            }
            else
            {
                if (unit.SubmarineAdvantagesDisadvantages)
                {
                    //Submarines can't attack land
                    return false;
                }
            }

            if (tileTo.UnitsHere.Count == 0)
            {
#if DEBUG
                Console.WriteLine("No units on tile for attack");
#endif
                return false;
            }

            if (!unit.CanAttackAirUnits && tileTo.UnitsHere.Any(u => u.Domain == UnitGas.Air))
            {
                game.Players[unit.Owner.Id].MoveBlocked(unit, BlockedReason.CannotAttackAirUnits);
                return false;
            }

            Attack(game, unit, tileTo);
            return true;
        }

        private static void Attack(IGame game, Unit attacker, Tile tile)
        {           

            // Primary defender is the enemy unit with the largest defense factor.  An
            // undefended city can be reached here when an AI routine calls AttackAtTile
            // directly rather than going through MoveC2, so guard against an empty defender
            // stack and capture the city by movement instead of indexing UnitsHere[0].
            var defenders = tile.UnitsHere.Where(u => !u.Dead && u.Owner != attacker.Owner).ToList();
            if (defenders.Count == 0)
            {
                if (tile.CityHere != null && tile.CityHere.Owner != attacker.Owner)
                {
                    Moveto(game, attacker, tile.X, tile.Y);
                }

                return;
            }

            var groundDefenceFactor = tile.EffectsList.Where(e => e.Target == ImprovementConstants.GroundDefence).Sum(e=>e.Value);
            var defender = defenders[0];
            var defenseFactor = defender.DefenseFactor(attacker, tile, groundDefenceFactor);
            for (var i = 1; i < defenders.Count; i++)
            {
                var altDefenseFactor = defenders[i].DefenseFactor(attacker, tile, groundDefenceFactor);
                if (altDefenseFactor > defenseFactor)
                {
                    defender = defenders[i];
                    defenseFactor = altDefenseFactor;
                }
            }

            // Calculate odds of attacker winning combat (a round of battle)
            var attackFactor = attacker.AttackFactor(defender);
            if (attacker.MovePoints < game.Rules.Cosmic.MovementMultiplier)
            {
                //if attacker has less than one move point left attack at reduced strength
                attackFactor = attackFactor * attacker.MovePoints / game.Rules.Cosmic.MovementMultiplier;
            }

            //Calculate the firepower of the attacker and defender
            var fpA = attacker.FirepowerBase;
            var fpD = defender.FirepowerBase;

            if (attacker.Domain == UnitGas.Sea && defender.Domain == UnitGas.Ground)
            {
                // When a sea unit attacks a land unit, both units have their firepower reduced to 1
                fpA = 1;
                fpD = 1;
            }else if (attacker.Domain != UnitGas.Sea && defender.Domain == UnitGas.Sea &&
                      tile.Type != TerrainType.Ocean)
            {
                // Caught in port (A sea unit’s firepower is reduced to 1 when it is caught in port (or on a land square) by a land or air unit; The attacking air or land unit’s firepower is doubled)
                fpA *= 2;
                fpD = 1;
            }else if (attacker.Domain == UnitGas.Air && defender.Domain == UnitGas.Air && defender.FuelRange == 0)
            {
                // Helicopters attacked by fighters have firepower reduced to 1
                fpD = 1;
            }
            
            var probAttackerWins = defenseFactor >= attackFactor ? (attackFactor * 8 - 1) / (2 * defenseFactor * 8) : 1 - (defenseFactor * 8 + 1) / (2 * attackFactor * 8);

            // Battle -> Loop through combat rounds until a unit loses its HP
            var random = new Random();
            var combatRoundsAttackerWins = new List<bool>();  // Register combat outcomes
            var attackerHitpoints = new List<int>();  // Register attacker hitpoints in each round
            var defenderHitpoints = new List<int>();  // Register defender hitpoints in each round
            do
            {
                var rand = random.Next(0, 1000) / 1000.0;
                var attackerWinsRound = probAttackerWins > rand;
                attackerHitpoints.Add(attacker.RemainingHitpoints);
                defenderHitpoints.Add(defender.RemainingHitpoints);
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
            } while (attacker.RemainingHitpoints > 0 && defender.RemainingHitpoints > 0);

            var attackerWinsBattle = defender.RemainingHitpoints <= 0;


            var combatEventArgs = new CombatEventArgs(UnitEventType.Attack, attacker, defender, combatRoundsAttackerWins, attackerHitpoints, defenderHitpoints);

            for (int civId = 0; civId < tile.Visibility.Length; civId++)
            {
                if (tile.Visibility[civId] && tile.Map.IsCurrentlyVisible(tile, civId))
                {
                    var player = game.Players[civId];
                    player.CombatHappened(combatEventArgs);
                }
            }
            
            if (attackerWinsBattle)
            {
                ApplyPostCombatMovementCost(game, attacker);
                // Defender loses - kill all units on the tile (except if on city & if in fortress/airbase)
                if (tile.CityHere != null ||
                    tile.EffectsList.Any(e => e.Target == ImprovementConstants.NoStackElimination))
                {
                    game.Players[defender.Owner.Id].UnitLost(defender, attacker);
                    defender.Dead = true;
                    if (tile.CityHere != null &&
                        !tile.CityHere.Improvements.Any(i => i.Effects.ContainsKey(Effects.Walled)))
                    {
                        tile.CityHere.ShrinkCity(game);
                        game.UpdateTiles([tile]);
                    }
                    //_casualties.Add(defender);
                    //_units.Remove(defender);
                }
                else
                {
                    var deadUnits = tile.UnitsHere.ToList();
                    tile.UnitsHere.Clear();
                    foreach (var unit in deadUnits)
                    {
                        unit.Dead = true;
                        //_casualties.Add(unit);
                        //_units.Remove(unit);
                    }
                    game.Players[defender.Owner.Id].UnitsLost(deadUnits, attacker);
                }
            }
            else
            {
                
                attacker.Dead = true;
                game.Players[attacker.Owner.Id].UnitLost(attacker, defender);
                //_casualties.Add(attacker);
                //_units.Remove(attacker);
            }

            var updatedTiles = new List<Tile> { tile };
            if (attacker.CurrentLocation != null)
            {
                updatedTiles.Add(attacker.CurrentLocation);
            }

            game.UpdateTiles(updatedTiles.Distinct().ToList());

        }

        private static void ApplyPostCombatMovementCost(IGame game, Unit attacker)
        {
            var attackCost = game.Rules.Cosmic.MovementMultiplier;
            var movementLost = Math.Min(attacker.MaxMovePoints, attacker.MovePointsLost + attackCost);

            if (attacker.Domain != UnitGas.Air && attacker.HitpointsBase > 0 && attacker.HitPointsLost > 0)
            {
                var remainingHitpointRatio = Math.Max(0d, attacker.RemainingHitpoints) / attacker.HitpointsBase;
                var damageLimitedMovement = (int)Math.Round(attacker.MaxMovePoints * remainingHitpointRatio);
                var minimumMovement = attacker.Domain == UnitGas.Sea
                    ? Math.Min(attacker.MaxMovePoints, game.Rules.Cosmic.MovementMultiplier * 2)
                    : Math.Min(attacker.MaxMovePoints, game.Rules.Cosmic.MovementMultiplier);

                damageLimitedMovement = Math.Max(minimumMovement, damageLimitedMovement);
                movementLost = Math.Max(movementLost, attacker.MaxMovePoints - damageLimitedMovement);
            }

            attacker.MovePointsLost = Math.Clamp(movementLost, 0, attacker.MaxMovePoints);
        }

        private static void Moveto(IGame game, Unit unit, int destX, int destY)
        {
            var map = unit.CurrentLocation.Map;
            var tileFrom =  map.TileC2(unit.X, unit.Y);
            var tileTo = map.TileC2(destX, destY);
            if (!unit.IgnoreZonesOfControl && !IsFriendlyTile(tileTo, unit.Owner) && IsNextToEnemy(tileFrom, unit.Owner, unit.Domain) && IsNextToEnemy(tileTo, unit.Owner, unit.Domain))
            {
                game.Players[unit.Owner.Id].MoveBlocked(unit, BlockedReason.Zoc);
                return;
            }

            ExecuteUnitMove(game, unit, tileTo, tileFrom);
        }

        internal static void ExecuteUnitMove(IGame game, Unit unit, Tile tileTo, Tile tileFrom)
        {
            var isCity = tileTo.IsCityPresent;
            var unitMoved = isCity;
            var cosmicRules = game.Rules.Cosmic;
            var moveCost = cosmicRules.MovementMultiplier;
            switch (unit.Domain)
            {
                case UnitGas.Ground:
                {
                    if (tileTo.Type == TerrainType.Ocean)
                    {
                        //Check if we can board a ship there
                        var availableShip = tileTo.UnitsHere.FirstOrDefault(u =>
                            u.Owner == unit.Owner && u.ShipHold > u.CarriedUnits.Count);
                        if (availableShip != null)
                        {
                            availableShip.CarriedUnits.Add(unit);
                            moveCost = unit.MovePoints;
                            unit.InShip = availableShip;
                            unitMoved = true;
                            unit.Order = (int)OrderType.Sleep;
                        }

                        break;
                    }
                    
                    moveCost *= tileTo.MoveCost;
                    moveCost = MoveCost(tileTo, tileFrom, moveCost, cosmicRules);
                    if (unit.MaxMovePoints <= cosmicRules.MovementMultiplier && moveCost < cosmicRules.MovementMultiplier)
                    {
                        moveCost = unit.MovePoints;
                    }

                    // If alpine movement could be less use that
                    if (cosmicRules.AlpineMovement < moveCost && unit.Alpine)
                    {
                        moveCost = cosmicRules.AlpineMovement;
                    }

                    unitMoved = true;
                    break;
                }
                case UnitGas.Sea:
                {
                    if (tileTo.Type != TerrainType.Ocean)
                    {
                        if (!isCity && unit.CarriedUnits.Count > 0)
                        {
                            //Make landfall. We must capture the list since we want to modify it while we loop over it
                            var units = unit.CarriedUnits.ToList();
                            foreach (var u in units)
                            {
                                u.Order = (int)OrderType.NoOrders;
                                ExecuteUnitMove(game, u, tileTo, tileFrom);
                                u.InShip = null;
                            }

                            unit.CarriedUnits.Clear();
                            // It's okay to exit early here since the unit moved for the carried units will trigger the appropriate actions
                            return; 
                        }

                        break;
                    }

                    if (unit.ShipHold > 0 && unit.CarriedUnits.Count < unit.ShipHold)
                    {
                        if (tileFrom.Terrain.Type == TerrainType.Ocean)
                        {
                            foreach (var unaccountedUnit in tileFrom.UnitsHere
                                .Where(u => u.InShip == null &&
                                            u.Domain == UnitGas.Ground)
                                .Take(unit.ShipHold - unit.CarriedUnits.Count))
                            {
                                unaccountedUnit.InShip = unit;
                                unaccountedUnit.Order = (int)OrderType.Sleep;
                                unit.CarriedUnits.Add(unaccountedUnit);
                            }
                        }
                        else if (tileFrom.IsCityPresent)
                        {
                            foreach (var unaccountedUnit in tileFrom.UnitsHere
                                .Where(u => u.InShip == null &&
                                            u.Domain == UnitGas.Ground && u.Order == (int)OrderType.Sleep)
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
                case UnitGas.Air:
                {
                    if (unit.InShip != null)
                    {
                        unit.InShip.CarriedUnits.Remove(unit);
                        unit.InShip = null;
                    }

                    if (tileTo.EffectsList.Any(e=>e.Target == ImprovementConstants.Airbase) || tileTo.IsCityPresent)
                    {
                        moveCost = unit.MovePoints;
                    }
                    else
                    {
                        var carrier = tileTo.UnitsHere.FirstOrDefault(u =>
                            u.CanCarryAirUnits && u.CarriedUnits.Count < 20);
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
                case UnitGas.Special:
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
                unit.PrevXy = [unit.X, unit.Y];

                // Set new coords
                unit.X = tileTo.X;
                unit.Y = tileTo.Y;
                unit.CurrentLocation = tileTo;
                if (unit.CarriedUnits.Count > 0)
                {
                    unit.CarriedUnits.ForEach(u =>
                    {
                        u.PrevXy = unit.PrevXy;
                        u.X = unit.X;
                        u.Y = unit.Y;
                        u.CurrentLocation = tileTo;
                    });
                    if (isCity) //If we're docking activate units carried
                    {
                        unit.CarriedUnits.ForEach(u =>
                        {
                            u.Order = (int)OrderType.NoOrders;
                            u.InShip = null;
                        });
                        unit.CarriedUnits.Clear();
                    }
                }
                else if (unit.InShip != null)
                {
                    unit.InShip.CarriedUnits.Remove(unit);
                    unit.InShip = null;
                }

                if (unit.Order != (int)OrderType.GoTo)
                {
                    unit.Order = (int)OrderType.NoOrders;
                }
                
                for (var civId = 0; civId < unit.CurrentLocation.Visibility.Length; civId++)
                {
                    if (unit.CurrentLocation.Visibility[civId])
                    {
                        game.Players[civId].UnitMoved(unit, tileTo, tileFrom);
                    }
                }
                
                game.ActivePlayer.ActiveTile = tileTo;
                var mapUpdates = new List<Tile>();
                foreach (var neighbourTile in tileTo.Neighbours(unit.TwoSpaceVisibility))
                {
                    if(!neighbourTile.IsVisible(unit.Owner.Id))
                    {
                        neighbourTile.SetVisible(unit.Owner.Id);
                        mapUpdates.Add(neighbourTile);
                    }
                }
                
                if(tileTo.CityHere != null && tileTo.CityHere.Owner.Id != unit.Owner.Id)
                {
                    var loser = tileTo.CityHere.Owner;
                    tileTo.CityHere.ShrinkCity(game);
                    if (tileTo.CityHere != null)
                    {
                        tileTo.CityHere.EliminateCityUnits(game);
                        tileTo.CityHere.Owner.Cities.Remove(tileTo.CityHere);
                        tileTo.CityHere.Owner = unit.Owner;
                        unit.Owner.Cities.Add(tileTo.CityHere);
                        
                        game.Players[loser.Id].CityLost(tileTo.CityHere);

                        if (!game.ScenarioData.ForbidTechFromConquests)
                        {
                            var techs = AdvanceFunctions.CalculateResearchTheft(game, unit.Owner, loser);
                            if (techs.Count > 0)
                            {
                                game.Players[unit.Owner.Id].SelectTechFromConquest(techs);
                            }
                        }
                        game.Players[unit.Owner.Id].CityCaptured(tileTo.CityHere);
                        
                    }
                    mapUpdates.Add(tileTo);
                }else if (tileTo.HasGoodyHut)
                {
                    var outcome = tileTo.ConsumeGoodyHut(unit);
                    if (outcome.OutcomeType == "AdvancedTribe")
                    {
                        ApplyAdvancedTribeOutcome(game, unit, tileTo, outcome, mapUpdates);
                    }
                    else if (outcome.OutcomeType == "Nomads")
                    {
                        ApplyNomadsOutcome(game, outcome);
                    }
                    else if (outcome.OutcomeType == "Barbarians")
                    {
                        ApplyBarbarianOutcome(game, unit, tileTo, outcome, mapUpdates);
                    }

                    game.Players[unit.Owner.Id].GoodyHutTriggered(unit, outcome);

                    mapUpdates.Add(tileTo);
                }

                if (mapUpdates.Count > 0)
                {
                    game.UpdateTiles(mapUpdates);
                }
            }
        }

        private static void ApplyAdvancedTribeOutcome(IGame game, Unit unit, Tile tile,
            Model.Core.GoodyHuts.Outcomes.GoodyHutOutcomeResult outcome, IList<Tile> mapUpdates)
        {
            var cityNearby = tile.CityHere != null || tile.Neighbours().Any(t => t.IsCityPresent);
            if (tile.Type != TerrainType.Ocean && !tile.Terrain.Impassable && !cityNearby)
            {
                tile.HasGoodieHut = false;
                var cityName = CityActions.GetCityName(unit.Owner, game);
                var city = CityActions.BuildCity(unit, game, cityName);

                // A hut-created advanced tribe founds a city, but the unit that opened
                // the hut survives and stands inside the new city. BuildCity normally
                // consumes a settler, so restore the triggering unit explicitly.
                unit.Dead = false;
                unit.X = tile.X;
                unit.Y = tile.Y;
                unit.MapIndex = tile.Z;
                unit.MovePointsLost = unit.MaxMovePoints;
                if (!tile.UnitsHere.Contains(unit))
                {
                    tile.UnitsHere.Add(unit);
                }

                tile.SetVisible(unit.Owner.Id);
                mapUpdates.Add(tile);
                foreach (var neighbour in tile.Neighbours())
                {
                    if (neighbour.Visibility.Length > unit.Owner.Id)
                    {
                        neighbour.SetVisible(unit.Owner.Id);
                    }
                    mapUpdates.Add(neighbour);
                }

                outcome.Message = $"The villagers found the city of {city.Name} and join your civilization.";
                return;
            }

            tile.HasGoodieHut = false;
            unit.Owner.Money += 25;
            outcome.Message = "The villagers cannot found a city here, but they welcome your people with gifts worth 25 gold.";
            mapUpdates.Add(tile);
        }


        private static void ApplyBarbarianOutcome(IGame game, Unit triggeringUnit, Tile hutTile,
            Model.Core.GoodyHuts.Outcomes.GoodyHutOutcomeResult outcome, IList<Tile> mapUpdates)
        {
            var barbarianCiv = game.AllCivilizations.FirstOrDefault(c => c.PlayerType == PlayerType.Barbarians)
                               ?? game.AllCivilizations.FirstOrDefault(c => c.Id == 0);
            if (barbarianCiv != null)
            {
                barbarianCiv.Alive = true;
            }

            if (barbarianCiv == null)
            {
                outcome.Message = "The village is deserted, but ominous tracks lead away from it.";
                return;
            }

            var barbarianUnitDefinition = GetBarbarianUnitDefinition(game, triggeringUnit);
            if (barbarianUnitDefinition == null)
            {
                outcome.Message = "The village is deserted, but ominous tracks lead away from it.";
                return;
            }

            var spawnTiles = hutTile.Neighbours()
                .Where(tile => tile.Type != TerrainType.Ocean)
                .Where(tile => !tile.Terrain.Impassable)
                .Where(tile => tile.CityHere == null)
                .Where(tile => tile.UnitsHere.All(u => u.Owner == barbarianCiv))
                .OrderBy(tile => Math.Abs(tile.X - triggeringUnit.X) + Math.Abs(tile.Y - triggeringUnit.Y))
                .Take(Math.Max(1, Math.Min(3, game.BarbarianActivity + 1)))
                .ToList();

            if (spawnTiles.Count == 0)
            {
                var fallbackTile = hutTile.Neighbours()
                    .FirstOrDefault(tile => tile.Type != TerrainType.Ocean && !tile.Terrain.Impassable && tile.CityHere == null);
                if (fallbackTile != null)
                {
                    spawnTiles.Add(fallbackTile);
                }
            }

            var created = 0;
            foreach (var spawnTile in spawnTiles)
            {
                var barbarian = CreateBarbarianUnit(barbarianCiv, barbarianUnitDefinition, spawnTile, veteran: game.DifficultyLevel >= 3);
                barbarian.MovePointsLost = barbarian.MaxMovePoints;
                spawnTile.SetVisible(triggeringUnit.Owner.Id);
                created++;
                if (!mapUpdates.Contains(spawnTile))
                {
                    mapUpdates.Add(spawnTile);
                }
            }

            outcome.Message = created == 0
                ? "A barbarian horde was near, but could not reach this village."
                : created == 1
                    ? "A barbarian warrior appears near the village!"
                    : "A barbarian horde appears near the village!";
        }

        private static UnitDefinition? GetBarbarianUnitDefinition(IGame game, Unit triggeringUnit)
        {
            var preferredTypes = triggeringUnit.Owner.Epoch <= 1
                ? new[] { UnitType.Horsemen, UnitType.Warriors, UnitType.Archers }
                : new[] { UnitType.Dragoons, UnitType.Crusaders, UnitType.Horsemen, UnitType.Warriors };

            foreach (var preferredType in preferredTypes)
            {
                var index = (int)preferredType;
                if (index >= 0 && index < game.Rules.UnitTypes.Length)
                {
                    return game.Rules.UnitTypes[index];
                }
            }

            return game.Rules.UnitTypes.FirstOrDefault(unit => unit.Domain == UnitGas.Ground && unit.Attack > 0);
        }

        private static Unit CreateBarbarianUnit(Civilization owner, UnitDefinition unitDefinition, Tile tile, bool veteran)
        {
            var unit = new Unit
            {
                Id = owner.Units.Count != 0 ? owner.Units.Max(u => u.Id) + 1 : 0,
                Order = (int)OrderType.NoOrders,
                Owner = owner,
                Veteran = veteran,
                X = tile.X,
                Y = tile.Y,
                MapIndex = tile.Z,
                TypeDefinition = unitDefinition,
                NeedsSupport = false
            };

            owner.Units.Add(unit);
            unit.CurrentLocation = tile;
            return unit;
        }
        private static void ApplyNomadsOutcome(IGame game, Model.Core.GoodyHuts.Outcomes.GoodyHutOutcomeResult outcome)
        {
            if (outcome.CreatedUnit == null)
            {
                return;
            }

            var settlerDefinition = game.Rules.UnitTypes.FirstOrDefault(u => u.IsSettler)
                                    ?? game.Rules.UnitTypes.FirstOrDefault(u => u.AIrole == AiRoleType.Settle)
                                    ?? game.Rules.UnitTypes.FirstOrDefault(u =>
                                        string.Equals(u.Name, "Settlers", StringComparison.OrdinalIgnoreCase) ||
                                        string.Equals(u.Name, "Settler", StringComparison.OrdinalIgnoreCase));
            if (settlerDefinition == null)
            {
                return;
            }

            outcome.CreatedUnit.TypeDefinition = settlerDefinition;
            outcome.CreatedUnit.MovePointsLost = 0;
            outcome.Message = "You discover a band of wandering nomads. They agree to join your tribe as Settlers.";
        }

        internal static int MoveCost(Tile tileTo, Tile tileFrom, int moveCost, CosmicRules cosmicRules)
        {
            foreach (var movementEffect in tileFrom.EffectsList.Where(e =>
                         e.Target == ImprovementConstants.Movement)
                    )
            {
                var matchingEffect = tileTo.EffectsList.Where(e =>
                    e.Source == movementEffect.Source && e.Target == ImprovementConstants.Movement).MinBy(i=>i.Value);
                if (matchingEffect == null) continue;

                if (matchingEffect.Level < movementEffect.Level)
                {
                    if (matchingEffect.Value < moveCost)
                    {
                        moveCost = matchingEffect.Value;
                    }
                }
                else
                {
                    if (movementEffect.Value < moveCost)
                    {
                        moveCost = movementEffect.Value;
                    }
                }
            }

            if (cosmicRules.RiverMovement < moveCost && tileFrom.River && tileTo.River &&
                Math.Abs(tileTo.X - tileFrom.X) == 1 &&
                Math.Abs(tileTo.Y - tileFrom.Y) == 1) //For rivers only for diagonal movement
            {
                moveCost = cosmicRules.RiverMovement;
            }

            return moveCost;
        }

        internal static bool IsFriendlyTile(Tile tileTo, Civilization unitOwner)
        {
            return tileTo.UnitsHere.Any(u => u.Owner == unitOwner) ||
                   (tileTo.CityHere != null && tileTo.CityHere.Owner == unitOwner);
        }

        internal static bool IsNextToEnemy(Tile tile, Civilization civ, UnitGas domain)
        {
            return tile.Neighbours().Any(t =>
                t.UnitsHere.Any(u => u.Owner != civ && u.InShip == null && u.Domain == domain));
        }

        public static IEnumerable<Tile> GetPossibleMoves(Tile tile, Unit unit)
        {
            var neighbours = unit switch
            {
                { Domain: UnitGas.Ground } => tile.Neighbours().Where(n =>
                    n.Type != TerrainType.Ocean || n.UnitsHere.Any(u =>
                        u.Owner == unit.Owner && u.ShipHold > 0 && u.CarriedUnits.Count < u.ShipHold)),

                { Domain: UnitGas.Sea, SubmarineAdvantagesDisadvantages: true } =>
                    tile.Neighbours().Where(t =>
                        t.Type == TerrainType.Ocean || (t.CityHere != null && t.CityHere.OwnerId == unit.Owner.Id)),
                { Domain: UnitGas.Sea } =>
                    tile.UnitsHere.Any(u =>
                        u is { Domain: UnitGas.Ground, MovePoints: > 0 } && u.InShip == unit)

                        ? tile.Neighbours().Where(n =>
                            !n.Terrain.Impassable)

                        : tile.Neighbours().Where(t => t.Type == TerrainType.Ocean ||
                                                       (t.CityHere != null && t.CityHere.OwnerId == unit.Owner.Id) ||
                                                       t.UnitsHere.Any(u => u.Owner != unit.Owner)),


                _ => tile.Neighbours().Where(n => !n.Terrain.Impassable)
            };
            if (unit.IgnoreZonesOfControl || !IsNextToEnemy(tile, unit.Owner, unit.Domain))
            {
                return neighbours;
            }
            return neighbours
                .Where(n => n.UnitsHere.Count > 0 || !IsNextToEnemy(n, unit.Owner, unit.Domain));
        }

        public static IList<int> GetIslandsFor(Unit unit)
        {
            if (unit.Domain == UnitGas.Sea)
            {
                return unit.CurrentLocation!.Type == TerrainType.Ocean
                    ? new List<int> { unit.CurrentLocation.Island }
                    : unit.CurrentLocation.Neighbours().Where(t => t.Type == TerrainType.Ocean).Select(t => t.Island)
                        .Distinct().ToList();
            }

            if (unit.CurrentLocation!.Type == TerrainType.Ocean)
            {
                return unit.CurrentLocation.Neighbours().Where(n => n.Type != TerrainType.Ocean)
                    .Select(n => n.Island).Distinct().ToList();
            }

            return new[] { unit.CurrentLocation.Island };
        }
    }
}
