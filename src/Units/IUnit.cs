using System.Drawing;
using civ2.Enums;

namespace civ2.Units
{
    public interface IUnit
    {
        //From RULES.TXT
        string Name { get; }
        TechType UntilTech { get; }
        int MaxMovePoints { get; }
        int Range { get; }
        int Attack { get; }
        int Defense { get; }
        int MaxHitPoints { get; }
        int Firepower { get; }
        int Cost { get; }
        int ShipHold { get; }
        int AIrole { get; }
        TechType PrereqTech { get; }
        string Flags { get; }

        int Id { get; set; }
        int MovePoints { get; set; }
        int HitPoints { get; set; }
        UnitType Type { get; set; }
        UnitGAS GAS { get; }
        OrderType Order { get; set; }
        int X { get; set; }
        int Y { get; set; }
        int MovementCounter { get; set; }
        bool FirstMove { get; set; }
        bool GreyStarShield { get; set; }
        bool Veteran { get; set; }
        int CivId { get; set; }
        int LastMove { get; set; }
        int CaravanCommodity { get; set; }
        int HomeCity { get; set; }
        int GoToX { get; set; }
        int GoToY { get; set; }
        int LinkOtherUnitsOnTop { get; set; }
        int LinkOtherUnitsUnder { get; set; }
        int Counter { get; set; }
        int[] LastXY { get; set; }
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
        Bitmap GraphicMapPanel { get; }
    }
}
