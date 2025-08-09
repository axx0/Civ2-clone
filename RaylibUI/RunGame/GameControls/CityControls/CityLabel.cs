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

        if (AbsolutePosition.HasValue)
        {
            var absolutePosition = AbsolutePosition.Value.ScaleAll(_cityWindow.Scale);
            Bounds = new Rectangle(Controller.Location.X + Controller.LayoutPadding.Left + absolutePosition.X,
                Controller.Location.Y + Controller.LayoutPadding.Top + absolutePosition.Y, absolutePosition.Width, absolutePosition.Height);
        }
    }
}
