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
                    if (!MapPanel.ViewingPiecesMode && Game.Instance.ActiveUnit != null) Actions.GiveOrder(OrderType.MoveSW); 
                    break;
                case Keys.Down:
                case Keys.NumPad2:
                    if (!MapPanel.ViewingPiecesMode && Game.Instance.ActiveUnit != null) Actions.GiveOrder(OrderType.MoveS); 
                    break;
                case Keys.NumPad3:
                    if (!MapPanel.ViewingPiecesMode && Game.Instance.ActiveUnit != null) Actions.GiveOrder(OrderType.MoveSE); 
                    break;
                case Keys.Left:
                case Keys.NumPad4:
                    if (!MapPanel.ViewingPiecesMode && Game.Instance.ActiveUnit != null) Actions.GiveOrder(OrderType.MoveW); 
                    break;
                case Keys.Right:
                case Keys.NumPad6:
                    if (!MapPanel.ViewingPiecesMode && Game.Instance.ActiveUnit != null) Actions.GiveOrder(OrderType.MoveE); 
                    break;
                case Keys.NumPad7:
                    if (!MapPanel.ViewingPiecesMode && Game.Instance.ActiveUnit != null) Actions.GiveOrder(OrderType.MoveNW); 
                    break;
                case Keys.Up:
                case Keys.NumPad8:
                    if (!MapPanel.ViewingPiecesMode && Game.Instance.ActiveUnit != null) Actions.GiveOrder(OrderType.MoveN); 
                    break;
                case Keys.NumPad9:
                    if (!MapPanel.ViewingPiecesMode && Game.Instance.ActiveUnit != null) Actions.GiveOrder(OrderType.MoveNE); 
                    break;
                case Keys.B:
                    if (!MapPanel.ViewingPiecesMode && Game.Instance.ActiveUnit != null) Actions.GiveOrder(OrderType.BuildCity); 
                    break;
                case Keys.F:
                    if (!MapPanel.ViewingPiecesMode && Game.Instance.ActiveUnit != null) Actions.GiveOrder(OrderType.Fortify); 
                    break;
                case Keys.G:
                    if (!MapPanel.ViewingPiecesMode && Game.Instance.ActiveUnit != null) Actions.GiveOrder(OrderType.GoTo); 
                    break;
                case Keys.H:
                    if (!MapPanel.ViewingPiecesMode && Game.Instance.ActiveUnit != null) Actions.GiveOrder(OrderType.GoHome); 
                    break;
                case Keys.I:
                    if (!MapPanel.ViewingPiecesMode && Game.Instance.ActiveUnit != null) Actions.GiveOrder(OrderType.BuildIrrigation); 
                    break;
                case Keys.K:
                    if (!MapPanel.ViewingPiecesMode && Game.Instance.ActiveUnit != null) Actions.GiveOrder(OrderType.Automate); 
                    break;
                case Keys.M:
                    if (!MapPanel.ViewingPiecesMode && Game.Instance.ActiveUnit != null) Actions.GiveOrder(OrderType.BuildMine); 
                    break;
                case Keys.O:
                    if (!MapPanel.ViewingPiecesMode && Game.Instance.ActiveUnit != null) Actions.GiveOrder(OrderType.Transform); 
                    break;
                case Keys.P:
                    if (!MapPanel.ViewingPiecesMode && Game.Instance.ActiveUnit != null) Actions.GiveOrder(OrderType.CleanPollution); 
                    break;
                case Keys.R:
                    if (!MapPanel.ViewingPiecesMode && Game.Instance.ActiveUnit != null) Actions.GiveOrder(OrderType.BuildRoad); 
                    break;
                case Keys.S:
                    if (!MapPanel.ViewingPiecesMode && Game.Instance.ActiveUnit != null) Actions.GiveOrder(OrderType.Sleep); 
                    break;
                case Keys.Enter:
                    if (Options.AlwaysWaitAtEndOfTurn && !Actions.AnyUnitsAwaitingOrders(Data.HumanPlayer)) Actions.GiveOrder(OrderType.EndTurn);
                    //TODO: if enter pressed when view piece above city --> enter city view
                    break;
                //TODO: case Keys.W: wait
                //TODO: case Keys.U: unload
                //TODO: case Keys.A: activate unit
                //TODO: case Keys.X: zoom out
                //TODO: case Keys.Z: zoom in
                //TODO: case Keys.Space: skip turn
                //TODO: case Keys.Enter: ??
                //TODO: case (Keys.Control | Keys.N): EndPlayerTurn_Click(null, null); break;
                //TODO: case (Keys.Shift | Keys.C): FindCity_Click(null, null); break;
                //TODO: case (Keys.Shift | Keys.D): Disband_Click(null, null); break;
                //TODO: case (Keys.Shift | Keys.H): ViewThroneRoom_Click(null, null); break;
                //TODO: case (Keys.Shift | Keys.P): Pillage_Click(null, null); break;
                //TODO: case (Keys.Shift | Keys.R): Revolution_Click(null, null); break;
                //TODO: case (Keys.Shift | Keys.T): TaxRate_Click(null, null); break;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }


    }
}
