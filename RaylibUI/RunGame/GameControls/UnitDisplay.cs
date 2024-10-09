using System.Numerics;
using Civ2engine.Units;
using Model;
using Model.Core;
using Raylib_CSharp.Transformations;
using RaylibUI.RunGame.GameControls.Mapping;

namespace RaylibUI.RunGame.GameControls;

public class UnitDisplay : BaseControl
{
    private Vector2 _previousLocation;
    private readonly List<IViewElement> _unitTextures;
    private float _scale;

    public UnitDisplay(IControlLayout controller, Unit unit, IGame game, Vector2 location,
        IUserInterface activeInterface,
        float scale = 1f) : base(controller)
    {
        _previousLocation = location;
        _scale = scale;
        _unitTextures = new List<IViewElement>();
        var size = ImageUtils.GetUnitTextures(unit, activeInterface, game, _unitTextures, location, true);
        Bounds = new Rectangle(location.X, location.Y, size.X * scale, size.Y * scale);
    }


    public override void Draw(bool pulse)
    {
        if (_previousLocation != Location)
        {
            var diff = Location - _previousLocation;
            foreach (var element in _unitTextures)
            {
                element.Location += diff;
            }
            _previousLocation = Location;
        }
        foreach (var element in _unitTextures)
        {
            element.Draw(element.Location, scale: _scale, isShaded: element.IsShaded);
        }

        //Graphics.DrawRectangleLines((int)Bounds.X, (int)Bounds.Y, (int)Bounds.Width, (int)Bounds.Height, Color.Red);

        base.Draw(pulse);
    }
}