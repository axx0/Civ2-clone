using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Units
{
    internal class Transport : BaseUnit
    {
        public Transport() : base(50, 0, 3, 3, 1, 5)
        {
            Type = UnitType.Transport;
            Name = "Transport";
        }
    }
}
