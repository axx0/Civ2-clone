using System.Numerics;
using Civ2engine.Units;
using Model;
using Raylib_cs;
using RaylibUI.RunGame.GameControls.Mapping;

namespace RaylibUI.RunGame.GameControls;

public class UnitDisplay : BaseControl
{
    private Vector2 _location;
    private List<IViewElement> _unitTextures;
    private float _scale;

    public UnitDisplay(IControlLayout controller, Unit unit, Vector2 location, IUserInterface activeInterface,
        float scale = 1f) : base(controller)
    {
        _location = location;
        _scale = scale;
        _unitTextures = new List<IViewElement>();
        var size = ImageUtils.GetUnitTextures(unit, activeInterface, _unitTextures, location, true);
        Bounds = new Rectangle(location.X, location.Y, size.X, size.Y);
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
        base.Draw(pulse);
    }
}