using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using Civ2engine.Units;
using Civ2engine.Enums;
using Civ2engine.Events;

namespace Civ2engine
{
    public partial class Game : BaseInstance
    {
        public static event EventHandler<WaitAtTurnEndEventArgs> OnWaitAtTurnEnd;
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
                                    // Start movement timer only if move was successful (eg didn't hit obstacle)
                                    if (_activeUnit.Move(order))
                                    {
                                        OnUnitEvent?.Invoke(null, new UnitEventArgs(UnitEventType.MoveCommand));
                                        //StartUnitMovementTimer();   //Initiate unit movement timer
                                    }

                                    break;
                                }
                            case UnitMovementOrderResultType.AttackUnit:
                                break;
                            case UnitMovementOrderResultType.AttackCity:
                                break;
                        }

                        break;
                    }
                case OrderType.SkipTurn:
                    _activeUnit.SkipTurn();
                    UpdateUnit(_activeUnit);
                    break;
            }
        }

        private UnitMovementOrderResultType DetermineUnitMovementOrderResult(OrderType movementDirection)
        {
            int[] deltaXY = new int[] { 0, 0 };
            switch (movementDirection)
            {
                case OrderType.MoveSW:
                    deltaXY = new int[] { -1, 1 };
                    break;
                case OrderType.MoveS:
                    deltaXY = new int[] { 0, 2 };
                    break;
                case OrderType.MoveSE:
                    deltaXY = new int[] { 1, 1 };
                    break;
                case OrderType.MoveE:
                    deltaXY = new int[] { 2, 0 };
                    break;
                case OrderType.MoveNE:
                    deltaXY = new int[] { 1, 1 };
                    break;
                case OrderType.MoveN:
                    deltaXY = new int[] { 0, -2 };
                    break;
                case OrderType.MoveNW:
                    deltaXY = new int[] { -1, -1 };
                    break;
                case OrderType.MoveW:
                    deltaXY = new int[] { -2, 0 };
                    break;
            }
            int[] newXY = { _activeUnit.X + deltaXY[0], _activeUnit.Y + deltaXY[1] };

            // Determine what happens after command
            // Enemy city is present
            if (_units.Any(city => city.X == newXY[0] && city.Y == newXY[1] && city.Owner != _activeUnit.Owner))
            {
                return UnitMovementOrderResultType.AttackCity;
            }
            // Enemy unit is present
            else if (_units.Any(unit => unit.X == newXY[0] && unit.Y == newXY[1] && unit.Owner != _activeUnit.Owner))
            {
                return UnitMovementOrderResultType.AttackUnit;
            }
            // Movement
            else
            {
                return UnitMovementOrderResultType.Movement;
            }
        }

        public void UpdateUnit(IUnit unit)
        {
            // If unit is not waiting order, chose next unit in line, otherwise update its orders
            if (!unit.AwaitingOrders)
            {
                ChooseNextUnit();
            }
            else
            {
                switch (unit.Order)
                {
                    case OrderType.BuildIrrigation:
                        if (unit.Counter == 2)
                        {
                            if (Map.Tile[unit.X, unit.Y].Irrigation == false) //Build irrigation
                            {
                                Map.Tile[unit.X, unit.Y].Irrigation = true;
                            }
                            else if ((Map.Tile[unit.X, unit.Y].Irrigation == true) && (Map.Tile[unit.X, unit.Y].Farmland == false)) //Build farms
                            {
                                Map.Tile[unit.X, unit.Y].Farmland = true;
                            }
                            //Game.TerrainTile = Draw.DrawMap();  //Update game map
                            //unit.Action = OrderType.NoOrders;
                        }
                        break;
                    case OrderType.BuildRoad:
                        if (unit.Counter == 2)
                        {
                            if (Map.Tile[unit.X, unit.Y].Road == false) //Build road
                            {
                                Map.Tile[unit.X, unit.Y].Road = true;
                            }
                            else if ((Map.Tile[unit.X, unit.Y].Road == true) && (Map.Tile[unit.X, unit.Y].Railroad == false)) //Build railroad
                            {
                                Map.Tile[unit.X, unit.Y].Railroad = true;
                            }
                            //Game.TerrainTile = Draw.DrawMap();  //Update game map
                            //unit.Action = OrderType.NoOrders;
                        }
                        break;
                    case OrderType.BuildMine:
                        if (unit.Counter == 2)
                        {
                            Map.Tile[unit.X, unit.Y].Mining = true;
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

        // Choose next unit for orders. If all units ended turn, update cities.
        private void ChooseNextUnit()
        {
            // End turn if no units awaiting orders
            if (!_activeCiv.AnyUnitsAwaitingOrders)
            {
                if (Options.AlwaysWaitAtEndOfTurn)
                {
                    OnWaitAtTurnEnd?.Invoke(null, new WaitAtTurnEndEventArgs());
                }
                else
                {
                    NewPlayerTurn();
                }
            }
            // Choose next unit
            else
            {
                // Create an list of indexes of units awaiting orders
                var indexUAO = new List<int>();
                foreach (IUnit unit in _units.Where(u => u.Owner == _activeCiv && u.AwaitingOrders)) indexUAO.Add(unit.Id);

                //int indexActUnit = Game.GetUnits.FindIndex(unit => unit == Game.ActiveUnit);  //Determine index of unit that is currently still active but just ended turn

                // Currently active unit is at beginning/end of list ==> choose next unit from beginning of list
                if ((indexUAO.First() > _activeUnit.Id) || (indexUAO.Last() <= _activeUnit.Id))
                {
                    _activeUnit = _units[indexUAO.First()];
                }
                // Otherwise choose next unit from currently active unit in the list
                else
                {
                    for (int i = 0; i < indexUAO.Count - 1; i++)
                        if ((_activeUnit.Id >= indexUAO[i]) && (_activeUnit.Id < indexUAO[i + 1])) _activeUnit = _units[indexUAO[i + 1]];
                }

                OnUnitEvent?.Invoke(null, new UnitEventArgs(UnitEventType.NewUnitActivated));
            }
        }
    }
}
