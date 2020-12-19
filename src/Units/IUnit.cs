using System.Drawing;
using civ2.Enums;

namespace civ2.Units
{
    public interface IUnit
    {
        //From RULES.TXT
        string Name { get; }
        AdvanceType? UntilTech { get; }
        UnitGAS Domain { get; }
        int MaxMovePoints { get; }
        int FuelRange { get; }
        int AttackFactor { get; }
        int DefenseFactor { get; }
        int MaxHitpoints { get; }
        int Firepower { get; }
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
        int HitPoints { get; }
        int HitPointsLost { get; set; }
        UnitType Type { get; set; }
        OrderType Order { get; set; }
        int X { get; set; }
        int Y { get; set; }
        int Xpx { get; }
        int Ypx { get; }
        int MovementCounter { get; set; }
        bool FirstMove { get; set; }
        bool GreyStarShield { get; set; }
        bool Veteran { get; set; }
        Civilization Owner { get; set; }
        int LastMove { get; set; }
        CommodityType CaravanCommodity { get; set; }
        City HomeCity { get; set; }
        int GoToX { get; set; }
        int GoToY { get; set; }
        int LinkOtherUnitsOnTop { get; set; }
        int LinkOtherUnitsUnder { get; set; }
        int Counter { get; set; }
        int[] LastXY { get; set; }
        int[] LastXYpx { get; }
        void BuildCity();
        void BuildRoad();
        void BuildMines();
        void BuildIrrigation();
        bool Move(OrderType movementDirection);
        void SkipTurn();
        void Fortify();
        void Transform();
        void Sleep();
        bool TurnEnded { get; set; }
        bool AwaitingOrders { get; set; }
        bool IsInCity { get; }
        bool IsInStack { get; }
        bool IsLastInStack { get; }
        Bitmap Graphic(bool isInStack, int zoom);

    }
}
