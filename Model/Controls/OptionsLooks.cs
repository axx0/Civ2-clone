using Model.Interface;
using Raylib_CSharp.Colors;
using Raylib_CSharp.Fonts;
using System.Numerics;

namespace Model.Controls;

public class OptionsLooks
{
    public Font Font { get; set; } = Fonts.Tnr;
    public int FontSize { get; set; } = 12;
    public Color TextColorFront { get; set; } = Color.Black;
    public Color TextColorShadow { get; set; } = Color.Black;
    public Vector2 TextShadowOffset { get; set; } = new(1, 1);
    public float IconScale { get; set; } = 1.0f;
}