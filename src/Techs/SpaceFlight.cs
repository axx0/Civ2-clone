using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Techs
{
    internal class SpaceFlight : BaseTech
    {
        public SpaceFlight() : base(4, 1, TechType.Computers, TechType.Rocketry, 3, 3)
        {
            Type = TechType.SpaceFlight;
            Name = "Space Flight";
        }
    }
}
