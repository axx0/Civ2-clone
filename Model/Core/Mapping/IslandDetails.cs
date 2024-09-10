using System.Collections.Generic;

namespace Civ2engine.MapObjects;

public class IslandDetails
{
    private int _id;
    public List<Tile> Tiles { get; } = new();

    public int Id
    {
        get => _id;
        set
        {
            Tiles.ForEach(t => t.Island = value);
            _id = value;
        }
    }
}