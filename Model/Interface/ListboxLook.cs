using Raylib_CSharp.Colors;
using Raylib_CSharp.Fonts;
using System.Numerics;

namespace Model.Interface;

public class ListboxLooks
{
    public Color BoxBackgroundColor { get; set; } = Color.Blank;
    public Color BoxLineColor { get; set; } = Color.Blank;

    public Font Font { get; set; } = Fonts.Tnr;
    public int FontSize { get; set; } = 12;
    public Color TextColorFront { get; set; } = Color.Black;
    public Color TextColorShadow { get; set; } = Color.Black;
    public Vector2 TextShadowOffset { get; set; } = new(1, 1);

    public Font SelectedTextFont { get; set; } = Fonts.Tnr;
    public Color SelectedTextBackgroundColor { get; set; } = Color.Gray;
    public Color SelectedTextColorFront { get; set; } = Color.White;
    public Color SelectedTextColorShadow { get; set; } = Color.Black;
    public Vector2 SelectedTextShadowOffset { get; set; } = new(1, 1);
}