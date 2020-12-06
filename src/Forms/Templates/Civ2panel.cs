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
        public Panel DrawPanel;
        private string? Title { get; set; }
        private bool ButtonsExist { get; set; }
        private int XtraSpacingUp => Title != null ? 27 : 0;
        private int XtraSpacingDwn => ButtonsExist ? 35 : 0;

        public Civ2panel(int width, int height, string _title, bool _buttonsExist)
        {
            Title = _title;
            ButtonsExist = _buttonsExist;

            Size = new Size(width, height);
            BackgroundImage = Images.PanelOuterWallpaper;
            this.Paint += new PaintEventHandler(Civ2panel_Paint);

            DrawPanel = new Panel()
            {
                Location = new Point(11, XtraSpacingUp + 11),
                Size = new Size(Width - 22, Height - 22 - XtraSpacingUp - XtraSpacingDwn),
                BackgroundImage = Images.PanelInnerWallpaper
            };
            Controls.Add(DrawPanel);
        }

        // Draw border around panel
        public virtual void Civ2panel_Paint(object sender, PaintEventArgs e)
        {
            // Title (if exists)
            if (Title != null)
            {
                StringFormat sf = new StringFormat
                {
                    LineAlignment = StringAlignment.Center,
                    Alignment = StringAlignment.Center
                };
                e.Graphics.DrawString(Title, new Font("Times New Roman", 17), new SolidBrush(Color.Black), new Point(this.Width / 2 + 1, 20 + 1), sf);
                e.Graphics.DrawString(Title, new Font("Times New Roman", 17), new SolidBrush(Color.FromArgb(135, 135, 135)), new Point(this.Width / 2, 20), sf);
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
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 9, 9 + XtraSpacingUp, 9 + (Width - 18 - 1), 9 + XtraSpacingUp);   // 1st layer of border
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 9, 9 + XtraSpacingUp, 9, Height - XtraSpacingDwn - 9 - 1);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), Width - 9 - 1, 9 + XtraSpacingUp, Width - 9 - 1, Height - XtraSpacingDwn - 9 - 1);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), 9, Height - XtraSpacingDwn - 9 - 1, Width - 9 - 1, Height - XtraSpacingDwn - 9 - 1);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 10, 10 + XtraSpacingUp, 9 + (Width - 18 - 2), 10 + XtraSpacingUp);   // 2nd layer of border
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 10, 10 + XtraSpacingUp, 10, Height - XtraSpacingDwn - 9 - 2);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), Width - 9 - 2, 10 + XtraSpacingUp, Width - 9 - 2, Height - XtraSpacingDwn - 9 - 2);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), 10, Height - XtraSpacingDwn - 9 - 2, Width - 9 - 2, Height - XtraSpacingDwn - 9 - 2);
        }

    }
}
