using Raylib_CSharp.Colors;
using Raylib_CSharp.Fonts;
using Raylib_CSharp.Transformations;
using RaylibUI.BasicTypes.Controls;
using System.Numerics;

namespace RaylibUI.RunGame.GameControls.CityControls;

public class CityLabel : LabelControl
{
    private readonly CityWindow _cityWindow;
    private readonly int _baseFontSize;

    public CityLabel(CityWindow controller, string text, Font font, int fontSize, Color colorFront, Color colorShadow, TextAlignment alignment = TextAlignment.Center) : base(controller, text, true, font: font, fontSize: fontSize, colorFront: colorFront, colorShadow: colorShadow, alignment: alignment, shadowOffset: new Vector2(1, 1))
    {
        _cityWindow = controller;
        _baseFontSize = fontSize;
    }

    public override void OnResize()
    {
        FontSize = _baseFontSize + (int)(16 * (_cityWindow.Scale - 1));
    }
}
