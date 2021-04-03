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
                                    // Check if move was success (eg didn't hit obstacle)
                                    if (_activeUnit.Move(order))
                                    {
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
                var unitsOnTargetSquare = _units.Where(unit => unit.X == newXY[0] && unit.Y == newXY[1]);
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
                if (!_activeUnit.AwaitingOrders || !GetActiveUnits.Contains(_activeUnit))
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
                if (!_activeUnit.AwaitingOrders || !GetActiveUnits.Contains(_activeUnit))
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
            // End turn if no units awaiting orders
            if (!_activeCiv.AnyUnitsAwaitingOrders)
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
                // Create an list of indexes of units awaiting orders
                var indexUAO = new List<int>();
                foreach (IUnit unit in _units.Where(u => u.Owner == _activeCiv && !u.Dead && u.AwaitingOrders && _activeUnit != u)) indexUAO.Add(unit.Id);

                //int indexActUnit = Game.GetUnits.FindIndex(unit => unit == Game.ActiveUnit);  //Determine index of unit that is currently still active but just ended turn

                if (_activeUnit.Id < indexUAO.First())
                {
                    _activeUnit = _units[indexUAO.First()];
                }
                else if (_activeUnit.Id == indexUAO.First())
                {
                    _activeUnit = _units[indexUAO.First() + 1];
                }
                else if (_activeUnit.Id >= indexUAO.Last())
                {
                    _activeUnit = _units[indexUAO.First()];
                }
                else
                {
                    for (int i = 0; i < indexUAO.Count; i++)
                    {
                        if ((_activeUnit.Id >= indexUAO[i]) && (_activeUnit.Id < indexUAO[i + 1]))
                        {
                            _activeUnit = _units[indexUAO[i + 1]];
                            break;
                        }
                    }
                }

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

            IUnit attacker = _activeUnit;

            // Determine direction of attack and all units on that square
            int[] deltaXY = new int[] { 0, 0 };
            switch (attackDirection)
            {
                case OrderType.MoveSW: deltaXY = new int[] { -1, 1 }; break;
                case OrderType.MoveS: deltaXY = new int[] { 0, 2 }; break;
                case OrderType.MoveSE: deltaXY = new int[] { 1, 1 }; break;
                case OrderType.MoveE: deltaXY = new int[] { 2, 0 }; break;
                case OrderType.MoveNE: deltaXY = new int[] { 1, 1 }; break;
                case OrderType.MoveN: deltaXY = new int[] { 0, -2 }; break;
                case OrderType.MoveNW: deltaXY = new int[] { -1, -1 }; break;
                case OrderType.MoveW: deltaXY = new int[] { -2, 0 }; break;
            }
            int[] newXY = { attacker.X + deltaXY[0], attacker.Y + deltaXY[1] };
            var unitsOnTargetSquare = _units.Where(unit => unit.X == newXY[0] && unit.Y == newXY[1]).ToList();

            // Primary defender is the unit with largest defense factor
            IUnit defender = unitsOnTargetSquare.OrderBy(u => u.DefenseFactor(attacker)).First();

            // Calculate odds of attacker winning combat (a round of battle)
            double A = attacker.AttackFactor(defender);
            double D = defender.DefenseFactor(attacker);
            double FP_A = attacker.Firepower(true, defender);
            double FP_D = defender.Firepower(false, attacker);
            double probAttackerWins;
            if (D >= A)
            {
                probAttackerWins = (A * 8 - 1) / (2 * D * 8);
            }
            else
            {
                probAttackerWins = 1 - ((D * 8 + 1) / (2 * A * 8));
            }

            // Battle -> Loop through combat rounds until a unit loses its HP
            Random random = new Random();
            double rand;
            bool attackerWinsRound;
            List<bool> combatRoundsAttackerWins = new List<bool>();  // Register combat outcomes
            List<int> attackerHitpoints = new List<int>();  // Register attacker hitpoints in each round
            List<int> defenderHitpoints = new List<int>();  // Register defender hitpoints in each round
            do
            {
                rand = random.Next(0, 1000) / 1000.0;   // Generate a random number between 0 and 1 to determine who won
                attackerWinsRound = probAttackerWins > rand;
                attackerHitpoints.Add(attacker.HitPoints);
                defenderHitpoints.Add(defender.HitPoints);
                if (attackerWinsRound)
                {
                    defender.HitPointsLost += (int)FP_A;
                    combatRoundsAttackerWins.Add(true);
                }
                else 
                { 
                    attacker.HitPointsLost += (int)FP_D;
                    combatRoundsAttackerWins.Add(false);
                }
            } while (attacker.HitPoints > 0 && defender.HitPoints > 0);

            bool attackerWinsBattle = defender.HitPoints <= 0;

            if (attackerWinsBattle)
            {
                // Defender loses - kill all units on the tile (except if on city & if in fortress/airbase)
                if (defender.IsInCity || Map.TileC2(defender.X, defender.Y).Fortress || Map.TileC2(defender.X, defender.Y).Airbase)
                {
                    defender.Dead = true;
                    //_casualties.Add(defender);
                    //_units.Remove(defender);
                }
                else
                {
                    foreach (IUnit unit in unitsOnTargetSquare)
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
