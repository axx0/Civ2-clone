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
        int X { get; set; }
        int Y { get; set; }
        bool GreyStarShield { get; set; }
        bool Veteran { get; set; }
        int Civ { get; set; }
        int HitpointsLost { get; set; }
        int CaravanCommodity { get; set; }
        int Orders { get; set; }
        int HomeCity { get; set; }
        int GoToX { get; set; }
        int GoToY { get; set; }
        int LinkOtherUnitsOnTop { get; set; }
        int LinkOtherUnitsUnder { get; set; }


        int Attack { get;  }
        int Defense { get; }
        int MovesLeft { get; }
        string Name { get; }
        

    }
}
