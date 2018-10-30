using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Units
{
    internal class Cruiser : BaseUnit
    {
        public Cruiser() : base(80, 6, 6, 3, 2, 5)
        {
            Type = UnitType.Cruiser;
            Name = "Cruiser";
        }
    }
}
