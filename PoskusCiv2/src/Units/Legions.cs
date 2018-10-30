using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Units
{
    internal class Legions : BaseUnit
    {
        public Legions() : base(40, 4, 2, 1, 1, 1)
        {
            Type = UnitType.Legions;
            Name = "Legion";
        }
    }
}
