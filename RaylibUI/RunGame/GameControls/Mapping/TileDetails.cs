using Civ2engine;
using Civ2engine.Enums;
using Model.Images;
using Raylib_CSharp.Images;
using Raylib_CSharp.Textures;

namespace RaylibUI.RunGame.GameControls.Mapping;

public class TileDetails
{
    /// <summary>
    /// Composed tile graphic in CPU memory
    /// </summary>
    public Image Image { get; set; }

    private Texture2D? _texture;

    /// <summary>
    /// GPU copy of <see cref="Image"/>, uploaded the first time the tile is drawn on the map
    /// </summary>
    public Texture2D Texture
    {
        get
        {
            if (_texture == null)
            {
                var texture = Texture2D.LoadFromImage(Image);
                texture.SetFilter((TextureFilter)Settings.TextureFilter);
                _texture = texture;
            }
            return _texture.Value;
        }
    }

    public ForegroundImprovement? ForegroundElement { get; set; }

    /// <summary>
    /// Free the tile image and its texture (native/GPU memory)
    /// </summary>
    public void Unload()
    {
        Image.Unload();
        _texture?.Unload();
        _texture = null;
    }
}

public class ForegroundImprovement
{
    public IImageSource Image { get; set; }
}

public class UnitHidingImprovement : ForegroundImprovement
{
    public IImageSource UnitImage { get; set; }

    public UnitGas UnitDomain { get; set; }
}
