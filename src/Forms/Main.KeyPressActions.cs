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
                        if (!ViewPieceMode && _game.ActiveUnit?.MovementCounter == 0)
                        {
                            _game.IssueUnitOrder(OrderType.MoveSW);
                        }
                        else if (ViewPieceMode)
                        {
                            _game.ActiveXY[0]--;
                            _game.ActiveXY[1]++;
                            OnMapEvent?.Invoke(null, new MapEventArgs(MapEventType.ViewPieceMoved));
                        }
                        break;
                    }
                case Keys.Down:
                case Keys.NumPad2:
                    {
                        if (!ViewPieceMode && _game.ActiveUnit?.MovementCounter == 0)
                        {
                            _game.IssueUnitOrder(OrderType.MoveS);
                        }
                        else if (ViewPieceMode)
                        {
                            _game.ActiveXY[1] += 2;
                            OnMapEvent?.Invoke(null, new MapEventArgs(MapEventType.ViewPieceMoved));
                        }
                        break;
                    }
                case Keys.NumPad3:
                    {
                        if (!ViewPieceMode && _game.ActiveUnit?.MovementCounter == 0)
                        {
                            _game.IssueUnitOrder(OrderType.MoveSE);
                        }
                        else if (ViewPieceMode)
                        {
                            _game.ActiveXY[0]++;
                            _game.ActiveXY[1]++;
                            OnMapEvent?.Invoke(null, new MapEventArgs(MapEventType.ViewPieceMoved));
                        }
                        break;
                    }
                case Keys.Left:
                case Keys.NumPad4:
                    {
                        if (!ViewPieceMode && _game.ActiveUnit?.MovementCounter == 0)
                        {
                            _game.IssueUnitOrder(OrderType.MoveW);
                        }
                        else if (ViewPieceMode)
                        {
                            _game.ActiveXY[0] -= 2;
                            OnMapEvent?.Invoke(null, new MapEventArgs(MapEventType.ViewPieceMoved));
                        }
                        break;
                    }
                case Keys.Right:
                case Keys.NumPad6:
                    {
                        if (!ViewPieceMode && _game.ActiveUnit?.MovementCounter == 0)
                        {
                            _game.IssueUnitOrder(OrderType.MoveE);
                        }
                        else if (ViewPieceMode)
                        {
                            _game.ActiveXY[0] += 2;
                            OnMapEvent?.Invoke(null, new MapEventArgs(MapEventType.ViewPieceMoved));
                        }
                        break;
                    }
                case Keys.NumPad7:
                    {
                        if (!ViewPieceMode && _game.ActiveUnit?.MovementCounter == 0)
                        {
                            _game.IssueUnitOrder(OrderType.MoveNW);
                        }
                        else if (ViewPieceMode)
                        {
                            _game.ActiveXY[0]--;
                            _game.ActiveXY[1]--;
                            OnMapEvent?.Invoke(null, new MapEventArgs(MapEventType.ViewPieceMoved));
                        }
                        break;
                    }
                case Keys.Up:
                case Keys.NumPad8:
                    {
                        if (!ViewPieceMode && _game.ActiveUnit?.MovementCounter == 0)
                        {
                            _game.IssueUnitOrder(OrderType.MoveN);
                        }
                        else if (ViewPieceMode)
                        {
                            _game.ActiveXY[1] -= 2;
                            OnMapEvent?.Invoke(null, new MapEventArgs(MapEventType.ViewPieceMoved));
                        }
                        break;
                    }
                case Keys.NumPad9:
                    {
                        if (!ViewPieceMode && _game.ActiveUnit?.MovementCounter == 0)
                        {
                            _game.IssueUnitOrder(OrderType.MoveNE);
                        }
                        else if (ViewPieceMode)
                        {
                            _game.ActiveXY[0]++;
                            _game.ActiveXY[1]--;
                            OnMapEvent?.Invoke(null, new MapEventArgs(MapEventType.ViewPieceMoved));
                        }
                        break;
                    }
                case Keys.B:
                    if (!ViewPieceMode && _game.ActiveUnit?.MovementCounter == 0)
                        _game.IssueUnitOrder(OrderType.BuildCity);
                    break;
                case Keys.F:
                    if (!ViewPieceMode && _game.ActiveUnit?.MovementCounter == 0)
                        _game.IssueUnitOrder(OrderType.Fortify);
                    break;
                case Keys.G:
                    if (!ViewPieceMode && _game.ActiveUnit?.MovementCounter == 0)
                        _game.IssueUnitOrder(OrderType.GoTo);
                    break;
                case Keys.H:
                    if (!ViewPieceMode && _game.ActiveUnit?.MovementCounter == 0)
                        _game.IssueUnitOrder(OrderType.GoHome);
                    break;
                case Keys.I:
                    if (!ViewPieceMode && _game.ActiveUnit?.MovementCounter == 0)
                        _game.IssueUnitOrder(OrderType.BuildIrrigation);
                    break;
                case Keys.K:
                    if (!ViewPieceMode && _game.ActiveUnit?.MovementCounter == 0)
                        _game.IssueUnitOrder(OrderType.Automate);
                    break;
                case Keys.M:
                    if (!ViewPieceMode && _game.ActiveUnit?.MovementCounter == 0)
                        _game.IssueUnitOrder(OrderType.BuildMine);
                    break;
                case Keys.O:
                    if (!ViewPieceMode && _game.ActiveUnit?.MovementCounter == 0)
                        _game.IssueUnitOrder(OrderType.Transform);
                    break;
                case Keys.P:
                    if (!ViewPieceMode && _game.ActiveUnit?.MovementCounter == 0)
                        _game.IssueUnitOrder(OrderType.CleanPollution);
                    break;
                case Keys.R:
                    if (!ViewPieceMode && _game.ActiveUnit?.MovementCounter == 0)
                        _game.IssueUnitOrder(OrderType.BuildRoad);
                    break;
                case Keys.S:
                    if (!ViewPieceMode && _game.ActiveUnit?.MovementCounter == 0)
                        _game.IssueUnitOrder(OrderType.Sleep);
                    break;
                case Keys.Enter:
                    if (_game.Options.AlwaysWaitAtEndOfTurn && !_game.PlayerCiv.AnyUnitsAwaitingOrders)
                        _game.NewPlayerTurn();
                    else
                        OnCheckIfCityCanBeViewed?.Invoke(null, new CheckIfCityCanBeViewedEventArgs());  //if enter pressed when view piece above city --> enter city view
                    break;
                case Keys.Space:
                    if (_game.ActiveUnit?.MovementCounter == 0)
                        _game.IssueUnitOrder(OrderType.SkipTurn);
                    break;
                case Keys.W:
                    if (_game.ActiveUnit != null) _game.ChooseNextUnit();
                    break;
                case Keys.Y:
                    {
                        if (_game.WhichCivsMapShown == _game.ActiveCiv.Id) _game.WhichCivsMapShown = 8;   //show entire map
                        else if (_game.WhichCivsMapShown == 8) _game.WhichCivsMapShown = _game.ActiveCiv.Id;   //show current civ's map view
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
