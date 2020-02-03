using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RTciv2.Enums;
using RTciv2.Units;
using RTciv2.Imagery;
using RTciv2.Terrains;
using RTciv2.Improvements;
using RTciv2.Forms;
using RTciv2.Events;

namespace RTciv2
{
    static class Actions
    {
        public static event EventHandler<MoveUnitCommandEventArgs> OnMoveUnitCommand;
        public static event EventHandler<NewUnitChosenEventArgs> OnNewUnitChosen;
        public static event EventHandler<NewPlayerTurnEventArgs> OnNewPlayerTurn;
        public static event EventHandler<WaitAtTurnEndEventArgs> OnWaitAtTurnEnd;

        public static void UpdateUnit(IUnit unit)
        {
            //If unit is not waiting order, chose next unit in line, otherwise update its orders
            if (!unit.AwaitingOrders)
                ChooseNextUnit(); 
            else
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

            //Application.OpenForms.OfType<StatusForm>().First().RefreshStatusForm();
            //Application.OpenForms.OfType<MapForm>().First().RefreshMapForm();
        }

        //Chose next unit for orders. If all units ended turn, update cities.
        public static void ChooseNextUnit()
        {
            if (!AnyUnitsAwaitingOrders(Data.HumanPlayer))  //end turn if no units awaiting orders
            {
                if (Options.AlwaysWaitAtEndOfTurn)
                    OnWaitAtTurnEnd?.Invoke(null, new WaitAtTurnEndEventArgs());
                else
                    NewPlayerTurn();
            }
            else    //chose next unit
            {
                //Create an array of indexes of units awaiting orders
                List<int> indexUAO = new List<int>();
                for (int i = 0; i < Game.Units.Count; i++)
                    if ((Game.Units[i].Civ == Data.HumanPlayer) && Game.Units[i].AwaitingOrders) indexUAO.Add(i);

                int indexActUnit = Game.Units.FindIndex(unit => unit == Game.Instance.ActiveUnit);  //Determine index of unit that is currently still active but just ended turn

                if ((indexUAO[0] > indexActUnit) || (indexUAO[indexUAO.Count - 1] <= indexActUnit))  //currently active unit is at beginning/end of list ==> chose next unit from beginning of list
                    Game.Instance.ActiveUnit = Game.Units[indexUAO[0]];
                else    //otherwise chose next unit from currently active unit in the list
                    for (int i = 0; i < indexUAO.Count - 1; i++)
                        if ((indexActUnit >= indexUAO[i]) && (indexActUnit < indexUAO[i + 1])) Game.Instance.ActiveUnit = Game.Units[indexUAO[i + 1]];

                OnNewUnitChosen?.Invoke(null, new NewUnitChosenEventArgs());    //run event that new unit was chosen
            }
        }

        //Update stats of all cities
        public static void CitiesTurn()
        {
            foreach (City city in Game.Cities.Where(a => a.Owner == Data.HumanPlayer))
            {
                //city.NewTurn();
            }
        }

        public static void NewPlayerTurn()
        {
            Data.TurnNumber++;

            //Set all units to active
            foreach (IUnit unit in Game.Units.Where(n => n.Civ == Data.HumanPlayer))
            {
                unit.TurnEnded = false;
                unit.MovePoints = unit.MaxMovePoints;

                //Increase counters
                if ((unit.Order == OrderType.BuildIrrigation) || (unit.Order == OrderType.BuildRoad) || (unit.Order == OrderType.BuildMine)) unit.Counter += 1;
            }

            //Update all cities
            CitiesTurn();

            //Choose next unit
            ChooseNextUnit();

            OnNewPlayerTurn?.Invoke(null, new NewPlayerTurnEventArgs());
        }

        public static void BuildCity(string cityName)
        {
            int x = Game.Instance.ActiveUnit.X;
            int y = Game.Instance.ActiveUnit.Y;
            bool[] improvements = new bool[34];
            bool[] wonders = new bool[28];
            for (int i = 0; i < 34; i++) improvements[i] = false;
            for (int i = 0; i < 28; i++) wonders[i] = false;
            Game.CreateCity(x, y, false, false, false, false, false, false, false, false, false, Game.Instance.ActiveUnit.Civ, 1, Game.Instance.ActiveUnit.Civ, 0, 0, 0, cityName, 0, 0, 0, 0, improvements, 0, 0, 0, 0, 0, 0, 0, 0, 0, wonders);

            DeleteUnit(Game.Instance.ActiveUnit);
        }

