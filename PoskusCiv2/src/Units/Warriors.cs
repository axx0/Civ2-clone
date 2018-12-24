using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Units
{
    internal class Warriors : BaseUnit
    {
        public Warriors() : base(TechType.Feudalism, UnitGAS.Ground, 1, 0, 1, 1, 1, 1, 1, 0, TechType.None, "000000000000000")
        {
            Type = UnitType.Warriors;
            Name = "Warriors";
        }
    }
}
