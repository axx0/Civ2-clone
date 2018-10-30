using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Units
{
    internal class Ironclad : BaseUnit
    {
        public Ironclad() : base(60, 4, 4, 3, 1, 4)
        {
            Type = UnitType.Ironclad;
            Name = "Ironclad";
        }
    }
}
