using Eto.Drawing;

namespace EtoFormsUI
{
    public static partial class Draw
    {
        // Draw text at destination point
        public static void Text(Graphics g, string text, Font font, Color frontColor, Point dest, bool centerHorizontally, bool centerVertically, Color? shadowColor = null, int shadowOffsetX = 0, int shadowOffsetY = 0)
        {
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

            //g.AntiAlias = false;
            //g.ImageInterpolation = ImageInterpolation.Low;
            if (shadowOffsetX != 0 || shadowOffsetY != 0) g.DrawText(shadow, new Point(dest.X + shadowOffsetX, dest.Y + shadowOffsetY));
            g.DrawText(front, dest);
        }

        // Draw text in frame
        public static void Text(Graphics g, string text, Font font, Color frontColor, Rectangle frame, FormattedTextAlignment horizAlignment, Color? shadowColor = null, int shadowOffsetX = 0, int shadowOffsetY = 0)
        {
            if (shadowOffsetX != 0 || shadowOffsetY != 0) g.DrawText(font, new SolidBrush(shadowColor ?? Colors.Black), frame, text, FormattedTextWrapMode.None, horizAlignment, FormattedTextTrimming.None);
            g.DrawText(font, new SolidBrush(frontColor), new Rectangle(frame.X + 1, frame.Y + 1, frame.Width, frame.Height), text, FormattedTextWrapMode.None, horizAlignment, FormattedTextTrimming.None);
        }
    }
}
