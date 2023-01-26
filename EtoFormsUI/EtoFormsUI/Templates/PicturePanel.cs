using Eto.Forms;
using Eto.Drawing;
using EtoFormsUI.Menu;
using Model;

namespace EtoFormsUI
{
    public class PicturePanel : Civ2panel
    {
        private readonly Image _image;

        public string Key { get; init; }

        public PicturePanel(InterfaceStyle look, Image image, string key) : base(look, image.Width + 2 * 11, image.Height + 2 * 11, 11, 11)
        {
            _image = image;

            Key = key;

            InnerPanel.Paint += Panel_Paint;
        }

        private void Panel_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(_image, 0, 0);
        }
    }
}
