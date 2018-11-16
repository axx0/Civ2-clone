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
        public static void Update()
        {
            if (Game.Instance.ActiveUnit.TurnEnded || (Game.Instance.ActiveUnit.Action == UnitAction.Fortified) || (Game.Instance.ActiveUnit.Action == UnitAction.Fortify) || (Game.Instance.ActiveUnit.Action == UnitAction.TransformTerr) || (Game.Instance.ActiveUnit.Action == UnitAction.Sentry))
            {
                Game.Instance.ActiveUnit.TurnEnded = true;
                NextUnit();
            }

            Application.OpenForms.OfType<StatusForm>().First().InvalidatePanel();
            Application.OpenForms.OfType<MapForm>().First().InvalidatePanel();
        }

        //Chose next unit for orders
        public static void NextUnit()
        {
            //Move on to next unit
            bool allUnitsEndedTurn = true;
            foreach (IUnit _unit in Game.Units.Where(n => n.Civ == Game.Data.WhichHumanPlayerIsUsed))
            {
                if (!_unit.TurnEnded)   //First unit on list which hasn't ended turns is activated
                {
                    Game.Instance.ActiveUnit = _unit;
                    Game.Instance.ActiveUnit.FirstMove = true;

                    allUnitsEndedTurn = false;

                    //Center view on new unit in MapForm
                    MapForm.offsetX = 2 * _unit.X + (_unit.Y % 2) - 2 * (MapForm.CenterBoxX - 1);  //for centering view on new unit
                    MapForm.offsetY = _unit.Y - 2 * (MapForm.CenterBoxY - 1);

                    break;
                }
            }

            if (allUnitsEndedTurn) { NewTurn(); }

            Update();
        }

        public static void NewTurn()
        {
            Game.Data.TurnNumber += 1;

            //At beginning of turn, set all units to active
            foreach (IUnit unit in Game.Units.Where(n => n.Civ == Game.Data.WhichHumanPlayerIsUsed))
            {
                unit.TurnEnded = false;
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
                default: break;
            }
        }
    }
}
