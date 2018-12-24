using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Units
{
    internal class Phalanx : BaseUnit
    {
        public Phalanx() : base(TechType.Feudalism, 0, 1, 0, 1, 2, 1, 1, 2, 0, 1, TechType.BronzeWork, "000000000000000")
        {
            Type = UnitType.Phalanx;
            Name = "Phalanx";
        }
    }
}
