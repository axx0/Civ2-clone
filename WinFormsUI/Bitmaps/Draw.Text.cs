using System.Drawing;

namespace WinFormsUI
{
    public static partial class Draw
    {
        public static void Text(Graphics g, string text, Font font, StringAlignment horizAlignment, StringAlignment vertAlignment, Color frontColor, Point dest, Color? shadowColor = null, int shadowOffsetX = 0, int shadowOffsetY = 0)
        {
            using var sf = new StringFormat
            {
                LineAlignment = vertAlignment,
                Alignment = horizAlignment
            };

            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;  // Makes text sharp

            using var _brush1 = new SolidBrush(shadowColor ?? Color.Black);
            using var _brush2 = new SolidBrush(frontColor);
            g.DrawString(text, font, _brush1, new Point(dest.X + shadowOffsetX, dest.Y + shadowOffsetY), sf);
            g.DrawString(text, font, _brush2, dest, sf);
        }
    }
}
