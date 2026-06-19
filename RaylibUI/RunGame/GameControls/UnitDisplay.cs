using System.Numerics;
using Civ2engine.Units;
using Model;
using Model.Core;
using Model.Core.Units;
using RaylibUI.RunGame.GameControls.Mapping;
using RaylibUI.RunGame.GameControls.Mapping.Views.ViewElements;

namespace RaylibUI.RunGame.GameControls;

public class UnitDisplay : BaseControl
{
    private readonly List<IViewElement> _unitTextures;
    private readonly float _scale;
    private readonly Vector2 _size;

    public UnitDisplay(IControlLayout controller, IUnit unit, IGame game, Vector2 location,
        IUserInterface activeInterface, float scale = 1f, bool eventTransparent = false) : base(controller, eventTransparent)
    {
        _scale = scale;
        _unitTextures = [];

        // Keep child texture elements relative to this control. Passing the final UI
        // location into ImageUtils made high-resolution unit art much easier to draw
        // outside its parent when the parent later moved the control during layout.
        _size = ImageUtils.GetUnitTextures(unit, activeInterface, game, _unitTextures, Vector2.Zero, noStacking: true, useMapArt: true);
        Location = location;
    }

    public override int GetPreferredWidth() => (int)Math.Ceiling(_size.X * _scale);
    public override int GetPreferredHeight() => (int)Math.Ceiling(_size.Y * _scale);
    public override int Width => GetPreferredWidth();
    public override int Height => GetPreferredHeight();

    public override void Draw(bool pulse)
    {
        var origin = new Vector2(Bounds.X, Bounds.Y);
        foreach (var element in _unitTextures)
        {
            element.Draw(origin + element.Location, scale: GetSafeDrawScale(element), isShaded: element.IsShaded);
        }

        base.Draw(pulse);
    }

    private float GetSafeDrawScale(IViewElement element)
    {
        if (element is not TextureElement texture || _size.X <= 0 || _size.Y <= 0)
        {
            return _scale;
        }

        var renderScale = Math.Max(0.0001f, texture.RenderScale);
        var drawWidth = texture.Texture.Width * renderScale * _scale;
        var drawHeight = texture.Texture.Height * renderScale * _scale;
        var maxWidth = Math.Max(1f, _size.X * _scale);
        var maxHeight = Math.Max(1f, _size.Y * _scale);

        if (drawWidth <= maxWidth && drawHeight <= maxHeight)
        {
            return _scale;
        }

        var fitScale = Math.Min(maxWidth / Math.Max(1f, drawWidth), maxHeight / Math.Max(1f, drawHeight));
        return _scale * Math.Max(0.01f, fitScale);
    }
}