        public static void DeleteUnit(IUnit unit)
        {
            if (Game.Instance.ActiveUnit == unit)
            {
                Game.Units.Remove(unit);
                ChooseNextUnit();
            }
            else
            {
                Game.Units.Remove(unit);
            }

        }

        public static void GiveOrder(OrderType order)
        {
            switch (order)
            {
                case OrderType.MoveSW:
                    OnMoveUnitCommand?.Invoke(null, new MoveUnitCommandEventArgs(Game.Instance.ActiveUnit.Move(-1, 1)));
                    break;
                case OrderType.MoveS:
                    OnMoveUnitCommand?.Invoke(null, new MoveUnitCommandEventArgs(Game.Instance.ActiveUnit.Move(0, 2)));
                    break;
                case OrderType.MoveSE:
                    OnMoveUnitCommand?.Invoke(null, new MoveUnitCommandEventArgs(Game.Instance.ActiveUnit.Move(1, 1)));
                    break;
                case OrderType.MoveW:
                    OnMoveUnitCommand?.Invoke(null, new MoveUnitCommandEventArgs(Game.Instance.ActiveUnit.Move(-2, 0)));
                    break;
                case OrderType.MoveE:
                    OnMoveUnitCommand?.Invoke(null, new MoveUnitCommandEventArgs(Game.Instance.ActiveUnit.Move(2, 0)));
                    break;
                case OrderType.MoveNW:
                    OnMoveUnitCommand?.Invoke(null, new MoveUnitCommandEventArgs(Game.Instance.ActiveUnit.Move(-1, -1)));
                    break;
                case OrderType.MoveN:
                    OnMoveUnitCommand?.Invoke(null, new MoveUnitCommandEventArgs(Game.Instance.ActiveUnit.Move(0, -2)));
                    break;
                case OrderType.MoveNE:
                    OnMoveUnitCommand?.Invoke(null, new MoveUnitCommandEventArgs(Game.Instance.ActiveUnit.Move(1, -1)));
                    break;
                case OrderType.BuildIrrigation: 
                    Game.Instance.ActiveUnit.BuildIrrigation();
                    UpdateUnit(Game.Instance.ActiveUnit);
                    break;
                case OrderType.BuildMine: 
                    Game.Instance.ActiveUnit.BuildMines();
                    UpdateUnit(Game.Instance.ActiveUnit);
                    break;
                case OrderType.GoTo:
                    //TODO: goto key pressed event
                    UpdateUnit(Game.Instance.ActiveUnit);
                    break;
                case OrderType.Fortify: 
                    Game.Instance.ActiveUnit.Fortify();
                    UpdateUnit(Game.Instance.ActiveUnit);
                    break;
                case OrderType.Sleep: 
                    Game.Instance.ActiveUnit.Sleep();
                    UpdateUnit(Game.Instance.ActiveUnit);
                    break;
                case OrderType.GoHome:
                    //TODO: gohome key pressed event
                    UpdateUnit(Game.Instance.ActiveUnit);
                    break;
                case OrderType.SkipTurn: 
                    Game.Instance.ActiveUnit.SkipTurn();
                    UpdateUnit(Game.Instance.ActiveUnit);
                    break;
                case OrderType.BuildCity: 
                    Game.Instance.ActiveUnit.BuildCity();
                    UpdateUnit(Game.Instance.ActiveUnit);
                    break;
                case OrderType.BuildRoad: 
                    Game.Instance.ActiveUnit.BuildRoad();
                    UpdateUnit(Game.Instance.ActiveUnit);
                    break;
                case OrderType.Transform: 
                    Game.Instance.ActiveUnit.Transform();
                    UpdateUnit(Game.Instance.ActiveUnit);
                    break;
                case OrderType.Automate:
                    //TODO: automate key pressed event
                    UpdateUnit(Game.Instance.ActiveUnit);
                    break;
                case OrderType.ActivateUnit:
                    //TODO: activate unit key pressed event
                    UpdateUnit(Game.Instance.ActiveUnit);
                    break;
                default: break;
            }
        }

        //find out if certain civ has any units awaiting orders
        public static bool AnyUnitsAwaitingOrders(int civId)
        {
            return Game.Units.Any(unit => unit.Civ == civId && unit.AwaitingOrders);
        }


    }
}
