using System.Numerics;
using Civ2engine.MapObjects;

namespace RaylibUI.RunGame.GameControls.Mapping;

public interface IViewElement
{
    Vector2 Location { get; set; }
    Tile Tile { get; set; }
    bool IsTerrain { get; }

    void Draw(Vector2 adjustedLocation, float scale = 1f);

    IViewElement CloneForLocation(Vector2 newLocation);
}