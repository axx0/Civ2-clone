using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RTciv2.Imagery;

namespace RTciv2.Forms
{
    public partial class CityStatusForm : Civ2form
    {
        DoubleBufferedPanel MainPanel;
        VScrollBar VerticalBar;
        public int BarValue { get; set; }       //starting value of view of horizontal bar
        //Draw Draw = new Draw();

        public CityStatusForm()
        {
            InitializeComponent();

            //Main panel
            MainPanel = new DoubleBufferedPanel
            {
                Location = new Point(9, 9),
                Size = new Size(604, 404)
            };
            Controls.Add(MainPanel);
            MainPanel.Paint += new PaintEventHandler(MainPanel_Paint);

            //Close button
            Civ2button CloseButton = new Civ2button
            {
                Location = new Point(4, 376),
                Size = new Size(596, 24),
                Font = new Font("Times New Roman", 11),
                Text = "Close"
            };
            MainPanel.Controls.Add(CloseButton);
            CloseButton.Click += new EventHandler(CloseButton_Click);

            //Vertical bar
            VerticalBar = new VScrollBar()
            {
                Location = new Point(583, 69),
                Size = new Size(17, 305),
                LargeChange = 1
                //Maximum = TO-DO...
            };
            MainPanel.Controls.Add(VerticalBar);
            VerticalBar.ValueChanged += new EventHandler(VerticalBarValueChanged);
        }

        private void CityStatusForm_Load(object sender, EventArgs e) { }

