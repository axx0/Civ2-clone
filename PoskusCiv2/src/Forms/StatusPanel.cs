using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using RTciv2.Imagery;

namespace RTciv2.Forms
{
    public partial class StatusPanel : Civ2panel
    {
        DoubleBufferedPanel StatsPanel, UnitPanel;

        public StatusPanel(int width, int height)
        {
            Size = new Size(width, height);
            this.Paint += new PaintEventHandler(StatusPanel_Paint);

            StatsPanel = new DoubleBufferedPanel()
            {
                Location = new Point(11, 38),
                Size = new Size(240, 60),
                BackgroundImage = Images.WallpaperStatusForm
            };
            Controls.Add(StatsPanel);

            UnitPanel = new DoubleBufferedPanel()
            {
                Location = new Point(11, 106),
                Size = new Size(240, Height - 117),
                BackgroundImage = Images.WallpaperStatusForm
            };
            Controls.Add(UnitPanel);
        }

        private void StatusPanel_Paint(object sender, PaintEventArgs e)
        {
            //Title
            StringFormat sf = new StringFormat();
            sf.LineAlignment = StringAlignment.Center;
            sf.Alignment = StringAlignment.Center;
            e.Graphics.DrawString("Status", new Font("Times New Roman", 18), new SolidBrush(Color.Black), new Point(this.Width / 2 + 1, 20 + 1), sf);
            e.Graphics.DrawString("Status", new Font("Times New Roman", 18), new SolidBrush(Color.FromArgb(135, 135, 135)), new Point(this.Width / 2, 20), sf);
            sf.Dispose();
            //Draw line borders of stats panel
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 9, 36, 252, 36);   //1st layer of border
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 9, 36, 9, 98);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), 9, 99, 252, 99);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), 252, 36, 252, 99);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 10, 37, 250, 37);   //2nd layer of border
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 10, 38, 10, 97);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), 10, 98, 251, 98);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), 251, 37, 251, 98);
            //Draw line borders of unit panel
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 9, 104, 252, 104);   //1st layer of border
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 9, 104, 9, 106 + UnitPanel.Height);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), 9, 107 + UnitPanel.Height, 252, 107 + UnitPanel.Height);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), 252, 104, 252, 105 + UnitPanel.Height);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 9, 105, 250, 105);   //2nd layer of border
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 10, 104, 10, 105 + UnitPanel.Height);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), 10, 106 + UnitPanel.Height, 252, 106 + UnitPanel.Height);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), 251, 105, 251, 105 + UnitPanel.Height);
        }

    }
}
