using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Techs
{
    internal class FusionPwr : BaseTech
    {
        public FusionPwr() : base(3, 0, TechType.NuclearPwr, TechType.Supercond, 3, 3)
        {
            Type = TechType.FusionPwr;
            Name = "Fusion Power";
        }
    }
}
