using System;
using System.Collections.Generic;
using System.Linq;
using Civ2engine.Units;
using Civ2engine.Enums;
using Civ2engine.Events;
using ExtensionMethods;

namespace Civ2engine
{
    public partial class Game : BaseInstance
    {
        public static event EventHandler<MapEventArgs> OnMapEvent;
        public static event EventHandler<UnitEventArgs> OnUnitEvent;

        public void IssueUnitOrder(OrderType order)
        {
            switch (order)
            {
                case OrderType.MoveSW:
                case OrderType.MoveS:
                case OrderType.MoveSE:
                case OrderType.MoveE:
                case OrderType.MoveNE:
                case OrderType.MoveN:
                case OrderType.MoveNW:
                case OrderType.MoveW:
                    {
                        // Movement - attack unit - attack city - conquer city
                        switch (DetermineUnitMovementOrderResult(order))
                        {
                            case UnitMovementOrderResultType.Movement:
                                {
                                    var unitsOnShip = UnitsOnShip(_activeUnit); // Write down units in ship before it moves

                                    // Check if move was success (eg didn't hit obstacle)
                                    if (_activeUnit.Move(order))
                                    {
                                        // If naval unit moved, move all units in hold
                                        foreach (IUnit unit in unitsOnShip)
                                        {
                                            unit.PrevXY = unit.XY;
                                            unit.X = _activeUnit.X;
                                            unit.Y = _activeUnit.Y;
                                            unit.Sleep();
                                        }

                                        OnUnitEvent?.Invoke(null, new UnitEventArgs(UnitEventType.MoveCommand));
                                    }
                                    break;
                                }
                            case UnitMovementOrderResultType.CombatStrength0CannotAttack:
                                {
                                    break;
                                }
                            case UnitMovementOrderResultType.CannotAttackAirUnit:
                                {
                                    break;
                                }
                            case UnitMovementOrderResultType.AttackUnit:
                                {
                                    Attack(order);
                                    break;
                                }
                            case UnitMovementOrderResultType.AttackCity:
                                {
                                    break;
                                }
                            case UnitMovementOrderResultType.CannotMoveOrAttack:
                                {
                                    break;
                                }
                        }

                        break;
                    }
                case OrderType.SkipTurn:
                    _activeUnit.SkipTurn();
                    UpdateActiveUnit();
                    break;
                case OrderType.Sleep:
                    _activeUnit.Sleep();
                    UpdateActiveUnit();
                    break;
            }
        }

        private UnitMovementOrderResultType DetermineUnitMovementOrderResult(OrderType movementDirection)
        {
            int[] newXY = _activeUnit.NewUnitCoords(movementDirection);

            // Determine what happens after command
            // ATTACK
            if (EnemyUnitsPresentHere(newXY[0], newXY[1]))
            {
                // Units with attack factor = 0 cannot attack
                if (_activeUnit.AttackBase == 0) return UnitMovementOrderResultType.CombatStrength0CannotAttack;

                // Get a list of units on target square
                var unitsOnTargetSquare = AllUnits.Where(unit => unit.X == newXY[0] && unit.Y == newXY[1]);
                // Find out if attack can be made based on unit domain
                if (_activeUnit.Domain == UnitGAS.Ground)    // LAND UNIT ATTACKING
                {
                    // Land units can attack only other units on land (except if there are any bombers on target square)
                    if (unitsOnTargetSquare.Any(unit => (unit.Type == UnitType.Bomber) || (unit.Type == UnitType.StlthBmbr))) return UnitMovementOrderResultType.CannotAttackAirUnit;
                    else return UnitMovementOrderResultType.AttackUnit;
                }
                else if (_activeUnit.Domain == UnitGAS.Air)  // AIR UNIT ATTACKING
                {
                    // Air units cannot attack airborne bombers unless their "Can attack air units" flag is set
                    if (unitsOnTargetSquare.Any(unit => (unit.Type == UnitType.Bomber) || (unit.Type == UnitType.StlthBmbr)) && !_activeUnit.CanAttackAirUnits) return UnitMovementOrderResultType.CannotAttackAirUnit;
                    else return UnitMovementOrderResultType.AttackUnit;
                }
                else    // SEA UNIT ATTACKING
                {
                    // Sea units cannot attack bombers
                    if (unitsOnTargetSquare.Any(unit => (unit.Type == UnitType.Bomber) || (unit.Type == UnitType.StlthBmbr))) return UnitMovementOrderResultType.CannotAttackAirUnit;
                    // Submarines can attack only sea units (check if enemy unit is not on ocean, remember that land units can be on ocean (transporting))
                    else if (_activeUnit.SubmarineAdvantagesDisadvantages && Map.TileC2(unitsOnTargetSquare.First().X, unitsOnTargetSquare.First().Y).Type != TerrainType.Ocean) return UnitMovementOrderResultType.CannotMoveOrAttack;
                    else return UnitMovementOrderResultType.AttackUnit;
                }
            }
            // Movement
            else
            {
                return UnitMovementOrderResultType.Movement;
            }
        }

