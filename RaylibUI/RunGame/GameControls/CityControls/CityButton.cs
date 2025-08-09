using Model.Images;
using Raylib_CSharp.Colors;
using Raylib_CSharp.Fonts;
using Raylib_CSharp.Transformations;
using RaylibUI.BasicTypes.Controls;
using RaylibUI.Controls;
using System.Numerics;

namespace RaylibUI.RunGame.GameControls.CityControls;

public class CityButton : Button
{
    private readonly CityWindow _cityWindow;
    private readonly int _baseFontSize;

    public CityButton(CityWindow controller, string text, Font? font = null, int fontSize = 12, IImageSource? backgroundImage = null) : base(controller, text, font, fontSize, backgroundImage: backgroundImage)
    {
        _cityWindow = controller;
        _baseFontSize = fontSize;
    }

    /// <summary>
    /// Absolute position without padding
    /// </summary>
    public Rectangle? AbsolutePositionNoPadding { get; set; }

    public override void OnResize()
    {
        Scale = _cityWindow.Scale;

        FontSize = _baseFontSize + (int)(16 * (Scale - 1));

        if (AbsolutePosition.HasValue)
        {
            var absolutePosition = AbsolutePosition.Value.ScaleAll(Scale);
            Bounds = new Rectangle(Controller.Location.X + Controller.LayoutPadding.Left + absolutePosition.X,
                Controller.Location.Y + Controller.LayoutPadding.Top + absolutePosition.Y, absolutePosition.Width, absolutePosition.Height);
        }
        else if (AbsolutePositionNoPadding.HasValue)
        {
            var absolutePosition = AbsolutePositionNoPadding.Value.ScaleAll(Scale);
            Bounds = new Rectangle(Controller.Location.X + absolutePosition.X,
                Controller.Location.Y + absolutePosition.Y, absolutePosition.Width, absolutePosition.Height);
        }
    }
}
