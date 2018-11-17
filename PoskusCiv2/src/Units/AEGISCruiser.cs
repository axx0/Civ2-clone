using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Units
{
    internal class AEGISCruiser : BaseUnit
    {
        public AEGISCruiser() : base(100, 8, 8, 3, 2, 5)
        {
            Type = UnitType.AEGISCruiser;
            LSA = UnitLSA.Sea;
            Name = "AEGIS Cruiser";
        }
    }
}
