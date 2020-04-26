using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RTciv2.Units;
using RTciv2.Enums;
using RTciv2.Events;
using System.Windows.Forms;

namespace RTciv2.GameActions
{
    public static partial class Actions
    {
        public static event EventHandler<WaitAtTurnEndEventArgs> OnWaitAtTurnEnd;
        public static event EventHandler<UnitEventArgs> OnUnitEvent;
        private static System.Windows.Forms.Timer UnitMovementTimer;

        public static void IssueUnitOrder(OrderType order)
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
                        //Movement - attack unit - attack city - conquer city
                        switch (DetermineUnitMovementOrderResult(order))
                        {
                            case UnitMovementOrderResultType.Movement:
                                {
                                    //Movement successful
                                    Game.Instance.ActiveUnit.Move(order);

                                    //Initiate unit movement timer
                                    StartUnitMovementTimer();

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
                    Game.Instance.ActiveUnit.SkipTurn();
                    UpdateUnit(Game.Instance.ActiveUnit);
                    break;
            }
        }

        private static void StartUnitMovementTimer()
        {
            Game.Instance.ActiveUnit.MovementCounter = 0;   //reset movement counter
            UnitMovementTimer = new System.Windows.Forms.Timer();
            UnitMovementTimer.Interval = 25;    //ms
            UnitMovementTimer.Start();
            UnitMovementTimer.Tick += new EventHandler(UnitMovementTimer_Tick);
        }

        private static void UnitMovementTimer_Tick(object sender, EventArgs e)
        {
            OnUnitEvent?.Invoke(null, new UnitEventArgs(UnitEventType.MoveCommand, Game.Instance.ActiveUnit.MovementCounter));

            Game.Instance.ActiveUnit.MovementCounter++;

            //Stop movement
            if (Game.Instance.ActiveUnit.MovementCounter == 8)
            {
                Game.Instance.ActiveUnit.MovementCounter = 0;   //reset movement counter
                UnitMovementTimer.Stop();
                UnitMovementTimer.Dispose();
                UpdateUnit(Game.Instance.ActiveUnit);
            }
        }

        private static UnitMovementOrderResultType DetermineUnitMovementOrderResult(OrderType movementDirection)
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
            int[] newXY = { Game.Instance.ActiveUnit.X + deltaXY[0], Game.Instance.ActiveUnit.Y + deltaXY[1] };

            //Determine what happens after command
            //Enemy city is present
            if (Game.Cities.Any(city => city.X == newXY[0] && city.Y == newXY[1] && city.Owner != Game.Instance.ActiveUnit.Civ))
            {
                return UnitMovementOrderResultType.AttackCity;
            }
            //Enemy unit is present
            else if (Game.Units.Any(unit => unit.X == newXY[0] && unit.Y == newXY[1] && unit.Civ != Game.Instance.ActiveUnit.Civ))
            {
                return UnitMovementOrderResultType.AttackUnit;
            }
            //Movement
            else
            {
                return UnitMovementOrderResultType.Movement;
            }
        }

        public static void UpdateUnit(IUnit unit)
        {
            //If unit is not waiting order, chose next unit in line, otherwise update its orders
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
                            if (Game.Map[unit.X, unit.Y].Irrigation == false) //Build irrigation
                            {
                                Game.Map[unit.X, unit.Y].Irrigation = true;
                            }
                            else if ((Game.Map[unit.X, unit.Y].Irrigation == true) && (Game.Map[unit.X, unit.Y].Farmland == false)) //Build farms
                            {
                                Game.Map[unit.X, unit.Y].Farmland = true;
                            }
                            //Game.Map = Draw.DrawMap();  //Update game map
                            //unit.Action = OrderType.NoOrders;
                        }
                        break;
                    case OrderType.BuildRoad:
                        if (unit.Counter == 2)
                        {
                            if (Game.Map[unit.X, unit.Y].Road == false) //Build road
                            {
                                Game.Map[unit.X, unit.Y].Road = true;
                            }
                            else if ((Game.Map[unit.X, unit.Y].Road == true) && (Game.Map[unit.X, unit.Y].Railroad == false)) //Build railroad
                            {
                                Game.Map[unit.X, unit.Y].Railroad = true;
                            }
                            //Game.Map = Draw.DrawMap();  //Update game map
                            //unit.Action = OrderType.NoOrders;
                        }
                        break;
                    case OrderType.BuildMine:
                        if (unit.Counter == 2)
                        {
                            Game.Map[unit.X, unit.Y].Mining = true;
                            //Game.Map = Draw.DrawMap();  //Update game map
                            //unit.Action = OrderType.NoOrders;
                        }
                        break;
                    default:
                        break;
                }
                OnUnitEvent?.Invoke(null, new UnitEventArgs(UnitEventType.StatusUpdate));
            }
        }

        //Chose next unit for orders. If all units ended turn, update cities.
        public static void ChooseNextUnit()
        {
            //End turn if no units awaiting orders
            if (!AnyUnitsAwaitingOrders(Data.HumanPlayer))
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
            //Choose next unit
            else
            {
                //Create an array of indexes of units awaiting orders
                List<int> indexUAO = new List<int>();
                for (int i = 0; i < Game.Units.Count; i++)
                    if ((Game.Units[i].Civ == Data.HumanPlayer) && Game.Units[i].AwaitingOrders) indexUAO.Add(i);

                int indexActUnit = Game.Units.FindIndex(unit => unit == Game.Instance.ActiveUnit);  //Determine index of unit that is currently still active but just ended turn

                //currently active unit is at beginning/end of list ==> choose next unit from beginning of list
                if ((indexUAO[0] > indexActUnit) || (indexUAO[indexUAO.Count - 1] <= indexActUnit))
                {
                    Game.Instance.ActiveUnit = Game.Units[indexUAO[0]];
                }
                //otherwise choose next unit from currently active unit in the list
                else
                {
                    for (int i = 0; i < indexUAO.Count - 1; i++)
                        if ((indexActUnit >= indexUAO[i]) && (indexActUnit < indexUAO[i + 1])) Game.Instance.ActiveUnit = Game.Units[indexUAO[i + 1]];
                }

                OnUnitEvent?.Invoke(null, new UnitEventArgs(UnitEventType.NewUnitActivated));
            }
        }
    }
}