        public void UpdateActiveUnit()
        {
            if (_activeCiv == _playerCiv)
            {
                // If active unit died OR is not waiting order, chose next unit in line, otherwise update its orders
                if (!_activeUnit.AwaitingOrders || !ActiveUnits.Contains(_activeUnit))
                {
                    ChooseNextUnit();
                }
                else
                {
                    switch (_activeUnit.Order)
                    {
                        case OrderType.BuildIrrigation:
                            if (_activeUnit.Counter == 2)
                            {
                                if (Map.TileC2(_activeUnit.X, _activeUnit.Y).Irrigation == false) // Build irrigation
                                {
                                    Map.TileC2(_activeUnit.X, _activeUnit.Y).Irrigation = true;
                                }
                                else if ((Map.TileC2(_activeUnit.X, _activeUnit.Y).Irrigation == true) && (Map.TileC2(_activeUnit.X, _activeUnit.Y).Farmland == false)) // Build farms
                                {
                                    Map.TileC2(_activeUnit.X, _activeUnit.Y).Farmland = true;
                                }
                                //Game.TerrainTile = Draw.DrawMap();  //Update game map
                                //unit.Action = OrderType.NoOrders;
                            }
                            break;
                        case OrderType.BuildRoad:
                            if (_activeUnit.Counter == 2)
                            {
                                if (Map.TileC2(_activeUnit.X, _activeUnit.Y).Road == false) // Build road
                                {
                                    Map.TileC2(_activeUnit.X, _activeUnit.Y).Road = true;
                                }
                                else if ((Map.TileC2(_activeUnit.X, _activeUnit.Y).Road == true) && (Map.TileC2(_activeUnit.X, _activeUnit.Y).Railroad == false)) // Build railroad
                                {
                                    Map.TileC2(_activeUnit.X, _activeUnit.Y).Railroad = true;
                                }
                                //Game.TerrainTile = Draw.DrawMap();  //Update game map
                                //unit.Action = OrderType.NoOrders;
                            }
                            break;
                        case OrderType.BuildMine:
                            if (_activeUnit.Counter == 2)
                            {
                                Map.TileC2(_activeUnit.X, _activeUnit.Y).Mining = true;
                                //Game.TerrainTile = Draw.DrawMap();  //Update game map
                                //unit.Action = OrderType.NoOrders;
                            }
                            break;
                        default:
                            break;
                    }
                    OnUnitEvent?.Invoke(null, new UnitEventArgs(UnitEventType.StatusUpdate));
                }
            }
            // AI
            else
            {
                if (!_activeUnit.AwaitingOrders || !ActiveUnits.Contains(_activeUnit))
                {
                    ChooseNextUnit();
                }
                else {
                    Game.IssueUnitOrder(AI_UnitOrder());
                }
            }
        }

        // Choose next unit for orders. If all units ended turn, update cities.
        private void ChooseNextUnit()
        {
            
            Unit nextUnit = null;
            int unitIndex;
            for (unitIndex = _activeUnit.Id + 1; unitIndex < AllUnits.Count && nextUnit == null; unitIndex++)
            {
                if (!AllUnits[unitIndex].Dead && AllUnits[unitIndex].Owner == _activeCiv && AllUnits[unitIndex].AwaitingOrders)
                {
                    nextUnit = AllUnits[unitIndex];
                }
            }

            for (unitIndex = 0; nextUnit == null && unitIndex < _activeUnit.Id; unitIndex++)
            {
                if (!AllUnits[unitIndex].Dead && AllUnits[unitIndex].Owner == _activeCiv && AllUnits[unitIndex].AwaitingOrders)
                {
                    nextUnit = AllUnits[unitIndex];
                }
            }
            // End turn if no units awaiting orders
            if (nextUnit == null)
            {
                if (Options.AlwaysWaitAtEndOfTurn && _activeCiv == _playerCiv)
                {
                    OnMapEvent?.Invoke(null, new MapEventArgs(MapEventType.WaitAtEndOfTurn));
                }
                else
                {
                    ChoseNextCiv();
                }
            }
            // Choose next unit
            else
            {
                _activeUnit = nextUnit;

                // Player => wait for new command
                if (_activeCiv == _playerCiv)
                {
                    OnUnitEvent?.Invoke(null, new UnitEventArgs(UnitEventType.StatusUpdate));
                }
                // AI => Set new unit command
                else
                {
                    Game.IssueUnitOrder(AI_UnitOrder());
                }
            }
        }

        private void Attack(OrderType attackDirection)
        {
            // Logic from "The Complete Civilization II Combat Guide v1.1"

            var attacker = _activeUnit;

            // Determine direction of attack and all units on that square
            var deltaXY = attackDirection switch
            {
                OrderType.MoveSW => new[] {-1, 1},
                OrderType.MoveS => new[] {0, 2},
                OrderType.MoveSE => new[] {1, 1},
                OrderType.MoveE => new[] {2, 0},
                OrderType.MoveNE => new[] {1, 1},
                OrderType.MoveN => new[] {0, -2},
                OrderType.MoveNW => new[] {-1, -1},
                OrderType.MoveW => new[] {-2, 0},
                _ => new[] {0, 0}
            };
            int[] newXY = { attacker.X + deltaXY[0], attacker.Y + deltaXY[1] };
            var cityOnTargetSquare = Game.GetCities.FirstOrDefault(c => c.X == newXY[0] && c.Y == newXY[1]);
            var unitsOnTargetSquare = AllUnits.Where(unit => unit.X == newXY[0] && unit.Y == newXY[1]).ToList();

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
                      Map.TileC2(defender.X, defender.Y).Type != TerrainType.Ocean)
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
                attacker.MovePointsLost += Rules.Cosmic.MovementMultiplier;
                // Defender loses - kill all units on the tile (except if on city & if in fortress/airbase)
                if (defender.IsInCity || Map.TileC2(defender.X, defender.Y).Fortress || Map.TileC2(defender.X, defender.Y).Airbase)
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

            OnUnitEvent?.Invoke(null, new UnitEventArgs(UnitEventType.Attack, attacker, defender, combatRoundsAttackerWins, attackerHitpoints, defenderHitpoints));
        }
    }
}
