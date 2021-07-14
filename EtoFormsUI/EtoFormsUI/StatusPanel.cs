using System;
using System.Linq;
using System.Globalization;
using System.Collections.Generic;
using Eto.Forms;
using Eto.Drawing;
using Civ2engine;
using Civ2engine.Events;
using Civ2engine.Enums;
using Civ2engine.Units;
using System.Diagnostics;

namespace EtoFormsUI
{
    public class StatusPanel : Panel
    {
        private Game Game => Game.Instance;
        private Map Map => Game.CurrentMap;

        private readonly Main main;
        private readonly Drawable mainPanel, statsPanel, unitPanel;
        private bool eotWhite; // End of turn text color is white?
        private readonly UITimer timer;

        public bool WaitingAtEndOfTurn =>
            Game.GetPlayerCiv == Game.GetActiveCiv && 
            !Game.GetActiveCiv.AnyUnitsAwaitingOrders &&
            Game.Options.AlwaysWaitAtEndOfTurn;

        public static event EventHandler<MapEventArgs> OnMapEvent;

        public StatusPanel(Main parent, int width, int height)
        {
            main = parent;
            Size = new Size(width, height);
            eotWhite = true;

            // Main panel
            mainPanel = new Drawable() { Size = new Size(width, height) };
            mainPanel.Paint += MainPanel_Paint;

            var MainPanelLayout = new PixelLayout() { Size = new Size(mainPanel.Width, mainPanel.Height) };

            // Stats panel
            statsPanel = new Drawable() { Size = new Size(240, 60) };
            statsPanel.Paint += StatsPanel_Paint;
            statsPanel.MouseUp += Panel_Click;
            MainPanelLayout.Add(statsPanel, 11, 38);

            // Unit panel
            unitPanel = new Drawable() { Size = new Size(240, this.Height - 117) };
            unitPanel.Paint += UnitPanel_Paint;
            unitPanel.MouseUp += Panel_Click;
            MainPanelLayout.Add(unitPanel, 11, 106);

            mainPanel.Content = MainPanelLayout;
            Content = mainPanel;

            MapPanel.OnMapEvent += MapEventHappened;
            Game.OnMapEvent += MapEventHappened;
            //Main.OnMapEvent += MapEventHappened;
            //Game.OnWaitAtTurnEnd += InitiateWaitAtTurnEnd;
            Game.OnPlayerEvent += PlayerEventHappened;
            Game.OnUnitEvent += UnitEventHappened;

            // Timer for "end of turn" message
            timer = new UITimer() { Interval = 0.5 };
            timer.Elapsed += (sender, e) => unitPanel.Invalidate();
        }

