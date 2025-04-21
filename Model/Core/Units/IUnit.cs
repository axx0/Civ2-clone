using Civ2engine;
using Civ2engine.MapObjects;

namespace Model.Core.Units
{
    public interface IUnit
    {
        int HitpointsBase { get; }
        int RemainingHitpoints { get; }
        int Type { get; }
        int Order { get; set; }

        Civilization Owner { get; set; }

        bool IsInStack { get; }
        
        Tile CurrentLocation { get; }
    }
}
