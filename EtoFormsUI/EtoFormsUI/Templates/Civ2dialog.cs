using System;
using Eto.Forms;
using Eto.Drawing;

namespace EtoFormsUI
{
    public abstract class Civ2dialog : Dialog
    {
        protected Drawable Surface;
        protected PixelLayout Layout;
        private readonly int _paddingTop, _paddingBtm;
        private readonly string _title;

        public Civ2dialog(Main parent, int width, int height, int paddingTopInnerPanel, int paddingBtmInnerPanel, string title = null)
        {
            foreach (MenuItem item in parent.Menu.Items) item.Enabled = false;

            WindowStyle = WindowStyle.None;
            MovableByWindowBackground = true;
            
            Size = new Size(width, height);
            _paddingTop = paddingTopInnerPanel;
            _paddingBtm = paddingBtmInnerPanel;
            _title = title;

            Layout = new PixelLayout() { Size = new Size(width, height) };

            // Drawable surface
            Surface = new Drawable() { Size = new Size(width, height), CanFocus = false };
            Surface.Paint += Surface_Paint;
            
            Layout.Add(Surface, 0, 0);
        }


        private void Surface_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.AntiAlias = false;

            // Paint outer wallpaper
            var imgSize = Images.PanelOuterWallpaper.Size;
            for (int row = 0; row < this.Height / imgSize.Height + 1; row++)
            {
                for (int col = 0; col < this.Width / imgSize.Width + 1; col++)
                {
                    e.Graphics.DrawImage(Images.PanelOuterWallpaper, col * imgSize.Width, row * imgSize.Height);
                }
            }

            // Paint panel borders
            // Outer border
            using var _pen1 = new Pen(Color.FromArgb(227, 227, 227));
            using var _pen2 = new Pen(Color.FromArgb(105, 105, 105));
            using var _pen3 = new Pen(Color.FromArgb(255, 255, 255));
            using var _pen4 = new Pen(Color.FromArgb(160, 160, 160));
            using var _pen5 = new Pen(Color.FromArgb(240, 240, 240));
            using var _pen6 = new Pen(Color.FromArgb(223, 223, 223));
            using var _pen7 = new Pen(Color.FromArgb(67, 67, 67));
            e.Graphics.DrawLine(_pen1, 0, 0, this.Width - 2, 0);   // 1st layer of border
            e.Graphics.DrawLine(_pen1, 0, 0, 0, this.Height - 2);
            e.Graphics.DrawLine(_pen2, this.Width - 1, 0, this.Width - 1, this.Height - 1);
            e.Graphics.DrawLine(_pen2, 0, this.Height - 1, this.Width - 1, this.Height - 1);
            e.Graphics.DrawLine(_pen3, 1, 1, this.Width - 3, 1);   // 2nd layer of border
            e.Graphics.DrawLine(_pen3, 1, 1, 1, this.Height - 3);
            e.Graphics.DrawLine(_pen4, this.Width - 2, 1, this.Width - 2, this.Height - 2);
            e.Graphics.DrawLine(_pen4, 1, this.Height - 2, this.Width - 2, this.Height - 2);
            e.Graphics.DrawLine(_pen5, 2, 2, this.Width - 4, 2);   // 3rd layer of border
            e.Graphics.DrawLine(_pen5, 2, 2, 2, this.Height - 4);
            e.Graphics.DrawLine(_pen5, this.Width - 3, 2, this.Width - 3, this.Height - 3);
            e.Graphics.DrawLine(_pen5, 2, this.Height - 3, this.Width - 3, this.Height - 3);
            e.Graphics.DrawLine(_pen6, 3, 3, this.Width - 5, 3);   // 4th layer of border
            e.Graphics.DrawLine(_pen6, 3, 3, 3, this.Height - 5);
            e.Graphics.DrawLine(_pen7, this.Width - 4, 3, this.Width - 4, this.Height - 4);
            e.Graphics.DrawLine(_pen7, 3, this.Height - 4, this.Width - 4, this.Height - 4);
            e.Graphics.DrawLine(_pen6, 4, 4, this.Width - 6, 4);   // 5th layer of border
            e.Graphics.DrawLine(_pen6, 4, 4, 4, this.Height - 6);
            e.Graphics.DrawLine(_pen7, this.Width - 5, 4, this.Width - 5, this.Height - 5);
            e.Graphics.DrawLine(_pen7, 4, this.Height - 5, this.Width - 5, this.Height - 5);

            // Inner border
            e.Graphics.DrawLine(_pen7, 9, _paddingTop - 1, 9 + (Width - 18 - 1), _paddingTop - 1);   // 1st layer of border
            e.Graphics.DrawLine(_pen7, 10, _paddingTop - 1, 10, Height - _paddingBtm - 1);
            e.Graphics.DrawLine(_pen6, Width - 11, _paddingTop - 1, Width - 11, Height - _paddingBtm - 1);
            e.Graphics.DrawLine(_pen6, 9, Height - _paddingBtm, Width - 9 - 1, Height - _paddingBtm);
            e.Graphics.DrawLine(_pen7, 10, _paddingTop - 2, 9 + (Width - 18 - 2), _paddingTop - 2);   // 2nd layer of border
            e.Graphics.DrawLine(_pen7, 9, _paddingTop - 2, 9, Height - _paddingBtm);
            e.Graphics.DrawLine(_pen6, Width - 10, _paddingTop - 2, Width - 10, Height - _paddingBtm);
            e.Graphics.DrawLine(_pen6, 9, Height - _paddingBtm + 1, Width - 9 - 1, Height - _paddingBtm + 1);

            // Paint inner wallpaper
            imgSize = Images.PanelInnerWallpaper.Size;
            Rectangle rectS;
            for (int row = 0; row < (this.Height - _paddingTop - _paddingBtm) / imgSize.Height + 1; row++)
            {
                for (int col = 0; col < (this.Width - 2 * 11) / imgSize.Width + 1; col++)
                {
                    rectS = new Rectangle(0, 0, Math.Min(this.Width - 2 * 11 - col * imgSize.Width, imgSize.Width), Math.Min(this.Height - _paddingBtm - _paddingTop - row * imgSize.Height, imgSize.Height));
                    e.Graphics.DrawImage(Images.PanelInnerWallpaper, rectS, new Point(col * imgSize.Width + 11, row * imgSize.Height + _paddingTop));
                }
            }

            // Paint title (if it exists)
            if (_title != null)
            {
                Draw.Text(e.Graphics, _title, new Font("Times new roman", 17, FontStyle.Bold), Color.FromArgb(135, 135, 135), new Point(this.Width / 2, _paddingTop / 2), true, true, Colors.Black, 1, 1);
            }
        }
    }
}
