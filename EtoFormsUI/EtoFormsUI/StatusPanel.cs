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

        private readonly Main main;
        private readonly Drawable mainPanel, statsPanel, unitPanel;
        private bool eotWhite; // End of turn text color is white?
        private readonly UITimer timer;

        public bool WaitingAtEndOfTurn { get; set; }

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
            var imgSize = MapImages.PanelOuterWallpaper.Size;
            for (int row = 0; row < this.Height / imgSize.Height + 1; row++)
            {
                for (int col = 0; col < this.Width / imgSize.Width + 1; col++)
                {
                    e.Graphics.DrawImage(MapImages.PanelOuterWallpaper, col * imgSize.Width, row * imgSize.Height);
                }
            }

            // Paint title
            Draw.Text(e.Graphics, "Status", new Font("Times new roman", 17, FontStyle.Bold),
                Color.FromArgb(135, 135, 135), new Point(this.Width / 2, 38 / 2), true, true, Colors.Black, 1, 1);
            // Paint panel borders
            // Outer border
            using var _pen1 = new Pen(Color.FromArgb(227, 227, 227));
            using var _pen2 = new Pen(Color.FromArgb(105, 105, 105));
            using var _pen3 = new Pen(Color.FromArgb(255, 255, 255));
            using var _pen4 = new Pen(Color.FromArgb(160, 160, 160));
            using var _pen5 = new Pen(Color.FromArgb(240, 240, 240));
            using var _pen6 = new Pen(Color.FromArgb(223, 223, 223));
            using var _pen7 = new Pen(Color.FromArgb(67, 67, 67));
            e.Graphics.DrawLine(_pen1, 0, 0, this.Width - 2, 0); // 1st layer of border
            e.Graphics.DrawLine(_pen1, 0, 0, 0, this.Height - 2);
            e.Graphics.DrawLine(_pen2, this.Width - 1, 0, this.Width - 1, this.Height - 1);
            e.Graphics.DrawLine(_pen2, 0, this.Height - 1, this.Width - 1, this.Height - 1);
            e.Graphics.DrawLine(_pen3, 1, 1, this.Width - 3, 1); // 2nd layer of border
            e.Graphics.DrawLine(_pen3, 1, 1, 1, this.Height - 3);
            e.Graphics.DrawLine(_pen4, this.Width - 2, 1, this.Width - 2, this.Height - 2);
            e.Graphics.DrawLine(_pen4, 1, this.Height - 2, this.Width - 2, this.Height - 2);
            e.Graphics.DrawLine(_pen5, 2, 2, this.Width - 4, 2); // 3rd layer of border
            e.Graphics.DrawLine(_pen5, 2, 2, 2, this.Height - 4);
            e.Graphics.DrawLine(_pen5, this.Width - 3, 2, this.Width - 3, this.Height - 3);
            e.Graphics.DrawLine(_pen5, 2, this.Height - 3, this.Width - 3, this.Height - 3);
            e.Graphics.DrawLine(_pen6, 3, 3, this.Width - 5, 3); // 4th layer of border
            e.Graphics.DrawLine(_pen6, 3, 3, 3, this.Height - 5);
            e.Graphics.DrawLine(_pen7, this.Width - 4, 3, this.Width - 4, this.Height - 4);
            e.Graphics.DrawLine(_pen7, 3, this.Height - 4, this.Width - 4, this.Height - 4);
            e.Graphics.DrawLine(_pen6, 4, 4, this.Width - 6, 4); // 5th layer of border
            e.Graphics.DrawLine(_pen6, 4, 4, 4, this.Height - 6);
            e.Graphics.DrawLine(_pen7, this.Width - 5, 4, this.Width - 5, this.Height - 5);
            e.Graphics.DrawLine(_pen7, 4, this.Height - 5, this.Width - 5, this.Height - 5);
            // Draw line borders of stats panel
            e.Graphics.DrawLine(_pen7, 9, 36, 252, 36); // 1st layer of border
            e.Graphics.DrawLine(_pen7, 9, 36, 9, 98);
            e.Graphics.DrawLine(_pen6, 9, 99, 252, 99);
            e.Graphics.DrawLine(_pen6, 252, 36, 252, 99);
            e.Graphics.DrawLine(_pen7, 10, 37, 250, 37); // 2nd layer of border
            e.Graphics.DrawLine(_pen7, 10, 38, 10, 97);
            e.Graphics.DrawLine(_pen6, 10, 98, 251, 98);
            e.Graphics.DrawLine(_pen6, 251, 37, 251, 98);
            // Draw line borders of unit panel
            e.Graphics.DrawLine(_pen7, 9, 104, 252, 104); // 1st layer of border
            e.Graphics.DrawLine(_pen7, 9, 104, 9, 106 + unitPanel.Height);
            e.Graphics.DrawLine(_pen6, 9, 107 + unitPanel.Height, 252, 107 + unitPanel.Height);
            e.Graphics.DrawLine(_pen6, 252, 104, 252, 105 + unitPanel.Height);
            e.Graphics.DrawLine(_pen7, 9, 105, 250, 105); // 2nd layer of border
            e.Graphics.DrawLine(_pen7, 10, 104, 10, 105 + unitPanel.Height);
            e.Graphics.DrawLine(_pen6, 10, 106 + unitPanel.Height, 252, 106 + unitPanel.Height);
            e.Graphics.DrawLine(_pen6, 251, 105, 251, 105 + unitPanel.Height);
        }

        private void StatsPanel_Paint(object sender, PaintEventArgs e)
        {
            // Paint wallpaper
            var imgSize = MapImages.PanelInnerWallpaper.Size;
            for (int row = 0; row < this.Height / imgSize.Height + 1; row++)
            {
                for (int col = 0; col < this.Width / imgSize.Width + 1; col++)
                {
                    e.Graphics.DrawImage(MapImages.PanelInnerWallpaper, col * imgSize.Width, row * imgSize.Height);
                }
            }

            using var _font = new Font("Times New Roman", 12, FontStyle.Bold);
            Draw.Text(e.Graphics,
                Game.GetPlayerCiv.Population.ToString("###,###",
                    new NumberFormatInfo() { NumberDecimalSeparator = "," }) + " People", _font,
                Color.FromArgb(51, 51, 51), new Point(5, 2), false, false, Color.FromArgb(191, 191, 191), 1, 1);
            Draw.Text(e.Graphics, Game.GetGameYearString, _font, Color.FromArgb(51, 51, 51), new Point(5, 20), false,
                false, Color.FromArgb(191, 191, 191), 1, 1);
            Draw.Text(e.Graphics, $"{Game.GetPlayerCiv.Money} Gold 5.0.5", _font, Color.FromArgb(51, 51, 51),
                new Point(5, 38), false, false, Color.FromArgb(191, 191, 191), 1, 1);
        }

        private void UnitPanel_Paint(object sender, PaintEventArgs e)
        {
            // Paint wallpaper
            var imgSize = MapImages.PanelInnerWallpaper.Size;
            for (int row = 0; row < this.Height / imgSize.Height + 1; row++)
            {
                for (int col = 0; col < this.Width / imgSize.Width + 1; col++)
                {
                    e.Graphics.DrawImage(MapImages.PanelInnerWallpaper, col * imgSize.Width, row * imgSize.Height);
                }
            }

            // AI turn civ indicator
            if (Game.GetActiveCiv != Game.GetPlayerCiv)
                e.Graphics.FillRectangle(MapImages.PlayerColours[Game.GetActiveCiv.Id].LightColour,
                    new Rectangle(unitPanel.Width - 8, unitPanel.Height - 6, 8, 6));

            // Don't update the panel if it's enemy turn
            if (Game.GetActiveCiv != Game.GetPlayerCiv)
                return;

            using var font = new Font("Times new roman", 12, FontStyle.Bold);

            var panelStyle = new PanelStyle(font);

            main.CurrentGameMode.DrawStatusPanel(e.Graphics, panelStyle, unitPanel.Height);


            // Blinking "end of turn" message
            if (WaitingAtEndOfTurn)
            {
                using var _font2 = new Font("Times New Roman", 12, FontStyle.Bold);
                Color _EoTcolor = eotWhite ? Colors.White : Color.FromArgb(135, 135, 135);
                Draw.Text(e.Graphics, "End of Turn", _font2, _EoTcolor, new Point(5, unitPanel.Height - 51), false,
                    false, Colors.Black, 1, 0);
                Draw.Text(e.Graphics, "(Press ENTER)", _font2, _EoTcolor, new Point(10, unitPanel.Height - 33), false,
                    false, Colors.Black, 1, 0);
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
                case PlayerEventType.WaitingAtEndOfTurn:
                {
                    WaitingAtEndOfTurn = true;
                    timer.Start();
                    break;
                }
            }
        }

        private void UnitEventHappened(object sender, UnitEventArgs e)
        {
            switch (e.EventType)
            {
                // Unit movement animation event was raised
                case UnitEventType.MoveCommand:
                case UnitEventType.StatusUpdate:
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
            WaitingAtEndOfTurn = false;
            if (Game.ProcessEndOfTurn())
            {
                Game.ChoseNextCiv();
            }
        }
    }
}
