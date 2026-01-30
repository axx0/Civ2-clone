using Model;
using Model.Controls;
using RaylibUI.Controls;

namespace RaylibUI.RunGame.GameControls.CityControls;

public class CityButton : Button
{
    private readonly CityWindow _cityWindow;
    private readonly IUserInterface _active;
    private readonly CityButtonProperties _properties;

    public CityButton(CityWindow controller, string key) : 
        base(controller, controller.CityWindowProps.Buttons[key].Text, controller.MainWindow.ActiveInterface.Look.CityWindowFont)
    {
        _cityWindow = controller;
        _active = controller.MainWindow.ActiveInterface;
        _properties = controller.CityWindowProps.Buttons[key];
    }

    public override void OnResize()
    {
        Scale = _cityWindow.Scale;

        FontSize = _active.Look.CityWindowFontSize + (int)(12 * (Scale - 1));

        if (Parent == _cityWindow)
        {
            Location = new(_cityWindow.LayoutPadding.Left + _properties.Box.X * Scale,
                    _cityWindow.LayoutPadding.Top + _properties.Box.Y * Scale);
        }
        else
        {
            Location = new(_properties.Box.X * Scale, _properties.Box.Y * Scale);
        }
        Width = (int)(_properties.Box.Width * Scale);
        Height = (int)(_properties.Box.Height * Scale);
    }
}