        private void MainPanel_Paint(object sender, PaintEventArgs e)
        {
            // Paint wallpaper
            var imgSize = Images.PanelOuterWallpaper.Size;
            for (int row = 0; row < this.Height / imgSize.Height + 1; row++)
            {
                for (int col = 0; col < this.Width / imgSize.Width + 1; col++)
                {
                    e.Graphics.DrawImage(Images.PanelOuterWallpaper, col * imgSize.Width, row * imgSize.Height);
                }
            }
            // Paint title
            Draw.Text(e.Graphics, "Status", new Font("Times new roman", 17, FontStyle.Bold), Color.FromArgb(135, 135, 135), new Point(this.Width / 2, 38 / 2), true, true, Colors.Black, 1, 1);
            // Paint panel borders
            // Outer border
            using var _pen1 = new Pen(Color.FromArgb(227, 227, 227));
            using var _pen2 = new Pen(Color.FromArgb(105, 105, 105));
            using var _pen3 = new Pen(Color.FromArgb(255, 255, 255));
            using var _pen4 = new Pen(Color.FromArgb(160, 160, 160));
            using var _pen5 = new Pen(Color.FromArgb(240, 240, 240));
            using var _pen6 = new Pen(Color.FromArgb(223, 223, 223));
            using var _pen7 = new Pen(Color.FromArgb(67, 67, 67));
            e.Graphics.DrawLine(_pen1, 0, 0, this.Width - 2, 0);   // 1st layer of border
            e.Graphics.DrawLine(_pen1, 0, 0, 0, this.Height - 2);
            e.Graphics.DrawLine(_pen2, this.Width - 1, 0, this.Width - 1, this.Height - 1);
            e.Graphics.DrawLine(_pen2, 0, this.Height - 1, this.Width - 1, this.Height - 1);
            e.Graphics.DrawLine(_pen3, 1, 1, this.Width - 3, 1);   // 2nd layer of border
            e.Graphics.DrawLine(_pen3, 1, 1, 1, this.Height - 3);
            e.Graphics.DrawLine(_pen4, this.Width - 2, 1, this.Width - 2, this.Height - 2);
            e.Graphics.DrawLine(_pen4, 1, this.Height - 2, this.Width - 2, this.Height - 2);
            e.Graphics.DrawLine(_pen5, 2, 2, this.Width - 4, 2);   // 3rd layer of border
            e.Graphics.DrawLine(_pen5, 2, 2, 2, this.Height - 4);
            e.Graphics.DrawLine(_pen5, this.Width - 3, 2, this.Width - 3, this.Height - 3);
            e.Graphics.DrawLine(_pen5, 2, this.Height - 3, this.Width - 3, this.Height - 3);
            e.Graphics.DrawLine(_pen6, 3, 3, this.Width - 5, 3);   // 4th layer of border
            e.Graphics.DrawLine(_pen6, 3, 3, 3, this.Height - 5);
            e.Graphics.DrawLine(_pen7, this.Width - 4, 3, this.Width - 4, this.Height - 4);
            e.Graphics.DrawLine(_pen7, 3, this.Height - 4, this.Width - 4, this.Height - 4);
            e.Graphics.DrawLine(_pen6, 4, 4, this.Width - 6, 4);   // 5th layer of border
            e.Graphics.DrawLine(_pen6, 4, 4, 4, this.Height - 6);
            e.Graphics.DrawLine(_pen7, this.Width - 5, 4, this.Width - 5, this.Height - 5);
            e.Graphics.DrawLine(_pen7, 4, this.Height - 5, this.Width - 5, this.Height - 5);
            // Draw line borders of stats panel
            e.Graphics.DrawLine(_pen7, 9, 36, 252, 36);   // 1st layer of border
            e.Graphics.DrawLine(_pen7, 9, 36, 9, 98);
            e.Graphics.DrawLine(_pen6, 9, 99, 252, 99);
            e.Graphics.DrawLine(_pen6, 252, 36, 252, 99);
            e.Graphics.DrawLine(_pen7, 10, 37, 250, 37);   // 2nd layer of border
            e.Graphics.DrawLine(_pen7, 10, 38, 10, 97);
            e.Graphics.DrawLine(_pen6, 10, 98, 251, 98);
            e.Graphics.DrawLine(_pen6, 251, 37, 251, 98);
            // Draw line borders of unit panel
            e.Graphics.DrawLine(_pen7, 9, 104, 252, 104);   // 1st layer of border
            e.Graphics.DrawLine(_pen7, 9, 104, 9, 106 + unitPanel.Height);
            e.Graphics.DrawLine(_pen6, 9, 107 + unitPanel.Height, 252, 107 + unitPanel.Height);
            e.Graphics.DrawLine(_pen6, 252, 104, 252, 105 + unitPanel.Height);
            e.Graphics.DrawLine(_pen7, 9, 105, 250, 105);   // 2nd layer of border
            e.Graphics.DrawLine(_pen7, 10, 104, 10, 105 + unitPanel.Height);
            e.Graphics.DrawLine(_pen6, 10, 106 + unitPanel.Height, 252, 106 + unitPanel.Height);
            e.Graphics.DrawLine(_pen6, 251, 105, 251, 105 + unitPanel.Height);
        }

        private void StatsPanel_Paint(object sender, PaintEventArgs e)
        {
            // Paint wallpaper
            var imgSize = Images.PanelInnerWallpaper.Size;
            for (int row = 0; row < this.Height / imgSize.Height + 1; row++)
            {
                for (int col = 0; col < this.Width / imgSize.Width + 1; col++)
                {
                    e.Graphics.DrawImage(Images.PanelInnerWallpaper, col * imgSize.Width, row * imgSize.Height);
                }
            }
            using var _font = new Font("Times New Roman", 12, FontStyle.Bold);
            Draw.Text(e.Graphics, Game.GetPlayerCiv.Population.ToString("###,###", new NumberFormatInfo() { NumberDecimalSeparator = "," }) + " People", _font, Color.FromArgb(51, 51, 51), new Point(5, 2), false, false, Color.FromArgb(191, 191, 191), 1, 1);
            Draw.Text(e.Graphics, Game.GetGameYearString, _font, Color.FromArgb(51, 51, 51), new Point(5, 20), false, false, Color.FromArgb(191, 191, 191), 1, 1);
            Draw.Text(e.Graphics, $"{Game.GetPlayerCiv.Money} Gold 5.0.5", _font, Color.FromArgb(51, 51, 51), new Point(5, 38), false, false, Color.FromArgb(191, 191, 191), 1, 1);
        }

