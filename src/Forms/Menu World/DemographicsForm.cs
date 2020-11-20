using System;
using System.Drawing;
using System.Windows.Forms;
using civ2.Bitmaps;

namespace civ2.Forms
{
    public partial class DemographicsForm : Civ2form
    {
        DoubleBufferedPanel MainPanel;

        public DemographicsForm()
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
        }

        private void DemographicsForm_Load(object sender, EventArgs e) { }

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
            e.Graphics.DrawImage(Images.DemographicsWallpaper, new Rectangle(2, 2, 600, 400));
            //Draw lines
            for (int row = 0; row < 12; row++)
            {
                e.Graphics.DrawLine(new Pen(Color.FromArgb(35, 135, 155)), 4, 58 + 25 * row, MainPanel.Width - 4, 58 + 25 * row);
            }
            //Text
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            e.Graphics.DrawString("Roman Demographics", new Font("Times New Roman", 13), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(302 + 1, 14 + 1), sf);
            e.Graphics.DrawString("Roman Demographics", new Font("Times New Roman", 13), new SolidBrush(Color.FromArgb(223, 223, 223)), new Point(302, 14), sf);
            sf.Alignment = StringAlignment.Near;
            //Approval rating
            int y = 47;
            e.Graphics.DrawString("Approval Rating:", new Font("Times New Roman", 13), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(4 + 1, y + 1), sf);
            e.Graphics.DrawString("Approval Rating:", new Font("Times New Roman", 13), new SolidBrush(Color.FromArgb(223, 223, 223)), new Point(4, y), sf);
            e.Graphics.DrawString("?%", new Font("Times New Roman", 13), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(181 + 1, y + 1), sf);
            e.Graphics.DrawString("?%", new Font("Times New Roman", 13), new SolidBrush(Color.FromArgb(223, 223, 223)), new Point(181, y), sf);
            e.Graphics.DrawString("?st", new Font("Times New Roman", 13), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(324 + 1, y + 1), sf);
            e.Graphics.DrawString("?st", new Font("Times New Roman", 13), new SolidBrush(Color.FromArgb(223, 223, 223)), new Point(324, y), sf);
            //Population
            y = 47 + 25 * 1;
            e.Graphics.DrawString("Population:", new Font("Times New Roman", 13), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(4 + 1, y + 1), sf);
            e.Graphics.DrawString("Population:", new Font("Times New Roman", 13), new SolidBrush(Color.FromArgb(223, 223, 223)), new Point(4, y), sf);
            string population = String.Format("{0:n0}", Game.Civs[1].Population);
            e.Graphics.DrawString(population, new Font("Times New Roman", 13), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(181 + 1, y + 1), sf);
            e.Graphics.DrawString(population, new Font("Times New Roman", 13), new SolidBrush(Color.FromArgb(223, 223, 223)), new Point(181, y), sf);
            e.Graphics.DrawString("?st", new Font("Times New Roman", 13), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(324 + 1, y + 1), sf);
            e.Graphics.DrawString("?st", new Font("Times New Roman", 13), new SolidBrush(Color.FromArgb(223, 223, 223)), new Point(324, y), sf);
            //GNP
            y = 47 + 25 * 2;
            e.Graphics.DrawString("GNP:", new Font("Times New Roman", 13), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(4 + 1, y + 1), sf);
            e.Graphics.DrawString("GNP:", new Font("Times New Roman", 13), new SolidBrush(Color.FromArgb(223, 223, 223)), new Point(4, y), sf);
            e.Graphics.DrawString("? million $", new Font("Times New Roman", 13), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(181 + 1, y + 1), sf);
            e.Graphics.DrawString("? million $", new Font("Times New Roman", 13), new SolidBrush(Color.FromArgb(223, 223, 223)), new Point(181, y), sf);
            e.Graphics.DrawString("?st", new Font("Times New Roman", 13), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(324 + 1, y + 1), sf);
            e.Graphics.DrawString("?st", new Font("Times New Roman", 13), new SolidBrush(Color.FromArgb(223, 223, 223)), new Point(324, y), sf);
            //Mfg goods
            y = 47 + 25 * 3;
            e.Graphics.DrawString("Mfg. Goods:", new Font("Times New Roman", 13), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(4 + 1, y + 1), sf);
            e.Graphics.DrawString("Mfg. Goods:", new Font("Times New Roman", 13), new SolidBrush(Color.FromArgb(223, 223, 223)), new Point(4, y), sf);
            e.Graphics.DrawString("? Mtons", new Font("Times New Roman", 13), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(181 + 1, y + 1), sf);
            e.Graphics.DrawString("? Mtons", new Font("Times New Roman", 13), new SolidBrush(Color.FromArgb(223, 223, 223)), new Point(181, y), sf);
            e.Graphics.DrawString("?st", new Font("Times New Roman", 13), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(324 + 1, y + 1), sf);
            e.Graphics.DrawString("?st", new Font("Times New Roman", 13), new SolidBrush(Color.FromArgb(223, 223, 223)), new Point(324, y), sf);
            //Land area
            y = 47 + 25 * 4;
            e.Graphics.DrawString("Land Area:", new Font("Times New Roman", 13), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(4 + 1, y + 1), sf);
            e.Graphics.DrawString("Land Area:", new Font("Times New Roman", 13), new SolidBrush(Color.FromArgb(223, 223, 223)), new Point(4, y), sf);
            e.Graphics.DrawString("? sq. miles", new Font("Times New Roman", 13), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(181 + 1, y + 1), sf);
            e.Graphics.DrawString("? sq. miles", new Font("Times New Roman", 13), new SolidBrush(Color.FromArgb(223, 223, 223)), new Point(181, y), sf);
            e.Graphics.DrawString("?st", new Font("Times New Roman", 13), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(324 + 1, y + 1), sf);
            e.Graphics.DrawString("?st", new Font("Times New Roman", 13), new SolidBrush(Color.FromArgb(223, 223, 223)), new Point(324, y), sf);
            //Literacy
            y = 47 + 25 * 5;
            e.Graphics.DrawString("Literacy:", new Font("Times New Roman", 13), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(4 + 1, y + 1), sf);
            e.Graphics.DrawString("Literacy:", new Font("Times New Roman", 13), new SolidBrush(Color.FromArgb(223, 223, 223)), new Point(4, y), sf);
            e.Graphics.DrawString("?%", new Font("Times New Roman", 13), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(181 + 1, y + 1), sf);
            e.Graphics.DrawString("?%", new Font("Times New Roman", 13), new SolidBrush(Color.FromArgb(223, 223, 223)), new Point(181, y), sf);
            e.Graphics.DrawString("?st", new Font("Times New Roman", 13), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(324 + 1, y + 1), sf);
            e.Graphics.DrawString("?st", new Font("Times New Roman", 13), new SolidBrush(Color.FromArgb(223, 223, 223)), new Point(324, y), sf);
            //Disease
            y = 47 + 25 * 6;
            e.Graphics.DrawString("Disease:", new Font("Times New Roman", 13), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(4 + 1, y + 1), sf);
            e.Graphics.DrawString("Disease:", new Font("Times New Roman", 13), new SolidBrush(Color.FromArgb(223, 223, 223)), new Point(4, y), sf);
            e.Graphics.DrawString("?%", new Font("Times New Roman", 13), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(181 + 1, y + 1), sf);
            e.Graphics.DrawString("?%", new Font("Times New Roman", 13), new SolidBrush(Color.FromArgb(223, 223, 223)), new Point(181, y), sf);
            e.Graphics.DrawString("?st", new Font("Times New Roman", 13), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(324 + 1, y + 1), sf);
            e.Graphics.DrawString("?st", new Font("Times New Roman", 13), new SolidBrush(Color.FromArgb(223, 223, 223)), new Point(324, y), sf);
            //Pollution
            y = 47 + 25 * 7;
            e.Graphics.DrawString("Pollution:", new Font("Times New Roman", 13), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(4 + 1, y + 1), sf);
            e.Graphics.DrawString("Pollution:", new Font("Times New Roman", 13), new SolidBrush(Color.FromArgb(223, 223, 223)), new Point(4, y), sf);
            e.Graphics.DrawString("? tons", new Font("Times New Roman", 13), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(181 + 1, y + 1), sf);
            e.Graphics.DrawString("? tons", new Font("Times New Roman", 13), new SolidBrush(Color.FromArgb(223, 223, 223)), new Point(181, y), sf);
            e.Graphics.DrawString("?st", new Font("Times New Roman", 13), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(324 + 1, y + 1), sf);
            e.Graphics.DrawString("?st", new Font("Times New Roman", 13), new SolidBrush(Color.FromArgb(223, 223, 223)), new Point(324, y), sf);
            //Life expectancy
            y = 47 + 25 * 8;
            e.Graphics.DrawString("Life Expectancy:", new Font("Times New Roman", 13), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(4 + 1, y + 1), sf);
            e.Graphics.DrawString("Life Expectancy:", new Font("Times New Roman", 13), new SolidBrush(Color.FromArgb(223, 223, 223)), new Point(4, y), sf);
            e.Graphics.DrawString("? years", new Font("Times New Roman", 13), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(181 + 1, y + 1), sf);
            e.Graphics.DrawString("? years", new Font("Times New Roman", 13), new SolidBrush(Color.FromArgb(223, 223, 223)), new Point(181, y), sf);
            e.Graphics.DrawString("?st", new Font("Times New Roman", 13), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(324 + 1, y + 1), sf);
            e.Graphics.DrawString("?st", new Font("Times New Roman", 13), new SolidBrush(Color.FromArgb(223, 223, 223)), new Point(324, y), sf);
            //Family size
            y = 47 + 25 * 9;
            e.Graphics.DrawString("Family Size:", new Font("Times New Roman", 13), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(4 + 1, y + 1), sf);
            e.Graphics.DrawString("Family Size:", new Font("Times New Roman", 13), new SolidBrush(Color.FromArgb(223, 223, 223)), new Point(4, y), sf);
            e.Graphics.DrawString("? children", new Font("Times New Roman", 13), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(181 + 1, y + 1), sf);
            e.Graphics.DrawString("? children", new Font("Times New Roman", 13), new SolidBrush(Color.FromArgb(223, 223, 223)), new Point(181, y), sf);
            e.Graphics.DrawString("?st", new Font("Times New Roman", 13), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(324 + 1, y + 1), sf);
            e.Graphics.DrawString("?st", new Font("Times New Roman", 13), new SolidBrush(Color.FromArgb(223, 223, 223)), new Point(324, y), sf);
            //Military service
            y = 47 + 25 * 10;
            e.Graphics.DrawString("Military Service:", new Font("Times New Roman", 13), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(4 + 1, y + 1), sf);
            e.Graphics.DrawString("Military Service:", new Font("Times New Roman", 13), new SolidBrush(Color.FromArgb(223, 223, 223)), new Point(4, y), sf);
            e.Graphics.DrawString("? years", new Font("Times New Roman", 13), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(181 + 1, y + 1), sf);
            e.Graphics.DrawString("? years", new Font("Times New Roman", 13), new SolidBrush(Color.FromArgb(223, 223, 223)), new Point(181, y), sf);
            e.Graphics.DrawString("?st", new Font("Times New Roman", 13), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(324 + 1, y + 1), sf);
            e.Graphics.DrawString("?st", new Font("Times New Roman", 13), new SolidBrush(Color.FromArgb(223, 223, 223)), new Point(324, y), sf);
            //Annual income
            y = 47 + 25 * 11;
            e.Graphics.DrawString("Annual Income:", new Font("Times New Roman", 13), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(4 + 1, y + 1), sf);
            e.Graphics.DrawString("Annual Income:", new Font("Times New Roman", 13), new SolidBrush(Color.FromArgb(223, 223, 223)), new Point(4, y), sf);
            e.Graphics.DrawString("?$ per capita", new Font("Times New Roman", 13), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(181 + 1, y + 1), sf);
            e.Graphics.DrawString("?$ per capita", new Font("Times New Roman", 13), new SolidBrush(Color.FromArgb(223, 223, 223)), new Point(181, y), sf);
            e.Graphics.DrawString("?st", new Font("Times New Roman", 13), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(324 + 1, y + 1), sf);
            e.Graphics.DrawString("?st", new Font("Times New Roman", 13), new SolidBrush(Color.FromArgb(223, 223, 223)), new Point(324, y), sf);
            //Productivity
            y = 47 + 25 * 12;
            e.Graphics.DrawString("Productivity:", new Font("Times New Roman", 13), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(4 + 1, y + 1), sf);
            e.Graphics.DrawString("Productivity:", new Font("Times New Roman", 13), new SolidBrush(Color.FromArgb(223, 223, 223)), new Point(4, y), sf);
            e.Graphics.DrawString("?", new Font("Times New Roman", 13), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(181 + 1, y + 1), sf);
            e.Graphics.DrawString("?", new Font("Times New Roman", 13), new SolidBrush(Color.FromArgb(223, 223, 223)), new Point(181, y), sf);
            e.Graphics.DrawString("?st", new Font("Times New Roman", 13), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(324 + 1, y + 1), sf);
            e.Graphics.DrawString("?st", new Font("Times New Roman", 13), new SolidBrush(Color.FromArgb(223, 223, 223)), new Point(324, y), sf);

            sf.Dispose();
        }

        private void CloseButton_Click(object sender, EventArgs e) { Close(); }        
    }
}
