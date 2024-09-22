namespace Civ2engine;

// ReSharper disable once ClassNeverInstantiated.Global
public class CityTerrainEffect
{
    public int? Terrain { get; set; }
    
    public int Resource { get; set; }

    public int? Improvement { get; set; }

    public int Action { get; set; } = Add;

    public int Value { get; set; }
    public int Level { get; set; } = 0;

    public const int Add = 0;
    public const int AddExtra = 1;
    public const int Multiply = 2;
}