using System.Collections.Generic;
using Model.Constants;
using Neo.IronLua;
// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming

namespace Civ2engine.Scripting.ScriptObjects;

public class CityImprovement
{
    private readonly Improvement _improvement;

    public CityImprovement(Improvement improvement)
    {
        _improvement = improvement;
    }
    
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
        _improvement.TerrainEffects.Add(effect);
    }

    public Dictionary<Effects, int> Effects => _improvement.Effects;
}