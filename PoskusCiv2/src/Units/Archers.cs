using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Units
{
    internal class Archers : BaseUnit
    {
        public Archers() : base(TechType.Gunpowder, 0, 1, 0, 3, 2, 1, 1, 3, 0, 1, TechType.WarriorCode, "000000000000000")
        {
            Type = UnitType.Archers;
            Name = "Archers";
        }
    }
}
