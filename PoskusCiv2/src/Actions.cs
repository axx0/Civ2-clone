using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PoskusCiv2.Enums;
using PoskusCiv2.Units;
using PoskusCiv2.Imagery;
using PoskusCiv2.Terrains;
using PoskusCiv2.Improvements;
using PoskusCiv2.Forms;


namespace PoskusCiv2
{
    static class Actions
    {
        static bool allUnitsEndedTurn;

        public static void UpdateUnit(IUnit unit)
        {
            //If unit has ended turn
            if (unit.TurnEnded) ChooseNextUnit();

            //Check if unit has done irrigating
            if (unit.Action == OrderType.BuildIrrigation)
            {
                if (unit.Counter == 2)
                {
                    if (Game.Terrain[unit.X, unit.Y].Irrigation == false) //Build irrigation
                    {
                        Game.Terrain[unit.X, unit.Y].Irrigation = true;
                    }
                    else if ((Game.Terrain[unit.X, unit.Y].Irrigation == true) && (Game.Terrain[unit.X, unit.Y].Farmland == false)) //Build farms
                    {
                        Game.Terrain[unit.X, unit.Y].Farmland = true;
                    }
                    Game.Map = Draw.DrawMap();  //Update game map
                    //unit.Action = OrderType.NoOrders;
                }
            }
            else if (unit.Action == OrderType.BuildRoad)
            {
                if (unit.Counter == 2)
                {
                    if (Game.Terrain[unit.X, unit.Y].Road == false) //Build road
                    {
                        Game.Terrain[unit.X, unit.Y].Road = true;
                    }
                    else if ((Game.Terrain[unit.X, unit.Y].Road == true) && (Game.Terrain[unit.X, unit.Y].Railroad == false)) //Build railroad
                    {
                        Game.Terrain[unit.X, unit.Y].Railroad = true;
                    }
                    Game.Map = Draw.DrawMap();  //Update game map
                    //unit.Action = OrderType.NoOrders;
                }
            }
            else if (unit.Action == OrderType.BuildMine)
            {
                if (unit.Counter == 2)
                {
                    Game.Terrain[unit.X, unit.Y].Mining = true;
                    Game.Map = Draw.DrawMap();  //Update game map
                    //unit.Action = OrderType.NoOrders;
                }
            }
            Application.OpenForms.OfType<StatusForm>().First().RefreshStatusForm();
            Application.OpenForms.OfType<MapForm>().First().RefreshMapForm();
        }

        //Chose next unit for orders. If all units ended turn, update cities.
        public static void ChooseNextUnit()
        {
            //Set next active unit by increasing unit index.
            //First get index of current unit in list
            int index = Game.Units.FindIndex(unit => unit == Game.Instance.ActiveUnit);
            for (int i = index; i < Game.Units.Count; i++)
            {

            }

            //Move on to next unit
            allUnitsEndedTurn = true;
            foreach (IUnit unit in Game.Units.Where(n => n.Civ == Game.Data.HumanPlayerUsed))
            {
                if (!unit.TurnEnded)   //First unit on list which hasn't ended turns is activated
                {
                    Game.Instance.ActiveUnit = unit;
                    //Game.Instance.ActiveUnit.FirstMove = true;

                    allUnitsEndedTurn = false;

                    //If necessary, center view on new unit in MapForm
                    Application.OpenForms.OfType<MapForm>().First().MoveMapViewIfNecessary();

                    //Set active box coords to next unit
                    MapForm.ActiveBoxX = Game.Instance.ActiveUnit.X2;
                    MapForm.ActiveBoxY = Game.Instance.ActiveUnit.Y2;

                    break;
                }
            }

            //If all units ended turn ==> start new turn. If not, update unit stats.
            if (allUnitsEndedTurn)
            {
                //If "wait at end of turn is enabled" show the message in status form & wait for ENTER pressed
                if (Game.Options.AlwaysWaitAtEndOfTurn)
                {
                    MapForm.ViewingPiecesMode = true;
                    Application.OpenForms.OfType<StatusForm>().First().ShowEndOfTurnMessage();
                    Application.OpenForms.OfType<MainCiv2Window>().First().UpdateOrdersMenu();
                }
                else NewTurn();
            }
            else
            {
                //Unit is active. Make sure the menus are enabled.
                MapForm.ViewingPiecesMode = false;
                UpdateUnit(Game.Instance.ActiveUnit);
                Application.OpenForms.OfType<MainCiv2Window>().First().UpdateOrdersMenu();
            }
        }

        //Update stats of all cities
        public static void CitiesTurn()
        {
            foreach (City city in Game.Cities.Where(a => a.Owner == Game.Data.HumanPlayerUsed))
            {
                //city.NewTurn();
            }
        }

        public static void NewTurn()
        {
            Game.Data.TurnNumber += 1;

            //Set all units to active
            foreach (IUnit unit in Game.Units.Where(n => n.Civ == Game.Data.HumanPlayerUsed))
            {
                unit.TurnEnded = false;
                unit.MovePoints = unit.MaxMovePoints;

                //Increase counters
                if ((unit.Action == OrderType.BuildIrrigation) || (unit.Action == OrderType.BuildRoad) || (unit.Action == OrderType.BuildMine)) unit.Counter += 1;
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
            if (Game.Options.AlwaysWaitAtEndOfTurn && allUnitsEndedTurn)
            {
                if (action == "ENTER")
                {
                    Application.OpenForms.OfType<StatusForm>().First().HideEndOfTurnMessage();
                    MapForm.ViewingPiecesMode = false; //reset it
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
    }
}
