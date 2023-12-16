using System.Numerics;
using Civ2engine.MapObjects;
using Raylib_cs;

namespace RaylibUI.RunGame.GameControls.Mapping;

public class HealthBar : RectangleElement
{
    public HealthBar(Vector2 location, Tile tile, int remainingHitPoints, int hitPointsBase, Vector2 offset) : base(location, tile, offset)
    {
        var hpBarX = (int)Math.Floor((float)remainingHitPoints * 12 / hitPointsBase);
        Color = hpBarX switch
        {
            <= 3 => new Color(243, 0, 0, 255),
            <= 8 => new Color(255, 223, 79, 255),
            _ => new Color(87, 171, 39, 255)
        };
        Size = new Vector2(hpBarX, 3);
        BaseHitpoints = hitPointsBase;
    }

    public int BaseHitpoints { get; }
}