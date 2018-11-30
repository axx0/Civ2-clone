using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Units
{
    public interface IUnit
    {
        UnitType Type { get; }
        UnitLSA LSA { get; }
        UnitAction Action { get; set; }
        int X { get; set; }
        int Y { get; set; }
        int X2 { get; }
        int Y2 { get; }
        bool FirstMove { get; set; }
        bool GreyStarShield { get; set; }
        bool Veteran { get; set; }
        int Civ { get; set; }
        int MovesMade { get; set; }
        int HitpointsLost { get; set; }
        int LastMove { get; set; }
        int CaravanCommodity { get; set; }
        int Orders { get; set; }
        int HomeCity { get; set; }
        int GoToX { get; set; }
        int GoToY { get; set; }
        int LinkOtherUnitsOnTop { get; set; }
        int LinkOtherUnitsUnder { get; set; }

        void Move(int moveX, int moveY);
        void SkipTurn();
        void Fortify();
        void Irrigate();
        void Terraform();
        void Sentry();
        void BuildRoad();
        bool TurnEnded { get; set; }

        int Attack { get;  }
        int Defense { get; }
        int StartingMoves { get; }
        string Name { get; }
    }
}
