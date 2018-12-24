using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Units
{
    internal class Engineers : BaseUnit
    {
        public Engineers() : base(TechType.None, UnitGAS.Ground, 2, 0, 0, 2, 2, 1, 4, 0, TechType.Explosives, "000000000000000")
        {
            Type = UnitType.Engineers;
            Name = "Engineers";
        }
    }
}