        private void MainPanel_Paint(object sender, PaintEventArgs e)
        {
            //Add border lines
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 0, 0, MainPanel.Width - 2, 0);   //1st layer of border
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 0, 0, 0, MainPanel.Height - 2);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), MainPanel.Width - 1, 0, MainPanel.Width - 1, MainPanel.Height - 1);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), 0, MainPanel.Height - 1, MainPanel.Width - 1, MainPanel.Height - 1);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 1, 1, MainPanel.Width - 3, 1);   //2nd layer of border
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 1, 1, 1, MainPanel.Height - 3);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), MainPanel.Width - 2, 1, MainPanel.Width - 2, MainPanel.Height - 2);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), 1, MainPanel.Height - 2, MainPanel.Width - 2, MainPanel.Height - 2);
            //Draw background
            e.Graphics.DrawImage(Images.CityStatusWallpaper, new Rectangle(2, 2, 600, 400));
            //Text
            string bcad = (Data.GameYear < 0) ? "B.C." : "A.D.";
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            e.Graphics.DrawString("CITY STATUS", new Font("Times New Roman", 14), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(302 + 2, 3 + 1), sf);
            e.Graphics.DrawString("CITY STATUS", new Font("Times New Roman", 14), new SolidBrush(Color.FromArgb(223, 223, 223)), new Point(302, 3), sf);
            e.Graphics.DrawString("Kingdom of the " + Game.Civs[1].TribeName, new Font("Times New Roman", 14), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(302 + 2, 24 + 1), sf);
            e.Graphics.DrawString("Kingdom of the " + Game.Civs[1].TribeName, new Font("Times New Roman", 14), new SolidBrush(Color.FromArgb(223, 223, 223)), new Point(302, 24), sf);
            e.Graphics.DrawString("King " + Game.Civs[1].LeaderName + ": " + Math.Abs(Data.GameYear).ToString() + " " + bcad, new Font("Times New Roman", 14), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(302 + 2, 45 + 1), sf);
            e.Graphics.DrawString("King " + Game.Civs[1].LeaderName + ": " + Math.Abs(Data.GameYear).ToString() + " " + bcad, new Font("Times New Roman", 14), new SolidBrush(Color.FromArgb(223, 223, 223)), new Point(302, 45), sf);
            //Cities
            int count = 0;
            foreach (City city in Game.Cities.Where(n => n.Owner == 1))
            {
                //City image
                //e.Graphics.DrawImage(Draw.DrawCity(city, true), new Point(4 + 64 * ((count + 1) % 2), 69 + 24 * count));
                //City name
                e.Graphics.DrawString(city.Name, new Font("Times New Roman", 11, FontStyle.Bold), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(142 + 1, 82 + 24 * count + 1));
                e.Graphics.DrawString(city.Name, new Font("Times New Roman", 11, FontStyle.Bold), new SolidBrush(Color.FromArgb(223, 223, 223)), new Point(142, 82 + 24 * count));
                //Food production
                e.Graphics.DrawString(city.Food.ToString(), new Font("Times New Roman", 11, FontStyle.Bold), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(255 + 1, 82 + 24 * count + 1), sf);
                e.Graphics.DrawString(city.Food.ToString(), new Font("Times New Roman", 11, FontStyle.Bold), new SolidBrush(Color.FromArgb(223, 223, 223)), new Point(255, 82 + 24 * count), sf);
                e.Graphics.DrawImage(Images.CitymapFoodLarge, new Point(265, 85 + 24 * count));
                //Shields
                e.Graphics.DrawString(city.ShieldProduction.ToString(), new Font("Times New Roman", 11, FontStyle.Bold), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(292 + 1, 82 + 24 * count + 1), sf);
                e.Graphics.DrawString(city.ShieldProduction.ToString(), new Font("Times New Roman", 11, FontStyle.Bold), new SolidBrush(Color.FromArgb(223, 223, 223)), new Point(292, 82 + 24 * count), sf);
                e.Graphics.DrawImage(Images.CitymapSupportLarge, new Point(297, 85 + 24 * count));
                //Trade
                e.Graphics.DrawString(city.Trade.ToString(), new Font("Times New Roman", 11, FontStyle.Bold), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(329 + 1, 82 + 24 * count + 1), sf);
                e.Graphics.DrawString(city.Trade.ToString(), new Font("Times New Roman", 11, FontStyle.Bold), new SolidBrush(Color.FromArgb(223, 223, 223)), new Point(329, 82 + 24 * count), sf);
                e.Graphics.DrawImage(Images.CitymapTradeLarge, new Point(335, 85 + 24 * count));
                //Item in production
                int item = city.ItemInProduction;
                if (city.ItemInProduction < 62) //Unit is in production
                {
                    e.Graphics.DrawString(ReadFiles.UnitName[item] + " (" + city.ShieldsProgress.ToString() + "/" + (10 * ReadFiles.UnitCost[item]).ToString() + ")", new Font("Times New Roman", 11, FontStyle.Bold), new SolidBrush(Color.Black), new Point(367 + 1, 82 + 24 * count + 1));
                    e.Graphics.DrawString(ReadFiles.UnitName[item] + " (" + city.ShieldsProgress.ToString() + "/" + (10 * ReadFiles.UnitCost[item]).ToString() + ")", new Font("Times New Roman", 11, FontStyle.Bold), new SolidBrush(Color.FromArgb(255, 223, 79)), new Point(367, 82 + 24 * count));
                }
                else    //improvement
                {
                    e.Graphics.DrawString(ReadFiles.ImprovementName[item - 62 + 1] + " (" + city.ShieldsProgress.ToString() + "/" + (10 * ReadFiles.ImprovementCost[item]).ToString() + ")", new Font("Times New Roman", 11, FontStyle.Bold), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(367 + 1, 82 + 24 * count + 1));
                    e.Graphics.DrawString(ReadFiles.ImprovementName[item - 62 + 1] + " (" + city.ShieldsProgress.ToString() + "/" + (10 * ReadFiles.ImprovementCost[item]).ToString() + ")", new Font("Times New Roman", 11, FontStyle.Bold), new SolidBrush(Color.FromArgb(223, 223, 223)), new Point(367, 82 + 24 * count));
                }               
                count++;
            }
            sf.Dispose();
        }

        private void CloseButton_Click(object sender, EventArgs e) { Close(); }

        //Once slider value changes --> redraw list
        private void VerticalBarValueChanged(object sender, EventArgs e)
        {
            BarValue = VerticalBar.Value;
            Refresh();
        }
    }
}
