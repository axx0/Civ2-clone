using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Units
{
    internal class Bomber : BaseUnit
    {
        public Bomber() : base(120, 12, 1, 2, 2, 8)
        {
            Type = UnitType.Bomber;
            GAS = UnitGAS.Air;
            Name = "Bomber";
        }
    }
}
