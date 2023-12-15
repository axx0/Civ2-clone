using System.Numerics;
using Civ2engine.Units;
using Model;
using Raylib_cs;
using RaylibUI.RunGame.GameControls.Mapping;

namespace RaylibUI.RunGame.GameControls;

public class UnitDisplay : BaseControl
{
    private readonly Unit _unit;
    private List<IViewElement> _unitTextures;

    public UnitDisplay(IControlLayout controller, Unit unit, Vector2 location, IUserInterface activeInterface) : base(controller)
    {
        _unit = unit;
        _unitTextures = new List<IViewElement>();
        var size = ImageUtils.GetUnitTextures(unit, activeInterface, _unitTextures, location, true);
        var heightVec = new Vector2(0, size.Y);
        _unitTextures.ForEach(t=>t.Location = t.Location + heightVec);
        Bounds = new Rectangle(location.X, location.Y, size.X, size.Y);
    }

    public override void Draw(bool pulse)
    {
        foreach (var element in _unitTextures)
        {
            element.Draw(element.Location);
        }
        base.Draw(pulse);
    }
}