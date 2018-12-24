using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Units
{
    internal class Settlers : BaseUnit
    {
        public Settlers() : base(TechType.Explosives, UnitGAS.Ground, 1, 0, 0, 1, 2, 1, 40, 0, TechType.None, "000000000000000")
        {
            Type = UnitType.Settlers;
            Name = "Settlers";
        }
    }
}
