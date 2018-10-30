using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Units
{
    internal class Artillery : BaseUnit
    {
        public Artillery() : base(50, 10, 1, 2, 2, 1)
        {
            Type = UnitType.Artillery;
            Name = "Artillery";
        }
    }
}
