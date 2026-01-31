using System.Numerics;
using Civ2engine.Units;
using Model;
using Model.Core;
using Model.Core.Units;
using Raylib_CSharp.Colors;
using Raylib_CSharp.Rendering;
using Raylib_CSharp.Transformations;
using RaylibUI.RunGame.GameControls.Mapping;

namespace RaylibUI.RunGame.GameControls;

public class UnitDisplay : BaseControl
{
    private Vector2 _previousLocation;
    private readonly List<IViewElement> _unitTextures;
    private float _scale;
    private Vector2 _size;

    public UnitDisplay(IControlLayout controller, IUnit unit, IGame game, Vector2 location,
        IUserInterface activeInterface, float scale = 1f, bool eventTransparent = false) : base(controller, eventTransparent)
    {
        _previousLocation = location;
        _scale = scale;
        _unitTextures = new List<IViewElement>();
        _size = ImageUtils.GetUnitTextures(unit, activeInterface, game, _unitTextures, location, true);
        Location = location;
    }

    public override int GetPreferredWidth() => (int)(_size.X * _scale);
    public override int GetPreferredHeight() => (int)(_size.Y * _scale);
    public override int Width => GetPreferredWidth();
    public override int Height => GetPreferredHeight();

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
            var parentLoc = new Vector2(Parent.Bounds.X, Parent.Bounds.Y);
            element.Draw(parentLoc + element.Location, scale: _scale, isShaded: element.IsShaded);
        }

        //Graphics.DrawRectangleLinesEx(Bounds, 1f, Color.Red);

        base.Draw(pulse);
    }
}