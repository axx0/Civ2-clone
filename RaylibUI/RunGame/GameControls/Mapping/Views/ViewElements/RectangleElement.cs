using System.Numerics;
using Civ2engine.MapObjects;
using Raylib_cs;

namespace RaylibUI.RunGame.GameControls.Mapping;

public class RectangleElement : IViewElement
{
    protected Color Color;
    protected Vector2 Size;

    public RectangleElement(Vector2 location, Tile tile, int width, int height, Color color ) : this(location,tile, new Vector2(width, height), color)
    {
        
    }

    protected RectangleElement(Vector2 location, Tile tile)
    {
        Location = location;
        Tile = tile;
    }

    private RectangleElement(Vector2 location, Tile tile, Vector2 size, Color color)
    {
        Size = size;
        Color = color;
        Location = location;
        Tile = tile;
    }

    public Vector2 Location { get; set; }
    public Tile Tile { get; set; }
    public bool IsTerrain => false;
    public void Draw(Vector2 adjustedLocation)
    {
        Raylib.DrawRectangleV(adjustedLocation, Size, Color);
    }

    public IViewElement CloneForLocation(Vector2 newLocation)
    {
        return new RectangleElement(newLocation, Tile, Size, Color);
    }
}