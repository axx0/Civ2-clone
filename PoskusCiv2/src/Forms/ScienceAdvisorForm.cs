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
    public partial class ScienceAdvisorForm : Civ2form
    {
        DoubleBufferedPanel MainPanel;
        HScrollBar HorizontalBar;
        public int BarValue { get; set; }       //starting value of view of horizontal bar
        List<int> DiscoveredTechs;

        public ScienceAdvisorForm()
        {
            InitializeComponent();

            BarValue = 0;

            //Main panel
            MainPanel = new DoubleBufferedPanel
            {
                Location = new Point(9, 9),
                Size = new Size(604, 404)
            };
            Controls.Add(MainPanel);
            MainPanel.Paint += new PaintEventHandler(MainPanel_Paint);

            //Goal button
            Civ2button GoalButton = new Civ2button
            {
                Location = new Point(4, 376),
                Size = new Size(297, 24),
                Font = new Font("Times New Roman", 11),
                Text = "Goal"
            };
            MainPanel.Controls.Add(GoalButton);
            GoalButton.Click += new EventHandler(GoalButton_Click);

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

            //Horizontal bar
            HorizontalBar = new HScrollBar()
            {
                Location = new Point(4, 355),
                Size = new Size(596, 17),
                LargeChange = 1,
                Maximum = Game.Civs[1].Techs.Sum() / 8  //8 techs shown per column. Maximum=0 if no of techs <= 8, maximum=1 for techs=9...16, etc. (slider cannot move if maximum=0, it can move 1 move if maximum=1, it can move 2 moves if maximum=2, ...)
            };
            MainPanel.Controls.Add(HorizontalBar);
            HorizontalBar.ValueChanged += new EventHandler(HorizontalBarValueChanged);

            //Create a list of discovered techs
            DiscoveredTechs = new List<int>();
            for (int i = 0; i < 89; i++)    //browse through all techs
            {
                if (Game.Civs[1].Techs[i] == 1) DiscoveredTechs.Add(i);
            }
        }

        private void ScienceAdvisorForm_Load(object sender, EventArgs e) { }

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
            e.Graphics.DrawImage(Images.ScienceAdvWallpaper, new Rectangle(2, 2, 600, 400));
            //Draw white rectangle
            e.Graphics.DrawRectangle(new Pen(Color.White), new Rectangle(4, 73, 595, 53));
            //Draw text
            string bcad;
            if (Game.Data.GameYear < 0) { bcad = "B.C."; }
            else { bcad = "A.D."; }
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            e.Graphics.DrawString("SCIENCE ADVISOR", new Font("Times New Roman", 14), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(302 + 2, 3 + 1), sf);
            e.Graphics.DrawString("SCIENCE ADVISOR", new Font("Times New Roman", 14), new SolidBrush(Color.FromArgb(223, 223, 223)), new Point(302, 3), sf);
            e.Graphics.DrawString("Kingdom of the " + Game.Civs[1].TribeName, new Font("Times New Roman", 14), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(302 + 2, 24 + 1), sf);
            e.Graphics.DrawString("Kingdom of the " + Game.Civs[1].TribeName, new Font("Times New Roman", 14), new SolidBrush(Color.FromArgb(223, 223, 223)), new Point(302, 24), sf);
            e.Graphics.DrawString("King " + Game.Civs[1].LeaderName + ": " + Math.Abs(Game.Data.GameYear).ToString() + " " + bcad, new Font("Times New Roman", 14), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(302 + 2, 45 + 1), sf);
            e.Graphics.DrawString("King " + Game.Civs[1].LeaderName + ": " + Math.Abs(Game.Data.GameYear).ToString() + " " + bcad, new Font("Times New Roman", 14), new SolidBrush(Color.FromArgb(223, 223, 223)), new Point(302, 45), sf);
            e.Graphics.DrawString("Researching: " + ReadFiles.TechName[Game.Civs[1].ReseachingTech], new Font("Times New Roman", 14), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(302 + 2, 74 + 1), sf);
            e.Graphics.DrawString("Researching: " + ReadFiles.TechName[Game.Civs[1].ReseachingTech], new Font("Times New Roman", 14), new SolidBrush(Color.FromArgb(223, 223, 223)), new Point(302, 74), sf);
            e.Graphics.DrawString("Discoveries Every 80 Turns", new Font("Times New Roman", 14), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(302 + 2, 130 + 1), sf);
            e.Graphics.DrawString("Discoveries Every 80 Turns", new Font("Times New Roman", 14), new SolidBrush(Color.FromArgb(223, 223, 223)), new Point(302, 130), sf);
            sf.Dispose();
            //Write discovered techs
            int count = 0;
            for (int i = BarValue * 8; i < DiscoveredTechs.Count(); i++)
            {
                int x = 198 * (count / 8);
                int y = 22 * (count % 8);
                e.Graphics.DrawImage(Images.ResearchIcons[ReadFiles.TechCategory[DiscoveredTechs[i]], ReadFiles.TechEpoch[DiscoveredTechs[i]]], new Point(4 + x, 159 + y));
                e.Graphics.DrawString(ReadFiles.TechName[DiscoveredTechs[i]], new Font("Times New Roman", 11, FontStyle.Bold), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(x + 42 + 2, y + 160 + 1));
                e.Graphics.DrawString(ReadFiles.TechName[DiscoveredTechs[i]], new Font("Times New Roman", 11, FontStyle.Bold), new SolidBrush(Color.FromArgb(63, 187, 199)), new Point(x + 42, y + 160));
                count++;
                if (count == 24) break; //only 24 can be shown at a time (3 columns)
            }
        }

        private void GoalButton_Click(object sender, EventArgs e) { }

        private void CloseButton_Click(object sender, EventArgs e) { Close(); }

        //Once slider value changes --> redraw list
        private void HorizontalBarValueChanged(object sender, EventArgs e)
        {
            BarValue = HorizontalBar.Value;
            Refresh();
        }
    }
}
