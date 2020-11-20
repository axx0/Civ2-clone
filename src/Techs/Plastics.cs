using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Techs
{
    internal class Plastics : BaseTech
    {
        public Plastics() : base(4, 1, TechType.Refining, TechType.SpaceFlight, 3, 4)
        {
            Type = TechType.Plastics;
            Name = "Plastics";
        }
    }
}
