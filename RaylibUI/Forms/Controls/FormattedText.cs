using Raylib_CSharp.Colors;
using Raylib_CSharp.Fonts;
using Raylib_CSharp.Rendering;
using System.Numerics;

namespace RaylibUI.Forms;

public class FormattedText : Control
{
    public string Text { get; set; }
    public HorizontalAlignment HorizontalAlignment { get; set; } = HorizontalAlignment.Left;
    public VerticalAlignment VerticalAlignment { get; set; } = VerticalAlignment.Top;
    public Font Font { get; set; }// = Fonts.DefaultFont;
    public int FontSize { get; set; } = 20;
    public Color Color { get; set; } = Color.Black;
    public int MaxWidth { get; set; } = -1;
    public List<string> WrappedText => MaxWidth == -1 ? new List<string>() { Text } : null; // CtrlHelpers.GetWrappedTexts(Text, MaxWidth, FontSize);

    public int MeasureWidth()
    {
        return (int)TextManager.MeasureTextEx(Font,
                                         (from text in WrappedText
                                          orderby TextManager.MeasureTextEx(Font, text, FontSize, 1.0f).X 
                                          descending select text).ToList().FirstOrDefault(), 
                                         FontSize, 1.0f).X;
    }

    public int MeasureHeight() => WrappedText.Sum(text => (int)TextManager.MeasureTextEx(Font, text, FontSize, 1.0f).Y);

    public void Draw(int x, int y)
    {
        int height = 0;
        for (int row = 0; row < WrappedText.Count; row++)
        {
            int xDraw = x, yDraw = y;
            if (HorizontalAlignment == HorizontalAlignment.Center)
            {
                xDraw = x - (int)TextManager.MeasureTextEx(Font, WrappedText[row], FontSize, 1.0f).X / 2;
            }
            else if (HorizontalAlignment == HorizontalAlignment.Right)
            {
                xDraw = x - (int)TextManager.MeasureTextEx(Font, WrappedText[row], FontSize, 1.0f).X;
            }

            if (VerticalAlignment == VerticalAlignment.Center)
            {
                yDraw = y - (int)TextManager.MeasureTextEx(Font, WrappedText[row], FontSize, 1.0f).Y / 2;
            }
            else if (VerticalAlignment == VerticalAlignment.Bottom)
            {
                yDraw = y - (int)TextManager.MeasureTextEx(Font, WrappedText[row], FontSize, 1.0f).Y;
            }

            Graphics.DrawTextEx(Font, WrappedText[row], new Vector2(xDraw, yDraw + height), FontSize, 1.0f, Color);
            height += (int)TextManager.MeasureTextEx(Font, WrappedText[row], FontSize, 1.0f).Y;
        }
    }
}

public enum HorizontalAlignment
{
    Left,
    Right,
    Center
}

public enum VerticalAlignment
{
    Top,
    Bottom,
    Center
}

