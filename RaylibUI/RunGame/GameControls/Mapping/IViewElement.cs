using System.Numerics;
using Civ2engine.MapObjects;
using Raylib_cs;

namespace RaylibUI.RunGame.GameControls.Mapping;

public interface IViewElement
{
    Vector2 Location { get; set; }
    Tile Tile { get; set; }
    bool IsTerrain { get; }

    void Draw(Vector2 adjustedLocation);

    IViewElement CloneForLocation(Vector2 newLocation);
}

public class RectangleElement : IViewElement
{
    private readonly Color _color;
    private readonly Vector2 _size;

    public RectangleElement(Vector2 location, Tile tile, int width, int height, Color color ) : this(location,tile, new Vector2(width, height), color)
    {
        
    }

    private RectangleElement(Vector2 location, Tile tile, Vector2 size, Color color)
    {
        _size = size;
        _color = color;
        Location = location;
        Tile = tile;
    }

    public Vector2 Location { get; set; }
    public Tile Tile { get; set; }
    public bool IsTerrain => false;
    public void Draw(Vector2 adjustedLocation)
    {
        Raylib.DrawRectangleV(adjustedLocation, _size, _color);
    }

    public IViewElement CloneForLocation(Vector2 newLocation)
    {
        return new RectangleElement(newLocation, Tile, _size, _color);
    }
}