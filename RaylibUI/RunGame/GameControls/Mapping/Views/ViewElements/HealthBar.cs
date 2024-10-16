using System.Numerics;
using Civ2engine.MapObjects;
using Model;
using Model.ImageSets;

namespace RaylibUI.RunGame.GameControls.Mapping;

public class HealthBar : RectangleElement
{
    public HealthBar(Vector2 location, Tile tile, int remainingHitPoints, int hitPointsBase, Vector2 offset, UnitShield shield) : 
        base(location, tile, offset)
    {
        var hpBarX = (int)Math.Floor((float)remainingHitPoints * shield.HPbarSize.X / hitPointsBase);
        
        if (hpBarX <= shield.HPbarSizeForColours[0])
        {
            Color = shield.HPbarColours[0];
        }
        else if (hpBarX <= shield.HPbarSizeForColours[1])
        {
            Color = shield.HPbarColours[1];
        }
        else
        {
            Color = shield.HPbarColours[2];
        }
        
        Size = new Vector2(hpBarX, shield.HPbarSize.Y);
        BaseHitpoints = hitPointsBase;
    }

    public int BaseHitpoints { get; }
}