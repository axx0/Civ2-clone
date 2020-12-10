using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using System.Globalization;
using civ2.Bitmaps;
using civ2.Units;
using civ2.Events;
using civ2.Enums;

namespace civ2.Forms
{
    public partial class StatusPanel : DoubleBufferedPanel
    {
        Game Game => Game.Instance;
        Map Map => Map.Instance;

        private readonly DoubleBufferedPanel StatsPanel, UnitPanel;
        private readonly Timer Timer = new Timer();
        private bool WaitingAtEndOfTurn { get; set; }

        public static event EventHandler<MapEventArgs> OnMapEvent;

        public StatusPanel(int _width, int _height)
        {
            BackgroundImage = Images.PanelOuterWallpaper;
            Size = new Size(_width, _height);
            this.Paint += StatusPanel_Paint;
            MapPanel.OnMapEvent += MapEventHappened;
            MainWindow.OnMapEvent += MapEventHappened;
            Game.OnWaitAtTurnEnd += InitiateWaitAtTurnEnd;
            Game.OnPlayerEvent += PlayerEventHappened;
            Game.OnUnitEvent += UnitEventHappened;

            StatsPanel = new DoubleBufferedPanel()
            {
                Location = new Point(11, 38),
                Size = new Size(240, 60),
                BackgroundImage = Images.PanelInnerWallpaper
            };
            Controls.Add(StatsPanel);
            StatsPanel.Paint += StatsPanel_Paint;
            StatsPanel.MouseClick += Panel_Click;

            UnitPanel = new DoubleBufferedPanel()
            {
                Location = new Point(11, 106),
                Size = new Size(240, Height - 117),
                BackgroundImage = Images.PanelInnerWallpaper
            };
            Controls.Add(UnitPanel);
            UnitPanel.Paint += UnitPanel_Paint;
            UnitPanel.MouseClick += Panel_Click;

            // Timer for "end of turn" message
            Timer.Tick += Timer_Tick;
            Timer.Interval = 500;   // ms
        }

