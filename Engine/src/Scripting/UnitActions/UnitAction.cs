using Civ2engine.MapObjects;
using Civ2engine.Scripting.ScriptObjects;
using Model.Core.Units;
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global

namespace Civ2engine.Scripting.UnitActions;

public abstract class UnitAction(Unit baseUnit, string type, Game game, Tile? tile = null)
{
    public string actionType { get; } = type;
    
    public UnitApi unit { get; } = new(baseUnit, game);
    
    public int priority { get; set; }
    
    public TileApi tile { get; } = new(tile ?? baseUnit.CurrentLocation, game);
    
    internal readonly Unit BaseUnit = baseUnit;

    public abstract void Execute();
}