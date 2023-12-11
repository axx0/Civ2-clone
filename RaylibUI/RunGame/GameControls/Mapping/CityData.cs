using System.Numerics;
using Civ2engine.MapObjects;
using Model.ImageSets;
using Raylib_cs;

namespace RaylibUI.RunGame.GameControls.Mapping;

public class CityData : TextureElement
{
    public CityData(PlayerColour color, string name, Texture2D texture, Vector2 location, Tile tile) : base(texture, location,tile)
    {
        Color = color;
        Name = name;
    }

    public string Name { get; }
    public PlayerColour Color { get; }
}