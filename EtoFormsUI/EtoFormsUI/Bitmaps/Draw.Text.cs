using Eto.Drawing;

namespace EtoFormsUI
{
    public static partial class Draw
    {
        public static void Text(Graphics g, string text, Font font, Color frontColor, Point dest, bool centerHorizontally, bool centerVertically, Color? shadowColor = null, int shadowOffsetX = 0, int shadowOffsetY = 0)
        {
            //g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;  // Makes text sharp

            var front = new FormattedText()
            {
                Font = font,
                ForegroundBrush = new SolidBrush(frontColor),
                Text = text
            };

            var shadow = new FormattedText()
            {
                Font = font,
                ForegroundBrush = new SolidBrush(shadowColor ?? Colors.Black),
                Text = text
            };

            var textSize = front.Measure();

            // Center the text
            if (centerHorizontally) dest.X -= (int)(textSize.Width / 2);
            if (centerVertically) dest.Y -= (int)(textSize.Height / 2);

            if (shadowOffsetX != 0 || shadowOffsetY != 0) g.DrawText(shadow, new Point(dest.X + shadowOffsetX, dest.Y + shadowOffsetY));
            g.DrawText(front, dest);
        }
    }
}
