using Eto.Forms;
using Eto.Drawing;

namespace EtoFormsUI
{
    public class Civ2button : Button
    {
        public Civ2button(string text, int width, int height)
        {
            Text = text;
            Font = new Font("Times new roman", 11);
            Size = new Size(width, height);
            BackgroundColor = Color.FromArgb(192, 192, 192);
        }
    }
}
