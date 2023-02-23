using Raylib_cs;
using System.Numerics;

namespace RaylibUI.Controls;

public class FormattedText
{
    public string Text { get; set; }
    public HorizontalAlignment HorizontalAlignment { get; set; } = HorizontalAlignment.Left;
    public VerticalAlignment VerticalAlignment { get; set; } = VerticalAlignment.Top;
    public Font Font { get; set; } = Raylib.GetFontDefault();
    public int FontSize { get; set; } = 20;
    public Color Color { get; set; } = Color.BLACK;
    public int MaxWidth { get; set; } = -1;
    public List<string> WrappedText => MaxWidth == -1 ? new List<string>() { Text } : CtrlHelpers.GetWrappedTexts(Text, MaxWidth, FontSize);

    public int MeasureWidth()
    {
        return (int)Raylib.MeasureTextEx(Font,
                                         (from text in WrappedText
                                          orderby Raylib.MeasureTextEx(Font, text, FontSize, 1.0f).X 
                                          descending select text).ToList().FirstOrDefault(), 
                                         FontSize, 1.0f).X;
    }

    public int MeasureHeight() => WrappedText.Sum(text => (int)Raylib.MeasureTextEx(Font, text, FontSize, 1.0f).Y);

    public void Draw(int x, int y)
    {
        int height = 0;
        for (int row = 0; row < WrappedText.Count; row++)
        {
            Raylib.DrawTextEx(Font, WrappedText[row], new Vector2(x, y + height), FontSize, 1.0f, Color);
            height += (int)Raylib.MeasureTextEx(Font, WrappedText[row], FontSize, 1.0f).Y;
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

