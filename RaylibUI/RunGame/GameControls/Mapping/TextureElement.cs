using System.Numerics;
using Civ2engine.MapObjects;
using Raylib_cs;

namespace RaylibUI.RunGame.GameControls.Mapping;

public class TextureElement : IViewElement
{
    public TextureElement(Texture2D texture, Vector2 location, Tile tile, bool isTerrain = false)
    {
        Texture = texture;
        Location = location;
        Tile = tile;
        IsTerrain = isTerrain;
    }

    public Texture2D Texture { get; init; }
    
    public Vector2 Location { get; set; }
    
    public Tile Tile { get; set; }
    public bool IsTerrain { get; }

    public void Draw(Vector2 adjustedLocation)
    {
        Raylib.DrawTextureEx(Texture,
            adjustedLocation - new Vector2(0, Texture.height),
            0f,
            1f,
            Color.WHITE);
    }

    public IViewElement CloneForLocation(Vector2 newLocation)
    {
        return new TextureElement(Texture, newLocation, Tile, IsTerrain);
    }
}