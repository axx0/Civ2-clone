using Eto.Drawing;

namespace EtoFormsUI
{
    public class Civ2button : CustomEtoButton
    {
        public Civ2button(string text, int width, int height, Font font, Image backgroundImage = null)
        {
            Text = text;
            Font = font;
            Size = new Size(width, height);

            if (backgroundImage == null)
            {
                var _tilePic = new Bitmap(width, height, PixelFormat.Format32bppRgba);
                using (var g = new Graphics(_tilePic))
                {
                    g.AntiAlias = false;
                    g.DrawRectangle(Color.FromArgb(100, 100, 100), 0, 0, width - 1, height - 1);
                    g.FillRectangle(Color.FromArgb(192, 192, 192), 3, 3, width - 6, height - 6);
                    g.DrawLine(Color.FromArgb(128, 128, 128), 2, height - 2, width - 2, height - 2);
                    g.DrawLine(Color.FromArgb(128, 128, 128), 3, height - 3, width - 2, height - 3);
                    g.DrawLine(Color.FromArgb(128, 128, 128), width - 2, 2, width - 2, height - 3);
                    g.DrawLine(Color.FromArgb(128, 128, 128), width - 3, 3, width - 3, height - 3);
                }

                BackgroundImage = _tilePic;
            }
            else BackgroundImage = backgroundImage;
        }
    }
}
