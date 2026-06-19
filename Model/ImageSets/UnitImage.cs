using System.Numerics;
using Model.Images;

namespace Model.ImageSets;

public class UnitImage
{
    public IImageSource Image { get; set; } = null!;

    /// <summary>Classic Civ2-sized sprite used by city/status/dialog UI.</summary>
    public IImageSource UiImage => Image;

    /// <summary>Optional high-resolution sprite used only by zoomable map rendering.</summary>
    public IImageSource? MapImage { get; set; }

    public Vector2 FlagLoc { get; set; }

    /// <summary>
    /// Size the unit occupies in Civ2's logical map coordinates.
    /// High-resolution unit textures are drawn into this box instead of at
    /// their full source pixel size, so they stay the same size at normal zoom
    /// but keep their detail when the map is zoomed in.
    /// </summary>
    public Vector2 LogicalSize { get; set; }

    /// <summary>
    /// Offset, in logical unit pixels, used to align a source texture inside
    /// <see cref="LogicalSize"/>. This lets square FOSS unit art sit inside the
    /// original 64x48/64x64 unit footprint without changing map layout.
    /// </summary>
    public Vector2 DrawOffset { get; set; }

    /// <summary>
    /// Scale applied to the source texture before the current map/UI zoom scale.
    /// Original Civ2 sprites use 1; 1024px FOSS art uses a small value that fits
    /// the art into <see cref="LogicalSize"/>.
    /// </summary>
    public float DrawScale { get; set; } = 1f;
}
