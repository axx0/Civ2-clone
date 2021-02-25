using Eto.Forms;
using Eto.Drawing;

namespace EtoFormsUI
{
    public class Civ2button : Button
    {
        public Civ2button(string text, int width, int height, Font font)
        {
            Text = text;
            Font = font;
            Size = new Size(width, height);
            BackgroundColor = Color.FromArgb(192, 192, 192);
        }
    }
}
