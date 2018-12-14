using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Techs
{
    internal class Flight : BaseTech
    {
        public Flight() : base(4, -1, TechType.Combustion, TechType.TheoryGrav, 2, 4)
        {
            Type = TechType.Flight;
            Name = "Flight";
        }
    }
}
