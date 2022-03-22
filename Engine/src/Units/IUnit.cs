using Civ2engine.Enums;

namespace Civ2engine.Units
{
    public interface IUnit
    {
        int HitpointsBase { get; }
        int RemainingHitpoints { get; }
        UnitType Type { get; }
        OrderType Order { get; set; }

        Civilization Owner { get; set; }

        bool IsInStack { get; }
    }
}
