using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PoskusCiv2.Enums;
using PoskusCiv2.Units;
using PoskusCiv2.Terrains;
using PoskusCiv2.Improvements;
using PoskusCiv2.Forms;


namespace PoskusCiv2
{
    static class Actions
    {
        public static void UpdateUnit(IUnit unit)
        {
            //If unit has ended turn
            if (unit.TurnEnded) { NextUnit(); }

            //Check if unit has done irrigating
            if (unit.Action == UnitAction.BuildIrrigation)
            {
                if (unit.Counter == 2)
                {                    
                    //Game.Terrain[unit.X, unit.Y] = TerrainType.Ir
                    unit.Action = UnitAction.NoOrders;
                }
            }

            Application.OpenForms.OfType<StatusForm>().First().InvalidatePanel();
            Application.OpenForms.OfType<MapForm>().First().InvalidatePanel();
        }

        //Chose next unit for orders
        public static void NextUnit()
        {
            //Move on to next unit
            bool allUnitsEndedTurn = true;
            foreach (IUnit unit in Game.Units.Where(n => n.Civ == Game.Data.WhichHumanPlayerIsUsed))
            {
                if (!unit.TurnEnded)   //First unit on list which hasn't ended turns is activated
                {
                    Game.Instance.ActiveUnit = unit;
                    //Game.Instance.ActiveUnit.FirstMove = true;

                    allUnitsEndedTurn = false;

                    //Center view on new unit in MapForm
                    MapForm.offsetX = Game.Instance.ActiveUnit.X2 - 2 * (MapForm.CenterBoxX - 1);  //for centering view on new unit
                    MapForm.offsetY = Game.Instance.ActiveUnit.Y2 - 2 * (MapForm.CenterBoxY - 1);

                    break;
                }
            }

            if (allUnitsEndedTurn) { NewTurn(); }

            UpdateUnit(Game.Instance.ActiveUnit);
        }

        public static void NewTurn()
        {
            Game.Data.TurnNumber += 1;

            //At beginning of turn, set all units to active
            foreach (IUnit unit in Game.Units.Where(n => n.Civ == Game.Data.WhichHumanPlayerIsUsed))
            {
                //Increase counters
                if (unit.Action == UnitAction.BuildIrrigation)
                {
                    unit.Counter += 1;
                    UpdateUnit(unit);
                }

                unit.TurnEnded = false;
                unit.MovePointsLost = 0;
            }
        }

        public static void UnitKeyboardAction(char pressedKey)
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
                case 's': Game.Instance.ActiveUnit.Sentry(); break;
                case 'f': Game.Instance.ActiveUnit.Fortify(); break;
                case 'i': Game.Instance.ActiveUnit.Irrigate(); break;
                case 'o': Game.Instance.ActiveUnit.Terraform(); break;
                case 'r': Game.Instance.ActiveUnit.BuildRoad(); break;
                default: break;
            }
        }
    }
}
