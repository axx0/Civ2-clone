using System.Numerics;
using Raylib_CSharp.Colors;
using Raylib_CSharp.Fonts;
using Raylib_CSharp.Rendering;

namespace RaylibUI;

public static class TextRendering
{
    public static readonly Color StrongBlack = new(8, 8, 8, 255);
    public static readonly Color SoftLightShadow = new(230, 230, 230, 210);

    // Keep sizing conservative. The previous aggressive upscale made the text fill controls
    // and amplified filtering artifacts. Clean text beats oversized text here.
    private const float UiTextScale = 1.0f;
    private const float MapTextScale = 1.0f;
    public const int MinimumUiFontSize = 16;
    public const int MinimumMapFontSize = 16;

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

    public static void Draw(Font font, string text, Vector2 position, float fontSize, float spacing, Color color)
    {
        Draw(font, text, position, ToFontSize(fontSize), spacing, color);
    }

    // "Readable" now prioritizes sharpness. Small UI copy should render cleanly rather than
    // with a fuzzy drop shadow.
    public static void DrawReadable(Font font, string text, Vector2 position, int fontSize, float spacing, Color color)
    {
        Draw(font, text, position, fontSize, spacing, color);
    }

    public static void DrawReadable(Font font, string text, Vector2 position, float fontSize, float spacing, Color color)
    {
        DrawReadable(font, text, position, ToFontSize(fontSize), spacing, color);
    }

    public static void DrawReadable(Font font, string text, Vector2 position, int fontSize, float spacing)
    {
        DrawReadable(font, text, position, fontSize, spacing, StrongBlack);
    }

    public static void DrawReadable(Font font, string text, Vector2 position, float fontSize, float spacing)
    {
        DrawReadable(font, text, position, ToFontSize(fontSize), spacing, StrongBlack);
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

        // For the small UI labels that were looking grainy, the extra shadow pass hurts legibility
        // more than it helps. Only keep the shadow for genuinely larger text or when the caller
        // explicitly requests a meaningful offset.
        if (ShouldDrawShadow(fontSize, shadow, shadowOffset, front))
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

        // A tight 1px outline keeps titles readable without producing the old grainy halo.
        if (fontSize >= 18)
        {
            Graphics.DrawTextEx(font, text, alignedPosition + new Vector2(-1, 0), fontSize, spacing, outline);
            Graphics.DrawTextEx(font, text, alignedPosition + new Vector2(1, 0), fontSize, spacing, outline);
            Graphics.DrawTextEx(font, text, alignedPosition + new Vector2(0, -1), fontSize, spacing, outline);
            Graphics.DrawTextEx(font, text, alignedPosition + new Vector2(0, 1), fontSize, spacing, outline);
        }

        Graphics.DrawTextEx(font, text, alignedPosition, fontSize, spacing, front);
    }

    private static bool ShouldDrawShadow(int fontSize, Color shadow, Vector2 shadowOffset, Color front)
    {
        if (shadowOffset == Vector2.Zero)
        {
            return false;
        }

        if (shadow.A == 0)
        {
            return false;
        }

        if (SameColor(shadow, front))
        {
            return false;
        }

        // Skip the shadow for small text where it muddies the glyph edges.
        if (fontSize <= 22)
        {
            return false;
        }

        return true;
    }

    private static bool SameColor(Color a, Color b)
    {
        return a.R == b.R && a.G == b.G && a.B == b.B && a.A == b.A;
    }

    private static int ToFontSize(float fontSize)
    {
        return Math.Max(1, (int)MathF.Round(fontSize));
    }

    private static Vector2 PixelAlign(Vector2 position)
    {
        return new Vector2(MathF.Round(position.X), MathF.Round(position.Y));
    }
}
