using Civ2engine.MapObjects;
using Model.ImageSets;

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
            TotalWidth = map.Tile.GetLength(0) * tileSet.TileWidth.ZoomScale(map.Zoom),
            TotalHeight = map.Tile.GetLength(1) * tileSet.HalfHeight.ZoomScale(map.Zoom) + tileSet.HalfHeight.ZoomScale(map.Zoom),
            HalfHeight = tileSet.HalfHeight.ZoomScale(map.Zoom),
            TileHeight = tileSet.TileHeight.ZoomScale(map.Zoom),
            TileWidth = tileSet.TileWidth.ZoomScale(map.Zoom),
            HalfWidth = tileSet.HalfWidth.ZoomScale(map.Zoom),
            DiagonalCut = tileSet.DiagonalCut.ZoomScale(map.Zoom).ZoomScale(map.Zoom)
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

        return _dimensions[mapIndex];
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