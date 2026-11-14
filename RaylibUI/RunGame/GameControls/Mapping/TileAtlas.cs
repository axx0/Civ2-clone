using Raylib_CSharp.Colors;
using Raylib_CSharp.Images;
using Raylib_CSharp.Textures;
using Raylib_CSharp.Transformations;

namespace RaylibUI.RunGame.GameControls.Mapping;

/// <summary>
/// Terrain tiles packed into a few large textures (pages) instead of one small texture per tile.
/// </summary>
/// <remarks>
/// Pages use point filtering: tiles are only drawn at 1:1 (into the terrain render textures), and smoothing
/// is applied when that composed picture is scaled. Point filtering also keeps neighbouring slots from bleeding.
/// </remarks>
public class TileAtlas
{
    private const int PageSize = 2048;

    private readonly int _slotWidth;
    private readonly int _slotHeight;
    private readonly int _slotsPerRow;
    private readonly int _slotsPerPage;

    private readonly List<Texture2D> _pages = new();
    private readonly Stack<int> _freeSlots = new();
    private int _nextSlot;

    public TileAtlas(int slotWidth, int slotHeight)
    {
        _slotWidth = slotWidth;
        _slotHeight = slotHeight;
        _slotsPerRow = PageSize / slotWidth;
        _slotsPerPage = _slotsPerRow * (PageSize / slotHeight);
    }

    /// <summary>
    /// Copy a tile image into a free slot. The image is converted to RGBA in place, but not freed.
    /// </summary>
    /// <returns>Slot index (for <see cref="Free"/>), page texture and the tile's rectangle in it</returns>
    public (int Slot, Texture2D Page, Rectangle Source) Add(ref Image image)
    {
        if (image.Width != _slotWidth || image.Height != _slotHeight)
        {
            image.ResizeCanvas(_slotWidth, _slotHeight, 0, 0, new Color(0, 0, 0, 0));
        }

        if (image.Format != PixelFormat.UncompressedR8G8B8A8)
        {
            image.FromFormat(PixelFormat.UncompressedR8G8B8A8);
        }

        var slot = _freeSlots.Count > 0 ? _freeSlots.Pop() : _nextSlot++;
        var pageIndex = slot / _slotsPerPage;
        while (_pages.Count <= pageIndex)
        {
            _pages.Add(CreatePage());
        }

        var page = _pages[pageIndex];
        var source = GetSource(slot);
        // Overwrites the whole slot, transparent pixels included, so a reused slot keeps nothing of its old tile
        page.UpdateRec(source, image.Data);
        return (slot, page, source);
    }

    /// <summary>
    /// Make a slot available again (its pixels are overwritten when it is reused)
    /// </summary>
    public void Free(int slot)
    {
        _freeSlots.Push(slot);
    }

    /// <summary>
    /// Free all slots, keeping the pages for reuse
    /// </summary>
    public void Clear()
    {
        _freeSlots.Clear();
        _nextSlot = 0;
    }

    private Rectangle GetSource(int slot)
    {
        var index = slot % _slotsPerPage;
        return new Rectangle(index % _slotsPerRow * _slotWidth, index / _slotsPerRow * _slotHeight,
            _slotWidth, _slotHeight);
    }

    private static Texture2D CreatePage()
    {
        var image = Image.GenColor(PageSize, PageSize, new Color(0, 0, 0, 0));
        var page = Texture2D.LoadFromImage(image);
        image.Unload();
        page.SetFilter(TextureFilter.Point);
        return page;
    }
}
