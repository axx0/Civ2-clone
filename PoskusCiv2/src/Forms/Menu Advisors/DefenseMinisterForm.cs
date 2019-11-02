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
using RTciv2.Units;

namespace RTciv2.Forms
{
    public partial class DefenseMinisterForm : Civ2form
    {
        DoubleBufferedPanel MainPanel;
        VScrollBar VerticalBar;
        public int BarValue { get; set; }       //starting value of view of horizontal bar
        Civ2button CasualtiesButton;
        Draw Draw = new Draw();
        int[] ActiveUnitCount = new int[62];
        int[] UnitInProductionCount = new int[62];
        bool StatisticsActive;  //true=statistics are shown, false=casualties are shown

        public DefenseMinisterForm()
        {
            InitializeComponent();
            StatisticsActive = true;

            //Main panel
            MainPanel = new DoubleBufferedPanel
            {
                Location = new Point(9, 9),
                Size = new Size(604, 404)
            };
            Controls.Add(MainPanel);
            MainPanel.Paint += new PaintEventHandler(MainPanel_Paint);

            //Casualties button
            CasualtiesButton = new Civ2button
            {
                Location = new Point(4, 376),
                Size = new Size(297, 24),
                Font = new Font("Times New Roman", 11),
                Text = "Casualties"
            };
            MainPanel.Controls.Add(CasualtiesButton);
            CasualtiesButton.Click += new EventHandler(CasualtiesButton_Click);

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

            //Count active units, units in production
            for (int i = 0; i < 62; i++)
            {
                //Count active units
                foreach (IUnit unit in Game.Units.Where(n => n.Civ == 1))   //search just for your civ
                {
                    if (unit.Name == ReadFiles.UnitName[i]) ActiveUnitCount[i]++;
                }
                //Count units in production
                foreach (City city in Game.Cities.Where(n => n.Owner == 1)) //only search in cities for your civ
                {
                    if (city.ItemInProduction == i) UnitInProductionCount[i]++;
                }
            }            
        }

        private void DefenseMinisterForm_Load(object sender, EventArgs e) { }

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
            e.Graphics.DrawImage(Images.DefenseMinWallpaper, new Rectangle(2, 2, 600, 400));
            //Draw text
            string statText;
            if (StatisticsActive) { statText = "Statistics"; }
            else { statText = "Casualties"; }
            string bcad;
            if (Game.Data.GameYear < 0) { bcad = "B.C."; }
            else { bcad = "A.D."; }
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            e.Graphics.DrawString("DEFENSE MINISTER: " + statText, new Font("Times New Roman", 14), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(302 + 2, 3 + 1), sf);
            e.Graphics.DrawString("DEFENSE MINISTER: " + statText, new Font("Times New Roman", 14), new SolidBrush(Color.FromArgb(223, 223, 223)), new Point(302, 3), sf);
            e.Graphics.DrawString("Kingdom of the " + Game.Civs[1].TribeName, new Font("Times New Roman", 14), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(302 + 2, 24 + 1), sf);
            e.Graphics.DrawString("Kingdom of the " + Game.Civs[1].TribeName, new Font("Times New Roman", 14), new SolidBrush(Color.FromArgb(223, 223, 223)), new Point(302, 24), sf);
            e.Graphics.DrawString("King " + Game.Civs[1].LeaderName + ": " + Math.Abs(Game.Data.GameYear).ToString() + " " + bcad, new Font("Times New Roman", 14), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(302 + 2, 45 + 1), sf);
            e.Graphics.DrawString("King " + Game.Civs[1].LeaderName + ": " + Math.Abs(Game.Data.GameYear).ToString() + " " + bcad, new Font("Times New Roman", 14), new SolidBrush(Color.FromArgb(223, 223, 223)), new Point(302, 45), sf);
            //Units
            if (StatisticsActive)
            {
                int count = 0;
                for (int i = 0; i < 62; i++)
                {
                    if (ActiveUnitCount[i] > 0)
                    {
                        int civId = 1;  //your civ only
                                        //Image of unit
                        e.Graphics.DrawImage(Draw.DrawUnitType(i, civId), new Point(4 + 64 * ((count + 1) % 2), 69 + 24 * count));
                        //Unit name
                        e.Graphics.DrawString(ReadFiles.UnitName[i], new Font("Times New Roman", 11, FontStyle.Bold), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(142 + 1, 85 + 24 * count + 1));
                        e.Graphics.DrawString(ReadFiles.UnitName[i], new Font("Times New Roman", 11, FontStyle.Bold), new SolidBrush(Color.FromArgb(223, 223, 223)), new Point(142, 85 + 24 * count));
                        //Unit attack/defense/movement
                        e.Graphics.DrawString(ReadFiles.UnitAttack[i] + "/" + ReadFiles.UnitDefense[i] + "/" + ReadFiles.UnitMove[i], new Font("Times New Roman", 11, FontStyle.Bold), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(255 + 1, 85 + 24 * count + 1), sf);
                        e.Graphics.DrawString(ReadFiles.UnitAttack[i] + "/" + ReadFiles.UnitDefense[i] + "/" + ReadFiles.UnitMove[i], new Font("Times New Roman", 11, FontStyle.Bold), new SolidBrush(Color.FromArgb(223, 223, 223)), new Point(255, 85 + 24 * count), sf);
                        //Hitpoints/firepower
                        e.Graphics.DrawString(ReadFiles.UnitHitp[i] + "/" + ReadFiles.UnitFirepwr[i], new Font("Times New Roman", 11, FontStyle.Bold), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(300 + 1, 85 + 24 * count + 1), sf);
                        e.Graphics.DrawString(ReadFiles.UnitHitp[i] + "/" + ReadFiles.UnitFirepwr[i], new Font("Times New Roman", 11, FontStyle.Bold), new SolidBrush(Color.FromArgb(223, 223, 223)), new Point(300, 85 + 24 * count), sf);
                        //No of active units
                        e.Graphics.DrawString(ActiveUnitCount[i].ToString() + " active", new Font("Times New Roman", 11, FontStyle.Bold), new SolidBrush(Color.Black), new Point(325 + 1, 85 + 24 * count + 1));
                        e.Graphics.DrawString(ActiveUnitCount[i].ToString() + " active", new Font("Times New Roman", 11, FontStyle.Bold), new SolidBrush(Color.FromArgb(255, 223, 79)), new Point(325, 85 + 24 * count));
                        //No of units in production
                        if (UnitInProductionCount[i] > 0)
                        {
                            e.Graphics.DrawString(UnitInProductionCount[i].ToString() + " in prod", new Font("Times New Roman", 11, FontStyle.Bold), new SolidBrush(Color.Black), new Point(390 + 1, 85 + 24 * count + 1));
                            e.Graphics.DrawString(UnitInProductionCount[i].ToString() + " in prod", new Font("Times New Roman", 11, FontStyle.Bold), new SolidBrush(Color.FromArgb(63, 187, 199)), new Point(390, 85 + 24 * count));
                        }
                        count++;
                    }
                }
            }
            else
            {
                //TO-DO show casualties
            }
            sf.Dispose();
        }

        //Switch between statistics (shows active units) & casualties (shows dead units)
        private void CasualtiesButton_Click(object sender, EventArgs e)
        {
            if (StatisticsActive)
            {
                StatisticsActive = false;
                CasualtiesButton.Text = "Statistics";
            }
            else
            {
                StatisticsActive = true;
                CasualtiesButton.Text = "Casualties";
            }
            MainPanel.Refresh();
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
