using Model;
using Raylib_CSharp.Colors;
using Raylib_CSharp.Rendering;
using System.Numerics;

namespace RaylibUI.RunGame.GameControls.CityControls;

public class UnitSupportBox : BaseControl
{
    private readonly CityWindow _cityWindow;
    private readonly int _numberOfRows;
    private readonly int _numberOfColumns;
    private readonly IUserInterface _active;

    public UnitSupportBox(CityWindow cityWindow) : base(cityWindow)
    {
        _cityWindow = cityWindow;
        _active = _cityWindow.CurrentGameScreen.Main.ActiveInterface;
        _numberOfRows = cityWindow.CityWindowProps.UnitSupport.Rows;
        _numberOfColumns = cityWindow.CityWindowProps.UnitSupport.Columns;
    }

    public override void OnResize()
    {
        Controls = [];
        
        var pos = _cityWindow.CityWindowProps.UnitSupport.Position;
        Location = new(_cityWindow.LayoutPadding.Left + pos.X * _cityWindow.Scale,
            _cityWindow.LayoutPadding.Top + pos.Y * _cityWindow.Scale);
        Width = (int)(pos.Width * _cityWindow.Scale);
        Height = (int)(pos.Height * _cityWindow.Scale);

        base.OnResize();

        Recalculate();
    }

    private void Recalculate()
    {
        var units = _cityWindow.City.SupportedUnits;
        if (units.Count > 0)
        {
            var unitRec = _active.UnitImages.UnitRectangle;
            var requireHeight = (Height - 4) / (float)_numberOfRows;
            var scale = requireHeight / unitRec.Height;
            var requiredWidth = (Width - 15) / _numberOfColumns;
            var row = 0;
            var initialX = 2;
            var location = new Vector2(initialX, 0);
            var rowLimit = Width;
            for (int i = 0; i < units.Count && row < _numberOfRows; i++)
            {
                Controls.Add(new UnitDisplay(_cityWindow, units[i], _cityWindow.CurrentGameScreen.Game, location, _active, scale ));
                location = location with { X = location.X + requiredWidth };
                if (location.X + requiredWidth > rowLimit)
                {
                    row++;
                    location = new Vector2(initialX, location.Y + requireHeight);
                }
            }
        }
    }
}