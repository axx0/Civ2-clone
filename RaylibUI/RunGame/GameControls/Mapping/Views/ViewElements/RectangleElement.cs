using System.Numerics;
using Civ2engine.MapObjects;
using Raylib_cs;

namespace RaylibUI.RunGame.GameControls.Mapping;

public class RectangleElement : IViewElement
{
    protected Color Color;
    protected Vector2 Size;

    public RectangleElement(Vector2 location, Tile tile, int Width, int Height, Color color, Vector2 offset ) : this(location,tile, new Vector2(Width, Height), color, offset)
    {
        
    }

    protected RectangleElement(Vector2 location, Tile tile, Vector2 offset)
    {
        Location = location;
        Tile = tile;
        Offset = offset;
    }

    private RectangleElement(Vector2 location, Tile tile, Vector2 size, Color color, Vector2 offset)
    {
        Size = size;
        Color = color;
        Location = location;
        Tile = tile;
        Offset = offset;
    }

    public Vector2 Location { get; set; }
    public Tile Tile { get; set; }
    public Vector2 Offset { get; }
    public bool IsTerrain => false;
    public void Draw(Vector2 adjustedLocation, float scale = 1f)
    {
        
        Raylib.DrawRectangleV( adjustedLocation - Offset + Offset * scale, Size * scale, Color);
    }

    public IViewElement CloneForLocation(Vector2 newLocation)
    {
        return new RectangleElement(newLocation, Tile, Size, Color, Offset);
    }
}