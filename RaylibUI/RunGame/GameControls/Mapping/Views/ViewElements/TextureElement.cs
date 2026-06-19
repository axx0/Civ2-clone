using System.Numerics;
using Civ2engine.MapObjects;
using Model.Core.Mapping;
using Raylib_CSharp.Colors;
using Raylib_CSharp.Rendering;
using Raylib_CSharp.Textures;

namespace RaylibUI.RunGame.GameControls.Mapping.Views.ViewElements;

public class TextureElement : IViewElement
{
    public TextureElement(Texture2D texture, Vector2 location, Tile tile, bool isTerrain = false, bool isShaded = false,
        Vector2? offset = null, float renderScale = 1f, Vector2? maxDrawSize = null)
    {
        Texture = texture;
        Location = location;
        Tile = tile;
        IsTerrain = isTerrain;
        IsShaded = isShaded;
        Offset = offset ?? Vector2.Zero;
        RenderScale = renderScale;
        MaxDrawSize = maxDrawSize;
    }

    /// <summary>
    /// Used for sub elements in a set of elements to scale their locations.
    /// </summary>
    public Vector2 Offset { get; set; }

    /// <summary>
    /// Scale applied before the current map/UI zoom scale. Most textures use 1.
    /// High-resolution unit art uses a smaller value so it keeps Civ2's logical
    /// unit footprint at normal zoom while still rendering from the high-res
    /// source texture when zoomed in.
    /// </summary>
    public float RenderScale { get; }

    /// <summary>
    /// Optional logical-size clamp. This is primarily used for high-resolution
    /// FOSS unit art: the source texture can be 1024px, but it must still occupy
    /// the same logical Civ2 unit footprint on the map and in city/status UI.
    /// </summary>
    public Vector2? MaxDrawSize { get; }

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

        var drawScale = scale * RenderScale;
        if (MaxDrawSize is { } maxDrawSize && Texture.Width > 0 && Texture.Height > 0)
        {
            var maxWidth = Math.Max(1f, maxDrawSize.X * scale);
            var maxHeight = Math.Max(1f, maxDrawSize.Y * scale);
            drawScale = Math.Min(drawScale, Math.Min(maxWidth / Texture.Width, maxHeight / Texture.Height));
        }

        Graphics.DrawTextureEx(Texture,
            adjustedLocation + Offset * scale,
            0f,
            Math.Max(0.01f, drawScale),
            Color.White);

        if (isShaded)
        {
            Graphics.EndShaderMode();
        }
    }

    public IViewElement CloneForLocation(Vector2 newLocation)
    {
        return new TextureElement(Texture, newLocation, Tile, IsTerrain, IsShaded, Offset, RenderScale, MaxDrawSize);
    }
}
