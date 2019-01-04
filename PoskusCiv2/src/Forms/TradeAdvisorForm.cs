using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PoskusCiv2.Imagery;

namespace PoskusCiv2.Forms
{
    public partial class TradeAdvisorForm : Civ2form
    {
        DoubleBufferedPanel MainPanel;
        VScrollBar VerticalBar;
        public int BarValue { get; set; }       //starting value of view of horizontal bar
        int TotalCost, TotalIncome, TotalScience, Discoveries;
        int[] NoOfImprovements = new int[67];   //In order according to RULES.TXT
        int[] UpkeepOfImprovements = new int[67];
        Draw Draw = new Draw();

        public TradeAdvisorForm()
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

            //Casualties button
            Civ2button SupplyDemandButton = new Civ2button
            {
                Location = new Point(4, 376),
                Size = new Size(297, 24),
                Font = new Font("Times New Roman", 11),
                Text = "Casualties"
            };
            MainPanel.Controls.Add(SupplyDemandButton);
            SupplyDemandButton.Click += new EventHandler(SupplyDemandButton_Click);

            //Close button
            Civ2button CloseButton = new Civ2button
            {
                Location = new Point(303, 376),
                Size = new Size(297, 24),
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

            //Calculate total numbers
            TotalCost = 0;
            TotalIncome = 0;
            TotalScience = 0;
            Discoveries = 0;
            foreach (City city in Game.Cities.Where(n => n.Owner == 1))
            {
                for (int i = 0; i < city.Improvements.Count(); i++)
                {
                    NoOfImprovements[city.Improvements[i].Id]++;
                    UpkeepOfImprovements[city.Improvements[i].Id] += city.Improvements[i].Upkeep;
                    TotalCost += city.Improvements[i].Upkeep;
                }
                TotalIncome += city.Trade;
                TotalScience += city.Sci;
                Discoveries += 0;               
            }
        }

