using Civ2engine.IO;
using Civ2engine.MapObjects;
using Model.Core.Mapping;
using Model.ImageSets;
using RaylibUI.RunGame.Commands;

namespace RaylibUI.RunGame.GameControls.Mapping;

public class TileTextureCache
{
    private readonly GameScreen _parentScreen;

    private readonly List<TileDetails?[,]> _mapTileTextures = new();

    private readonly List<int> _seenMaps = new();
    private readonly List<TerrainSet> _tileSets = new();
    private readonly List<MapDimensions> _dimensions = new();

    /// <summary>
    /// GPU storage for all composed tiles (all maps share it, tiles have the same size)
    /// </summary>
    private readonly TileAtlas _atlas = new((int)MapImage.TileRec.Width, (int)MapImage.TileRec.Height);

    public TileTextureCache(GameScreen parentScreen)
    {
        _parentScreen = parentScreen;
    }

    public TileDetails GetTileDetails(Tile tile, int civilizationId)
    {
        var mapIndex = _seenMaps.IndexOf(tile.Map.MapIndex);
        if (mapIndex == -1)
        {
            mapIndex = SetupMap(tile.Map);
        }

        return _mapTileTextures[mapIndex][tile.XIndex, tile.Y] ??= MakeTile(tile, mapIndex, civilizationId);
    }

    private TileDetails MakeTile(Tile tile, int mapIndex, int civilizationId)
    {
        var details = MapImage.MakeTileGraphic(tile, tile.Map, _tileSets[mapIndex], _parentScreen.Game, civilizationId);
        details.Atlas = _atlas;
        return details;
    }

    private int SetupMap(Map map)
    {
        int mapIndex;
        mapIndex = _seenMaps.Count;
        _seenMaps.Add(map.MapIndex);
        _mapTileTextures.Add(new TileDetails?[map.XDim, map.YDim]);

        var tileSet = _parentScreen.Main.ActiveInterface.TileSets[map.MapIndex];
        _tileSets.Add(tileSet);
        _dimensions.Add(new MapDimensions
        {
            TotalWidth = map.Tile.GetLength(0) * tileSet.TileWidth,
            TotalHeight = map.Tile.GetLength(1) * tileSet.HalfHeight + tileSet.HalfHeight,
            HalfHeight = tileSet.HalfHeight,
            TileHeight = tileSet.TileHeight,
            TileWidth = tileSet.TileWidth,
            HalfWidth = tileSet.HalfWidth,
            DiagonalCut = tileSet.DiagonalCut,

        });
        return mapIndex;
    }

    public MapDimensions GetDimensions(Map map, int zoom)
    {
        var cacheIndex = _seenMaps.IndexOf(map.MapIndex);
        if (cacheIndex == -1)
        {
            cacheIndex = SetupMap(map);
        }

        //return _dimensions[mapIndex];
        return new MapDimensions
        {
            TotalWidth = _dimensions[cacheIndex].TotalWidth.ZoomScale(zoom),
            TotalHeight = _dimensions[cacheIndex].TotalHeight.ZoomScale(zoom),
            HalfHeight = _dimensions[cacheIndex].HalfHeight.ZoomScale(zoom),
            TileHeight = _dimensions[cacheIndex].TileHeight.ZoomScale(zoom),
            TileWidth = _dimensions[cacheIndex].TileWidth.ZoomScale(zoom),
            HalfWidth = _dimensions[cacheIndex].HalfWidth.ZoomScale(zoom),
            DiagonalCut = _dimensions[cacheIndex].DiagonalCut.ZoomScale(zoom).ZoomScale(zoom),
        };
    }

    public void Redraw(Tile tile, int civilizationId)
    {
        var mapIndex = _seenMaps.IndexOf(tile.Map.MapIndex);
        if (mapIndex == -1)
        {
            mapIndex = SetupMap(tile.Map);
        }

        _mapTileTextures[mapIndex][tile.XIndex, tile.Y]?.Unload();

        _mapTileTextures[mapIndex][tile.XIndex, tile.Y] = MakeTile(tile, mapIndex, civilizationId);
    }

    public void Clear()
    {
        foreach (var tiles in _mapTileTextures)
        {
            foreach (var tile in tiles)
            {
                tile?.Unload();
            }
        }

        _atlas.Clear();
        _seenMaps.Clear();
        _mapTileTextures.Clear();
        _dimensions.Clear();
        _tileSets.Clear();
    }
}
