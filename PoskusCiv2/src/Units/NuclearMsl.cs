using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Units
{
    internal class NuclearMsl : BaseUnit
    {
        public NuclearMsl() : base(160, 99, 0, 1, 1, 16)
        {
            Type = UnitType.NuclearMsl;
            GAS = UnitGAS.Air;
            Name = "Nuclear Msl.";
        }
    }
}
