using Model;
using Model.Controls;
using RaylibUI.BasicTypes.Controls;

namespace RaylibUI.RunGame.GameControls.CityControls;

public class CityLabel : LabelControl
{
    private readonly CityWindow _cityWindow;
    private readonly IUserInterface _active;
    private readonly CityLabelProperties _properties;

    public CityLabel(CityWindow controller, CityLabelProperties properties) : 
        base(controller, properties.Text, true, font: controller.MainWindow.ActiveInterface.Look.CityWindowFont, colorFront: properties.Color,
        colorShadow: properties.ColorShadow, horizontalAlignment: properties.Alignment, shadowOffset: properties.ShadowOffset)
    {
        _cityWindow = controller;
        _active = _cityWindow.MainWindow.ActiveInterface;
        _properties = properties;
    }

    public override void OnResize()
    {
        var scale = _cityWindow.Scale;
        FontSize = _active.Look.CityWindowFontSize + (int)(12 * (scale - 1));
        if (Parent == _cityWindow)
        {
            Location = new(_cityWindow.LayoutPadding.Left + _properties.Box.X * scale,
                _cityWindow.LayoutPadding.Top + _properties.Box.Y * scale);
        }
        else
        {
            Location = new(_properties.Box.X * scale, _properties.Box.Y * scale);
        }
        Width = (int)(_properties.Box.Width * scale);
        Height = (int)(_properties.Box.Height * scale);
    }
}
