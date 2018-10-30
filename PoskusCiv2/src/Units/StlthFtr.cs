using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Units
{
    internal class StlthFtr : BaseUnit
    {
        public StlthFtr() : base(80, 8, 4, 2, 2, 14)
        {
            Type = UnitType.StlthFtr;
            Name = "Stlth Ftr.";
        }
    }
}
