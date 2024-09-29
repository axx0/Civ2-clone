using System.Numerics;
using Model;

namespace RaylibUI.RunGame.GameControls.CityControls;

public class UnitSupportBox : BaseControl
{
    private readonly CityWindow _cityWindow;
    private readonly int _numberOfRows;
    private readonly int _numberOfColumns;

    public UnitSupportBox(CityWindow cityWindow, UnitSupport unitSupport) : base(cityWindow)
    {
        _cityWindow = cityWindow;
        AbsolutePosition = unitSupport.Position;
        _numberOfRows = unitSupport.Rows;
        _numberOfColumns = unitSupport.Columns;
    }

    public override void OnResize()
    {
        base.OnResize();
        Recalculate();
    }
    private void Recalculate()
    {
        var units = _cityWindow.City.SupportedUnits;
        if(units.Count > 0)
        {
            var activeInterface = _cityWindow.CurrentGameScreen.Main.ActiveInterface;
            var unitRec = activeInterface.UnitImages.UnitRectangle; 
            var requireHeight = (Height -4) / (float)_numberOfRows;
            var scale = requireHeight / unitRec.Height;
            var requiredWidth = (Bounds.Width - 15) / _numberOfColumns;
            var row = 0;
            var initialX = Bounds.X +2;
            var location = new Vector2(initialX, Bounds.Y);
            var rowLimit = Bounds.X + Bounds.Width;
            var children = new List<IControl>();
            for (int i = 0; i < units.Count && row < _numberOfRows; i++)
            {
                children.Add(new UnitDisplay(_cityWindow, units[i], _cityWindow.CurrentGameScreen.Game, location,activeInterface, scale ));
                location = location with { X = location.X + requiredWidth };
                if (location.X + requiredWidth > rowLimit)
                {
                    row++;
                    location = new Vector2(initialX, location.Y + requireHeight);
                }
            }

            Children = children;
        }
    }

    public override void Draw(bool pulse)
    {
        base.Draw(pulse);
        if (Children != null)
        {
            foreach (var control in Children)
            {
                control.Draw(pulse);
            }
        }
    }
}