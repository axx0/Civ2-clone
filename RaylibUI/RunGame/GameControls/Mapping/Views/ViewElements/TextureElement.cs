using System.Numerics;
using Civ2engine.MapObjects;
using Raylib_CSharp.Colors;
using Raylib_CSharp.Rendering;
using Raylib_CSharp.Textures;

namespace RaylibUI.RunGame.GameControls.Mapping.Views.ViewElements;

public class TextureElement : IViewElement
{
    public TextureElement(Texture2D texture, Vector2 location, Tile tile, bool isTerrain = false, bool isShaded = false, Vector2? offset = null)
    {
        Texture = texture;
        Location = location;
        Tile = tile;
        IsTerrain = isTerrain;
        IsShaded = isShaded;
        Offset = offset ?? Vector2.Zero;
    }

    /// <summary>
    /// Used for sub elements in a set of elements to scale their locations
    /// </summary>
    public Vector2 Offset { get; set; }

    public Texture2D Texture { get; init; }
    
    public Vector2 Location { get; set; }
    
    public Tile Tile { get; set; }
    public bool IsTerrain { get; }
    public bool IsShaded { get; }
    
    public void Draw(Vector2 adjustedLocation, float scale = 1f, bool isShaded = false)
    {
        if (isShaded)
        {
            Graphics.BeginShaderMode(Shaders.Grayscale);
        }

        Graphics.DrawTextureEx(Texture,
            adjustedLocation + Offset * scale,
            0f,
            scale,
            Color.White);

        if (isShaded)
        {
            Graphics.EndShaderMode();
        }
    }

    public IViewElement CloneForLocation(Vector2 newLocation)
    {
        return new TextureElement(Texture, newLocation, Tile, IsTerrain, offset: Offset);
    }
}