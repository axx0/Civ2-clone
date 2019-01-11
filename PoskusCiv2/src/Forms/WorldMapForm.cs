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
    public partial class WorldMapForm : Civ2form
    {
        public MainCiv2Window mainCiv2Window;
        DoubleBufferedPanel MinimapPanel;

        public WorldMapForm(MainCiv2Window _mainCiv2Window)
        {
            InitializeComponent();
            mainCiv2Window = _mainCiv2Window;

            Paint += new PaintEventHandler(WorldMapForm_Paint);
            Size = new Size((int)((_mainCiv2Window.ClientSize.Width) * 0.1375), (int)((_mainCiv2Window.ClientSize.Height - 30) * 0.15));

            //Minimap panel
            MinimapPanel = new DoubleBufferedPanel
            {
                Location = new Point(9, 38),
                Size = new Size(this.ClientSize.Width - 19, this.ClientSize.Height - 47),
                BackColor = Color.Black
            };
            Controls.Add(MinimapPanel);
            MinimapPanel.Paint += new PaintEventHandler(MinimapPanel_Paint);
        }

        private void WorldMapForm_Load(object sender, EventArgs e) { }

        private void WorldMapForm_Paint(object sender, PaintEventArgs e)
        {
            StringFormat sf = new StringFormat();
            sf.LineAlignment = StringAlignment.Center;
            sf.Alignment = StringAlignment.Center;
            e.Graphics.DrawString("World", new Font("Times New Roman", 19), new SolidBrush(Color.Black), new Point(this.Width / 2 + 1, 20 + 1), sf);
            e.Graphics.DrawString("World", new Font("Times New Roman", 19), new SolidBrush(Color.FromArgb(135, 135, 135)), new Point(this.Width / 2, 20), sf);
            sf.Dispose();
        }

        private void MinimapPanel_Paint(object sender, PaintEventArgs e)
        {
            //Draw line borders
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 0, 0, MinimapPanel.Width - 2, 0);   //1st layer of border
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 0, 0, 0, MinimapPanel.Height - 2);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), MinimapPanel.Width - 1, 0, MinimapPanel.Width - 1, MinimapPanel.Height - 1);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), 0, MinimapPanel.Height - 1, MinimapPanel.Width - 1, MinimapPanel.Height - 1);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 1, 1, MinimapPanel.Width - 3, 1);   //2nd layer of border
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 1, 1, 1, MinimapPanel.Height - 3);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), MinimapPanel.Width - 2, 1, MinimapPanel.Width - 2, MinimapPanel.Height - 2);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), 1, MinimapPanel.Height - 2, MinimapPanel.Width - 2, MinimapPanel.Height - 2);
        }

    }
}
