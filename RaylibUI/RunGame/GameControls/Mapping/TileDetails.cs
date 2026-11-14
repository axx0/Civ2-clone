using Civ2engine.Enums;
using Model.Images;
using Raylib_CSharp.Images;
using Raylib_CSharp.Textures;
using Raylib_CSharp.Transformations;

namespace RaylibUI.RunGame.GameControls.Mapping;

public class TileDetails
{
    /// <summary>
    /// Composed tile graphic in CPU memory, freed once it is copied into the tile atlas
    /// </summary>
    public Image Image { get; set; }

    /// <summary>
    /// Atlas the tile goes into the first time it is drawn (set by the tile cache)
    /// </summary>
    internal TileAtlas? Atlas { get; set; }

    private int _slot = -1;
    private Texture2D _page;
    private Rectangle _source;

    /// <summary>
    /// Atlas page holding this tile; use together with <see cref="Source"/>
    /// </summary>
    public Texture2D Texture
    {
        get
        {
            Upload();
            return _page;
        }
    }

    /// <summary>
    /// The tile's rectangle in <see cref="Texture"/>
    /// </summary>
    public Rectangle Source
    {
        get
        {
            Upload();
            return _source;
        }
    }

    /// <summary>
    /// Copy the image into the atlas (on first use, so it happens while drawing) and free the CPU copy
    /// </summary>
    private void Upload()
    {
        if (_slot != -1 || Atlas == null || Image.Data == IntPtr.Zero) return;

        var image = Image;
        (_slot, _page, _source) = Atlas.Add(ref image);
        image.Unload();
        Image = default;
    }

    public ForegroundImprovement? ForegroundElement { get; set; }

    /// <summary>
    /// Free the tile image or its atlas slot
    /// </summary>
    public void Unload()
    {
        if (_slot != -1)
        {
            Atlas?.Free(_slot);
            _slot = -1;
        }
        else if (Image.Data != IntPtr.Zero)
        {
            Image.Unload();
            Image = default;
        }

        // An unloaded tile must not take a slot again
        Atlas = null;
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