        private void UnitPanel_Paint(object sender, PaintEventArgs e)
        {
            // Paint wallpaper
            var imgSize = Images.PanelInnerWallpaper.Size;
            for (int row = 0; row < this.Height / imgSize.Height + 1; row++)
            {
                for (int col = 0; col < this.Width / imgSize.Width + 1; col++)
                {
                    e.Graphics.DrawImage(Images.PanelInnerWallpaper, col * imgSize.Width, row * imgSize.Height);
                }
            }

            // AI turn civ indicator
            if (Game.GetActiveCiv != Game.GetPlayerCiv) 
                e.Graphics.FillRectangle(MapImages.PlayerColours[Game.GetActiveCiv.Id].LightColour, new Rectangle(unitPanel.Width - 8, unitPanel.Height - 6, 8, 6));

            // Don't update the panel if it's enemy turn
            if (Game.GetActiveCiv != Game.GetPlayerCiv) 
                return;

            using var _font = new Font("Times new roman", 12, FontStyle.Bold);
            var _frontColor = Color.FromArgb(51, 51, 51);
            var _backColor = Color.FromArgb(191, 191, 191);
            var activeXY = main.CurrentGameMode.ActiveXY;
            var activeTile = Map.IsValidTileC2(activeXY[0], activeXY[1])
                ? Game.CurrentMap.TileC2(activeXY[0], activeXY[1])
                : null;

            // View piece mode
            if (main.CurrentGameMode == main.ViewPiece)
            {
                Draw.Text(e.Graphics, "Viewing Pieces", _font, Colors.White, new Point(119, 10), true, true, Colors.Black, 1, 0);

                // Draw location & tile type on active square
                if (activeTile != null)
                {
                    Draw.Text(e.Graphics,
                        $"Loc: ({activeXY[0]}, {activeXY[1]}) {activeTile.Island}",
                        _font, _frontColor, new Point(5, 27), false, false, _backColor, 1, 1);
                    Draw.Text(e.Graphics,
                        $"({activeTile.Type})", _font,
                        _frontColor, new Point(5, 45), false, false, _backColor, 1, 1);
                }
                //int count;
                //for (count = 0; count < Math.Min(_unitsOnThisTile.Count, maxUnitsToDraw); count++)
                //{
                //    //e.Graphics.DrawImage(ModifyImage.Resize(Draw.Unit(UnitsOnThisTile[count], false, 0), (int)Math.Round(64 * 1.15), (int)Math.Round(48 * 1.15)), 6, 70 + count * 56);
                //    //e.Graphics.DrawImage(ModifyImage.Resize(Draw.Unit(UnitsOnThisTile[count], false, 0), 0), 6, 70 + count * 56);  // TODO: do this again!!!
                //    Draw.Text(e.Graphics, _unitsOnThisTile[count].HomeCity.Name, _font, StringAlignment.Near, StringAlignment.Near, _frontColor, new Point(79, 70 + count * 56), _backColor, 1, 1);
                //    Draw.Text(e.Graphics, _unitsOnThisTile[count].Order.ToString(), _font, StringAlignment.Near, StringAlignment.Near, _frontColor, new Point(79, 88 + count * 56), _backColor, 1, 1); // TODO: give proper conversion of orders to string
                //    Draw.Text(e.Graphics, _unitsOnThisTile[count].Name, _font, StringAlignment.Near, StringAlignment.Near, _frontColor, new Point(79, 106 + count * 56), _backColor, 1, 1);
                //}
                //if (count < _unitsOnThisTile.Count)
                //{
                //    string _moreUnits = (_unitsOnThisTile.Count - count == 1) ? "More Unit" : "More Units";
                //    Draw.Text(e.Graphics, $"({_unitsOnThisTile.Count() - count} {_moreUnits})", _font, StringAlignment.Near, StringAlignment.Near, _frontColor, new Point(5, UnitPanel.Height - 27), _backColor, 1, 1);
                //}
            }
            // Moving units mode
            else
            {
                Draw.Text(e.Graphics, "Moving Units", _font, Colors.White, new Point(119, 10), true, true, Colors.Black, 1, 0);

                // Show active unit info
                Draw.Unit(e.Graphics, Game.ActiveUnit, false, 1, new Point(7, 27));
                
                // Show move points correctly
                var commonMultiplier = Game.Rules.Cosmic.MovementMultiplier;
                var remainingFullPoints = Game.ActiveUnit.MovePoints / commonMultiplier;
                var fractionalMove = Game.ActiveUnit.MovePoints % commonMultiplier;
                
                string moveText;
                if (fractionalMove > 0)
                {
                    var gcf = Utils.GreatestCommonFactor(fractionalMove, commonMultiplier);
                    moveText =
                        $"Moves: {(remainingFullPoints > 0 ? remainingFullPoints : "")} {fractionalMove / gcf}/{commonMultiplier / gcf}";
                }
                else
                {
                    moveText = $"Moves: {remainingFullPoints}";
                }

                Draw.Text(e.Graphics, moveText, _font, _frontColor, new Point(79, 25), false, false, _backColor, 1, 1);
                
                // Show other unit info
                var _cityName = (Game.ActiveUnit.HomeCity == null) ? "NONE" : Game.ActiveUnit.HomeCity.Name;
                Draw.Text(e.Graphics, _cityName, _font, _frontColor, new Point(79, 43), false, false, _backColor, 1, 1);
                Draw.Text(e.Graphics, Game.GetActiveCiv.Adjective, _font, _frontColor, new Point(79, 61), false, false, _backColor, 1, 1);
                var _column = 83;
                Draw.Text(e.Graphics, Game.ActiveUnit.Name, _font, _frontColor, new Point(5, _column), false, false, _backColor, 1, 1);
                _column += 18;

                if (activeTile != null)
                {
                    Draw.Text(e.Graphics, $"({activeTile.Type})", _font, _frontColor, new Point(5, _column), false,
                        false, _backColor, 1, 1);

                    // If road/railroad/irrigation/farmland/mine present
                    string improvementText = null;
                    if (activeTile.Railroad) improvementText = "Railroad";
                    else if (activeTile.Road) improvementText = "Road";

                    if (activeTile.Mining)
                        improvementText = improvementText != null ? $"{improvementText}, Mining" : "Mining";
                    else if (activeTile.Farmland)
                        improvementText = improvementText != null ? $"{improvementText}, Farmland" : "Farmland";
                    else if (activeTile.Irrigation)
                        improvementText = improvementText != null ? $"{improvementText}, Irrigation" : "Irrigation";

                    if (!string.IsNullOrEmpty(improvementText))
                    {
                        _column += 18;
                        Draw.Text(e.Graphics, $"({improvementText})", _font, _frontColor, new Point(5, _column), false,
                            false, _backColor, 1, 1);
                    }

                    // If airbase/fortress present
                    if (activeTile.Airbase || activeTile.Fortress)
                    {
                        _column += 18;
                        string _airbaseText = null;
                        if (activeTile.Fortress) _airbaseText = "Fortress";
                        if (activeTile.Airbase) _airbaseText = "Airbase";
                        Draw.Text(e.Graphics, $"({_airbaseText})", _font, _frontColor, new Point(5, _column), false,
                            false, _backColor, 1, 1);
                    }

                    // If pollution present
                    if (activeTile.Pollution)
                    {
                        _column += 18;
                        Draw.Text(e.Graphics, "(Pollution)", _font, _frontColor, new Point(5, _column), false, false,
                            _backColor, 1, 1);
                    }

                    _column += 5;

                    // Show info for other units on the tile
                    int drawCount = 0;
                    foreach (IUnit unit in activeTile.UnitsHere.Where(u => u != Game.ActiveUnit))
                    {
                        // First check if there is vertical space still left for drawing in panel
                        if (_column + 69 > unitPanel.Height) break;

                        // Draw unit
                        Draw.Unit(e.Graphics, unit, false, 1, new Point(7, _column + 27));
                        // Show other unit info
                        _column += 20;
                        _cityName = (unit.HomeCity == null) ? "NONE" : unit.HomeCity.Name;
                        Draw.Text(e.Graphics, _cityName, _font, _frontColor, new Point(80, _column), false, false,
                            _backColor, 1, 1);
                        _column += 18;
                        Draw.Text(e.Graphics, Order2string(unit.Order), _font, _frontColor, new Point(80, _column),
                            false, false, _backColor, 1, 1);
                        _column += 18;
                        Draw.Text(e.Graphics, unit.Name, _font, _frontColor, new Point(80, _column), false, false,
                            _backColor, 1, 1);

                        //System.Diagnostics.Debug.WriteLine($"{unit.Name} drawn");

                        drawCount++;
                    }

                    // If not all units were drawn print a message
                    if (activeTile.UnitsHere.Count - 1 != drawCount) // -1 because you must not count in active unit
                    {
                        _column += 22;
                        moveText = activeTile.UnitsHere.Count - 1 - drawCount == 1 ? "Unit" : "Units";
                        Draw.Text(e.Graphics, $"({activeTile.UnitsHere.Count - 1 - drawCount} More {moveText})", _font,
                            _frontColor, new Point(9, _column), false, false, _backColor, 1, 1);
                    }
                }
            }

            // Blinking "end of turn" message
            if (WaitingAtEndOfTurn)
            {
                using var _font2 = new Font("Times New Roman", 12, FontStyle.Bold);
                Color _EoTcolor = eotWhite ? Colors.White : Color.FromArgb(135, 135, 135);
                Draw.Text(e.Graphics, "End of Turn", _font2, _EoTcolor, new Point(5, unitPanel.Height - 51), false, false, Colors.Black, 1, 0);
                Draw.Text(e.Graphics, "(Press ENTER)", _font2, _EoTcolor, new Point(10, unitPanel.Height - 33), false, false, Colors.Black, 1, 0);
                eotWhite = !eotWhite;
            }
        }

