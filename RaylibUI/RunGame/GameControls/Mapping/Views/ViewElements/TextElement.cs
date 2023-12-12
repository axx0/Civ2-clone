using System.Numerics;
using Civ2engine.MapObjects;
using Raylib_cs;

namespace RaylibUI.RunGame.GameControls.Mapping;

public class TextElement : IViewElement
{
    private readonly string _text;
    private readonly Vector2 _size;


    public TextElement(string text, Vector2 loc, int height, Tile tile)
    {
        _text = text;
        Location = loc;
        Tile = tile;
        _size = Raylib.MeasureTextEx(Fonts.AlternativeFont, text, height, 1);
    }

    private TextElement(string text, Vector2 loc, Vector2 size, Tile tile)
    {
        _text = text;
        Location = loc;
        _size = size;
        Tile = tile;
    }

    public Vector2 Location { get; set; }
    public Tile Tile { get; set; }
    public bool IsTerrain => false;

    public void Draw(Vector2 adjustedLocation)
    {
        Raylib.DrawTextEx(Fonts.AlternativeFont, _text, adjustedLocation - new Vector2(_size.X / 2, 0), _size.Y, 1,
            Color.BLACK);
    }

    public IViewElement CloneForLocation(Vector2 newLocation)
    {
        return new TextElement(_text, newLocation, _size, Tile);
    }
}