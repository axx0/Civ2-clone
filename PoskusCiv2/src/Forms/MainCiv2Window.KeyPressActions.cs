using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RTciv2.Enums;
using RTciv2.Events;
using RTciv2.GameActions;

namespace RTciv2.Forms
{
    public partial class MainCiv2Window : Form
    {
        public static event EventHandler<CheckIfCityCanBeViewedEventArgs> OnCheckIfCityCanBeViewed;

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.NumPad1:
                    {
                        if (!MapPanel.ViewPiecesMode && Game.Instance.ActiveUnit != null && Game.Instance.ActiveUnit.MovementCounter == 0)
                        {
                            Actions.IssueUnitOrder(OrderType.MoveSW);
                        }
                        else if (MapPanel.ViewPiecesMode)
                        {
                            MapPanel.ActiveXY[0] -= 1;
                            MapPanel.ActiveXY[1] += 1;
                            OnMapEvent?.Invoke(null, new MapEventArgs(MapEventType.ViewPieceMoved));
                        }
                        break;
                    }
                case Keys.Down:
                case Keys.NumPad2:
                    {
                        if (!MapPanel.ViewPiecesMode && Game.Instance.ActiveUnit != null && Game.Instance.ActiveUnit.MovementCounter == 0)
                        {
                            Actions.IssueUnitOrder(OrderType.MoveS);
                        }
                        else if (MapPanel.ViewPiecesMode)
                        {
                            MapPanel.ActiveXY[1] += 2;
                            OnMapEvent?.Invoke(null, new MapEventArgs(MapEventType.ViewPieceMoved));
                        }
                        break;
                    }
                case Keys.NumPad3:
                    {
                        if (!MapPanel.ViewPiecesMode && Game.Instance.ActiveUnit != null && Game.Instance.ActiveUnit.MovementCounter == 0)
                        {
                            Actions.IssueUnitOrder(OrderType.MoveSE);
                        }
                        else if (MapPanel.ViewPiecesMode)
                        {
                            MapPanel.ActiveXY[0] += 1;
                            MapPanel.ActiveXY[1] += 1;
                            OnMapEvent?.Invoke(null, new MapEventArgs(MapEventType.ViewPieceMoved));
                        }
                        break;
                    }
                case Keys.Left:
                case Keys.NumPad4:
                    {
                        if (!MapPanel.ViewPiecesMode && Game.Instance.ActiveUnit != null && Game.Instance.ActiveUnit.MovementCounter == 0)
                        {
                            Actions.IssueUnitOrder(OrderType.MoveW);
                        }
                        else if (MapPanel.ViewPiecesMode)
                        {
                            MapPanel.ActiveXY[0] -= 2;
                            OnMapEvent?.Invoke(null, new MapEventArgs(MapEventType.ViewPieceMoved));
                        }
                        break;
                    }
                case Keys.Right:
                case Keys.NumPad6:
                    {
                        if (!MapPanel.ViewPiecesMode && Game.Instance.ActiveUnit != null && Game.Instance.ActiveUnit.MovementCounter == 0)
                        {
                            Actions.IssueUnitOrder(OrderType.MoveE);
                        }
                        else if (MapPanel.ViewPiecesMode)
                        {
                            MapPanel.ActiveXY[0] += 2;
                            OnMapEvent?.Invoke(null, new MapEventArgs(MapEventType.ViewPieceMoved));
                        }
                        break;
                    }
                case Keys.NumPad7:
                    {
                        if (!MapPanel.ViewPiecesMode && Game.Instance.ActiveUnit != null && Game.Instance.ActiveUnit.MovementCounter == 0)
                        {
                            Actions.IssueUnitOrder(OrderType.MoveNW);
                        }
                        else if (MapPanel.ViewPiecesMode)
                        {
                            MapPanel.ActiveXY[0] -= 1;
                            MapPanel.ActiveXY[1] -= 1;
                            OnMapEvent?.Invoke(null, new MapEventArgs(MapEventType.ViewPieceMoved));
                        }
                        break;
                    }
                case Keys.Up:
                case Keys.NumPad8:
                    {
                        if (!MapPanel.ViewPiecesMode && Game.Instance.ActiveUnit != null && Game.Instance.ActiveUnit.MovementCounter == 0)
                        {
                            Actions.IssueUnitOrder(OrderType.MoveN);
                        }
                        else if (MapPanel.ViewPiecesMode)
                        {
                            MapPanel.ActiveXY[1] -= 2;
                            OnMapEvent?.Invoke(null, new MapEventArgs(MapEventType.ViewPieceMoved));
                        }
                        break;
                    }
                case Keys.NumPad9:
                    {
                        if (!MapPanel.ViewPiecesMode && Game.Instance.ActiveUnit != null && Game.Instance.ActiveUnit.MovementCounter == 0)
                        {
                            Actions.IssueUnitOrder(OrderType.MoveNE);
                        }
                        else if (MapPanel.ViewPiecesMode)
                        {
                            MapPanel.ActiveXY[0] += 1;
                            MapPanel.ActiveXY[1] -= 1;
                            OnMapEvent?.Invoke(null, new MapEventArgs(MapEventType.ViewPieceMoved));
                        }
                        break;
                    }
                case Keys.B:
                    if (!MapPanel.ViewPiecesMode && Game.Instance.ActiveUnit != null && Game.Instance.ActiveUnit.MovementCounter == 0) 
                        Actions.IssueUnitOrder(OrderType.BuildCity); 
                    break;
                case Keys.F:
                    if (!MapPanel.ViewPiecesMode && Game.Instance.ActiveUnit != null && Game.Instance.ActiveUnit.MovementCounter == 0) 
                        Actions.IssueUnitOrder(OrderType.Fortify); 
                    break;
                case Keys.G:
                    if (!MapPanel.ViewPiecesMode && Game.Instance.ActiveUnit != null && Game.Instance.ActiveUnit.MovementCounter == 0) 
                        Actions.IssueUnitOrder(OrderType.GoTo); 
                    break;
                case Keys.H:
                    if (!MapPanel.ViewPiecesMode && Game.Instance.ActiveUnit != null && Game.Instance.ActiveUnit.MovementCounter == 0) 
                        Actions.IssueUnitOrder(OrderType.GoHome); 
                    break;
                case Keys.I:
                    if (!MapPanel.ViewPiecesMode && Game.Instance.ActiveUnit != null && Game.Instance.ActiveUnit.MovementCounter == 0) 
                        Actions.IssueUnitOrder(OrderType.BuildIrrigation); 
                    break;
                case Keys.K:
                    if (!MapPanel.ViewPiecesMode && Game.Instance.ActiveUnit != null && Game.Instance.ActiveUnit.MovementCounter == 0) 
                        Actions.IssueUnitOrder(OrderType.Automate); 
                    break;
                case Keys.M:
                    if (!MapPanel.ViewPiecesMode && Game.Instance.ActiveUnit != null && Game.Instance.ActiveUnit.MovementCounter == 0) 
                        Actions.IssueUnitOrder(OrderType.BuildMine); 
                    break;
                case Keys.O:
                    if (!MapPanel.ViewPiecesMode && Game.Instance.ActiveUnit != null && Game.Instance.ActiveUnit.MovementCounter == 0) 
                        Actions.IssueUnitOrder(OrderType.Transform); 
                    break;
                case Keys.P:
                    if (!MapPanel.ViewPiecesMode && Game.Instance.ActiveUnit != null && Game.Instance.ActiveUnit.MovementCounter == 0) 
                        Actions.IssueUnitOrder(OrderType.CleanPollution); 
                    break;
                case Keys.R:
                    if (!MapPanel.ViewPiecesMode && Game.Instance.ActiveUnit != null && Game.Instance.ActiveUnit.MovementCounter == 0) 
                        Actions.IssueUnitOrder(OrderType.BuildRoad); 
                    break;
                case Keys.S:
                    if (!MapPanel.ViewPiecesMode && Game.Instance.ActiveUnit != null && Game.Instance.ActiveUnit.MovementCounter == 0) 
                        Actions.IssueUnitOrder(OrderType.Sleep); 
                    break;
                case Keys.Enter:
                    if (Options.AlwaysWaitAtEndOfTurn && !Actions.AnyUnitsAwaitingOrders(Data.HumanPlayer))
                        Actions.NewPlayerTurn();
                    else 
                        OnCheckIfCityCanBeViewed?.Invoke(null, new CheckIfCityCanBeViewedEventArgs());  //if enter pressed when view piece above city --> enter city view
                    break;
                case Keys.Space:
                    if (Game.Instance.ActiveUnit != null && Game.Instance.ActiveUnit.MovementCounter == 0) 
                        Actions.IssueUnitOrder(OrderType.SkipTurn);
                    break;
                case Keys.W:
                    if (Game.Instance.ActiveUnit != null) Actions.ChooseNextUnit();
                    break;
                case Keys.Y:
                    OnMapEvent?.Invoke(null, new MapEventArgs(MapEventType.ToggleBetweenCurrentEntireMapView));
                    break;

                    //TODO: case Keys.U: unload
                    //TODO: case Keys.A: activate unit
                    //TODO: case Keys.X: zoom out
                    //TODO: case Keys.Z: zoom in
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