        private void Panel_Click(object sender, MouseEventArgs e)
        {
            if (main.CurrentGameMode.PanelClick(Game, main))
            {
                unitPanel.Invalidate();
            }
            // if (WaitingAtEndOfTurn)
            // {
            //     End_WaitAtEndOfTurn();
            // }
            // else
            // {
            //     Map.ViewPieceMode = !Map.ViewPieceMode;
            //     unitPanel.Invalidate();
            //     OnMapEvent?.Invoke(null, new MapEventArgs(MapEventType.SwitchViewMovePiece));
            // }
        }

        private void MapEventHappened(object sender, MapEventArgs e)
        {
            switch (e.EventType)
            {
                case MapEventType.MapViewChanged:
                    {
                        unitPanel.Invalidate();
                        break;
                    }
                case MapEventType.WaitAtEndOfTurn:
                    {
                        timer.Start();
                        break;
                    }
                default: break;
            }
        }

        private void PlayerEventHappened(object sender, PlayerEventArgs e)
        {
            switch (e.EventType)
            {
                case PlayerEventType.NewTurn:
                    {
                        unitPanel.Invalidate();
                        break;
                    }
                default: break;
            }
        }

        private void UnitEventHappened(object sender, UnitEventArgs e)
        {
            switch (e.EventType)
            {
                // Unit movement animation event was raised
                case UnitEventType.MoveCommand:
                    {
                        unitPanel.Invalidate();
                        break;
                    }
                case UnitEventType.StatusUpdate:
                    {
                        unitPanel.Invalidate();
                        break;
                    }
                case UnitEventType.NewUnitActivated:
                    {
                        unitPanel.Invalidate();
                        break;
                    }
                default:
                    break;
            }
        }

        public void End_WaitAtEndOfTurn()
        {
            timer.Stop();
            Game.ChoseNextCiv();
        }

        // Concert an order enum to string
        private string Order2string(OrderType order)
        {
            if (order == OrderType.BuildAirbase) return "Build Airbase";
            else if (order == OrderType.BuildFortress) return "Build Fortress";
            else if (order == OrderType.BuildIrrigation) return "Build Irrigation";
            else if (order == OrderType.BuildMine) return "Build Mine";
            else if (order == OrderType.BuildRoad) return "Build Road";
            else if (order == OrderType.CleanPollution) return "Clean Pollution";
            else if (order == OrderType.Fortified) return "Fortify";
            else if (order == OrderType.GoTo) return "Go to xxx";
            else if (order == OrderType.NoOrders) return "No Orders";
            else if (order == OrderType.Sleep) return "Sleep";
            else if (order == OrderType.Transform) return "Transform";
            else return null;
        }
    }
}
