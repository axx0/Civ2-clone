using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Techs
{
    internal class CombArms : BaseTech
    {
        public CombArms() : base(5, -1, TechType.MobileWarf, TechType.AdvFlight, 3, 0)
        {
            Type = TechType.CombArms;
            Name = "Combined Arms";
        }
    }
}
