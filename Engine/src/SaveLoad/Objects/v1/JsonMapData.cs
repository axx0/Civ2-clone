using System.Collections.Generic;
using System.Linq;
using Civ2engine.MapObjects;
using Civ2engine.SaveLoad.SerializationUtils;

namespace Civ2engine.SaveLoad;

public class JsonMapData
{
    public JsonMapData()
    {
        
    }
    public JsonMapData(Map map, bool includePlayerData)
    {
        FlatWorld = map.Flat;
        MapRevealed = map.MapRevealed;
        WhichCivsMapShown = map.WhichCivsMapShown;
        Zoom = map.Zoom;
        ClickedXy = map.StartingClickedXy;
        Xdim = map.XDim;
        Ydim = map.YDim;
        ResourceSeed = map.ResourceSeed;
        LocatorYdim = map.LocatorYdim;
        LocatorXdim = map.LocatorXdim;
        var tiles = new List<TileData>();
        for (var col = 0; col < map.XDim; col++)
        {
            for (var row = 0; row < map.YDim; row++)
            {
                var tile = map.Tile[col, row];
                var tileData = new TileData
                {
                    T = (int)tile.Terrain.Type,
                    R = tile.River
                };
                
                if (includePlayerData)
                {
                    tileData.V = tile.Visibility.Clamp();
                    tileData.I = tile.Improvements;
                    tileData.P = tile.PlayerKnowledge;
                }
                tiles.Add(tileData);
            }
        }

        Tiles = tiles;
    }

    public bool FlatWorld { get; set; }
    public bool MapRevealed { get; set; }
    public int WhichCivsMapShown { get; set; }
    public int Zoom { get; set; }
    public int[] ClickedXy { get; set; }
    public int Xdim { get; set; }
    public int Ydim { get; set; }
    
    public List<TileData> Tiles { get; set; }
    public int ResourceSeed { get; set; }
    public int LocatorXdim { get; set; }
    public int LocatorYdim { get; set; }
}