        private void StatusPanel_Paint(object sender, PaintEventArgs e)
        {
            // Title
            StringFormat sf = new StringFormat();
            sf.LineAlignment = StringAlignment.Center;
            sf.Alignment = StringAlignment.Center;
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
            e.Graphics.DrawString("Status", new Font("Times New Roman", 17, FontStyle.Bold), new SolidBrush(Color.Black), new Point(this.Width / 2 + 1, 20 + 1), sf);
            e.Graphics.DrawString("Status", new Font("Times New Roman", 17, FontStyle.Bold), new SolidBrush(Color.FromArgb(135, 135, 135)), new Point(this.Width / 2, 20), sf);
            sf.Dispose();
            // Outer border
            e.Graphics.DrawLine(new Pen(Color.FromArgb(227, 227, 227)), 0, 0, this.Width - 2, 0);   // 1st layer of border
            e.Graphics.DrawLine(new Pen(Color.FromArgb(227, 227, 227)), 0, 0, 0, this.Height - 2);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(105, 105, 105)), this.Width - 1, 0, this.Width - 1, this.Height - 1);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(105, 105, 105)), 0, this.Height - 1, this.Width - 1, this.Height - 1);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(255, 255, 255)), 1, 1, this.Width - 3, 1);   // 2nd layer of border
            e.Graphics.DrawLine(new Pen(Color.FromArgb(255, 255, 255)), 1, 1, 1, this.Height - 3);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(160, 160, 160)), this.Width - 2, 1, this.Width - 2, this.Height - 2);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(160, 160, 160)), 1, this.Height - 2, this.Width - 2, this.Height - 2);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(240, 240, 240)), 2, 2, this.Width - 4, 2);   // 3rd layer of border
            e.Graphics.DrawLine(new Pen(Color.FromArgb(240, 240, 240)), 2, 2, 2, this.Height - 4);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(240, 240, 240)), this.Width - 3, 2, this.Width - 3, this.Height - 3);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(240, 240, 240)), 2, this.Height - 3, this.Width - 3, this.Height - 3);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), 3, 3, this.Width - 5, 3);   // 4th layer of border
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), 3, 3, 3, this.Height - 5);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), this.Width - 4, 3, this.Width - 4, this.Height - 4);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 3, this.Height - 4, this.Width - 4, this.Height - 4);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), 4, 4, this.Width - 6, 4);   // 5th layer of border
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), 4, 4, 4, this.Height - 6);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), this.Width - 5, 4, this.Width - 5, this.Height - 5);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 4, this.Height - 5, this.Width - 5, this.Height - 5);
            // Draw line borders of stats panel
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 9, 36, 252, 36);   // 1st layer of border
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 9, 36, 9, 98);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), 9, 99, 252, 99);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), 252, 36, 252, 99);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 10, 37, 250, 37);   // 2nd layer of border
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 10, 38, 10, 97);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), 10, 98, 251, 98);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), 251, 37, 251, 98);
            // Draw line borders of unit panel
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 9, 104, 252, 104);   // 1st layer of border
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 9, 104, 9, 106 + UnitPanel.Height);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), 9, 107 + UnitPanel.Height, 252, 107 + UnitPanel.Height);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), 252, 104, 252, 105 + UnitPanel.Height);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 9, 105, 250, 105);   // 2nd layer of border
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 10, 104, 10, 105 + UnitPanel.Height);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), 10, 106 + UnitPanel.Height, 252, 106 + UnitPanel.Height);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), 251, 105, 251, 105 + UnitPanel.Height);
            e.Dispose();
        }

        private void StatsPanel_Paint(object sender, PaintEventArgs e)
        {
            string showYear = (Game.GameYear < 0) ? $"{Math.Abs(Game.GameYear)} B.C." : $"A.D. {Math.Abs(Game.GameYear)}";
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
            e.Graphics.DrawString(Game.PlayerCiv.Population.ToString("###,###", new NumberFormatInfo() { NumberDecimalSeparator = "," }) + " People", new Font("Times New Roman", 10, FontStyle.Bold), new SolidBrush(Color.FromArgb(191, 191, 191)), new Point(5 + 1, 2 + 1));
            e.Graphics.DrawString(Game.PlayerCiv.Population.ToString("###,###", new NumberFormatInfo() { NumberDecimalSeparator = "," }) + " People", new Font("Times New Roman", 10, FontStyle.Bold), new SolidBrush(Color.FromArgb(51, 51, 51)), new Point(5, 2));
            e.Graphics.DrawString(showYear, new Font("Times New Roman", 10, FontStyle.Bold), new SolidBrush(Color.FromArgb(191, 191, 191)), new Point(5 + 1, 20 + 1));
            e.Graphics.DrawString(showYear, new Font("Times New Roman", 10, FontStyle.Bold), new SolidBrush(Color.FromArgb(51, 51, 51)), new Point(5, 20));
            e.Graphics.DrawString($"{Game.PlayerCiv.Money} Gold 5.0.5", new Font("Times New Roman", 10, FontStyle.Bold), new SolidBrush(Color.FromArgb(191, 191, 191)), new Point(5 + 1, 38 + 1));
            e.Graphics.DrawString($"{Game.PlayerCiv.Money} Gold 5.0.5", new Font("Times New Roman", 10, FontStyle.Bold), new SolidBrush(Color.FromArgb(51, 51, 51)), new Point(5, 38));
        }

        private void UnitPanel_Paint(object sender, PaintEventArgs e)
        {
            StringFormat sf = new StringFormat();
            //sf.LineAlignment = StringAlignment.Center;
            sf.Alignment = StringAlignment.Center;
            Font font = new Font("Times new roman", 10, FontStyle.Bold);
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
            // List all units on active tile
            //List<IUnit> UnitsOnThisTile = new List<IUnit>();
            //foreach (IUnit unit in Game.GetUnits.Where(a => (a.X == Game.ActiveCursorXY[0]) && (a.Y == Game.ActiveCursorXY[1])))
            //    UnitsOnThisTile.Add(unit);
            List<IUnit> UnitsOnThisTile = Game.GetUnits.Where(u => u.X == Game.ActiveCursorXY[0] && u.Y == Game.ActiveCursorXY[1]).ToList();
            int maxUnitsToDraw = (int)Math.Floor((double)((UnitPanel.Height - 66) / 56));

            if (MapPanel.ViewPiecesMode)
            {
                e.Graphics.DrawString("Viewing Pieces", font, new SolidBrush(Color.Black), new Point(120 + 1, 0), sf);
                e.Graphics.DrawString("Viewing Pieces", font, new SolidBrush(Color.White), new Point(120, 0), sf);
                e.Graphics.DrawString($"Loc: ({Game.ActiveCursorXY[0]}, {Game.ActiveCursorXY[1]}) {Map.Tile[(Game.ActiveCursorXY[0] - Game.ActiveCursorXY[1] % 2) / 2, Game.ActiveCursorXY[1]].Island}", font, new SolidBrush(Color.FromArgb(191, 191, 191)), 6, 28);
                e.Graphics.DrawString($"Loc: ({Game.ActiveCursorXY[0]}, {Game.ActiveCursorXY[1]}) {Map.Tile[(Game.ActiveCursorXY[0] - Game.ActiveCursorXY[1] % 2) / 2, Game.ActiveCursorXY[1]].Island}", font, new SolidBrush(Color.FromArgb(51, 51, 51)), 5, 27);
                e.Graphics.DrawString($"({Map.Tile[(Game.ActiveCursorXY[0] - Game.ActiveCursorXY[1] % 2) / 2, Game.ActiveCursorXY[1]].Type})", font, new SolidBrush(Color.FromArgb(191, 191, 191)), 6, 46);
                e.Graphics.DrawString($"({Map.Tile[(Game.ActiveCursorXY[0] - Game.ActiveCursorXY[1] % 2) / 2, Game.ActiveCursorXY[1]].Type})", font, new SolidBrush(Color.FromArgb(51, 51, 51)), 5, 45);

                int count;
                for (count = 0; count < Math.Min(UnitsOnThisTile.Count, maxUnitsToDraw); count++)
                {
                    e.Graphics.DrawImage(ModifyImage.ResizeImage(Draw.Unit(UnitsOnThisTile[count], false, 0), (int)Math.Round(64 * 1.15), (int)Math.Round(48 * 1.15)), 6, 70 + count * 56);
                    e.Graphics.DrawString(UnitsOnThisTile[count].HomeCity.Name, font, new SolidBrush(Color.FromArgb(191, 191, 191)), 80, 71 + count * 56);
                    e.Graphics.DrawString(UnitsOnThisTile[count].HomeCity.Name, font, new SolidBrush(Color.FromArgb(51, 51, 51)), 79, 70 + count * 56);
                    e.Graphics.DrawString(UnitsOnThisTile[count].Order.ToString(), font, new SolidBrush(Color.FromArgb(191, 191, 191)), 80, 89 + count * 56); // TODO: give proper conversion of orders to string
                    e.Graphics.DrawString(UnitsOnThisTile[count].Order.ToString(), font, new SolidBrush(Color.FromArgb(51, 51, 51)), 79, 88 + count * 56);
                    e.Graphics.DrawString(UnitsOnThisTile[count].Name, font, new SolidBrush(Color.FromArgb(191, 191, 191)), 80, 107 + count * 56);
                    e.Graphics.DrawString(UnitsOnThisTile[count].Name, font, new SolidBrush(Color.FromArgb(51, 51, 51)), 79, 106 + count * 56);
                }
                if (count < UnitsOnThisTile.Count())
                {
                    string moreUnits = (UnitsOnThisTile.Count() - count == 1) ? "More Unit" : "More Units";
                    e.Graphics.DrawString($"({UnitsOnThisTile.Count() - count} {moreUnits})", font, new SolidBrush(Color.FromArgb(191, 191, 191)), 6, UnitPanel.Height - 26); ;
                    e.Graphics.DrawString($"({UnitsOnThisTile.Count() - count} {moreUnits})", font, new SolidBrush(Color.FromArgb(51, 51, 51)), 5, UnitPanel.Height - 27); ;
                }
            }
            else    // Moving units mode
            {
                e.Graphics.DrawString("Moving Units", font, new SolidBrush(Color.Black), new Point(120 + 1, 0), sf);
                e.Graphics.DrawString("Moving Units", font, new SolidBrush(Color.White), new Point(120, 0), sf);

                int count;
                for(count = 0; count < Math.Min(UnitsOnThisTile.Count, maxUnitsToDraw); count++)
                {
                    if (Game.Instance.ActiveUnit == UnitsOnThisTile[count])
                    {
                        e.Graphics.DrawImage(ModifyImage.ResizeImage(Draw.Unit(Game.Instance.ActiveUnit, false, 8), (int)Math.Round(64 * 1.15), (int)Math.Round(48 * 1.15)), 6, 27);
                        // Show move points correctly
                        int fullMovPts = Game.Instance.ActiveUnit.MovePoints / 3;
                        int remMovPts = Game.Instance.ActiveUnit.MovePoints % 3;
                        if (remMovPts == 0) // Only show full move pts
                        {
                            e.Graphics.DrawString($"Moves: {fullMovPts}", font, new SolidBrush(Color.FromArgb(191, 191, 191)), 80, 26);
                            e.Graphics.DrawString($"Moves: {fullMovPts}", font, new SolidBrush(Color.FromArgb(51, 51, 51)), 79, 25);
                        }
                        else    // Also show remainer of move points
                        {
                            e.Graphics.DrawString($"Moves: {fullMovPts} {remMovPts}/3", font, new SolidBrush(Color.FromArgb(191, 191, 191)), 80, 26);
                            e.Graphics.DrawString($"Moves: {fullMovPts} {remMovPts}/3", font, new SolidBrush(Color.FromArgb(51, 51, 51)), 79, 25);
                        }
                        string cityName = (Game.ActiveUnit.HomeCity == null) ? "NONE" : Game.ActiveUnit.HomeCity.Name;
                        e.Graphics.DrawString(cityName, font, new SolidBrush(Color.FromArgb(191, 191, 191)), 80, 44);
                        e.Graphics.DrawString(cityName, font, new SolidBrush(Color.FromArgb(51, 51, 51)), 79, 43);
                        e.Graphics.DrawString(Game.ActiveCiv.Adjective, font, new SolidBrush(Color.FromArgb(191, 191, 191)), 80, 62);
                        e.Graphics.DrawString(Game.ActiveCiv.Adjective, font, new SolidBrush(Color.FromArgb(51, 51, 51)), 79, 61);
                        e.Graphics.DrawString(Game.ActiveUnit.Name, font, new SolidBrush(Color.FromArgb(191, 191, 191)), 6, 84);
                        e.Graphics.DrawString(Game.ActiveUnit.Name, font, new SolidBrush(Color.FromArgb(51, 51, 51)), 5, 83);
                        e.Graphics.DrawString($"({Map.Tile[(Game.ActiveCursorXY[0] - Game.ActiveCursorXY[1] % 2) / 2, Game.ActiveCursorXY[1]].Type})", font, new SolidBrush(Color.FromArgb(191, 191, 191)), 6, 102);
                        e.Graphics.DrawString($"({Map.Tile[(Game.ActiveCursorXY[0] - Game.ActiveCursorXY[1] % 2) / 2, Game.ActiveCursorXY[1]].Type})", font, new SolidBrush(Color.FromArgb(51, 51, 51)), 5, 101);
                    }
                    else
                    {
                        e.Graphics.DrawImage(ModifyImage.ResizeImage(Draw.Unit(UnitsOnThisTile[count], false, 0), (int)Math.Round(64 * 1.15), (int)Math.Round(48 * 1.15)), 6, 70 + count * 56);
                        e.Graphics.DrawString(UnitsOnThisTile[count].HomeCity.Name, font, new SolidBrush(Color.FromArgb(191, 191, 191)), 80, 71 + count * 56);
                        e.Graphics.DrawString(UnitsOnThisTile[count].HomeCity.Name, font, new SolidBrush(Color.FromArgb(51, 51, 51)), 79, 70 + count * 56);
                        e.Graphics.DrawString(UnitsOnThisTile[count].Order.ToString(), font, new SolidBrush(Color.FromArgb(191, 191, 191)), 80, 89 + count * 56); // TODO: give proper conversion of orders to string
                        e.Graphics.DrawString(UnitsOnThisTile[count].Order.ToString(), font, new SolidBrush(Color.FromArgb(51, 51, 51)), 79, 88 + count * 56);
                        e.Graphics.DrawString(UnitsOnThisTile[count].Name, font, new SolidBrush(Color.FromArgb(191, 191, 191)), 80, 107 + count * 56);
                        e.Graphics.DrawString(UnitsOnThisTile[count].Name, font, new SolidBrush(Color.FromArgb(51, 51, 51)), 79, 106 + count * 56);
                    }
                }
            }

            // Blinking "end of turn" message
            if (WaitingAtEndOfTurn)
            {
                Color EoTcolor;
                if (BoolSwitcher) EoTcolor = Color.White;
                else EoTcolor = Color.FromArgb(135, 135, 135);
                e.Graphics.DrawString("End of Turn", new Font("Times new roman", 12, FontStyle.Bold), new SolidBrush(Color.Black), 6, UnitPanel.Height - 51);
                e.Graphics.DrawString("End of Turn", new Font("Times new roman", 12, FontStyle.Bold), new SolidBrush(EoTcolor), 5, UnitPanel.Height - 51);
                e.Graphics.DrawString("(Press ENTER)", new Font("Times new roman", 12, FontStyle.Bold), new SolidBrush(Color.Black), 11, UnitPanel.Height - 33);
                e.Graphics.DrawString("(Press ENTER)", new Font("Times new roman", 12, FontStyle.Bold), new SolidBrush(EoTcolor), 10, UnitPanel.Height - 33);
            }

            sf.Dispose();
            e.Dispose();
            font.Dispose();
        }

        private void Panel_Click(object sender, MouseEventArgs e)
        {
            if (WaitingAtEndOfTurn)
            {
                WaitingAtEndOfTurn = false;
                Game.NewPlayerTurn();
            }
            else
            {
                MapPanel.ViewPiecesMode = !MapPanel.ViewPiecesMode;
                UnitPanel.Refresh();
                OnMapEvent?.Invoke(null, new MapEventArgs(MapEventType.SwitchViewMovePieces));
            }
        }

        private void MapEventHappened(object sender, MapEventArgs e)
        {
            switch (e.EventType)
            {
                case MapEventType.MapViewChanged:
                    {
                        UnitPanel.Refresh();
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
                        WaitingAtEndOfTurn = false;
                        StatsPanel.Refresh();
                        UnitPanel.Refresh();
                        break;
                    }
                default: break;
            }
        }

        private void UnitEventHappened(object sender, UnitEventArgs e)
        {
            switch (e.EventType)
            {
                //Unit movement animation event was raised
                case UnitEventType.MoveCommand:
                    {
                        break;
                    }
                case UnitEventType.StatusUpdate:
                    {
                        UnitPanel.Refresh();
                        break;
                    }
                case UnitEventType.NewUnitActivated:
                    {
                        UnitPanel.Refresh();
                        break;
                    }
                default:
                    break;
            }
        }

        private void InitiateWaitAtTurnEnd(object sender, WaitAtTurnEndEventArgs e)
        {
            WaitingAtEndOfTurn = true;
            Timer.Start();
            UnitPanel.Refresh();
        }

        private void Timer_Tick (object sender, EventArgs e)
        {
            UnitPanel.Refresh();
        }

        bool _boolSwitcher;
        private bool BoolSwitcher
        {
            get
            {
                if (this == null) _boolSwitcher = true;
                _boolSwitcher = !_boolSwitcher;   // Change state when this is called
                return _boolSwitcher;
            }
        }
    }
}
