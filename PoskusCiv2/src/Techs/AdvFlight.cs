using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Techs
{
    internal class AdvFlight : BaseTech
    {
        public AdvFlight() : base(4, -2, TechType.Radio, TechType.MachineTools, 3, 4)
        {
            Type = TechType.AdvFlight;
            Name = "Advanced Flight";
        }
    }
}
