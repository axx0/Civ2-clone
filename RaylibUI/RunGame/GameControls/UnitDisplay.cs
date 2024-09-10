using System.Numerics;
using Civ2engine.Units;
using Model;
using Raylib_cs;
using RaylibUI.RunGame.GameControls.Mapping;

namespace RaylibUI.RunGame.GameControls;

public class UnitDisplay : BaseControl
{
    private List<IViewElement> _unitTextures;
    private float _scale;

    public UnitDisplay(IControlLayout controller, Unit unit, Vector2 location, IUserInterface activeInterface,
        float scale = 1f) : base(controller)
    {
        _location = location;
        _scale = scale;
        _unitTextures = new List<IViewElement>();
        var size = ImageUtils.GetUnitTextures(unit, activeInterface, _unitTextures, location, true);
        Bounds = new Rectangle(location.X, location.Y, size.X * scale, size.Y * scale);
    }

    private Vector2 _location;
    public Vector2 Location
    {
        get { return _location; }
        set { _location = value; }
    }

    public override void Draw(bool pulse)
    {
        if (_location != Location)
        {
            var diff = Location - _location;
            foreach (var element in _unitTextures)
            {
                element.Location += diff;
            }
            _location = Location;
        }
        foreach (var element in _unitTextures)
        {
            element.Draw(element.Location, scale: _scale, isShaded: element.IsShaded);
        }

        //Raylib.DrawRectangleLines((int)Bounds.X, (int)Bounds.Y, (int)Bounds.Width, (int)Bounds.Height, Color.Red);

        base.Draw(pulse);
    }
}