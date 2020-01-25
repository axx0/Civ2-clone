using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RTciv2.Enums;

namespace RTciv2.Forms
{
    public partial class MainCiv2Window : Form
    {
        
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.NumPad1:
                    if (Game.Instance.ActiveUnit != null) Game.Instance.ActiveUnit.Move(-1, 1);
                    break;
                case Keys.NumPad2:
                    if (Game.Instance.ActiveUnit != null) Game.Instance.ActiveUnit.Move(0, 2);
                    break;
                case Keys.NumPad3:
                    if (Game.Instance.ActiveUnit != null) Game.Instance.ActiveUnit.Move(1, 1);
                    break;
                case Keys.NumPad4:
                    if (Game.Instance.ActiveUnit != null) Game.Instance.ActiveUnit.Move(-2, 0);
                    break;
                case Keys.NumPad6:
                    if (Game.Instance.ActiveUnit != null) Game.Instance.ActiveUnit.Move(2, 0);
                    break;
                case Keys.NumPad7:
                    if (Game.Instance.ActiveUnit != null) Game.Instance.ActiveUnit.Move(-1, -1);
                    break;
                case Keys.NumPad8:
                    if (Game.Instance.ActiveUnit != null) Game.Instance.ActiveUnit.Move(0, -2);
                    break;
                case Keys.NumPad9:
                    if (Game.Instance.ActiveUnit != null) Game.Instance.ActiveUnit.Move(1, -1);
                    break;

                    //case Keys.Down: Actions.GiveCommand("Move S"); break;
                    //case Keys.Left: Actions.GiveCommand("Move W"); break;
                    //case Keys.Right: Actions.GiveCommand("Move E"); break;
                    //case Keys.Up: Actions.GiveCommand("Move N"); break;
                    //case Keys.A: ActivateUnit_Click(null, null); break;
                    //case Keys.B: BuildNewCity_Click(null, null); break;
                    //case Keys.F: Fortify_Click(null, null); break;
                    //case Keys.G: GoTo_Click(null, null); break;
                    //case Keys.H: GoHomeToNearestCity_Click(null, null); break;
                    //case Keys.I: BuildIrrigation_Click(null, null); break;
                    //case Keys.K: AutomateSettler_Click(null, null); break;
                    //case Keys.M: BuildMinesChangeForest_Click(null, null); break;
                    //case Keys.O: Actions.GiveCommand("Terraform"); break;
                    //case Keys.P: CleanUpPollution_Click(null, null); break; //paradrop!!!
                    //case Keys.R: BuildRoad_Click(null, null); break;
                    //case Keys.S: Sleep_Click(null, null); break;
                    //case Keys.U: Unload_Click(null, null); break;
                    //case Keys.W: Wait_Click(null, null); break;
                    //case Keys.X: MapPanel.ZoomLvl--; break;
                    //case Keys.Z: MapPanel.ZoomLvl++; break;
                    //case Keys.Space: SkipTurn_Click(null, null); break;
                    //case Keys.Enter: Actions.GiveCommand("ENTER"); break;
                    //case (Keys.Control | Keys.N): EndPlayerTurn_Click(null, null); break;
                    //case (Keys.Shift | Keys.C): FindCity_Click(null, null); break;
                    //case (Keys.Shift | Keys.D): Disband_Click(null, null); break;
                    //case (Keys.Shift | Keys.H): ViewThroneRoom_Click(null, null); break;
                    //case (Keys.Shift | Keys.P): Pillage_Click(null, null); break;
                    //case (Keys.Shift | Keys.R): Revolution_Click(null, null); break;
                    //case (Keys.Shift | Keys.T): TaxRate_Click(null, null); break;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }


    }
}
