using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Units
{
    internal class StlthBmbr : BaseUnit
    {
        public StlthBmbr() : base(160, 14, 5, 2, 2, 12)
        {
            Type = UnitType.StlthBmbr;
            LSA = UnitLSA.Air;
            Name = "Stlth Bmbr.";
        }
    }
}
