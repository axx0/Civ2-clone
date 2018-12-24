using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Units
{
    internal class Paratroopers : BaseUnit
    {
        public Paratroopers() : base(60, 6, 4, 2, 1, 1)
        {
            Type = UnitType.Paratroopers;
            GAS = UnitGAS.Air;
            Name = "Paratroopers";
        }
    }
}
