using System;
using System.Windows.Forms;
using civ2.Enums;
using civ2.Events;

namespace civ2.Forms
{
    public partial class Main : Form
    {
        public static event EventHandler<CheckIfCityCanBeViewedEventArgs> OnCheckIfCityCanBeViewed;

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.NumPad1:
                    {
                        if (!ViewPieceMode && Game.ActiveUnit != null && Game.ActiveUnit.MovementCounter == 0)
                        {
                            Game.IssueUnitOrder(OrderType.MoveSW);
                        }
                        else if (ViewPieceMode)
                        {
                            Game.ActiveXY[0] -= 1;
                            Game.ActiveXY[1] += 1;
                            OnMapEvent?.Invoke(null, new MapEventArgs(MapEventType.ViewPieceMoved));
                        }
                        break;
                    }
                case Keys.Down:
                case Keys.NumPad2:
                    {
                        if (!ViewPieceMode && Game.ActiveUnit != null && Game.ActiveUnit.MovementCounter == 0)
                        {
                            Game.IssueUnitOrder(OrderType.MoveS);
                        }
                        else if (ViewPieceMode)
                        {
                            Game.ActiveXY[1] += 2;
                            OnMapEvent?.Invoke(null, new MapEventArgs(MapEventType.ViewPieceMoved));
                        }
                        break;
                    }
                case Keys.NumPad3:
                    {
                        if (!ViewPieceMode && Game.ActiveUnit != null && Game.ActiveUnit.MovementCounter == 0)
                        {
                            Game.IssueUnitOrder(OrderType.MoveSE);
                        }
                        else if (ViewPieceMode)
                        {
                            Game.ActiveXY[0] += 1;
                            Game.ActiveXY[1] += 1;
                            OnMapEvent?.Invoke(null, new MapEventArgs(MapEventType.ViewPieceMoved));
                        }
                        break;
                    }
                case Keys.Left:
                case Keys.NumPad4:
                    {
                        if (!ViewPieceMode && Game.ActiveUnit != null && Game.ActiveUnit.MovementCounter == 0)
                        {
                            Game.IssueUnitOrder(OrderType.MoveW);
                        }
                        else if (ViewPieceMode)
                        {
                            Game.ActiveXY[0] -= 2;
                            OnMapEvent?.Invoke(null, new MapEventArgs(MapEventType.ViewPieceMoved));
                        }
                        break;
                    }
                case Keys.Right:
                case Keys.NumPad6:
                    {
                        if (!ViewPieceMode && Game.ActiveUnit != null && Game.ActiveUnit.MovementCounter == 0)
                        {
                            Game.IssueUnitOrder(OrderType.MoveE);
                        }
                        else if (ViewPieceMode)
                        {
                            Game.ActiveXY[0] += 2;
                            OnMapEvent?.Invoke(null, new MapEventArgs(MapEventType.ViewPieceMoved));
                        }
                        break;
                    }
                case Keys.NumPad7:
                    {
                        if (!ViewPieceMode && Game.ActiveUnit != null && Game.ActiveUnit.MovementCounter == 0)
                        {
                            Game.IssueUnitOrder(OrderType.MoveNW);
                        }
                        else if (ViewPieceMode)
                        {
                            Game.ActiveXY[0] -= 1;
                            Game.ActiveXY[1] -= 1;
                            OnMapEvent?.Invoke(null, new MapEventArgs(MapEventType.ViewPieceMoved));
                        }
                        break;
                    }
                case Keys.Up:
                case Keys.NumPad8:
                    {
                        if (!ViewPieceMode && Game.ActiveUnit != null && Game.ActiveUnit.MovementCounter == 0)
                        {
                            Game.IssueUnitOrder(OrderType.MoveN);
                        }
                        else if (ViewPieceMode)
                        {
                            Game.ActiveXY[1] -= 2;
                            OnMapEvent?.Invoke(null, new MapEventArgs(MapEventType.ViewPieceMoved));
                        }
                        break;
                    }
                case Keys.NumPad9:
                    {
                        if (!ViewPieceMode && Game.ActiveUnit != null && Game.ActiveUnit.MovementCounter == 0)
                        {
                            Game.IssueUnitOrder(OrderType.MoveNE);
                        }
                        else if (ViewPieceMode)
                        {
                            Game.ActiveXY[0] += 1;
                            Game.ActiveXY[1] -= 1;
                            OnMapEvent?.Invoke(null, new MapEventArgs(MapEventType.ViewPieceMoved));
                        }
                        break;
                    }
                case Keys.B:
                    if (!ViewPieceMode && Game.ActiveUnit != null && Game.ActiveUnit.MovementCounter == 0)
                        Game.IssueUnitOrder(OrderType.BuildCity); 
                    break;
                case Keys.F:
                    if (!ViewPieceMode && Game.ActiveUnit != null && Game.ActiveUnit.MovementCounter == 0)
                        Game.IssueUnitOrder(OrderType.Fortify); 
                    break;
                case Keys.G:
                    if (!ViewPieceMode && Game.ActiveUnit != null && Game.ActiveUnit.MovementCounter == 0)
                        Game.IssueUnitOrder(OrderType.GoTo); 
                    break;
                case Keys.H:
                    if (!ViewPieceMode && Game.ActiveUnit != null && Game.ActiveUnit.MovementCounter == 0)
                        Game.IssueUnitOrder(OrderType.GoHome); 
                    break;
                case Keys.I:
                    if (!ViewPieceMode && Game.ActiveUnit != null && Game.ActiveUnit.MovementCounter == 0)
                        Game.IssueUnitOrder(OrderType.BuildIrrigation); 
                    break;
                case Keys.K:
                    if (!ViewPieceMode && Game.ActiveUnit != null && Game.ActiveUnit.MovementCounter == 0)
                        Game.IssueUnitOrder(OrderType.Automate); 
                    break;
                case Keys.M:
                    if (!ViewPieceMode && Game.ActiveUnit != null && Game.ActiveUnit.MovementCounter == 0)
                        Game.IssueUnitOrder(OrderType.BuildMine); 
                    break;
                case Keys.O:
                    if (!ViewPieceMode && Game.ActiveUnit != null && Game.ActiveUnit.MovementCounter == 0)
                        Game.IssueUnitOrder(OrderType.Transform); 
                    break;
                case Keys.P:
                    if (!ViewPieceMode && Game.ActiveUnit != null && Game.ActiveUnit.MovementCounter == 0)
                        Game.IssueUnitOrder(OrderType.CleanPollution); 
                    break;
                case Keys.R:
                    if (!ViewPieceMode && Game.ActiveUnit != null && Game.ActiveUnit.MovementCounter == 0)
                        Game.IssueUnitOrder(OrderType.BuildRoad); 
                    break;
                case Keys.S:
                    if (!ViewPieceMode && Game.ActiveUnit != null && Game.ActiveUnit.MovementCounter == 0)
                        Game.IssueUnitOrder(OrderType.Sleep); 
                    break;
                case Keys.Enter:
                    if (Game.Options.AlwaysWaitAtEndOfTurn && !Game.PlayerCiv.AnyUnitsAwaitingOrders)
                        Game.NewPlayerTurn();
                    else 
                        OnCheckIfCityCanBeViewed?.Invoke(null, new CheckIfCityCanBeViewedEventArgs());  //if enter pressed when view piece above city --> enter city view
                    break;
                case Keys.Space:
                    if (Game.ActiveUnit != null && Game.ActiveUnit.MovementCounter == 0)
                        Game.IssueUnitOrder(OrderType.SkipTurn);
                    break;
                case Keys.W:
                    if (Game.ActiveUnit != null) Game.ChooseNextUnit();
                    break;
                case Keys.Y:
                    {
                        if (Game.WhichCivsMapShown == Game.ActiveCiv.Id) Game.WhichCivsMapShown = 8;   //show entire map
                        else if (Game.WhichCivsMapShown == 8) Game.WhichCivsMapShown = Game.ActiveCiv.Id;   //show current civ's map view
                        OnMapEvent?.Invoke(null, new MapEventArgs(MapEventType.ToggleBetweenCurrentEntireMapView));
                        break;
                    }

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
