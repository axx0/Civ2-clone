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


namespace RTciv2
{
    static class Actions
    {
        static bool noUnitsAwaitingOrders;

        public static void UpdateUnit(IUnit unit)
        {
            //If unit has ended turn
            if (!unit.AwaitingOrders) ChooseNextUnit();

            //Check if unit has done irrigating
            if (unit.Order == OrderType.BuildIrrigation)
            {
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
            }
            else if (unit.Order == OrderType.BuildRoad)
            {
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
            }
            else if (unit.Order == OrderType.BuildMine)
            {
                if (unit.Counter == 2)
                {
                    Game.Map[unit.X, unit.Y].Mining = true;
                    //Game.Map = Draw.DrawMap();  //Update game map
                    //unit.Action = OrderType.NoOrders;
                }
            }
            Application.OpenForms.OfType<StatusForm>().First().RefreshStatusForm();
            //Application.OpenForms.OfType<MapForm>().First().RefreshMapForm();
        }

        //Chose next unit for orders. If all units ended turn, update cities.
        public static void ChooseNextUnit()
        {
            //Create an array of indexes of units awaiting orders
            List<int> indexUAO = new List<int>();
            for (int i = 0; i < Game.Units.Count; i++) 
                if ((Game.Units[i].Civ == Data.HumanPlayer) && Game.Units[i].AwaitingOrders) indexUAO.Add(i);

            //Move on to the next unit
            bool noUnitsAwaitingOrders = true;
            if (indexUAO.Any()) //there are still units awaiting orders
            {
                if (Data.SelectedUnitIndex < indexUAO[indexUAO.Count - 1]) //indexes of the next units in line are > index of unit that just moved
                {
                    for (int i = 0; i < indexUAO.Count; i++) 
                        if (indexUAO[i] > Data.SelectedUnitIndex) { Data.SelectedUnitIndex = indexUAO[i]; break; }   //chose next index
                }
                else    //else chose the 1st index in list (go to beginning of list)
                {
                    Data.SelectedUnitIndex = indexUAO[0];
                }

                Game.Instance.ActiveUnit = Game.Units[Data.SelectedUnitIndex];
                noUnitsAwaitingOrders = false;

                //If necessary, center view on new unit in MapForm
                //Application.OpenForms.OfType<MapForm>().First().MoveMapViewIfNecessary();

                //Set active box coords to next unit
                //MapForm.ActiveBoxX = Game.Instance.ActiveUnit.X;
                //MapForm.ActiveBoxY = Game.Instance.ActiveUnit.Y;
            }

            ////Move on to next unit
            //allUnitsEndedTurn = true;
            //foreach (IUnit unit in Game.Units.Where(n => n.Civ == Game.Data.HumanPlayerUsed))
            //{
            //    if (!unit.TurnEnded)   //First unit on list which hasn't ended turns is activated
            //    {
            //        Game.Instance.ActiveUnit = unit;
            //        //Game.Instance.ActiveUnit.FirstMove = true;

            //        allUnitsEndedTurn = false;

            //        //If necessary, center view on new unit in MapForm
            //        Application.OpenForms.OfType<MapForm>().First().MoveMapViewIfNecessary();

            //        //Set active box coords to next unit
            //        MapForm.ActiveBoxX = Game.Instance.ActiveUnit.X2;
            //        MapForm.ActiveBoxY = Game.Instance.ActiveUnit.Y2;

            //        break;
            //    }
            //}

            //If all units ended turn ==> start new turn. If not, update unit stats.
            if (noUnitsAwaitingOrders)
            {
                //If "wait at end of turn is enabled" show the message in status form & wait for ENTER pressed
                if (Options.AlwaysWaitAtEndOfTurn)
                {
                    //MapForm.ViewingPiecesMode = true;
                    Application.OpenForms.OfType<StatusForm>().First().ShowEndOfTurnMessage();
                    Application.OpenForms.OfType<MainCiv2Window>().First().UpdateOrdersMenu();
                }
                else NewTurn();
            }
            else
            {
                //Unit is active. Make sure the menus are enabled.
                //MapForm.ViewingPiecesMode = false;
                UpdateUnit(Game.Instance.ActiveUnit);
                Application.OpenForms.OfType<MainCiv2Window>().First().UpdateOrdersMenu();
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

        public static void NewTurn()
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

        public static void GiveCommand(string action)
        {
            //If "wait for end of turn" is enabled & all units have ended turn --> wait for ENTER and then make next game turn
            if (Options.AlwaysWaitAtEndOfTurn && !AnyUnitsAwaitingOrders(Data.HumanPlayer))
            {
                if (action == "ENTER")
                {
                    Application.OpenForms.OfType<StatusForm>().First().HideEndOfTurnMessage();
                    //MapForm.ViewingPiecesMode = false; //reset it
                    NewTurn();
                }

            }
            else
            {
                switch (action)
                {
                    case "Activate unit": break;
                    case "Automate": break;
                    case "Build city": Game.Instance.ActiveUnit.BuildCity(); break;
                    case "Build road": Game.Instance.ActiveUnit.BuildRoad(); break;
                    case "Build irrigation": Game.Instance.ActiveUnit.BuildIrrigation(); break;
                    case "Build mines/Change forest": Game.Instance.ActiveUnit.BuildMines(); break;
                    case "Fortify": Game.Instance.ActiveUnit.Fortify(); break;
                    case "Go Home": break;
                    case "Go To": Game.Instance.ActiveUnit.GoToX = 5; break;    //TO-DO
                    case "Move SW": Game.Instance.ActiveUnit.Move(-1, 1); break;
                    case "Move S": Game.Instance.ActiveUnit.Move(0, 2); break;
                    case "Move SE": Game.Instance.ActiveUnit.Move(1, 1); break;
                    case "Move E": Game.Instance.ActiveUnit.Move(2, 0); break;
                    case "Move W": Game.Instance.ActiveUnit.Move(-2, 0); break;
                    case "Move NW": Game.Instance.ActiveUnit.Move(-1, -1); break;
                    case "Move N": Game.Instance.ActiveUnit.Move(0, -2); break;
                    case "Move NE": Game.Instance.ActiveUnit.Move(1, -1); break;
                    case "Sleep": Game.Instance.ActiveUnit.Sleep(); break;
                    case "Skip turn": Game.Instance.ActiveUnit.SkipTurn(); break;
                    case "Terraform": Game.Instance.ActiveUnit.Transform(); break;
                    default: break;
                }
                UpdateUnit(Game.Instance.ActiveUnit);
            }
        }

        //find out if certain civ has any units awaiting orders
        static bool AnyUnitsAwaitingOrders(int civId)
        {
            List<int> indexUAO = new List<int>();            //Create an array of indexes of units awaiting orders
            for (int i = 0; i < Game.Units.Count; i++) if ((Game.Units[i].Civ == civId) && Game.Units[i].AwaitingOrders) indexUAO.Add(i);

            if (indexUAO.Any()) return true;
            else return false;
        }
    }
}
