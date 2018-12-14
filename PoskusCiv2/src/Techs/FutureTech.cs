using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Techs
{
    internal class FutureTech : BaseTech
    {
        public FutureTech() : base(1, 0, TechType.FusionPwr, TechType.Recycling, 3, 3)
        {
            Type = TechType.FutureTech;
            Name = "Future Technology";
        }
    }
}
