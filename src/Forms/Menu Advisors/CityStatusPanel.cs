using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using civ2.Bitmaps;

namespace civ2.Forms
{
    public partial class CityStatusPanel : Civ2panel
    {
        Game Game => Game.Instance;
        Map Map => Map.Instance;

        public Main Main;
        private VScrollBar _verticalBar;
        private int _barValue;   // Starting value of view of horizontal bar

        public CityStatusPanel(Main parent, int _width, int _height) : base(_width, _height, null, false)
        {
            Main = parent;

            // Add DrawPanel from base control
            Controls.Add(DrawPanel);
            DrawPanel.BackgroundImage = Images.CityStatusWallpaper;
            DrawPanel.Paint += DrawPanel_Paint;

            // Close button
            Civ2button _closeButton = new Civ2button
            {
                Location = new Point(2, 373),
                Size = new Size(596, 24),
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
        }

        private void DrawPanel_Paint(object sender, PaintEventArgs e)
        {
            // Text
            string bcad = (Game.GameYear < 0) ? "B.C." : "A.D.";
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            e.Graphics.DrawString("CITY STATUS", new Font("Times New Roman", 14), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(302 + 2, 3 + 1), sf);
            e.Graphics.DrawString("CITY STATUS", new Font("Times New Roman", 14), new SolidBrush(Color.FromArgb(223, 223, 223)), new Point(302, 3), sf);
            e.Graphics.DrawString("Kingdom of the " + Game.GetCivs[Game.ActiveCiv.Id].TribeName, new Font("Times New Roman", 14), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(302 + 2, 24 + 1), sf);
            e.Graphics.DrawString("Kingdom of the " + Game.GetCivs[Game.ActiveCiv.Id].TribeName, new Font("Times New Roman", 14), new SolidBrush(Color.FromArgb(223, 223, 223)), new Point(302, 24), sf);
            e.Graphics.DrawString("King " + Game.GetCivs[Game.ActiveCiv.Id].LeaderName + ": " + Math.Abs(Game.GameYear).ToString() + " " + bcad, new Font("Times New Roman", 14), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(302 + 2, 45 + 1), sf);
            e.Graphics.DrawString("King " + Game.GetCivs[Game.ActiveCiv.Id].LeaderName + ": " + Math.Abs(Game.GameYear).ToString() + " " + bcad, new Font("Times New Roman", 14), new SolidBrush(Color.FromArgb(223, 223, 223)), new Point(302, 45), sf);
            // Cities
            int count = 0;
            foreach (City city in Game.GetCities.Where(n => n.Owner == Game.ActiveCiv))
            {
                // City image
                e.Graphics.DrawImage(city.Graphic(true, 0), new Point(4 + 64 * ((count + 1) % 2), 69 + 24 * count));
                // City name
                e.Graphics.DrawString(city.Name, new Font("Times New Roman", 11, FontStyle.Bold), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(142 + 1, 82 + 24 * count + 1));
                e.Graphics.DrawString(city.Name, new Font("Times New Roman", 11, FontStyle.Bold), new SolidBrush(Color.FromArgb(223, 223, 223)), new Point(142, 82 + 24 * count));
                // Food production
                e.Graphics.DrawString(city.Food.ToString(), new Font("Times New Roman", 11, FontStyle.Bold), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(255 + 1, 82 + 24 * count + 1), sf);
                e.Graphics.DrawString(city.Food.ToString(), new Font("Times New Roman", 11, FontStyle.Bold), new SolidBrush(Color.FromArgb(223, 223, 223)), new Point(255, 82 + 24 * count), sf);
                e.Graphics.DrawImage(Images.CitymapFoodLarge, new Point(265, 85 + 24 * count));
                // Shields
                e.Graphics.DrawString(city.ShieldProduction.ToString(), new Font("Times New Roman", 11, FontStyle.Bold), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(292 + 1, 82 + 24 * count + 1), sf);
                e.Graphics.DrawString(city.ShieldProduction.ToString(), new Font("Times New Roman", 11, FontStyle.Bold), new SolidBrush(Color.FromArgb(223, 223, 223)), new Point(292, 82 + 24 * count), sf);
                e.Graphics.DrawImage(Images.CitymapSupportLarge, new Point(297, 85 + 24 * count));
                // Trade
                e.Graphics.DrawString(city.Trade.ToString(), new Font("Times New Roman", 11, FontStyle.Bold), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(329 + 1, 82 + 24 * count + 1), sf);
                e.Graphics.DrawString(city.Trade.ToString(), new Font("Times New Roman", 11, FontStyle.Bold), new SolidBrush(Color.FromArgb(223, 223, 223)), new Point(329, 82 + 24 * count), sf);
                e.Graphics.DrawImage(Images.CitymapTradeLarge, new Point(335, 85 + 24 * count));
                // Item in production
                int item = city.ItemInProduction;
                if (city.ItemInProduction < 62) // Unit is in production
                {
                    e.Graphics.DrawString(Game.Rules.UnitName[item] + " (" + city.ShieldsProgress.ToString() + "/" + (10 * Game.Rules.UnitCost[item]).ToString() + ")", new Font("Times New Roman", 11, FontStyle.Bold), new SolidBrush(Color.Black), new Point(367 + 1, 82 + 24 * count + 1));
                    e.Graphics.DrawString(Game.Rules.UnitName[item] + " (" + city.ShieldsProgress.ToString() + "/" + (10 * Game.Rules.UnitCost[item]).ToString() + ")", new Font("Times New Roman", 11, FontStyle.Bold), new SolidBrush(Color.FromArgb(255, 223, 79)), new Point(367, 82 + 24 * count));
                }
                else    // Improvement
                {
                    e.Graphics.DrawString(Game.Rules.ImprovementName[item - 62 + 1] + " (" + city.ShieldsProgress.ToString() + "/" + (10 * Game.Rules.ImprovementCost[item - 62 + 1]).ToString() + ")", new Font("Times New Roman", 11, FontStyle.Bold), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(367 + 1, 82 + 24 * count + 1));
                    e.Graphics.DrawString(Game.Rules.ImprovementName[item - 62 + 1] + " (" + city.ShieldsProgress.ToString() + "/" + (10 * Game.Rules.ImprovementCost[item - 62 + 1]).ToString() + ")", new Font("Times New Roman", 11, FontStyle.Bold), new SolidBrush(Color.FromArgb(223, 223, 223)), new Point(367, 82 + 24 * count));
                }
                count++;
            }
            sf.Dispose();
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
            Refresh();
        }
    }
}
