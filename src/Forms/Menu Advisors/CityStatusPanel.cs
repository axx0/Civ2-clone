﻿using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using civ2.Bitmaps;

namespace civ2.Forms
{
    public class CityStatusPanel : Civ2panel
    {
        private Game _game => Game.Instance;

        private readonly Main _main;
        private readonly VScrollBar _verticalBar;
        private int _barValue;   // Starting value of view of horizontal bar

        public CityStatusPanel(Main parent, int _width, int _height) : base(_width, _height, null, 11, 10)
        {
            _main = parent;

            // Add DrawPanel from base control
            Controls.Add(DrawPanel);
            DrawPanel.BackgroundImage = Images.CityStatusWallpaper;
            DrawPanel.Paint += DrawPanel_Paint;

            // Close button
            var _closeButton = new Civ2button
            {
                Location = new Point(2, 373),
                Size = new Size(596, 24),
                Text = "Close"
            };
            DrawPanel.Controls.Add(_closeButton);
            _closeButton.Click += CloseButton_Click;

            // Vertical bar
            _verticalBar = new VScrollBar()
            {
                Location = new Point(581, 66),
                Size = new Size(17, 305),
                LargeChange = 1
                // TODO: determine maximum value of Vscrollbar
            };
            DrawPanel.Controls.Add(_verticalBar);
            _verticalBar.ValueChanged += VerticalBarValueChanged;
        }

        private void DrawPanel_Paint(object sender, PaintEventArgs e)
        {
            // Text
            string bcad = (_game.GameYear < 0) ? "B.C." : "A.D.";
            using var sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            e.Graphics.DrawString("CITY STATUS", new Font("Times New Roman", 14), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(302 + 2, 3 + 1), sf);
            e.Graphics.DrawString("CITY STATUS", new Font("Times New Roman", 14), new SolidBrush(Color.FromArgb(223, 223, 223)), new Point(302, 3), sf);
            e.Graphics.DrawString($"Kingdom of the {_game.ActiveCiv.TribeName}", new Font("Times New Roman", 14), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(302 + 2, 24 + 1), sf);
            e.Graphics.DrawString($"Kingdom of the {_game.ActiveCiv.TribeName}", new Font("Times New Roman", 14), new SolidBrush(Color.FromArgb(223, 223, 223)), new Point(302, 24), sf);
            e.Graphics.DrawString($"King {_game.ActiveCiv.LeaderName} : {Math.Abs(_game.GameYear)} {bcad}", new Font("Times New Roman", 14), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(302 + 2, 45 + 1), sf);
            e.Graphics.DrawString($"King {_game.ActiveCiv.LeaderName} : {Math.Abs(_game.GameYear)} {bcad}", new Font("Times New Roman", 14), new SolidBrush(Color.FromArgb(223, 223, 223)), new Point(302, 45), sf);
            // Cities
            int count = 0;
            foreach (City city in _game.GetCities.Where(n => n.Owner == _game.ActiveCiv))
            {
                // City image
                Draw.City(e.Graphics, city, true, 0, new Point(4 + 64 * ((count + 1) % 2), 69 + 24 * count));
                //e.Graphics.DrawImage(city.Graphic(true, 0), new Point(4 + 64 * ((count + 1) % 2), 69 + 24 * count));  // OLD
                // City name
                e.Graphics.DrawString(city.Name, new Font("Times New Roman", 11, FontStyle.Bold), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(142 + 1, 82 + 24 * count + 1));
                e.Graphics.DrawString(city.Name, new Font("Times New Roman", 11, FontStyle.Bold), new SolidBrush(Color.FromArgb(223, 223, 223)), new Point(142, 82 + 24 * count));
                // Food production
                e.Graphics.DrawString(city.Food.ToString(), new Font("Times New Roman", 11, FontStyle.Bold), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(255 + 1, 82 + 24 * count + 1), sf);
                e.Graphics.DrawString(city.Food.ToString(), new Font("Times New Roman", 11, FontStyle.Bold), new SolidBrush(Color.FromArgb(223, 223, 223)), new Point(255, 82 + 24 * count), sf);
                e.Graphics.DrawImage(Images.CityFoodBig, new Point(265, 85 + 24 * count));
                // Shields
                e.Graphics.DrawString(city.ShieldProduction.ToString(), new Font("Times New Roman", 11, FontStyle.Bold), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(292 + 1, 82 + 24 * count + 1), sf);
                e.Graphics.DrawString(city.ShieldProduction.ToString(), new Font("Times New Roman", 11, FontStyle.Bold), new SolidBrush(Color.FromArgb(223, 223, 223)), new Point(292, 82 + 24 * count), sf);
                e.Graphics.DrawImage(Images.CitySupportBig, new Point(297, 85 + 24 * count));
                // Trade
                e.Graphics.DrawString(city.Trade.ToString(), new Font("Times New Roman", 11, FontStyle.Bold), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(329 + 1, 82 + 24 * count + 1), sf);
                e.Graphics.DrawString(city.Trade.ToString(), new Font("Times New Roman", 11, FontStyle.Bold), new SolidBrush(Color.FromArgb(223, 223, 223)), new Point(329, 82 + 24 * count), sf);
                e.Graphics.DrawImage(Images.CityTradeBig, new Point(335, 85 + 24 * count));
                // Item in production
                int item = city.ItemInProduction;
                if (city.ItemInProduction < 62) // Unit is in production
                {
                    e.Graphics.DrawString($"{_game.Rules.UnitName[item]} ( + {city.ShieldsProgress} / {(10 * _game.Rules.UnitCost[item])} )", new Font("Times New Roman", 11, FontStyle.Bold), new SolidBrush(Color.Black), new Point(367 + 1, 82 + 24 * count + 1));
                    e.Graphics.DrawString($"{_game.Rules.UnitName[item]} ( + {city.ShieldsProgress} / {(10 * _game.Rules.UnitCost[item])} )", new Font("Times New Roman", 11, FontStyle.Bold), new SolidBrush(Color.FromArgb(255, 223, 79)), new Point(367, 82 + 24 * count));
                }
                else    // Improvement
                {
                    e.Graphics.DrawString($"{_game.Rules.ImprovementName[item - 62 + 1]} ( {city.ShieldsProgress} / {(10 * _game.Rules.ImprovementCost[item - 62 + 1])} )", new Font("Times New Roman", 11, FontStyle.Bold), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(367 + 1, 82 + 24 * count + 1));
                    e.Graphics.DrawString($"{_game.Rules.ImprovementName[item - 62 + 1]} ( {city.ShieldsProgress} / {(10 * _game.Rules.ImprovementCost[item - 62 + 1])} )", new Font("Times New Roman", 11, FontStyle.Bold), new SolidBrush(Color.FromArgb(223, 223, 223)), new Point(367, 82 + 24 * count));
                }
                count++;
            }
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            this.Visible = false;
            this.Dispose();
        }

        // Once slider value changes --> redraw list
        private void VerticalBarValueChanged(object sender, EventArgs e)
        {
            _barValue = _verticalBar.Value;
            DrawPanel.Invalidate();
        }
    }
}
