using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using civ2.Bitmaps;
using civ2.Units;

namespace civ2.Forms
{
    public class TradeAdvisorPanel : Civ2panel
    {
        private Game _game => Game.Instance;

        private readonly Main _main;
        private readonly VScrollBar _verticalBar;
        private readonly int _totalCost, _totalIncome, _totalScience, _discoveries;
        private readonly int[] _upkeepOfImprovements, _noOfImprovements;   // In order according to RULES.TXT
        private int _barValue;

        public TradeAdvisorPanel(Main parent, int _width, int _height) : base(_width, _height, null, 11, 10)
        {
            _main = parent;

            // Add DrawPanel from base control
            Controls.Add(DrawPanel);
            DrawPanel.BackgroundImage = Images.TradeAdvWallpaper;
            DrawPanel.Paint += DrawPanel_Paint;

            // Casualties button
            var _supplyDemandButton = new Civ2button
            {
                Location = new Point(2, 373),
                Size = new Size(297, 24),
                Text = "Supply and Demand"
            };
            DrawPanel.Controls.Add(_supplyDemandButton);
            _supplyDemandButton.Click += SupplyDemandButton_Click;

            // Close button
            var _closeButton = new Civ2button
            {
                Location = new Point(301, 373),
                Size = new Size(297, 24),
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

            // Calculate total numbers
            _totalCost = 0;
            _totalIncome = 0;
            _totalScience = 0;
            _discoveries = 0;
            _noOfImprovements = new int[67];
            _upkeepOfImprovements = new int[67];
            foreach (City city in _game.GetCities.Where(n => n.Owner == _game.ActiveCiv))
            {
                for (int i = 0; i < city.Improvements.Count(); i++)
                {
                    _noOfImprovements[city.Improvements[i].Id]++;
                    _upkeepOfImprovements[city.Improvements[i].Id] += city.Improvements[i].Upkeep;
                    _totalCost += city.Improvements[i].Upkeep;
                }
                _totalIncome += city.Trade;
                _totalScience += city.Science;
                _discoveries += 0;
            }
        }

        private void DrawPanel_Paint(object sender, PaintEventArgs e)
        {
            // Text
            string bcad = (_game.GameYear < 0) ? "B.C." : "A.D.";
            using var font1 = new Font("Times New Roman", 14);
            using var font2 = new Font("Times New Roman", 11, FontStyle.Bold);
            using var font3 = new Font("Times New Roman", 13);
            Draw.Text(e.Graphics, "TRADE ADVISOR", font1, StringAlignment.Center, StringAlignment.Near, Color.FromArgb(223, 223, 223), new Point(302, 3), Color.FromArgb(67, 67, 67), 2, 1);
            Draw.Text(e.Graphics, $"Kingdom of the {_game.ActiveCiv.TribeName}", font1, StringAlignment.Center, StringAlignment.Near, Color.FromArgb(223, 223, 223), new Point(302, 24), Color.FromArgb(67, 67, 67), 2, 1);
            Draw.Text(e.Graphics, $"King {_game.ActiveCiv.LeaderName} : {Math.Abs(_game.GameYear)} {bcad}", font1, StringAlignment.Center, StringAlignment.Near, Color.FromArgb(223, 223, 223), new Point(302, 45), Color.FromArgb(67, 67, 67), 2, 1);

            // Trade & maintenance text
            Draw.Text(e.Graphics, "City Trade", font3, StringAlignment.Near, StringAlignment.Near, Color.White, new Point(140, 80), Color.Black, 1, 1);
            Draw.Text(e.Graphics, "Maintenance Costs", font3, StringAlignment.Center, StringAlignment.Near, Color.White, new Point(355, 80), Color.Black, 1, 1);

            // Cities
            int count = 0;
            foreach (City city in _game.GetCities.Where(n => n.Owner == _game.ActiveCiv))
            {
                // City image
                Draw.City(e.Graphics, city, true, 0, new Point(4 + 64 * ((count + 1) % 2), 95 + 24 * count));

                // City name
                Draw.Text(e.Graphics, city.Name, font2, StringAlignment.Near, StringAlignment.Near, Color.FromArgb(223, 223, 223), new Point(142, 105 + 24 * count), Color.FromArgb(67, 67, 67), 1, 1);

                // CITY TRADE
                // Trade
                Draw.Text(e.Graphics, city.Trade.ToString(), font2, StringAlignment.Center, StringAlignment.Near, Color.FromArgb(223, 223, 223), new Point(255, 108 + 24 * count), Color.FromArgb(67, 67, 67), 1, 1);
                e.Graphics.DrawImage(Images.CityTaxBig, new Point(260, 111 + 24 * count));

                // Science
                Draw.Text(e.Graphics, city.Science.ToString(), font2, StringAlignment.Center, StringAlignment.Near, Color.FromArgb(223, 223, 223), new Point(290, 108 + 24 * count), Color.FromArgb(67, 67, 67), 1, 1);
                e.Graphics.DrawImage(Images.CitySciBig, new Point(295, 111 + 24 * count));
                count++;
            }

            // MAINTENTANCE COSTS
            // Individual costs
            count = 0;
            for (int i = 0; i < 67; i++)
            {
                if ((_noOfImprovements[i] > 0) && (_game.Rules.ImprovementUpkeep[i] > 0))  // Only show improvements with upkeep > 0
                {
                    Draw.Text(e.Graphics, $"{_noOfImprovements[i]} {_game.Rules.ImprovementName[i]} (Cost: {_upkeepOfImprovements[i]})", font2, StringAlignment.Near, StringAlignment.Near, Color.FromArgb(255, 223, 79), new Point(355, 105 + 24 * count), Color.Black, 1, 1);
                    count++;
                }
            }
            Draw.Text(e.Graphics, $"Total Cost : {_totalCost}", font2, StringAlignment.Near, StringAlignment.Near, Color.FromArgb(255, 223, 79), new Point(355, 300), Color.Black, 1, 1);

            // TOTALS
            // Total cost
            Draw.Text(e.Graphics, $"Total Cost: {_totalCost}", font2, StringAlignment.Near, StringAlignment.Near, Color.White, new Point(142, 270), Color.Black, 1, 1);
            e.Graphics.DrawImage(Images.CityTaxBig, new Point(245, 270));

            // Total income
            Draw.Text(e.Graphics, $"Total Income: {_totalIncome}", font3, StringAlignment.Near, StringAlignment.Near, Color.White, new Point(142, 295), Color.Black, 1, 1);
            e.Graphics.DrawImage(Images.CityTaxBig, new Point(245, 295));

            // Total science
            Draw.Text(e.Graphics, $"Total Science: {_totalScience}", font3, StringAlignment.Near, StringAlignment.Near, Color.White, new Point(142, 320), Color.Black, 1, 1);
            e.Graphics.DrawImage(Images.CitySciBig, new Point(245, 320));

            // Discoveries
            Draw.Text(e.Graphics, $"Discoveries: {_discoveries} Turns", font3, StringAlignment.Near, StringAlignment.Near, Color.White, new Point(142, 345), Color.Black, 1, 1);
        }

        private void SupplyDemandButton_Click(object sender, EventArgs e)
        {
            // TODO: Add supply/demand panel
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
