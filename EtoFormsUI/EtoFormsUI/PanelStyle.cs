using Eto.Drawing;

namespace EtoFormsUI
{
    public class PanelStyle
    {
        public Color BackColor { get; }
        public Color FrontColor { get; }
        public Font Font { get; }

        public PanelStyle(Font font)
        {
            Font = font;
                
            FrontColor = Color.FromArgb(51, 51, 51);
            BackColor = Color.FromArgb(191, 191, 191);
        }
    }
}