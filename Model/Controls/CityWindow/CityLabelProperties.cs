using Raylib_CSharp.Colors;
using Raylib_CSharp.Transformations;
using System.Numerics;

namespace Model.Controls;

public class CityLabelProperties
{
    public string Text { get; init; }
    public Rectangle Box { get; init; }
    public Color Color { get; init; }
    public Color ColorShadow { get; init; }
    public Vector2 ShadowOffset { get; init; }
    public HorizontalAlignment Alignment { get; init; }

    public CityLabelProperties(string text, Rectangle box, Color color, Color colorShadow, HorizontalAlignment alignment = HorizontalAlignment.Center, Vector2? shadowOffset = null)
    {
        Text = text;
        Box = box;
        Color = color;
        ColorShadow = colorShadow;
        Alignment = alignment;
        ShadowOffset = shadowOffset ?? new(1, 1);
    }
}
