using System;
using System.Collections.Generic;
using System.Linq;
using civ2.Enums;
using civ2.Units;
using civ2.Bitmaps;
using civ2.Events;
using ExtensionMethods;

namespace civ2.GameActions
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
            foreach (IUnit unit in Game.Units.Where(n => n.CivId == Game.Instance.ActiveCiv.Id))
            {
                unit.TurnEnded = false;
                unit.MovePointsLost = 0;

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

        //Make visible (potential) hidden tiles when active unit has completed movement
        public static void UpdateWorldMapAfterUnitHasMoved()
        {
            //Offsets of tiles around active unit
            List<int[]> offsets = new List<int[]>
            {
                new int[] {0, -2},
                new int[] {1, -1},
                new int[] {2, 0},
                new int[] {1, 1},
                new int[] {0, 2},
                new int[] {-1, 1},
                new int[] {-2, 0},
                new int[] {-1, -1}
            };

            //For each offset make the tile visible if it isn't yet
            foreach (int[] offset in offsets)
            {
                int[] coords = Ext.Civ2xy(new int[] { Game.Instance.ActiveUnit.X, Game.Instance.ActiveUnit.Y });
                coords[0] += offset[0];
                coords[1] += offset[1];
                Game.TerrainTile[coords[0], coords[1]].Visibility[Game.Instance.ActiveCiv.Id] = true;
            }

            //Update the map image
            Images.RedrawMap(new int[] { Game.Instance.ActiveUnit.X, Game.Instance.ActiveUnit.Y });
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
            return Game.Units.Any(unit => unit.CivId == civId && unit.AwaitingOrders);
        }


    }
}
