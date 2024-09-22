using System.Collections.Generic;
using Civ2engine.Units;
// ReSharper disable UnusedMember.Global

namespace Civ2engine.Scripting.ScriptObjects;

public class UnitType
{
    private readonly UnitDefinition _unitDefinition;

    public UnitType(UnitDefinition unitDefinition)
    {
        _unitDefinition = unitDefinition;
    }
    
    public string name
    {
        get => _unitDefinition.Name;
        set => _unitDefinition.Name = value;
    }

    public Dictionary<UnitEffect, int> Effects => _unitDefinition.Effects;
}