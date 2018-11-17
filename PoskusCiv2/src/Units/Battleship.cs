using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Units
{
    internal class Battleship : BaseUnit
    {
        public Battleship() : base(160, 12, 12, 4, 2, 4)
        {
            Type = UnitType.Battleship;
            LSA = UnitLSA.Sea;
            Name = "Battleship";
        }
    }
}
