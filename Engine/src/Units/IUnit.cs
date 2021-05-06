using System.Drawing;
using Civ2engine.Enums;

namespace Civ2engine.Units
{
    public interface IUnit
    {
        //From RULES.TXT
        string Name { get; }
        bool Dead { get; set; }
        AdvanceType? UntilTech { get; }
        UnitGAS Domain { get; }
        int MaxMovePoints { get; }
        int FuelRange { get; }
        int AttackBase { get; }
        int DefenseBase { get; }
        int AttackFactor(IUnit defendingUnit);
        int DefenseFactor(IUnit attackingUnit);
        int FirepowerBase { get; }
        int Firepower(bool isThisUnitAttacker, IUnit otherUnit);
        int Cost { get; }
        int ShipHold { get; }
        AIroleType AIrole { get; }
        AdvanceType? PrereqAdvance { get; }
        bool TwoSpaceVisibility { get; }
        bool IgnoreZonesOfControl { get; }
        bool CanMakeAmphibiousAssaults { get; }
        bool SubmarineAdvantagesDisadvantages { get; }
        bool CanAttackAirUnits { get; }
        bool ShipMustStayNearLand { get; }
        bool NegatesCityWalls { get; }
        bool CanCarryAirUnits { get; }
        bool CanMakeParadrops { get; }
        bool Alpine { get; }
        bool X2onDefenseVersusHorse { get; }
        bool FreeSupportForFundamentalism { get; }
        bool DestroyedAfterAttacking { get; }
        bool X2onDefenseVersusAir { get; }
        bool UnitCanSpotSubmarines { get; }

        int Id { get; set; }
        int MovePoints { get; }
        int MovePointsLost { get; set; }
        int HitpointsBase { get; }
        int HitPoints { get; }
        int HitPointsLost { get; set; }
        UnitType Type { get; set; }
        OrderType Order { get; set; }
        int X { get; set; }
        int Y { get; set; }
        int[] XY { get; }
        int Xpx { get; }
        int Ypx { get; }
        int MovementCounter { get; set; }
        bool FirstMove { get; set; }
        bool GreyStarShield { get; set; }
        bool Veteran { get; set; }
        Civilization Owner { get; set; }
        CommodityType CaravanCommodity { get; set; }
        City HomeCity { get; set; }
        int GoToX { get; set; }
        int GoToY { get; set; }
        int LinkOtherUnitsOnTop { get; set; }
        int LinkOtherUnitsUnder { get; set; }
        int Counter { get; set; }
        int[] PrevXY { get; set; }
        int[] PrevXYpx { get; }
        void BuildCity();
        void BuildRoad();
        void BuildMines();
        void BuildIrrigation();
        bool Move(OrderType movementDirection);
        void SkipTurn();
        void Fortify();
        void Transform();
        void Sleep();
        bool TurnEnded { get; }
        bool AwaitingOrders { get; }
        bool IsInCity { get; }
        bool IsInStack { get; }
        bool IsLastInStack { get; }
        //Bitmap Graphic(bool isInStack, int zoom);

    }
}
