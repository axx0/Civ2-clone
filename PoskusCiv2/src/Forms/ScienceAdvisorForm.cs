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
                Maximum = 5
            };
            MainPanel.Controls.Add(HorizontalBar);
            HorizontalBar.ValueChanged += new EventHandler(HorizontalBarValueChanged);
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
            e.Graphics.DrawString("SCIENCE ADVISOR", new Font("Times New Roman", 14), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(302 + 2, 5 + 1), sf);
            e.Graphics.DrawString("SCIENCE ADVISOR", new Font("Times New Roman", 14), new SolidBrush(Color.FromArgb(223, 223, 223)), new Point(302, 5), sf);
            e.Graphics.DrawString("Kingdom of the " + Game.Civs[1].TribeName, new Font("Times New Roman", 14), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(302 + 2, 26 + 1), sf);
            e.Graphics.DrawString("Kingdom of the " + Game.Civs[1].TribeName, new Font("Times New Roman", 14), new SolidBrush(Color.FromArgb(223, 223, 223)), new Point(302, 26), sf);
            e.Graphics.DrawString("King " + Game.Civs[1].LeaderName + ": " + Math.Abs(Game.Data.GameYear).ToString() + " " + bcad, new Font("Times New Roman", 14), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(302 + 2, 47 + 1), sf);
            e.Graphics.DrawString("King " + Game.Civs[1].LeaderName + ": " + Math.Abs(Game.Data.GameYear).ToString() + " " + bcad, new Font("Times New Roman", 14), new SolidBrush(Color.FromArgb(223, 223, 223)), new Point(302, 47), sf);
            e.Graphics.DrawString("Researching: " + ReadFiles.TechName[Game.Civs[1].ReseachingTech], new Font("Times New Roman", 14), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(302 + 2, 74 + 1), sf);
            e.Graphics.DrawString("Researching: " + ReadFiles.TechName[Game.Civs[1].ReseachingTech], new Font("Times New Roman", 14), new SolidBrush(Color.FromArgb(223, 223, 223)), new Point(302, 74), sf);
            e.Graphics.DrawString("Discoveries Every 80 Turns", new Font("Times New Roman", 14), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(302 + 2, 130 + 1), sf);
            e.Graphics.DrawString("Discoveries Every 80 Turns", new Font("Times New Roman", 14), new SolidBrush(Color.FromArgb(223, 223, 223)), new Point(302, 130), sf);
            sf.Dispose();
            //Write discovered techs
            int count = 0;
            for (int i = 0; i < 89; i++)
            {
                if (Game.Civs[1].Techs[i] == true)
                {
                    int x = 198 * (count / 8);
                    int y = 22 * (count % 8);
                    e.Graphics.DrawImage(Images.ResearchIcons[0, 0], new Point(4 + x, 159 + y));
                    e.Graphics.DrawString(ReadFiles.TechName[i], new Font("Times New Roman", 11, FontStyle.Bold), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(x + 42 + 2, y + 160 + 1));
                    e.Graphics.DrawString(ReadFiles.TechName[i], new Font("Times New Roman", 11, FontStyle.Bold), new SolidBrush(Color.FromArgb(63, 187, 199)), new Point(x + 42, y + 160));
                    count++;
                    if (count == 24) break;
                }
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
