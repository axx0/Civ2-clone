using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;
using civ2.Bitmaps;

namespace civ2.Forms
{
    // A panel in civ2 style can have:
    // - a header (title)
    // - a footer (usually buttons)
    // - none
    // - both

    public partial class Civ2panel : DoubleBufferedPanel
    {
        public DoubleBufferedPanel DrawPanel;
        private readonly string _title;
        private readonly int _paddingTop, _paddingBtm;

        public Civ2panel(int width, int height, string title, int paddingTop, int paddingBottom)
        {
            _title = title;
            _paddingTop = paddingTop;
            _paddingBtm = paddingBottom;

            Size = new Size(width, height);
            BackgroundImage = Images.PanelOuterWallpaper;
            this.Paint += new PaintEventHandler(Civ2panel_Paint);

            DrawPanel = new DoubleBufferedPanel()
            {
                Location = new Point(11, _paddingTop),
                Size = new Size(Width - 22, Height - _paddingTop - _paddingBtm),
                BackgroundImage = Images.PanelInnerWallpaper
            };
            Controls.Add(DrawPanel);
        }

        // Draw border around panel
        public virtual void Civ2panel_Paint(object sender, PaintEventArgs e)
        {
            // Title (if exists)
            if (_title != null)
            {
                StringFormat sf = new StringFormat
                {
                    LineAlignment = StringAlignment.Center,
                    Alignment = StringAlignment.Center
                };
                e.Graphics.DrawString(_title, new Font("Times New Roman", 17), new SolidBrush(Color.Black), new Point(this.Width / 2 + 1, 20 + 1), sf);
                e.Graphics.DrawString(_title, new Font("Times New Roman", 17), new SolidBrush(Color.FromArgb(135, 135, 135)), new Point(this.Width / 2, 20), sf);
            }
            // Outer border
            e.Graphics.DrawLine(new Pen(Color.FromArgb(227, 227, 227)), 0, 0, this.Width - 2, 0);   // 1st layer of border
            e.Graphics.DrawLine(new Pen(Color.FromArgb(227, 227, 227)), 0, 0, 0, this.Height - 2);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(105, 105, 105)), this.Width - 1, 0, this.Width - 1, this.Height - 1);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(105, 105, 105)), 0, this.Height - 1, this.Width - 1, this.Height - 1);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(255, 255, 255)), 1, 1, this.Width - 3, 1);   // 2nd layer of border
            e.Graphics.DrawLine(new Pen(Color.FromArgb(255, 255, 255)), 1, 1, 1, this.Height - 3);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(160, 160, 160)), this.Width - 2, 1, this.Width - 2, this.Height - 2);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(160, 160, 160)), 1, this.Height - 2, this.Width - 2, this.Height - 2);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(240, 240, 240)), 2, 2, this.Width - 4, 2);   // 3rd layer of border
            e.Graphics.DrawLine(new Pen(Color.FromArgb(240, 240, 240)), 2, 2, 2, this.Height - 4);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(240, 240, 240)), this.Width - 3, 2, this.Width - 3, this.Height - 3);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(240, 240, 240)), 2, this.Height - 3, this.Width - 3, this.Height - 3);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), 3, 3, this.Width - 5, 3);   // 4th layer of border
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), 3, 3, 3, this.Height - 5);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), this.Width - 4, 3, this.Width - 4, this.Height - 4);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 3, this.Height - 4, this.Width - 4, this.Height - 4);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), 4, 4, this.Width - 6, 4);   // 5th layer of border
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), 4, 4, 4, this.Height - 6);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), this.Width - 5, 4, this.Width - 5, this.Height - 5);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 4, this.Height - 5, this.Width - 5, this.Height - 5);
            // Border for draw panel
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 9, _paddingTop - 1, 9 + (Width - 18 - 1), _paddingTop - 1);   // 1st layer of border
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 10, _paddingTop - 1, 10, Height - _paddingBtm - 1);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), Width - 11, _paddingTop - 1, Width - 11, Height - _paddingBtm - 1);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), 9, Height - _paddingBtm, Width - 9 - 1, Height - _paddingBtm);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 10, _paddingTop - 2, 9 + (Width - 18 - 2), _paddingTop - 2);   // 2nd layer of border
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 9, _paddingTop - 2, 9, Height - _paddingBtm);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), Width - 10, _paddingTop - 2, Width - 10, Height - _paddingBtm);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), 9, Height - _paddingBtm + 1, Width - 9 - 1, Height - _paddingBtm + 1);
        }

    }
}
