using System.Numerics;
using Civ2engine.MapObjects;
using Raylib_cs;

namespace RaylibUI.RunGame.GameControls.Mapping;

public class TextElement : IViewElement
{
    private readonly string _text;
    private readonly int _height;


    public TextElement(string text, Vector2 loc, int height, Tile tile, Vector2 offset)
    {
        _text = text;
        _height = height;
        Location = loc;
        Tile = tile;
        Offset = offset;
    }

    public Vector2 Offset { get; set; }

    public Vector2 Location { get; set; }
    public Tile Tile { get; set; }
    public bool IsTerrain => false;

    public void Draw(Vector2 adjustedLocation, float scale = 1f)
    {
        var loc = adjustedLocation - Offset + Offset * scale;
        
        var size = Raylib.MeasureTextEx(Fonts.AlternativeFont, _text, _height * scale, 1);
        Raylib.DrawTextEx(Fonts.AlternativeFont, _text, loc - new Vector2(size.X / 2, 0), _height * scale, 1,
            Color.BLACK);
    }

    public IViewElement CloneForLocation(Vector2 newLocation)
    {
        return new TextElement(_text, newLocation, _height, Tile, Offset);
    }
}