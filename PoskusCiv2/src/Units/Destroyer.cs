using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Units
{
    internal class Destroyer : BaseUnit
    {
        public Destroyer() : base(60, 4, 4, 3, 1, 6)
        {
            Type = UnitType.Destroyer;
            Name = "Destroyer";
        }
    }
}
