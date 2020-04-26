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

namespace RTciv2.GameActions
{
    public static partial class Actions
    {
        public static event EventHandler<PlayerEventArgs> OnPlayerEvent;

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

            OnPlayerEvent?.Invoke(null, new PlayerEventArgs(PlayerEventType.NewTurn));
        }

        public static void BuildCity(string cityName)
        {
            int x = Game.Instance.ActiveUnit.X;
            int y = Game.Instance.ActiveUnit.Y;
            bool[] improvements = new bool[34];
            bool[] wonders = new bool[28];
            for (int i = 0; i < 34; i++) improvements[i] = false;
            for (int i = 0; i < 28; i++) wonders[i] = false;
            //Game.CreateCity(x, y, false, false, false, false, false, false, false, false, false, Game.Instance.ActiveUnit.Civ, 1, Game.Instance.ActiveUnit.Civ, 0, 0, 0, cityName, 0, 0, 0, 0, improvements, 0, 0, 0, 0, 0, 0, 0, 0, 0, wonders);

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

        //public static void GiveOrder(OrderType order)
        //{
        //    switch (order)
        //    {
        //        //case OrderType.MoveSW:
        //        //    OnMoveUnitCommand?.Invoke(null, new MoveUnitCommandEventArgs(Game.Instance.ActiveUnit.Move(-1, 1)));
        //        //    break;
        //        //case OrderType.MoveS:
        //        //    OnMoveUnitCommand?.Invoke(null, new MoveUnitCommandEventArgs(Game.Instance.ActiveUnit.Move(0, 2)));
        //        //    break;
        //        //case OrderType.MoveSE:
        //        //    OnMoveUnitCommand?.Invoke(null, new MoveUnitCommandEventArgs(Game.Instance.ActiveUnit.Move(1, 1)));
        //        //    break;
        //        //case OrderType.MoveW:
        //        //    OnMoveUnitCommand?.Invoke(null, new MoveUnitCommandEventArgs(Game.Instance.ActiveUnit.Move(-2, 0)));
        //        //    break;
        //        //case OrderType.MoveE:
        //        //    OnMoveUnitCommand?.Invoke(null, new MoveUnitCommandEventArgs(Game.Instance.ActiveUnit.Move(2, 0)));
        //        //    break;
        //        //case OrderType.MoveNW:
        //        //    OnMoveUnitCommand?.Invoke(null, new MoveUnitCommandEventArgs(Game.Instance.ActiveUnit.Move(-1, -1)));
        //        //    break;
        //        //case OrderType.MoveN:
        //        //    OnMoveUnitCommand?.Invoke(null, new MoveUnitCommandEventArgs(Game.Instance.ActiveUnit.Move(0, -2)));
        //        //    break;
        //        //case OrderType.MoveNE:
        //        //    OnMoveUnitCommand?.Invoke(null, new MoveUnitCommandEventArgs(Game.Instance.ActiveUnit.Move(1, -1)));
        //        //    break;
        //        case OrderType.BuildIrrigation: 
        //            Game.Instance.ActiveUnit.BuildIrrigation();
        //            UpdateUnit(Game.Instance.ActiveUnit);
        //            break;
        //        case OrderType.BuildMine: 
        //            Game.Instance.ActiveUnit.BuildMines();
        //            UpdateUnit(Game.Instance.ActiveUnit);
        //            break;
        //        case OrderType.GoTo:
        //            //TODO: goto key pressed event
        //            UpdateUnit(Game.Instance.ActiveUnit);
        //            break;
        //        case OrderType.Fortify: 
        //            Game.Instance.ActiveUnit.Fortify();
        //            UpdateUnit(Game.Instance.ActiveUnit);
        //            break;
        //        case OrderType.Sleep: 
        //            Game.Instance.ActiveUnit.Sleep();
        //            UpdateUnit(Game.Instance.ActiveUnit);
        //            break;
        //        case OrderType.GoHome:
        //            //TODO: gohome key pressed event
        //            UpdateUnit(Game.Instance.ActiveUnit);
        //            break;
        //        case OrderType.SkipTurn: 
        //            Game.Instance.ActiveUnit.SkipTurn();
        //            UpdateUnit(Game.Instance.ActiveUnit);
        //            break;
        //        case OrderType.BuildCity: 
        //            Game.Instance.ActiveUnit.BuildCity();
        //            UpdateUnit(Game.Instance.ActiveUnit);
        //            break;
        //        case OrderType.BuildRoad: 
        //            Game.Instance.ActiveUnit.BuildRoad();
        //            UpdateUnit(Game.Instance.ActiveUnit);
        //            break;
        //        case OrderType.Transform: 
        //            Game.Instance.ActiveUnit.Transform();
        //            UpdateUnit(Game.Instance.ActiveUnit);
        //            break;
        //        case OrderType.Automate:
        //            //TODO: automate key pressed event
        //            UpdateUnit(Game.Instance.ActiveUnit);
        //            break;
        //        case OrderType.ActivateUnit:
        //            //TODO: activate unit key pressed event
        //            UpdateUnit(Game.Instance.ActiveUnit);
        //            break;
        //        default: break;
        //    }
        //}


        //find out if certain civ has any units awaiting orders
        public static bool AnyUnitsAwaitingOrders(int civId)
        {
            return Game.Units.Any(unit => unit.Civ == civId && unit.AwaitingOrders);
        }


    }
}
