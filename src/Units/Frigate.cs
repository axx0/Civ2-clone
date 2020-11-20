using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Units
{
    internal class Frigate : BaseUnit
    {
        public Frigate() : base(50, 4, 2, 2, 1, 4)
        {
            Type = UnitType.Frigate;
            GAS = UnitGAS.Air;
            Name = "Frigate";
        }
    }
}
