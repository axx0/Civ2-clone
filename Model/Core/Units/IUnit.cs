using Civ2engine.Enums;
using Civ2engine.MapObjects;

namespace Civ2engine.Units
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
