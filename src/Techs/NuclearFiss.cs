using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Techs
{
    internal class NuclearFiss : BaseTech
    {
        public NuclearFiss() : base(6, -2, TechType.AtomTheory, TechType.MassProd, 3, 3)
        {
            Type = TechType.NuclearFiss;
            Name = "Nuclear Fission";
        }
    }
}
