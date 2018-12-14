using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Techs
{
    internal class Rocketry : BaseTech
    {
        public Rocketry() : base(6, -2, TechType.AdvFlight, TechType.Electronics, 3, 0)
        {
            Type = TechType.Rocketry;
            Name = "Rocketry";
        }
    }
}
