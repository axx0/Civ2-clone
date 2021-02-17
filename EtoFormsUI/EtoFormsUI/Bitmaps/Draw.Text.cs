using Eto.Drawing;

namespace EtoFormsUI
{
    public static partial class Draw
    {
        public static void Text(Graphics g, string text, Font font, Color frontColor, Point dest, Color? shadowColor = null, int shadowOffsetX = 0, int shadowOffsetY = 0)
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

            g.DrawText(shadow, new Point(dest.X + shadowOffsetX - (int)textSize.Width / 2, dest.Y + shadowOffsetY - (int)textSize.Height / 2));
            g.DrawText(front, new Point(dest.X - (int)textSize.Width / 2, dest.Y - (int)textSize.Height / 2));
        }
    }
}