        private void TradeAdvisorForm_Load(object sender, EventArgs e) { }

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
            e.Graphics.DrawImage(Images.TradeAdvWallpaper, new Rectangle(2, 2, 600, 400));
            //Text
            string bcad;
            if (Game.Data.GameYear < 0) { bcad = "B.C."; }
            else { bcad = "A.D."; }
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            e.Graphics.DrawString("TRADE ADVISOR", new Font("Times New Roman", 14), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(302 + 2, 3 + 1), sf);
            e.Graphics.DrawString("TRADE ADVISOR", new Font("Times New Roman", 14), new SolidBrush(Color.FromArgb(223, 223, 223)), new Point(302, 3), sf);
            e.Graphics.DrawString("Kingdom of the " + Game.Civs[1].TribeName, new Font("Times New Roman", 14), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(302 + 2, 24 + 1), sf);
            e.Graphics.DrawString("Kingdom of the " + Game.Civs[1].TribeName, new Font("Times New Roman", 14), new SolidBrush(Color.FromArgb(223, 223, 223)), new Point(302, 24), sf);
            e.Graphics.DrawString("King " + Game.Civs[1].LeaderName + ": " + Math.Abs(Game.Data.GameYear).ToString() + " " + bcad, new Font("Times New Roman", 14), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(302 + 2, 45 + 1), sf);
            e.Graphics.DrawString("King " + Game.Civs[1].LeaderName + ": " + Math.Abs(Game.Data.GameYear).ToString() + " " + bcad, new Font("Times New Roman", 14), new SolidBrush(Color.FromArgb(223, 223, 223)), new Point(302, 45), sf);
            //Cities
            int count = 0;
            foreach (City city in Game.Cities.Where(n => n.Owner == 1))
            {
                //City image
                e.Graphics.DrawImage(Draw.DrawCity(city, true), new Point(4 + 64 * ((count + 1) % 2), 95 + 24 * count));
                //City name
                e.Graphics.DrawString(city.Name, new Font("Times New Roman", 11, FontStyle.Bold), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(142 + 1, 105 + 24 * count + 1));
                e.Graphics.DrawString(city.Name, new Font("Times New Roman", 11, FontStyle.Bold), new SolidBrush(Color.FromArgb(223, 223, 223)), new Point(142, 105 + 24 * count));

                //CITY TRADE
                //City trade text
                e.Graphics.DrawString("City Trade", new Font("Times New Roman", 14), new SolidBrush(Color.Black), new Point(140 + 1, 80 + 1));
                e.Graphics.DrawString("City Trade", new Font("Times New Roman", 14), new SolidBrush(Color.White), new Point(140, 80));
                //Trade
                e.Graphics.DrawString(city.Trade.ToString(), new Font("Times New Roman", 11, FontStyle.Bold), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(255 + 1, 108 + 24 * count + 1), sf);
                e.Graphics.DrawString(city.Trade.ToString(), new Font("Times New Roman", 11, FontStyle.Bold), new SolidBrush(Color.FromArgb(223, 223, 223)), new Point(255, 108 + 24 * count), sf);
                e.Graphics.DrawImage(Images.CitymapTaxLarge, new Point(260, 111 + 24 * count));
                //Science
                e.Graphics.DrawString(city.Sci.ToString(), new Font("Times New Roman", 11, FontStyle.Bold), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(290 + 1, 108 + 24 * count + 1), sf);
                e.Graphics.DrawString(city.Sci.ToString(), new Font("Times New Roman", 11, FontStyle.Bold), new SolidBrush(Color.FromArgb(223, 223, 223)), new Point(290, 108 + 24 * count), sf);
                e.Graphics.DrawImage(Images.CitymapSciLarge, new Point(295, 111 + 24 * count));
                count++;

                //MAINTENTANCE COSTS
                //Maintenance text
                e.Graphics.DrawString("Maintenance Costs", new Font("Times New Roman", 14), new SolidBrush(Color.Black), new Point(335 + 1, 80 + 1));
                e.Graphics.DrawString("Maintenance Costs", new Font("Times New Roman", 14), new SolidBrush(Color.White), new Point(335, 80));
                //Individual costs
                int count2 = 0;
                for (int i = 0; i < 67; i++)
                {
                    if ((NoOfImprovements[i] > 0) && (ReadFiles.ImprovementUpkeep[i] > 0))  //only show improvements with upkeep > 0
                    {
                        e.Graphics.DrawString(NoOfImprovements[i].ToString() + " " + ReadFiles.ImprovementName[i] + " (Cost: " + UpkeepOfImprovements[i] + ")", new Font("Times New Roman", 11, FontStyle.Bold), new SolidBrush(Color.Black), new Point(335 + 1, 105 + 24 * count2 + 1));
                        e.Graphics.DrawString(NoOfImprovements[i].ToString() + " " + ReadFiles.ImprovementName[i] + " (Cost: " + UpkeepOfImprovements[i] + ")", new Font("Times New Roman", 11, FontStyle.Bold), new SolidBrush(Color.FromArgb(255, 223, 79)), new Point(335, 105 + 24 * count2));
                        count2++;
                    }
                }
                e.Graphics.DrawString("Total Cost : " + TotalCost.ToString(), new Font("Times New Roman", 11, FontStyle.Bold), new SolidBrush(Color.Black), new Point(335 + 1, 300));
                e.Graphics.DrawString("Total Cost : " + TotalCost.ToString(), new Font("Times New Roman", 11, FontStyle.Bold), new SolidBrush(Color.FromArgb(255, 223, 79)), new Point(335, 300));

                //TOTALS
                //Total cost
                e.Graphics.DrawString("Total Cost: " + TotalCost.ToString(), new Font("Times New Roman", 11), new SolidBrush(Color.Black), new Point(142 + 1, 270 + 1));
                e.Graphics.DrawString("Total Cost: " + TotalCost.ToString(), new Font("Times New Roman", 11), new SolidBrush(Color.White), new Point(142, 270));
                e.Graphics.DrawImage(Images.CitymapTaxLarge, new Point(245, 270));
                //Total income
                e.Graphics.DrawString("Total Income: " + TotalIncome.ToString(), new Font("Times New Roman", 11), new SolidBrush(Color.Black), new Point(142 + 1, 295 + 1));
                e.Graphics.DrawString("Total Income: " + TotalIncome.ToString(), new Font("Times New Roman", 11), new SolidBrush(Color.White), new Point(142, 295));
                e.Graphics.DrawImage(Images.CitymapTaxLarge, new Point(245, 295));
                //Total science
                e.Graphics.DrawString("Total Science: " + TotalScience.ToString(), new Font("Times New Roman", 11), new SolidBrush(Color.Black), new Point(142 + 1, 320 + 1));
                e.Graphics.DrawString("Total Science: " + TotalScience.ToString(), new Font("Times New Roman", 11), new SolidBrush(Color.White), new Point(142, 320));
                e.Graphics.DrawImage(Images.CitymapSciLarge, new Point(245, 320));
                //Discoveries
                e.Graphics.DrawString("Discoveries: " + Discoveries.ToString() + " Turns", new Font("Times New Roman", 11), new SolidBrush(Color.Black), new Point(142 + 1, 345 + 1));
                e.Graphics.DrawString("Discoveries: " + Discoveries.ToString() + " Turns", new Font("Times New Roman", 11), new SolidBrush(Color.White), new Point(142, 345));
            }
            sf.Dispose();
        }

        private void SupplyDemandButton_Click(object sender, EventArgs e) { Close(); }

        private void CloseButton_Click(object sender, EventArgs e) { Close(); }

        //Once slider value changes --> redraw list
        private void VerticalBarValueChanged(object sender, EventArgs e)
        {
            BarValue = VerticalBar.Value;
            Refresh();
        }
    }
}
