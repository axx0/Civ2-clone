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
                    MapForm.offsetX = Game.Instance.ActiveUnit.X2 - 2 * (MapForm.CenterBoxX - 1);  //for centering view on new unit
                    MapForm.offsetY = Game.Instance.ActiveUnit.Y2 - 2 * (MapForm.CenterBoxY - 1);

                    //Do not allow to move out of map bounds by limiting offset
                    if (MapForm.offsetX < 0) MapForm.offsetX = 0;
                    if (MapForm.offsetX >= 2 * Game.Data.MapXdim - 2 * MapForm.BoxNoX) MapForm.offsetX = 2 * Game.Data.MapXdim - 2 * MapForm.BoxNoX;
                    if (MapForm.offsetY < 0) MapForm.offsetY = 0;
                    if (MapForm.offsetY >= Game.Data.MapYdim - 2 * MapForm.BoxNoY) MapForm.offsetY = Game.Data.MapYdim - 2 * MapForm.BoxNoY;

                    //After limiting offset, do not allow some combinations, e.g. (2,1)
                    if (Math.Abs((MapForm.offsetX - MapForm.offsetY) % 2) == 1)
                    {
                        if (MapForm.offsetX + 1 < Game.Data.MapXdim) MapForm.offsetX += 1;
                        else if (MapForm.offsetY + 1 < Game.Data.MapYdim) MapForm.offsetY += 1;
                        else if (MapForm.offsetX - 1 > 0) MapForm.offsetX -= 1;
                        else MapForm.offsetY -= 1;
                    }

                    break;
                }
            }
            
            //If all units ended turn ==> start new turn. If not, update unit stats.
            if (allUnitsEndedTurn)
            {
                //If "wait at end of turn is enabled" show the message in status form & wait for ENTER pressed
                if (Game.Options.AlwaysWaitAtEndOfTurn) Application.OpenForms.OfType<StatusForm>().First().ShowEndOfTurnMessage();
                else NewTurn();
            }
            else UpdateUnit(Game.Instance.ActiveUnit);
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

        public static void UnitKeyboardAction(char pressedKey)
        {
            //If "wait for end of turn" is enabled & all units have ended turn --> wait for ENTER and then make next game turn
            if (Game.Options.AlwaysWaitAtEndOfTurn && allUnitsEndedTurn)
            {
                switch (pressedKey)
                {
                    case (char)Keys.Enter:
                        {
                            Application.OpenForms.OfType<StatusForm>().First().HideEndOfTurnMessage();
                            NewTurn();
                            break;
                        }
                    default: break;
                }
            }
            else
            { 
                switch (pressedKey)
                {
                    case (char)Keys.Enter: break;
                    case (char)Keys.D1: Game.Instance.ActiveUnit.Move(-1, 1); break;
                    case (char)Keys.D2: Game.Instance.ActiveUnit.Move(0, 2); break;
                    case (char)Keys.D3: Game.Instance.ActiveUnit.Move(1, 1); break;
                    case (char)Keys.D4: Game.Instance.ActiveUnit.Move(-2, 0); break;
                    case (char)Keys.D6: Game.Instance.ActiveUnit.Move(2, 0); break;
                    case (char)Keys.D7: Game.Instance.ActiveUnit.Move(-1, -1); break;
                    case (char)Keys.D8: Game.Instance.ActiveUnit.Move(0, -2); break;
                    case (char)Keys.D9: Game.Instance.ActiveUnit.Move(1, -1); break;
                    case (char)Keys.Space: Game.Instance.ActiveUnit.SkipTurn(); break;
                    case 's': Game.Instance.ActiveUnit.Sleep(); break;
                    case 'f': Game.Instance.ActiveUnit.Fortify(); break;
                    case 'i': Game.Instance.ActiveUnit.Irrigate(); break;
                    case 'o': Game.Instance.ActiveUnit.Transform(); break;
                    case 'r': Game.Instance.ActiveUnit.BuildRoad(); break;
                    case 'm': Game.Instance.ActiveUnit.BuildMines(); break;
                    default: break;
                }
                UpdateUnit(Game.Instance.ActiveUnit);
            }
        }

    }
}
