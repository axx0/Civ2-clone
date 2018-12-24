using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Units
{
    internal class Trireme : BaseUnit
    {
        public Trireme() : base(40, 1, 1, 1, 1, 3)
        {
            Type = UnitType.Trireme;
            GAS = UnitGAS.Air;
            Name = "Trireme";
        }
    }
}
