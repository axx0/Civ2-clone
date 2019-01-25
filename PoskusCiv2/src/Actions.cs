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
            //Move on to next unit
            allUnitsEndedTurn = true;
            foreach (IUnit unit in Game.Units.Where(n => n.Civ == Game.Data.HumanPlayerUsed))
            {
                if (!unit.TurnEnded)   //First unit on list which hasn't ended turns is activated
                {
                    Game.Instance.ActiveUnit = unit;
                    //Game.Instance.ActiveUnit.FirstMove = true;

                    allUnitsEndedTurn = false;

                    //Center view on new unit in MapForm
                    MapForm.OffsetX = Game.Instance.ActiveUnit.X2 - 2 * (MapForm.CenterBoxX - 1);  //for centering view on new unit
                    MapForm.OffsetY = Game.Instance.ActiveUnit.Y2 - 2 * (MapForm.CenterBoxY - 1);

                    //Do not allow to move out of map bounds by limiting offset
                    if (MapForm.OffsetX < 0) MapForm.OffsetX = 0;
                    if (MapForm.OffsetX >= 2 * Game.Data.MapXdim - 2 * MapForm.BoxNoX) MapForm.OffsetX = 2 * Game.Data.MapXdim - 2 * MapForm.BoxNoX;
                    if (MapForm.OffsetY < 0) MapForm.OffsetY = 0;
                    if (MapForm.OffsetY >= Game.Data.MapYdim - 2 * MapForm.BoxNoY) MapForm.OffsetY = Game.Data.MapYdim - 2 * MapForm.BoxNoY;

                    //After limiting offset, do not allow some combinations, e.g. (2,1)
                    if (Math.Abs((MapForm.OffsetX - MapForm.OffsetY) % 2) == 1)
                    {
                        if (MapForm.OffsetX + 1 < Game.Data.MapXdim) MapForm.OffsetX += 1;
                        else if (MapForm.OffsetY + 1 < Game.Data.MapYdim) MapForm.OffsetY += 1;
                        else if (MapForm.OffsetX - 1 > 0) MapForm.OffsetX -= 1;
                        else MapForm.OffsetY -= 1;
                    }

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
                unit.MovePointsLost = 0;

                //Increase counters
                if ((unit.Action == OrderType.BuildIrrigation) || (unit.Action == OrderType.BuildRoad) || (unit.Action == OrderType.BuildMine)) unit.Counter += 1;
            }

            //Update all cities
            CitiesTurn();

            //Choose next unit
            ChooseNextUnit();
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
                    case "Build city": break;
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
