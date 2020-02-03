using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using RTciv2.Enums;

namespace RTciv2.Units
{
    public interface IUnit
    {
        //From RULES.TXT
        string Name { get; set; }
        TechType UntilTech { get; set; }
        int MaxMovePoints { get; set; }
        int MovePoints { get; set; }
        int Range { get; set; }
        int Attack { get; set; }
        int Defense { get; set; }
        int MaxHitPoints { get; set; }
        int HitPoints { get; set; }
        int Firepower { get; set; }
        int Cost { get; set; }
        int ShipHold { get; set; }
        int AIrole { get; set; }
        TechType PrereqTech { get; set; }
        string Flags { get; set; }

        int Id { get; set; }
        UnitType Type { get; set; }
        UnitGAS GAS { get; }
        OrderType Order { get; set; }
        int X { get; set; }
        int Y { get; set; }
        bool FirstMove { get; set; }
        bool GreyStarShield { get; set; }
        bool Veteran { get; set; }
        int Civ { get; set; }
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
        bool Move(int moveX, int moveY);
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
