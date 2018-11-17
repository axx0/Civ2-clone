using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Units
{
    internal class Marines : BaseUnit
    {
        public Marines() : base(60, 8, 5, 2, 1, 1)
        {
            Type = UnitType.Marines;
            LSA = UnitLSA.Land;
            Name = "Marines";
        }
    }
}
