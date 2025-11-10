using Civ2engine.Terrains;
using Model.Core;

namespace Civ2engine.Scripting;

public class BaseTerrain(Terrain terrain, int map)
{
    private readonly int _map = map;

    public Terrain Terrain { get; } = terrain;

    public bool isOcean => terrain.Type == TerrainType.Ocean;
}