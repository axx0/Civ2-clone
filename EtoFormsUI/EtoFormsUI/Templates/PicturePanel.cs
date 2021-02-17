using System;
using Eto.Forms;
using Eto.Drawing;

namespace EtoFormsUI
{
    public class PicturePanel : Civ2panel
    {
        private readonly Image _image;

        public PicturePanel(Image image) : base(image.Width + 2 * 11, image.Height + 2 * 11, 11, 11)
        {
            _image = image;

            InnerPanel.Paint += Panel_Paint;
        }

        private void Panel_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(_image, 0, 0);
        }
    }
}
