using Civ2engine.MapObjects;
using Model.Core;

namespace Civ2engine.Scripting;

public class TerrainApi(Tile tile)
{
    public bool isOcean => tile.Terrain.Type == TerrainType.Ocean;
}