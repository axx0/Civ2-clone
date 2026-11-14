using Model;
using Model.Controls;
using RaylibUI.Controls;

namespace RaylibUI.RunGame.GameControls.CityControls;

public class CityButton : Button
{
    private readonly CityWindow _cityWindow;
    private readonly CityButtonProperties _properties;

    public CityButton(CityWindow controller, string key) : 
        base(controller, controller.CityWindowProps.Buttons[key].Text, controller.MainWindow.ActiveInterface.Look.CityWindowFont)
    {
        _cityWindow = controller;
        _properties = controller.CityWindowProps.Buttons[key];
    }

    public override void OnResize()
    {
        Scale = _cityWindow.Scale;
        FontSize = _cityWindow.ScaledFontSize;
        _cityWindow.Place(this, _properties.Box);
    }
}
