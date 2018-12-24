using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Units
{
    internal class Submarine : BaseUnit
    {
        public Submarine() : base(60, 10, 2, 3, 2, 3)
        {
            Type = UnitType.Submarine;
            GAS = UnitGAS.Air;
            Name = "Submarine";
        }
    }
}
