﻿using Raylib_cs;
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
        return (int)Raylib.MeasureTextEx(Font,
                                         (from text in WrappedText
                                          orderby Raylib.MeasureTextEx(Font, text, FontSize, 1.0f).X 
                                          descending select text).ToList().FirstOrDefault(), 
                                         FontSize, 1.0f).X;
    }

    public int MeasureHeight() => WrappedText.Sum(text => (int)Raylib.MeasureTextEx(Font, text, FontSize, 1.0f).Y);

    public void Draw(int X, int Y)
    {
        int Height = 0;
        for (int row = 0; row < WrappedText.Count; row++)
        {
            int xDraw = X, yDraw = Y;
            if (HorizontalAlignment == HorizontalAlignment.Center)
            {
                xDraw = X - (int)Raylib.MeasureTextEx(Font, WrappedText[row], FontSize, 1.0f).X / 2;
            }
            else if (HorizontalAlignment == HorizontalAlignment.Right)
            {
                xDraw = X - (int)Raylib.MeasureTextEx(Font, WrappedText[row], FontSize, 1.0f).X;
            }

            if (VerticalAlignment == VerticalAlignment.Center)
            {
                yDraw = Y - (int)Raylib.MeasureTextEx(Font, WrappedText[row], FontSize, 1.0f).Y / 2;
            }
            else if (VerticalAlignment == VerticalAlignment.Bottom)
            {
                yDraw = Y - (int)Raylib.MeasureTextEx(Font, WrappedText[row], FontSize, 1.0f).Y;
            }

            Raylib.DrawTextEx(Font, WrappedText[row], new Vector2(xDraw, yDraw + Height), FontSize, 1.0f, Color);
            Height += (int)Raylib.MeasureTextEx(Font, WrappedText[row], FontSize, 1.0f).Y;
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

