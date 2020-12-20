using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using civ2.Bitmaps;
using civ2.Units;

namespace civ2.Forms
{
    public partial class TradeAdvisorPanel : Civ2panel
    {
        Game Game => Game.Instance;

        private readonly Main Main;
        private readonly VScrollBar _verticalBar;
        private readonly Civ2button _closeButton, _supplyDemandButton;
        private readonly int _totalCost, _totalIncome, _totalScience, _discoveries;
        private readonly int[] _upkeepOfImprovements, _noOfImprovements;   // In order according to RULES.TXT
        private int _barValue;

        public TradeAdvisorPanel(Main parent, int _width, int _height) : base(_width, _height, null, false)
        {
            Main = parent;

            // Add DrawPanel from base control
            Controls.Add(DrawPanel);
            DrawPanel.BackgroundImage = Images.TradeAdvWallpaper;
            DrawPanel.Paint += DrawPanel_Paint;

            // Casualties button
            _supplyDemandButton = new Civ2button
            {
                Location = new Point(2, 373),
                Size = new Size(297, 24),
                Font = new Font("Times New Roman", 11),
                Text = "Casualties"
            };
            DrawPanel.Controls.Add(_supplyDemandButton);
            _supplyDemandButton.Click += new EventHandler(SupplyDemandButton_Click);

            // Close button
            _closeButton = new Civ2button
            {
                Location = new Point(301, 373),
                Size = new Size(297, 24),
                Font = new Font("Times New Roman", 11),
                Text = "Close"
            };
            DrawPanel.Controls.Add(_closeButton);
            _closeButton.Click += new EventHandler(CloseButton_Click);

            // Vertical bar
            _verticalBar = new VScrollBar()
            {
                Location = new Point(581, 66),
                Size = new Size(17, 305),
                LargeChange = 1
                // TODO: determine maximum value of Vscrollbar
            };
            DrawPanel.Controls.Add(_verticalBar);
            _verticalBar.ValueChanged += new EventHandler(VerticalBarValueChanged);

            // Calculate total numbers
            _totalCost = 0;
            _totalIncome = 0;
            _totalScience = 0;
            _discoveries = 0;
            _noOfImprovements = new int[67];
            _upkeepOfImprovements = new int[67];
            foreach (City city in Game.GetCities.Where(n => n.Owner == Game.ActiveCiv))
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
            string bcad = (Game.GameYear < 0) ? "B.C." : "A.D.";
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            e.Graphics.DrawString("TRADE ADVISOR", new Font("Times New Roman", 14), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(302 + 2, 3 + 1), sf);
            e.Graphics.DrawString("TRADE ADVISOR", new Font("Times New Roman", 14), new SolidBrush(Color.FromArgb(223, 223, 223)), new Point(302, 3), sf);
            e.Graphics.DrawString($"Kingdom of the {Game.ActiveCiv.TribeName}", new Font("Times New Roman", 14), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(302 + 2, 24 + 1), sf);
            e.Graphics.DrawString($"Kingdom of the {Game.ActiveCiv.TribeName}", new Font("Times New Roman", 14), new SolidBrush(Color.FromArgb(223, 223, 223)), new Point(302, 24), sf);
            e.Graphics.DrawString($"King {Game.ActiveCiv.LeaderName} : {Math.Abs(Game.GameYear)} {bcad}", new Font("Times New Roman", 14), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(302 + 2, 45 + 1), sf);
            e.Graphics.DrawString($"King {Game.ActiveCiv.LeaderName} : {Math.Abs(Game.GameYear)} {bcad}", new Font("Times New Roman", 14), new SolidBrush(Color.FromArgb(223, 223, 223)), new Point(302, 45), sf);
            // Cities
            int count = 0;
            foreach (City city in Game.GetCities.Where(n => n.Owner == Game.ActiveCiv))
            {
                // City image
                e.Graphics.DrawImage(Draw.City(city, true, 0), new Point(4 + 64 * ((count + 1) % 2), 95 + 24 * count));
                // City name
                e.Graphics.DrawString(city.Name, new Font("Times New Roman", 11, FontStyle.Bold), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(142 + 1, 105 + 24 * count + 1));
                e.Graphics.DrawString(city.Name, new Font("Times New Roman", 11, FontStyle.Bold), new SolidBrush(Color.FromArgb(223, 223, 223)), new Point(142, 105 + 24 * count));

                // CITY TRADE
                // City trade text
                e.Graphics.DrawString("City Trade", new Font("Times New Roman", 13), new SolidBrush(Color.Black), new Point(140 + 1, 80 + 1));
                e.Graphics.DrawString("City Trade", new Font("Times New Roman", 13), new SolidBrush(Color.White), new Point(140, 80));
                // Trade
                e.Graphics.DrawString(city.Trade.ToString(), new Font("Times New Roman", 11, FontStyle.Bold), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(255 + 1, 108 + 24 * count + 1), sf);
                e.Graphics.DrawString(city.Trade.ToString(), new Font("Times New Roman", 11, FontStyle.Bold), new SolidBrush(Color.FromArgb(223, 223, 223)), new Point(255, 108 + 24 * count), sf);
                e.Graphics.DrawImage(Images.CitymapTaxLarge, new Point(260, 111 + 24 * count));
                // Science
                e.Graphics.DrawString(city.Science.ToString(), new Font("Times New Roman", 11, FontStyle.Bold), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(290 + 1, 108 + 24 * count + 1), sf);
                e.Graphics.DrawString(city.Science.ToString(), new Font("Times New Roman", 11, FontStyle.Bold), new SolidBrush(Color.FromArgb(223, 223, 223)), new Point(290, 108 + 24 * count), sf);
                e.Graphics.DrawImage(Images.CitymapSciLarge, new Point(295, 111 + 24 * count));
                count++;

                // MAINTENTANCE COSTS
                // Maintenance text
                e.Graphics.DrawString("Maintenance Costs", new Font("Times New Roman", 13), new SolidBrush(Color.Black), new Point(335 + 1, 80 + 1));
                e.Graphics.DrawString("Maintenance Costs", new Font("Times New Roman", 13), new SolidBrush(Color.White), new Point(335, 80));
                // Individual costs
                int count2 = 0;
                for (int i = 0; i < 67; i++)
                {
                    if ((_noOfImprovements[i] > 0) && (Game.Rules.ImprovementUpkeep[i] > 0))  //only show improvements with upkeep > 0
                    {
                        e.Graphics.DrawString($"{_noOfImprovements[i]} {Game.Rules.ImprovementName[i]} (Cost: {_upkeepOfImprovements[i]})", new Font("Times New Roman", 11, FontStyle.Bold), new SolidBrush(Color.Black), new Point(335 + 1, 105 + 24 * count2 + 1));
                        e.Graphics.DrawString($"{_noOfImprovements[i]} {Game.Rules.ImprovementName[i]} (Cost: {_upkeepOfImprovements[i]})", new Font("Times New Roman", 11, FontStyle.Bold), new SolidBrush(Color.FromArgb(255, 223, 79)), new Point(335, 105 + 24 * count2));
                        count2++;
                    }
                }
                e.Graphics.DrawString($"Total Cost : {_totalCost}", new Font("Times New Roman", 11, FontStyle.Bold), new SolidBrush(Color.Black), new Point(335 + 1, 300));
                e.Graphics.DrawString($"Total Cost : {_totalCost}", new Font("Times New Roman", 11, FontStyle.Bold), new SolidBrush(Color.FromArgb(255, 223, 79)), new Point(335, 300));

                // TOTALS
                // Total cost
                e.Graphics.DrawString($"Total Cost: {_totalCost}", new Font("Times New Roman", 11), new SolidBrush(Color.Black), new Point(142 + 1, 270 + 1));
                e.Graphics.DrawString($"Total Cost: {_totalCost}", new Font("Times New Roman", 11), new SolidBrush(Color.White), new Point(142, 270));
                e.Graphics.DrawImage(Images.CitymapTaxLarge, new Point(245, 270));
                // Total income
                e.Graphics.DrawString($"Total Income: {_totalIncome}", new Font("Times New Roman", 11), new SolidBrush(Color.Black), new Point(142 + 1, 295 + 1));
                e.Graphics.DrawString($"Total Income: {_totalIncome}", new Font("Times New Roman", 11), new SolidBrush(Color.White), new Point(142, 295));
                e.Graphics.DrawImage(Images.CitymapTaxLarge, new Point(245, 295));
                // Total science
                e.Graphics.DrawString($"Total Science: {_totalScience}", new Font("Times New Roman", 11), new SolidBrush(Color.Black), new Point(142 + 1, 320 + 1));
                e.Graphics.DrawString($"Total Science: {_totalScience}", new Font("Times New Roman", 11), new SolidBrush(Color.White), new Point(142, 320));
                e.Graphics.DrawImage(Images.CitymapSciLarge, new Point(245, 320));
                // Discoveries
                e.Graphics.DrawString($"Discoveries: {_discoveries} Turns", new Font("Times New Roman", 11), new SolidBrush(Color.Black), new Point(142 + 1, 345 + 1));
                e.Graphics.DrawString($"Discoveries: {_discoveries} Turns", new Font("Times New Roman", 11), new SolidBrush(Color.White), new Point(142, 345));
            }
            sf.Dispose();
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
