using Model.Constants;
using Model.Images;

namespace Civ2engine;

public class Improvement
{
    public int Type { get; set; }

    //From RULES.TXT
    public string Name { get; set; }

    public int Cost { get; set; }
    public int Upkeep { get; set; }
    public int Prerequisite { get; set; }

    public int ExpiresAt { get; set; } = -1;
    
    public bool IsWonder { get; set; }
        
    public Dictionary<Effects,int> Effects { get; } = new ();
    public List<CityTerrainEffect> TerrainEffects { get; set; } = new();
    public IImageSource? Icon { get; set; }
}