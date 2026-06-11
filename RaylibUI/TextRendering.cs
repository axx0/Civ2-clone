using System.Numerics;
using Raylib_CSharp.Colors;
using Raylib_CSharp.Fonts;
using Raylib_CSharp.Rendering;

namespace RaylibUI;

public static class TextRendering
{
    private const float UiTextScale = 1.55f;
    private const float MapTextScale = 1.45f;
    public const int MinimumUiFontSize = 20;
    public const int MinimumMapFontSize = 22;

    public static int LegibleUiFontSize(int fontSize)
    {
        return Math.Max((int)MathF.Round(fontSize * UiTextScale), MinimumUiFontSize);
    }

    public static int LegibleMapFontSize(int fontSize)
    {
        return Math.Max((int)MathF.Round(fontSize * MapTextScale), MinimumMapFontSize);
    }

    public static Vector2 Measure(Font font, string text, int fontSize, float spacing)
    {
        return TextManager.MeasureTextEx(font, text, fontSize, spacing);
    }

    public static void Draw(Font font, string text, Vector2 position, int fontSize, float spacing, Color color)
    {
        Graphics.DrawTextEx(font, text, PixelAlign(position), fontSize, spacing, color);
    }

    public static void DrawWithShadow(
        Font font,
        string text,
        Vector2 position,
        int fontSize,
        float spacing,
        Color front,
        Color shadow,
        Vector2 shadowOffset)
    {
        var alignedPosition = PixelAlign(position);
        if (shadowOffset != Vector2.Zero)
        {
            Graphics.DrawTextEx(font, text, alignedPosition + PixelAlign(shadowOffset), fontSize, spacing, shadow);
        }

        Graphics.DrawTextEx(font, text, alignedPosition, fontSize, spacing, front);
    }

    public static void DrawOutlined(
        Font font,
        string text,
        Vector2 position,
        int fontSize,
        float spacing,
        Color front,
        Color outline)
    {
        var alignedPosition = PixelAlign(position);
        Graphics.DrawTextEx(font, text, alignedPosition + new Vector2(-2, -2), fontSize, spacing, Color.White);
        Graphics.DrawTextEx(font, text, alignedPosition + new Vector2(2, -2), fontSize, spacing, Color.White);
        Graphics.DrawTextEx(font, text, alignedPosition + new Vector2(-2, 2), fontSize, spacing, Color.White);
        Graphics.DrawTextEx(font, text, alignedPosition + new Vector2(2, 2), fontSize, spacing, Color.White);
        Graphics.DrawTextEx(font, text, alignedPosition + new Vector2(-1, 0), fontSize, spacing, outline);
        Graphics.DrawTextEx(font, text, alignedPosition + new Vector2(1, 0), fontSize, spacing, outline);
        Graphics.DrawTextEx(font, text, alignedPosition + new Vector2(0, -1), fontSize, spacing, outline);
        Graphics.DrawTextEx(font, text, alignedPosition + new Vector2(0, 1), fontSize, spacing, outline);
        Graphics.DrawTextEx(font, text, alignedPosition, fontSize, spacing, front);
    }

    private static Vector2 PixelAlign(Vector2 position)
    {
        return new Vector2(MathF.Round(position.X), MathF.Round(position.Y));
    }
}
