using Model;
using Model.Controls;
using RaylibUI.BasicTypes.Controls;

namespace RaylibUI.RunGame.GameControls.CityControls;

public class CityLabel : LabelControl
{
    private readonly CityWindow _cityWindow;
    private readonly CityLabelProperties _properties;

    public CityLabel(CityWindow controller, CityLabelProperties properties) : 
        base(controller, properties.Text, true, font: controller.MainWindow.ActiveInterface.Look.CityWindowFont, colorFront: properties.Color,
        colorShadow: properties.ColorShadow, horizontalAlignment: properties.Alignment, shadowOffset: properties.ShadowOffset)
    {
        _cityWindow = controller;
        _properties = properties;
    }

    public override void OnResize()
    {
        FontSize = _cityWindow.ScaledFontSize;
        _cityWindow.Place(this, _properties.Box);
    }
}
