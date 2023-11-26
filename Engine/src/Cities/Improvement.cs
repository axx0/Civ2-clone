using System.Collections.Generic;
using Civ2engine.Enums;
using Civ2engine.Improvements;
using Neo.IronLua;

namespace Civ2engine;

public class Improvement
{
    public int Id => (int)Type;
    public ImprovementType Type { get; set; }

    //From RULES.TXT
    public string Name { get; set; }

    public int Cost { get; set; }
    public int Upkeep { get; set; }
    public int Prerequisite { get; set; }

    public int ExpiresAt { get; set; } = -1;
        
    public Dictionary<Effects,int> Effects { get; } = new ();
    public List<CityTerrainEffect> TerrainEffects { get; set; } = new();

    // ReSharper disable once InconsistentNaming
    //This is a Lua interopp method
    public void addTerrainEffect(LuaTable values)
    {
        var effect = new CityTerrainEffect
        {
            Resource = (int)values[nameof(CityTerrainEffect.Resource)],
            Value = (int)values[nameof(CityTerrainEffect.Value)]
        };
        if (((IDictionary<object, object>)values).TryGetValue(nameof(CityTerrainEffect.Terrain), out var terrain))
        {
            effect.Terrain = (int)terrain;
        }
        if (((IDictionary<object, object>)values).TryGetValue(nameof(CityTerrainEffect.Improvement), out var improvement))
        {
            effect.Improvement = (int)improvement;
        }
        if (((IDictionary<object, object>)values).TryGetValue(nameof(CityTerrainEffect.Action), out var action))
        {
            effect.Action = (int)action;
        }    
        if (((IDictionary<object, object>)values).TryGetValue(nameof(CityTerrainEffect.Level), out var level))
        {
            effect.Level = (int)level;
        }
        TerrainEffects.Add(effect);
    }
}