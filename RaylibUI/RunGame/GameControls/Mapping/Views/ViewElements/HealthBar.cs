using System.Numerics;
using Civ2engine.MapObjects;
using Model;
using Raylib_cs;

namespace RaylibUI.RunGame.GameControls.Mapping;

public class HealthBar : RectangleElement
{
    public HealthBar(Vector2 location, Tile tile, int remainingHitPoints, int hitPointsBase, Vector2 offset, IUserInterface active) : 
        base(location, tile, offset)
    {
        var hpBarX = (int)Math.Floor((float)remainingHitPoints * active.GetHPbarSize().X / hitPointsBase);
        Color = active.GetHPbarColour(hpBarX);
        Size = new Vector2(hpBarX, active.GetHPbarSize().Y);
        BaseHitpoints = hitPointsBase;
    }

    public int BaseHitpoints { get; }
}