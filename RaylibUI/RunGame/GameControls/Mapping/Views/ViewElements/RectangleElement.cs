using System.Numerics;
using Civ2engine.MapObjects;
using Raylib_CSharp.Colors;
using Raylib_CSharp.Rendering;

namespace RaylibUI.RunGame.GameControls.Mapping;

public class RectangleElement : IViewElement
{
    protected Color Color;
    protected Vector2 Size;

    public RectangleElement(Vector2 location, Tile tile, int width, int height, Color color, Vector2 offset ) : this(location,tile, new Vector2(width, height), color, offset)
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
    public bool IsShaded => false;
    public void Draw(Vector2 adjustedLocation, float scale = 1f, bool isShaded = false)
    {
        
        Graphics.DrawRectangleV(adjustedLocation + Offset * scale, Size * scale, Color);
    }

    public IViewElement CloneForLocation(Vector2 newLocation)
    {
        return new RectangleElement(newLocation, Tile, Size, Color, Offset);
    }
}