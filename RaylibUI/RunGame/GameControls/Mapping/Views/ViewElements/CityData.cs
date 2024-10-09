using System.Numerics;
using Civ2engine.MapObjects;
using Model.ImageSets;
using Raylib_CSharp.Textures;
using RaylibUI.RunGame.GameControls.Mapping.Views.ViewElements;

namespace RaylibUI.RunGame.GameControls.Mapping;

public class CityData : TextureElement
{
    public CityData(PlayerColour color, string name, int size, Vector2 sizeRectLoc, Texture2D texture, Vector2 location, Tile tile) : base(texture, location,tile)
    {
        Color = color;
        Name = name;
        Size = size;
        SizeRectLoc= sizeRectLoc;
    }

    public string Name { get; }
    public PlayerColour Color { get; }
    public int Size { get; }
    public Vector2 SizeRectLoc { get; }
}