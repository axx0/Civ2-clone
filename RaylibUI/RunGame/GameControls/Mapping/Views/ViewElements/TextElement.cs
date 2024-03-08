using System.Numerics;
using Civ2engine.MapObjects;
using Raylib_cs;
using Model.Interface;

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
    public bool IsShaded => false;

    public void Draw(Vector2 adjustedLocation, float scale = 1f, bool isShaded = false)
    {
        var loc = adjustedLocation - Offset + Offset * scale;
        
        var size = Raylib.MeasureTextEx(Fonts.Arial, _text, _height * scale, 1);
        Raylib.DrawTextEx(Fonts.Arial, _text, loc - new Vector2(size.X / 2, 0), _height * scale, 1,
            Color.Black);
    }

    public IViewElement CloneForLocation(Vector2 newLocation)
    {
        return new TextElement(_text, newLocation, _height, Tile, Offset);
    }
}