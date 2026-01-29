using Civ2engine.MapObjects;

namespace Model.Core.Mapping;

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

    public int TotalFertile { get; set; }
}