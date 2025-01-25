using Civ2engine.IO;
using Civ2engine.MapObjects;
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

        return _mapTileTextures[mapIndex][tile.XIndex, tile.Y] ??=
            MapImage.MakeTileGraphic(tile, tile.Map, _tileSets[mapIndex], _parentScreen.Game, civilizationId);
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

    public MapDimensions GetDimensions(Map map)
    {
        var mapIndex = _seenMaps.IndexOf(map.MapIndex);
        if (mapIndex == -1)
        {
            mapIndex = SetupMap(map);
        }

        //return _dimensions[mapIndex];
        return new MapDimensions
        {
            TotalWidth = _dimensions[mapIndex].TotalWidth.ZoomScale(map.Zoom),
            TotalHeight = _dimensions[mapIndex].TotalHeight.ZoomScale(map.Zoom),
            HalfHeight = _dimensions[mapIndex].HalfHeight.ZoomScale(map.Zoom),
            TileHeight = _dimensions[mapIndex].TileHeight.ZoomScale(map.Zoom),
            TileWidth = _dimensions[mapIndex].TileWidth.ZoomScale(map.Zoom),
            HalfWidth = _dimensions[mapIndex].HalfWidth.ZoomScale(map.Zoom),
            DiagonalCut = _dimensions[mapIndex].DiagonalCut.ZoomScale(map.Zoom).ZoomScale(map.Zoom),
        };
    }

    public void Redraw(Tile tile, int civilizationId)
    {
        var mapIndex = _seenMaps.IndexOf(tile.Map.MapIndex);
        if (mapIndex == -1)
        {
            mapIndex = SetupMap(tile.Map);
        }

        _mapTileTextures[mapIndex][tile.XIndex, tile.Y] =
            MapImage.MakeTileGraphic(tile, tile.Map, _tileSets[mapIndex], _parentScreen.Game, civilizationId);
    }
}